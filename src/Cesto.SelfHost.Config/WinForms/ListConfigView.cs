using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cesto.Config;

namespace Cesto.WinForms
{
	public partial class ListConfigView : UserControl
	{
		private readonly VirtualDataGridViewAdapter<IConfigParameter> _adapter;
		private readonly DisplaySettings _displaySettings = new DisplaySettings(typeof(ListConfigView));
		private readonly VirtualDataSource<string, IConfigParameter> _dataSource = new VirtualDataSource<string, IConfigParameter>(p => p.Name); 

		public ListConfigView()
		{
			InitializeComponent();
			_adapter = new VirtualDataGridViewAdapter<IConfigParameter>(DataGridView)
				.WithDisplaySettings(_displaySettings)
				.DefineCellValue(NameColumn, p => p.Name)
				.DefineCellValue(ParameterTypeColumn, p => p.ParameterType)
				.DefineCellValue(ValueColumn, p => p.GetDisplayText())
				.DefineCellValue(DescriptionColumn, p => p.Summary)
				.SetDefaultSortOrder(NameColumn)
				.SortBy(NameColumn);
			_adapter.DataSource = _dataSource;

			Load += delegate
			{
				BeginInvoke(new Action(() => SearchTextBox.Focus()));
				DataGridView.ClearSelection();
				DataGridView.CurrentCell = null;
				_dataSource.AddRange(ConfigParameter.All);
			};
		}

		private void ClearSearchTextButton_Click(object sender, EventArgs e)
		{
			SearchTextBox.Text = string.Empty;
		}

		private void SearchTextBox_TextChanged(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
			{
				_adapter.DataSource.Filter = null;
			}
			else
			{
				_adapter.DataSource.Filter = new Filter(SearchTextBox.Text).Apply;
			}
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			if (keyData == Keys.Enter)
			{
				EditButton.PerformClick();
				return true;
			}
			if (keyData == Keys.Escape)
			{
				ClearSearchTextButton.PerformClick();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}

		private class Filter
		{
			private readonly string _text;

			public Filter(string text)
			{
				_text = (text ?? string.Empty).Trim().ToLowerInvariant();
			}

			public bool Apply(IConfigParameter configParameter)
			{
				return configParameter.Name.ToLowerInvariant().Contains(_text) ||
					   configParameter.Description.ToLowerInvariant().Contains(_text);
			}
		}

		private void DataGridView_MoveUp(object sender, EventArgs e)
		{
			DataGridView.ClearSelection();
			DataGridView.CurrentCell = null;
			SearchTextBox.Focus();
		}

		private void DataGridView_EnterKeyPressed(object sender, EventArgs e)
		{
			EditConfigParameter();
		}

		private void SearchTextBox_DownKeyPressed(object sender, EventArgs e)
		{
			DataGridView.Focus();
			if (DataGridView.SelectedRows.Count > 0 && DataGridView.SelectedRows[0].Selected)
				SelectGridRow(1); // since first row is already highlighted
			else
				SelectGridRow(0);
		}

		public void SelectGridRow(int rowIndex)
		{
			// If row is greater than number of rows then set to the last row.
			if (rowIndex >= DataGridView.Rows.Count)
				rowIndex = DataGridView.Rows.Count - 1;

			if (rowIndex < 0)
			{
				DataGridView.ClearSelection();
				DataGridView.CurrentCell = null;
			}
			else
			{
				DataGridView.Rows[rowIndex].Selected = true;
				DataGridView.CurrentCell = DataGridView[0, rowIndex];
			}
		}

		private void EditButton_Click(object sender, EventArgs e)
		{
			EditConfigParameter();
		}

		private void EditConfigParameter()
		{
			var dialog = new EditParameterDialog {ConfigParameter = _adapter.Current};
			if (dialog.ShowDialog(this) == DialogResult.OK)
			{
				_dataSource.Update(_adapter.Current);
			}
		}

		private void DataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex >= 0 && e.RowIndex < _adapter.DataSource.Count)
			{
				EditConfigParameter();
			}
		}

		private void RefreshButton_Click(object sender, EventArgs e)
		{
			_dataSource.ReplaceContents(ConfigParameter.All);
		}
	}
}
