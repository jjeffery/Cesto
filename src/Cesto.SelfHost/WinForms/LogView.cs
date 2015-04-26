using System;
using System.Windows.Forms;
using Cesto.Logging;

namespace Cesto.WinForms
{
	public partial class LogView : UserControl
	{
		private readonly DisplaySettings _displaySettings = new DisplaySettings(typeof (LogView));
		private readonly DisplaySetting<bool> _showDebugSetting;
		private readonly DisplaySetting<bool> _showLoggerSetting;
		private readonly IVirtualDataSource<LogUI.Event> _dataSource = LogUI.DataSource;

		public LogView()
		{
			InitializeComponent();
			var adapter = new VirtualDataGridViewAdapter<LogUI.Event>(dataGridView)
				.DefineCellValue(timestampColumn, l => l.Timestamp)
				.DefineCellValue(levelColumn, l => l.Level.ToString())
				.DefineCellValue(loggerColumn, l => l.Logger)
				.DefineCellValue(messageColumn, l => l.Message)
				.WithDisplaySettings(_displaySettings);

			adapter.DataSource = _dataSource;
			adapter.ListChanged += ListChanged;

			_showDebugSetting = _displaySettings.BooleanSetting("ShowDebug");
			showDebugCheckBox.Checked = _showDebugSetting.GetValue();

			_showLoggerSetting = _displaySettings.BooleanSetting("ShowLogger");
			showLoggerCheckBox.Checked = _showLoggerSetting.GetValue();
			loggerColumn.Visible = showLoggerCheckBox.Checked;

			SetFilter();

			dataGridView.Disposed += delegate {
				adapter.DataSource = null;
				adapter.ListChanged -= ListChanged;
			};
		}

		private void ListChanged(object sender, EventArgs args)
		{
			foreach (DataGridViewCell cell in dataGridView.SelectedCells)
			{
				cell.Selected = false;
			}

			// when the number of rows changes, select the last row
			if (dataGridView.RowCount > 0)
			{
				DataGridViewRow row = dataGridView.Rows[dataGridView.RowCount - 1];

				// setting the current cell ensures that the data grid view row is visible on the grid
				dataGridView.CurrentCell = row.Cells[0];

				// set row as the selected row selected
				row.Selected = true;
			}
		}

		private void ShowDebugCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			_showDebugSetting.SetValue(showDebugCheckBox.Checked);
			SetFilter();
		}

		private void SetFilter()
		{
			if (showDebugCheckBox.Checked)
			{
				_dataSource.Filter = null;
			}
			else
			{
				_dataSource.Filter = e => e.Level != LogUI.Level.Debug;
			}
		}

		private void ShowLoggerCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			loggerColumn.Visible = showLoggerCheckBox.Checked;
			_showLoggerSetting.SetValue(showLoggerCheckBox.Checked);
		}
	}
}
