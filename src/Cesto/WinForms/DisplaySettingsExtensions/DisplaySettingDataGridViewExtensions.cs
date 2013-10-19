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

// ReSharper disable CheckNamespace
namespace Cesto.WinForms
{
	public static class DisplaySettingsDataGridViewExtensions
	{
		/// <summary>
		/// Save the width of a <see cref="DataGridViewColumn"/>. The name of the value
		/// is derived from the name of the <see cref="DataGridViewColumn"/>.
		/// </summary>
		/// <param name="settings">
		/// The <see cref="DisplaySettings"/> that will persist the column width.
		/// </param>
		/// <param name="column">The <see cref="DataGridViewColumn"/> whose width is to be persisted.</param>
		public static void SaveColumnWidth(this DisplaySettings settings, DataGridViewColumn column)
		{
			Verify.ArgumentNotNull(settings, "settings");
			Verify.ArgumentNotNull(column, "column");

			if (column.AutoSizeMode == DataGridViewAutoSizeColumnMode.None
				|| column.AutoSizeMode == DataGridViewAutoSizeColumnMode.NotSet)
			{
				string valueName = column.Name + ".ColumnWidth";
				int value = column.Width;
				settings.SetInt32(valueName, value);
			}
		}

		/// <summary>
		/// Save the widths of all columns in a <see cref="DataGridView"/>. The name of the values
		/// are derived from the names of the individual <see cref="DataGridViewColumn"/> objects.
		/// </summary>
		/// <param name="settings">
		/// The <see cref="DisplaySettings"/> that will persist the column width.
		/// </param>
		/// <param name="dataGridView">The <see cref="DataGridView"/> whose column widths are to be persisted.</param>
		public static void SaveColumnWidths(this DisplaySettings settings, DataGridView dataGridView)
		{
			Verify.ArgumentNotNull(settings, "settings");
			Verify.ArgumentNotNull(dataGridView, "dataGridView");

			foreach (DataGridViewColumn column in dataGridView.Columns)
			{
				settings.SaveColumnWidth(column);
			}
		}

		/// <summary>
		/// Load the widths of all columns in a <see cref="DataGridView"/>. The name of the values
		/// are derived from the names of the individual <see cref="DataGridViewColumn"/> objects.
		/// </summary>
		/// <param name="settings">
		/// The <see cref="DisplaySettings"/> that will persist the column width.
		/// </param>
		/// <param name="dataGridView">The <see cref="DataGridView"/> whose column widths are to be loaded.</param>
		public static void LoadColumnWidths(this DisplaySettings settings, DataGridView dataGridView)
		{
			Verify.ArgumentNotNull(settings, "settings");
			Verify.ArgumentNotNull(dataGridView, "dataGridView");

			foreach (DataGridViewColumn column in dataGridView.Columns)
			{
				if (column.AutoSizeMode == DataGridViewAutoSizeColumnMode.None
					|| column.AutoSizeMode == DataGridViewAutoSizeColumnMode.NotSet)
				{
					string valueName = column.Name + ".ColumnWidth";
					int value = settings.GetInt32(valueName, -1);
					if (value >= column.MinimumWidth)
					{
						column.Width = value;
					}
				}
			}
		}
	}
}