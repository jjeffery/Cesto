using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Cesto.Internal;

namespace Cesto.Threading
{
	/// <summary>
	/// A <see cref="WorkThread"/> operates as a background thread, and operates a message pump
	/// which is compatible with async/await processing.
	/// </summary>
	public class WorkThread : IDisposable
	{
		private Thread _thread;
		private readonly WaitQueue<Action> _queue = new WaitQueue<Action>();
		private readonly object _lockObject = new object();
		private volatile bool _stopImmediately;
		private static readonly ThreadLocal<WorkThread> CurrentWorkerThread = new ThreadLocal<WorkThread>();
		private readonly Dictionary<WaitHandle, Action> _waitHandleToAction = new Dictionary<WaitHandle, Action>();
		private WaitHandle[] _waitHandles;
		private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

		/// <summary>
		/// Event that is raised shortly after the work thread is started.
		/// </summary>
		public event EventHandler Started;

		/// <summary>
		/// Event that is raised just before the work thread stops.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Raising this event is the last thing the worker thread does before exiting its thread.
		/// Any attempt to <see cref="Post"/> or <see cref="Send"/> to the worker thread will fail
		/// with an <see cref="InvalidOperationException"/>.
		/// </para>
		/// <para>
		/// This event should be used with caution. No attempt is made to recover if this event
		/// handler throws an exception.
		/// </para>
		/// </remarks>
		public event EventHandler Stopped;

		/// <summary>
		/// Event that is raised when the work thread stops because of an unhandled exception.
		/// </summary>
		/// <remarks>
		/// If an event handler is able to set the <see cref="WorkThreadExceptionEventArgs.Handled"/>
		/// property, then the work thread will continue operation, otherwise it will terminate.
		/// </remarks>
		public event EventHandler<WorkThreadExceptionEventArgs> ExceptionThrown;

		/// <summary>
		/// The work thread Synchronization context
		/// </summary>
		public readonly SynchronizationContext SynchronizationContext;

		/// <summary>
		/// Cancellation token associated with the work thread. As soon as the work thread is requested to
		/// stop, this cancellation token is marked as cancellation requested.
		/// </summary>
		public CancellationToken CancellationToken
		{
			get { return _cancellationTokenSource.Token; }
		}

		/// <summary>
		/// Constructs a new <see cref="WorkThread"/> object.
		/// </summary>
		public WorkThread()
		{
			SynchronizationContext = new WorkerThreadSynchronizationContext(this);
		}

		/// <summary>
		/// If the work thread terminates because of an unhandled exception, this property
		/// will be that exception. Otherwise <c>null</c>.
		/// </summary>
		public Exception Exception { get; private set; }

		/// <summary>
		/// If the currently executing thread is a work thread, then this property will
		/// return the work thread instance. Otherwise it will return <c>null</c>.
		/// </summary>
		public static WorkThread Current
		{
			get { return CurrentWorkerThread.Value; }
		}

		/// <summary>
		/// The name of the work thread.
		/// </summary>
		/// <remarks>
		/// The underlying thread will receive this name, however this property should be set
		/// prior to calling <see cref="Start"/>. If it is set after the thread is started, the
		/// underlying thread will not receive this name.
		/// </remarks>
		public string Name { get; set; }

		/// <summary>
		/// Is the work thread alive.
		/// </summary>
		public bool IsAlive
		{
			get { return _thread != null && _thread.IsAlive; }
		}

		/// <summary>
		/// Start the work thread.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// The work thread has already been started.
		/// </exception>
		public void Start()
		{
			lock (_lockObject)
			{
				if (_thread != null)
				{
					throw new InvalidOperationException("Thread has been started");
				}
				_cancellationTokenSource = new CancellationTokenSource();
				_thread = new Thread(ThreadMain) { IsBackground = true };
				if (!string.IsNullOrEmpty(Name))
				{
					_thread.Name = Name;
				}
				Exception = null;
				_thread.Start();
			}
		}

		/// <summary>
		/// Stops the work thread. The thread will not empty its action queue before stopping.
		/// </summary>
		public void Dispose()
		{
			_stopImmediately = true;
			RequestStop();
		}

		/// <summary>
		/// Requests the worker thread to stop. The thread will empty its action queue before stopping.
		/// </summary>
		public void RequestStop()
		{
			_cancellationTokenSource.Cancel();
		}

