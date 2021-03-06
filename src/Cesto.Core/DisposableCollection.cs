﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cesto
{
	/// <summary>
	/// A collection of <see cref="IDisposable"/> objects. The collection implements <see cref="IDisposable"/>
	/// and when its <see cref="Dispose"/> method is called, it calls <see cref="Dispose"/> on all the items
	/// in the collection.
	/// </summary>
	/// <remarks>
	/// When the collection is disposed of, it clears all items in the collection after disposing of them.
	/// Attempting to add another item after the collection has been disposed will result in an
	/// <see cref="ObjectDisposedException"/>
	/// </remarks>
	/// <exception cref="ObjectDisposedException">
	/// Thrown if any attempt has been made to add another item to the collection after it has
	/// been disposed.
	/// </exception>
	public class DisposableCollection : Collection<IDisposable>, IDisposable
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public DisposableCollection() {}

		/// <summary>
		/// Construct a <see cref="DisposableCollection"/> and populate from an existing collection.
		/// </summary>
		/// <param name="list">List of <see cref="IDisposable"/> objects.</param>
		public DisposableCollection(IEnumerable<IDisposable> list)
		{
			foreach (var disposable in list)
			{
				Add(disposable);
			}
		}

		/// <summary>
		/// Indicates whether the collection has been disposed yet.
		/// </summary>
		public bool IsDisposed { get; private set; }

		public void Dispose()
		{
			if (!IsDisposed)
			{
				foreach (var disposable in this)
				{
					if (disposable != null)
					{
						try
						{
							disposable.Dispose();
						}
						catch (ObjectDisposedException)
						{
							// This should not happen, as Dispose should be able to be
							// called multiple times. Some implementations do not allow this
							// so catch the exception and ignore.
						}
					}
				}

				Clear();
				IsDisposed = true;
			}
		}

		protected override void InsertItem(int index, IDisposable item)
		{
			CheckDisposed();
			base.InsertItem(index, item);
		}

		// Note: no need to override RemoveItem or SetItem, because the collection
		// is empty after it is disposed. RemoveItem or SetItem will never be called
		// for an empty collection, and it is not possible to add items.

		private void CheckDisposed()
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException("DisposableCollection");
			}
		}
	}
}
