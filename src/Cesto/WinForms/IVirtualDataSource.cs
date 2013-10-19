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

namespace Cesto.WinForms
{
	/// <summary>
	///     Data source for a data grid view in virtual mode.
	/// </summary>
	/// <typeparam name="TItem">
	///     Type of object displayed in the data grid view.
	/// </typeparam>
	public interface IVirtualDataSource<TItem> where TItem : class
	{
		/// <summary>
		///     Event that is raised when the list has changed.
		/// </summary>
		/// <remarks>
		///     This event will be raised on the same thread that
		///     modified the underlying list.
		/// </remarks>
		event EventHandler ListChanged;

		/// <summary>
		///     Indicates that the underlying data structure list needs to be rebuilt.
		///     This means that the next call to <see cref="GetAt" /> or <see cref="ForEach" />
		///     will take longer.
		/// </summary>
		/// <remarks>
		///     The original intent was to allow a wait cursor to be displayed if a build list
		///     is required, but in practice it has not been of much use.
		/// </remarks>
		bool BuildListRequired { get; }

		/// <summary>
		///     Number of items in the underlying list.
		/// </summary>
		int Count { get; }

		/// <summary>
		///     Filters items from the list.
		/// </summary>
		Predicate<TItem> Filter { get; set; }

		/// <summary>
		///     Specifies whether the underlying data source can be sorted.
		/// </summary>
		bool CanSort { get; }

		/// <summary>
		///     Comparer for sorting items in the list.
		/// </summary>
		/// <remarks>
		///     If <see cref="CanSort" /> is <c>false</c>, then setting this
		///     property has no effect.
		/// </remarks>
		Comparison<TItem> Comparer { get; set; }

		/// <summary>
		///     Gets the item from the list at the specified index.
		/// </summary>
		/// <param name="index">Zero based index</param>
		/// <returns>
		///     Item at the specified index. If the index is out of range, returns null.
		/// </returns>
		TItem GetAt(int index);

		/// <summary>
		///     Iterate through each item in the list.
		/// </summary>
		/// <param name="action">
		///     Action to be performed on each item in the list.
		/// </param>
		/// <remarks>
		///     The underlying list is locked for update during this method.
		/// </remarks>
		void ForEach(Action<TItem> action);
	}
}