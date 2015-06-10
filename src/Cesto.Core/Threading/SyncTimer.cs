using System;
using System.Threading;

namespace Cesto.Threading
{
	/// <summary>
	/// A one-shot timer that performs an action when the timer expires.
	/// The action will be performed by posting to the SynchronizationContext.
	/// This can be used to ensure that the timer action is called on a particular thread.
	/// </summary>
	public class SyncTimer : IDisposable
	{
		// Action called when the timer expires
		private readonly Action _action;

		// Timer that does the work
		private readonly Timer _timer;

		/// <summary>
		/// The <see cref="SynchronizationContext"/> associated with this <see cref="SyncTimer"/>
		/// </summary>
		public readonly SynchronizationContext SynchronizationContext;

		/// <summary>
		/// Constructs a SyncTimer.
		/// </summary>
		/// <param name="action">
		/// The action to perform whenever the timer expires.
		/// </param>
		/// <param name="synchronizationContext">
		/// The synchronization context to use to execute the action when the timer expires.
		/// If not specified, the current synchronization context is used.
		/// </param>
		public SyncTimer(Action action, SynchronizationContext synchronizationContext = null)
		{
			_timer = new Timer(TimerCallback);
			_action = Verify.ArgumentNotNull(action, "action");
			if (synchronizationContext == null)
			{
				synchronizationContext = SynchronizationContext.Current;
				if (synchronizationContext == null)
				{
					throw new InvalidOperationException("No synchronization context specified, and no current synchronization context exists");
				}
				SynchronizationContext = synchronizationContext;
			}
		}

		// Called when the timer expires.
		private void TimerCallback(object state)
		{
			SynchronizationContext.Post(SynchronizationContextCallback, null);
		}

		// Called by the timer callback via the synchronization context
		private void SynchronizationContextCallback(object state)
		{
			// This method is called on the correct thread to call the action
			_action();
		}

		public void Dispose()
		{
			Stop();
			_timer.Dispose();
		}

		/// <summary>
		/// Starts the timer. If the timer was already running, then the timer is restarted.
		/// </summary>
		/// <param name="timeout">Timeout</param>
		public void Start(TimeSpan timeout)
		{
			Stop();
			_timer.Change(timeout, TimeSpan.FromMilliseconds(-1));
		}

		/// <summary>
		/// Starts the timer. If the timer was already running, then the timer is restarted.
		/// </summary>
		/// <param name="milliseconds">Timeout in milliseconds</param>
		public void Start(int milliseconds)
		{
			Stop();
			_timer.Change(milliseconds, Timeout.Infinite);
		}

		/// <summary>
		/// Stops the timer. The timer can be started again.
		/// </summary>
		public void Stop()
		{
			_timer.Change(Timeout.Infinite, Timeout.Infinite);
		}
	}
}
