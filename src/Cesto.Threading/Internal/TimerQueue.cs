using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cesto.Threading;

namespace Cesto.Internal
{
	/// <summary>
	/// Thread-safe collection of timers. Used by <see cref="WorkThread"/>
	/// </summary>
	public class TimerQueue : IDisposable
	{
		private readonly object _lock = new object();
		private readonly ISet<TimerInfo> _timers = new HashSet<TimerInfo>();
		private readonly SynchronizationContext _synchronizationContext;

		/// <summary>
		/// Construct a WorkQueue
		/// </summary>
		/// <param name="synchronizationContext">
		/// If non-null, whenever a timer expires, it will invoke the action on this SynchronizationContext.
		/// </param>
		public TimerQueue(SynchronizationContext synchronizationContext)
		{
			_synchronizationContext = synchronizationContext;
		}

		public IDisposable Schedule(TimeSpan timeout, Action action)
		{
			var timer = new TimerInfo(timeout, action, this);
			lock (_lock)
			{
				_timers.Add(timer);
			}
			return timer;
		}

		public void Dispose()
		{
			lock (_lock)
			{
				foreach (var timer in _timers.ToArray())
				{
					timer.Dispose();
				}
				_timers.Clear();
			}
		}

		private class TimerInfo : IDisposable
		{
			private static long _lastId;

			private readonly long _id;
			private readonly Timer _timer;
			private readonly Action _action;
			private readonly TimerQueue _timerQueue;
			private bool _disposed;

			public TimerInfo(TimeSpan timeout, Action action, TimerQueue timerQueue)
			{
				_id = Interlocked.Increment(ref _lastId);
				_timer = new Timer(TimerCallback, null, timeout, TimeSpan.FromMilliseconds(-1));
				_action = action;
				_timerQueue = timerQueue;
			}

			public override int GetHashCode()
			{
				return _id.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				var other = obj as TimerInfo;
				return other != null && other._id == _id;
			}

			public void Dispose()
			{
				lock (_timerQueue._lock)
				{
					if (!_disposed)
					{
						_disposed = true;
						_timer.Dispose();
						_timerQueue._timers.Remove(this);
					}
				}
			}

			private void TimerCallback(object state)
			{
				Dispose();

				// Do the work.
				if (_timerQueue._synchronizationContext == null)
				{
					_action();
				}
				else
				{
					_timerQueue._synchronizationContext.Post(s => _action(), null);
				}
			}
		}
	}
}
