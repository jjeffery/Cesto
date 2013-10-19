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

using System.Windows.Forms;

namespace Cesto.WinForms
{
	/// <summary>
	///     A callback for setting the checked status of a <see cref="DataGridViewCell" />.
	/// </summary>
	/// <typeparam name="T">
	///     The type of object in the associated <see cref="IVirtualDataSource{T}" />
	/// </typeparam>
	/// <param name="obj">
	///     The object in the virtual data source associated with the <see cref="DataGridViewRow" />
	///     that the <see cref="DataGridViewCell" /> belongs to.
	/// </param>
	/// <param name="isChecked">
	///     The check status. Either checked or unchecked.
	/// </param>
	public delegate void DataGridViewCellCheckCallback<in T>(T obj, bool isChecked);
}