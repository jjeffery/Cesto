using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Cesto.WinForms
{
	/// <summary>
	/// A virtual data source optimised for displaying larger event log displays. It
	/// does not provide any sorting functionality.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class EventLoggingDataSource2<T> : IVirtualDataSource<T> where T : class
	{
		/// <summary>
		/// When number of items in the list exceeds this value, the list will be truncated to the number
		/// of items specified by <see cref="PreferredCapacity"/>.
		/// </summary>
		public int MaximumCapacity = 1300;

		/// <summary>
		/// When the number of items in the list exceeds <see cref="MaximumCapacity"/>, the number of items
		/// will be truncated to this value.
		/// </summary>
		public int PreferredCapacity = 1000;

		// list of all event log entries
		private List<T> _list = new List<T>();

		// list of filtered event log entries
		private List<T> _filteredList;

		private readonly object _lockObject = new object();

		public event EventHandler ListChanged;

		public SynchronizationContext SynchronizationContext { get; set; }

		public bool BuildListRequired
		{
			// This is for indicating that a lengthy build list is
			// required after changing sort order or filter.
			get { return _filter != null && _filteredList == null; }
		}

		public int Count
		{
			get
			{
				lock (_lockObject)
				{
					if (_filter == null)
					{
						return _list.Count;
					}
					if (_filteredList == null)
					{
						_filteredList = CreateFilteredList();
					}
					return _filteredList.Count;
				}
			}
		}

		private Predicate<T> _filter;

		// No lock required
		public Predicate<T> Filter
		{
			get { return _filter; }
			set
			{
				bool raiseListChangedRequired = false;
				lock (_lockObject)
				{
					if (_filter != value)
					{
						_filter = value;
						_filteredList = null;
						raiseListChangedRequired = true;
					}
				}

				if (raiseListChangedRequired)
				{
					RaiseListChanged();
				}
			}
		}

		/// <summary>
		/// Callback for determining whether the event log entry has a log level of debug or lower.
		/// These event log entries will be removed prior to any other event log entries.
		/// </summary>
		public Predicate<T> IsDebugCallback { get; set; }

		public Comparison<T> Comparer { get; set; }

		public bool CanSort
		{
			get { return false; }
		}


		public void Clear()
		{
			lock (_lockObject)
			{
				_list.Clear();
				if (_filteredList != null)
				{
					_filteredList.Clear();
				}
			}
		}

		public T GetAt(int index)
		{
			lock (_lockObject)
			{
				List<T> list;
				if (_filter == null)
				{
					list = _list;
				}
				else
				{
					if (_filteredList == null)
					{
						_filteredList = CreateFilteredList();
					}
					list = _filteredList;
				}

				if (index >= list.Count)
				{
					return null;
				}
				try
				{
					return list[index];
				}
				catch (ArgumentOutOfRangeException)
				{
					// This used to happen before this class had a lock because of race conditions.
					// Should not happen anymore, but the code remains.
					return null;
				}
			}
		}

		public void ForEach(Action<T> action)
		{
			lock (_lockObject)
			{
				if (_filter == null)
				{
					_list.ForEach(action);
				}
				else
				{
					if (_filteredList == null)
					{
						_filteredList = CreateFilteredList();
					}
					_filteredList.ForEach(action);
				}
			}
		}

		public void Add(T item)
		{
			bool raiseListChangedRequired = false;

			lock (_lockObject)
			{
				// Because removing items from the front of the array is an O(N) operation
				// we only do it when the list gets to its maximum threshold. Then we remove
				// a decent portion of the list in one go.
				if (_list.Count >= MaximumCapacity)
				{
					var callback = IsDebugCallback;
					if (callback != null)
					{
						// number of items that need removing
						int removeRequired = _list.Count - PreferredCapacity;

						// number of items that have been removed
						int removeCount = 0;

						var list = new List<T>();
						foreach (var i in _list)
						{
							if (removeCount < removeRequired && callback(i))
							{
								removeCount++;
							}
							else
							{
								list.Add(i);
							}
						}
						_list = list;
					}

					// If there are still too many, then start removing from the beginning
					if (_list.Count >= MaximumCapacity)
					{
						_list.RemoveRange(0, _list.Count - PreferredCapacity);
					}
					raiseListChangedRequired = true;
					_filteredList = null;
				}

				_list.Add(item);
				if (_filter == null)
				{
					// no filter
					raiseListChangedRequired = true;
				}
				else
				{
					if (_filteredList == null)
					{
						_filteredList = CreateFilteredList();
					}
					if (_filter(item))
					{
						_filteredList.Add(item);
						raiseListChangedRequired = true;
					}
				}
			}

			if (raiseListChangedRequired)
			{
				RaiseListChanged();
			}
		}

		private void RaiseListChanged()
		{
			if (ListChanged != null)
			{
				if (SynchronizationContext == null)
				{
					ListChanged(this, EventArgs.Empty);
				}
				else
				{
					SynchronizationContext.Post(ListChangedSendOrPostCallback, null);
				}
			}
		}

		private void ListChangedSendOrPostCallback(object state)
		{
			ListChanged(this, EventArgs.Empty);
		}

		private List<T> CreateFilteredList()
		{
			return _list.Where(item => _filter(item)).ToList();
		}
	}
}