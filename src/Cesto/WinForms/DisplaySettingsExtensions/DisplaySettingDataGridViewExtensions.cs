using System.Windows.Forms;

// ReSharper disable CheckNamespace
namespace Cesto.WinForms
{
	public static class DisplaySettingsDataGridViewExtensions
	{
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

		public static void SaveColumnWidths(this DisplaySettings settings, DataGridView dataGridView)
		{
			Verify.ArgumentNotNull(settings, "settings");
			Verify.ArgumentNotNull(dataGridView, "dataGridView");

			foreach (DataGridViewColumn column in dataGridView.Columns)
			{
				settings.SaveColumnWidth(column);
			}
		}

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