		/// <summary>
		/// Blocks the current thread and waits for the worker thread to stop, or the timeout occurs. The worker thread will
		/// empty its action queue before stopping.
		/// </summary>
		/// <param name="milliseconds">Number of milliseconds to wait before giving up.</param>
		/// <returns>
		/// Returns <c>true</c> if the worker thread stopped in the time required, <c>false</c> otherwise.
		/// </returns>
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

		/// <summary>
		/// Associate an action with a <see cref="WaitHandle"/>. The action is
		/// called whenever the <see cref="WaitHandle"/> is set.
		/// </summary>
		/// <returns>
		/// Returns an <see cref="IDisposable"/>, which can be used for cancelling the
		/// association. Once the Dispose method is called, the action will no longer be
		/// called when the wait handle is set.
		/// </returns>
		/// <remarks>
		/// If the <see cref="WaitHandle"/> needs to be manually reset, it is the responsibility of
		/// the action to do this, otherwise an infinte loop will result.
		/// </remarks>
		public IDisposable SetAction(WaitHandle waitHandle, Action action)
		{
			if (IsOnWorkerThread())
			{
				DoSetAction(waitHandle, action);
			}
			else
			{
				Post(() => DoSetAction(waitHandle, action));
			}

			// Don't use a DisposableAction here, as it will implicitly
			// retain the action closure as well. Play it safe and return
			// a purpose-built object.
			return new RemoveActionDisposable(this, waitHandle);
		}

		private class RemoveActionDisposable : IDisposable
		{
			private readonly WorkThread _workThread;
			private readonly WaitHandle _waitHandle;

			public RemoveActionDisposable(WorkThread workThread, WaitHandle waitHandle)
			{
				_workThread = workThread;
				_waitHandle = waitHandle;
			}

			public void Dispose()
			{
				if (_workThread.IsOnWorkerThread())
				{
					_workThread.DoRemoveAction(_waitHandle);
				}
				else
				{
					_workThread.Post(() => _workThread.DoRemoveAction(_waitHandle));
				}
			}
		}

		/// <summary>
		/// Post an action to the work thread, but do not wait for it to be executed.
		/// </summary>
		/// <param name="action">Action to be performed on the work thread.</param>
		public void Post(Action action)
		{
			Verify.ArgumentNotNull(action, "action");
			CheckAlive();
			_queue.Enqueue(action);
		}

		/// <summary>
		/// Send an action to the work thread and wait for it to be executed.
		/// </summary>
		/// <param name="action">Action to perform.</param>
		public void Send(Action action)
		{
			Verify.ArgumentNotNull(action, "action");
			if (Current == this)
			{
				action();
			}
			else
			{
				CheckAlive();
				using (var wait = new ManualResetEvent(false))
				{
					// ReSharper disable AccessToDisposedClosure
					Post(() => {
						action();
						wait.Set();
					});
					// ReSharper restore AccessToDisposedClosure

					for (; ; )
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

		/// <summary>
		/// Send a function to the work thread and wait for the result.
		/// </summary>
		/// <typeparam name="T">Function return type</typeparam>
		/// <param name="func">Function to call on the work thread</param>
		/// <returns>Returns the return value of the function.</returns>
		public T Send<T>(Func<T> func)
		{
			Verify.ArgumentNotNull(func, "func");
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

		private void DoRemoveAction(WaitHandle waitHandle)
		{
			CheckWorkerThread();
			_waitHandleToAction.Remove(waitHandle);
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
				_queue.Clear();
				CurrentWorkerThread.Value = null;
				lock (_lockObject)
				{
					_thread = null;
				}
				SynchronizationContext.SetSynchronizationContext(null);

				// Note there is no attempt to recover from exceptions thrown in the event handler.
				RaiseStopped();
			}
		}

		private void MainLoop()
		{
			CheckWorkerThread();
			// process events until stop has been requested
			while (!_cancellationTokenSource.IsCancellationRequested)
			{
				DoOneThing(Timeout.Infinite);
			}

			// finish the queue of things to do (warning, make sure
			// this is not an infinite loop).
			while (!_stopImmediately && DoOneThing(0)) { }
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

		private void CheckAlive()
		{
			if (!IsAlive)
			{
				throw new InvalidOperationException("Worker thread is stopped");
			}
		}
	}
}
