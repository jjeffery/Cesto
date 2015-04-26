using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Cesto.Internal;

namespace Cesto.Threading
{
	/// <summary>
	/// Puts as much of the worker thread stuff in a separate class to
	/// keep the main PLC processing class simpler.
	/// </summary>
	public class WorkThread : IDisposable
	{
		private Thread _thread;
		private readonly WaitQueue<Action> _queue = new WaitQueue<Action>();
		private readonly object _lockObject = new object();
		private bool _stopRequested;
		private bool _stopImmediately;
		private static readonly ThreadLocal<WorkThread> CurrentWorkerThread = new ThreadLocal<WorkThread>();
		private readonly Dictionary<WaitHandle, Action> _waitHandleToAction = new Dictionary<WaitHandle, Action>();
		private readonly TimerQueue _timerQueue;
		private WaitHandle[] _waitHandles;
		private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

		public event EventHandler Started;
		public event EventHandler Stopped;
		public event EventHandler<WorkThreadExceptionEventArgs> ExceptionThrown;
		public readonly SynchronizationContext SynchronizationContext;

		public CancellationToken CancellationToken
		{
			get { return _cancellationTokenSource.Token; }
		}

		public WorkThread()
		{
			SynchronizationContext = new WorkerThreadSynchronizationContext(this);
			_timerQueue = new TimerQueue(SynchronizationContext);
		}

		public Exception Exception { get; set; }

		public static WorkThread Current
		{
			get { return CurrentWorkerThread.Value; }
		}

		public string Name { get; set; }

		public bool IsAlive
		{
			get { return _thread != null && _thread.IsAlive; }
		}

		public void Start()
		{
			lock (_lockObject)
			{
				if (_thread != null)
				{
					throw new InvalidOperationException("Thread has been started");
				}
				_cancellationTokenSource = new CancellationTokenSource();
				_thread = new Thread(ThreadMain) {IsBackground = true};
				if (!string.IsNullOrEmpty(Name))
				{
					_thread.Name = Name;
				}
				_stopRequested = false;
				Exception = null;
				_thread.Start();
			}
		}

		/// <summary>
		/// Stops the work thread. The thread will not empty its action queue before stopping
		/// </summary>
		public void Dispose()
		{
			// TODO: need to be better about stopping
			RequestStop();
		}

		/// <summary>
		/// Requests the worker thread to stop. The thread will empty its action queue before stopping
		/// </summary>
		public void RequestStop()
		{
			_cancellationTokenSource.Cancel();

			// Can be called from any thread
			if (IsOnWorkerThread())
			{
				DoRequestStop();
			}
			else
			{
				Post(() => DoRequestStop());
			}
		}

		public bool Join(int milliseconds = Timeout.Infinite)
		{
			if (Current == this)
			{
				throw new InvalidOperationException("Cannot join to your own worker thread");
			}

			Thread thread;
			lock (_lockObject)
			{
				thread = _thread;
			}
			if (thread == null)
			{
				return true;
			}

			return thread.Join(milliseconds);
		}

		public void Stop()
		{
			RequestStop();
			Join();
		}

		/// <summary>
		/// Associate an action with a <see cref="WaitHandle"/>. The action is
		/// called whenever the <see cref="WaitHandle"/> is set.
		/// </summary>
		public void SetAction(WaitHandle waitHandle, Action action)
		{
			if (IsOnWorkerThread())
			{
				DoSetAction(waitHandle, action);
			}
			else
			{
				Post(() => DoSetAction(waitHandle, action));
			}
		}

		public void Post(Action action)
		{
			// Can be called from any thread
			_queue.Enqueue(action);
		}

		public void Post(Func<Task> action)
		{
			var asyncAction = new Action(async () => AsyncAction(action));
			_queue.Enqueue(asyncAction);
		}

		private async void AsyncAction(Func<Task> action)
		{
			Exception exception = null;

			if (action != null)
			{
				try
				{
					await action();
				}
				catch (Exception ex)
				{
					exception = ex;
				}
			}

			if (exception != null)
			{
				var exceptionThrown = ExceptionThrown;
				if (exceptionThrown != null)
				{
					var eventArgs = new WorkThreadExceptionEventArgs(exception);
					exceptionThrown(this, eventArgs);
					if (eventArgs.Handled)
					{
						exception = null;
					}
				}
			}

			if (exception != null)
			{
				// unhandled exception, cannot throw because this would terminate the appdomain
				Exception = exception;
				_stopRequested = true;
				_stopImmediately = true;
			}
		}

		public void Send(Action action)
		{
			if (Current == this)
			{
				action();
			}
			else
			{
				if (!IsAlive)
				{
					throw new InvalidOperationException("Worker thread has stopped");
				}
				using (var wait = new ManualResetEvent(false))
				{
					// ReSharper disable AccessToDisposedClosure
					Post(() => {
						action();
						wait.Set();
					});
					// ReSharper restore AccessToDisposedClosure

					for (;;)
					{
						if (wait.WaitOne(500))
						{
							return;
						}
						if (!IsAlive)
						{
							throw new InvalidOperationException("Worker thread has stopped");
						}
					}
				}
			}
		}

