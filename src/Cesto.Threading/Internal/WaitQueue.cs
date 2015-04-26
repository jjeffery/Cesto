using System.Collections.Concurrent;
using System.Threading;

namespace Cesto.Internal
{
	/// <summary>
	/// A simple, thread-safe queue that exposes a <see cref="WaitHandle"/> that
	/// is triggered when an item is added to the queue, and when an item is successfully
	/// removed from the queue.
	/// </summary>
	public class WaitQueue<T>
	{
		private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
		private readonly AutoResetEvent _event = new AutoResetEvent(false);

		/// <summary>
		/// This wait handle is set when something *might* be available on the queue.
		/// </summary>
		public WaitHandle WaitHandle
		{
			get { return _event; }
		}

		/// <summary>
		/// Removes all items in the queue
		/// </summary>
		public void Clear()
		{
			_event.Reset();
			T item;
			while (_queue.TryDequeue(out item)) {}
		}

		/// <summary>
		/// Append an item to the queue, and set the <see cref="WaitHandle"/>
		/// </summary>
		public void Enqueue(T item)
		{
			_queue.Enqueue(item);
			_event.Set();
		}

		/// <summary>
		/// Attempt to retrieve an item from the queue.
		/// </summary>
		/// <remarks>
		/// If an item is successfully retrieved from the queue, the <see cref="WaitHandle"/>
		/// is set again.
		/// </remarks>
		public bool TryDequeue(out T item)
		{
			_event.Reset();
			var result = _queue.TryDequeue(out item);
			if (result)
			{
				_event.Set();
			}
			return result;
		}
	}
}
