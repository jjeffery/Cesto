#region License

// Copyright 2004-2013 John Jeffery <john@jeffery.id.au>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Cesto.WinForms
{
	/// <summary>
	///     A virtual data source optimised for displaying larger event log displays. It
	///     does not provide any sorting functionality.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class EventLoggingDataSource<T> : IVirtualDataSource<T> where T : class
	{
		/// <summary>
		///     When number of items in the list exceeds this value, the list will be truncated to the number
		///     of items specified by <see cref="PreferredCapacity" />.
		/// </summary>
		public int MaximumCapacity = 1300;

		/// <summary>
		///     When the number of items in the list exceeds <see cref="MaximumCapacity" />, the number of items
		///     will be truncated to this value.
		/// </summary>
		public int PreferredCapacity = 1000;

		// list of all event log entries
		private List<T> _list = new List<T>();

		// list of filtered event log entries
		private List<T> _filteredList;

		private readonly object _lockObject = new object();

		/// <summary>
		///     Create an new <see cref="EventLoggingDataSource{T}" /> object.
		/// </summary>
		public EventLoggingDataSource()
		{
			SynchronizationContext = SynchronizationContext.Current;
		}

		/// <summary>
		///     This event is raised when the contents of the data source have changed.
		/// </summary>
		public event EventHandler ListChanged;

		/// <summary>
		///     If this property is not null, then the <see cref="ListChanged" /> event will be raised
		///     via the <see cref="SynchronizationContext" />.
		/// </summary>
		public SynchronizationContext SynchronizationContext { get; set; }

		/// <summary>
		///     <see cref="IVirtualDataSource{T}.BuildListRequired" />.
		/// </summary>
		public bool BuildListRequired
		{
			// This is for indicating that a lengthy build list is
			// required after changing sort order or filter.
			get { return _filter != null && _filteredList == null; }
		}

		/// <summary>
		///     <see cref="IVirtualDataSource{T}.Count" />
		/// </summary>
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

		/// <summary>
		///     <see cref="IVirtualDataSource{T}.Filter" />
		/// </summary>
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
		///     Callback for determining whether the event log entry has a log level of debug or lower.
		///     These event log entries will be removed prior to any other event log entries.
		/// </summary>
		public Predicate<T> IsDebugCallback { get; set; }

		/// <summary>
		///     <see cref="IVirtualDataSource{T}.Comparer" />
		/// </summary>
		public Comparison<T> Comparer { get; set; }

		/// <summary>
		///     <see cref="IVirtualDataSource{T}.CanSort" />
		/// </summary>
		public bool CanSort
		{
			get { return false; }
		}

		/// <summary>
		///     Clear all items from the data source.
		/// </summary>
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

		/// <summary>
		///     <see cref="IVirtualDataSource{T}.GetAt" />
		/// </summary>
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

		/// <summary>
		///     <see cref="IVirtualDataSource{T}.ForEach" />
		/// </summary>
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

		/// <summary>
		///     Add an item to the data source.
		/// </summary>
		/// <param name="item">
		///     The item to add to the data source.
		/// </param>
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
			var listChanged = ListChanged;
			if (listChanged != null)
			{
				listChanged(this, EventArgs.Empty);
			}
		}

		private List<T> CreateFilteredList()
		{
			return _list.Where(item => _filter(item)).ToList();
		}
	}
}