		public IDisposable Schedule(TimeSpan timeout, Action action)
		{
			return _timerQueue.Schedule(timeout, action);
		}

		public T Send<T>(Func<T> func)
		{
			T result = default(T);
			Action action = () => result = func();
			Send(action);
			return result;
		}

		private void DoSetAction(WaitHandle waitHandle, Action action)
		{
			CheckWorkerThread();
			_waitHandleToAction.Remove(waitHandle);
			if (action != null)
			{
				_waitHandleToAction.Add(waitHandle, action);
			}
			_waitHandles = null;
		}

		private void CreateWaitHandles()
		{
			CheckWorkerThread();
			if (_waitHandles == null)
			{
				_waitHandles = new List<WaitHandle>(_waitHandleToAction.Keys).ToArray();
			}
		}

		private void DoRequestStop()
		{
			CheckWorkerThread();
			_stopRequested = true;
		}

		private void RaiseStarted()
		{
			CheckWorkerThread();
			var started = Started;
			if (started != null)
			{
				started(this, EventArgs.Empty);
			}
		}

		private void RaiseStopped()
		{
			CheckWorkerThread();
			var stopped = Stopped;
			if (stopped != null)
			{
				stopped(this, EventArgs.Empty);
			}
		}

		private void ThreadMain()
		{
			CurrentWorkerThread.Value = this;
			SynchronizationContext.SetSynchronizationContext(SynchronizationContext);

			try
			{
				RaiseStarted();
				SetAction(_queue.WaitHandle, HandleQueue);
				MainLoop();
			}
			catch (Exception ex)
			{
				Exception = ex;
			}
			finally
			{
				RaiseStopped();
				CurrentWorkerThread.Value = null;
				_timerQueue.Dispose();
				_queue.Clear();
				lock (_lockObject)
				{
					_thread = null;
				}
			}
		}

		private void MainLoop()
		{
			CheckWorkerThread();
			// process events until stop has been requested
			while (!_stopRequested)
			{
				DoOneThing(Timeout.Infinite);
			}

			// finish the queue of things to do (warning, make sure
			// this is not an infinite loop).
			while (!_stopImmediately && DoOneThing(0)) {}
		}

		/// <summary>
		/// Does one thing.
		/// </summary>
		/// <returns>
		/// Returns <c>true</c> if there was something to do, <c>false</c>
		/// if the timeout expired. Won't return <c>false</c> if
		/// <paramref name="timeout"/> is <c>Timeout.Infinite</c>.
		/// </returns>
		private bool DoOneThing(int timeout)
		{
			CheckWorkerThread();
			CreateWaitHandles();
			var index = WaitHandle.WaitAny(_waitHandles, timeout);
			if (index == WaitHandle.WaitTimeout)
			{
				// timed out
				return false;
			}

			var waitHandle = _waitHandles[index];
			Action action;
			if (_waitHandleToAction.TryGetValue(waitHandle, out action))
			{
				if (action != null)
				{
					try
					{
						action();
					}
					catch (Exception ex)
					{
						var exceptionThrown = ExceptionThrown;
						if (exceptionThrown == null)
						{
							throw;
						}

						var eventArgs = new WorkThreadExceptionEventArgs(ex);
						exceptionThrown(this, eventArgs);
						if (!eventArgs.Handled)
						{
							throw;
						}
					}
				}
			}

			return true;
		}

		private void HandleQueue()
		{
			CheckWorkerThread();
			Action action;
			if (_queue.TryDequeue(out action) && action != null)
			{
				action();
			}
		}

		private bool IsOnWorkerThread()
		{
			return ReferenceEquals(this, CurrentWorkerThread.Value);
		}

		/// <summary>
		/// Checks that the code is being called from the correct thread
		/// </summary>
		[Conditional("DEBUG")]
		private void CheckWorkerThread()
		{
			if (!IsOnWorkerThread())
			{
				var stackFrame = new StackFrame(1);
				var method = stackFrame.GetMethod();
				var message =
					string.Format("Method {0} was called from an incorrect thread. Indicates error in WorkerThread implementation",
						method.Name);
				throw new InvalidOperationException(message);
			}
		}
	}

	public class WorkerThreadSynchronizationContext : SynchronizationContext
	{
		private readonly WorkThread _workThread;

		public WorkerThreadSynchronizationContext(WorkThread workThread)
		{
			_workThread = Verify.ArgumentNotNull(workThread, "workerThread");
		}

		public override SynchronizationContext CreateCopy()
		{
			return new WorkerThreadSynchronizationContext(_workThread);
		}

		public override void Post(SendOrPostCallback d, object state)
		{
			_workThread.Post(() => d(state));
		}

		public override void Send(SendOrPostCallback d, object state)
		{
			if (WorkThread.Current == _workThread)
			{
				d(state);
			}
			else
			{
				using (var wait = new ManualResetEvent(false))
				{
					// ReSharper disable AccessToDisposedClosure
					_workThread.Post(() => {
						d(state);

						wait.Set();
					});
					// ReSharper restore AccessToDisposedClosure

					wait.WaitOne();
				}
			}
		}
	}
}
