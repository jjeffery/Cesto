using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Cesto.WinForms
{
	public delegate object DataGridViewCellValueCallback<in T>(T obj);

	public delegate void DataGridViewCellCheckCallback<in T>(T obj, bool isChecked);

	public class VirtualDataGridViewAdapter<T> : IDisposable where T : class
	{
		private readonly Dictionary<int, ColumnInfo> _columnInfos = new Dictionary<int, ColumnInfo>();

		private DataGridViewColumn[] _defaultSortOrder;
		private DataGridViewColumn _sortColumn;
		private SortOrder _sortOrder = SortOrder.None;
		private bool _isDisposed;
		private Comparison<T> _sortComparison;
		private bool _refreshPending;
		private ColumnInfo _checkColumn;

		public event EventHandler ListChanged;

		public DataGridView DataGridView { get; private set; }
		public DisplaySettings DisplaySettings { get; set; }
		public IVirtualDataSource<T> DataSource { get; set; }

		public Image CheckedImage { get; set; }
		public Image UncheckedImage { get; set; }
		public Image IndeterminateImage { get; set; }

		public VirtualDataGridViewAdapter(DataGridView dataGridView)
		{
			DataGridView = Verify.ArgumentNotNull(dataGridView, "dataGridView");
			DataGridView.VirtualMode = true;
		}

		public VirtualDataGridViewAdapter<T> Init(IVirtualDataSource<T> store)
		{
			if (DataSource != null)
			{
				throw new InvalidOperationException("DataSource has already been set");
			}
			DataSource = Verify.ArgumentNotNull(store, "store");
			DataSource.ListChanged += ((sender, args) => RefreshRequired());

			if (DisplaySettings != null)
			{
				DisplaySettings.LoadColumnWidths(DataGridView);
			}

			// Subscribe for events only after the store has been set, because these
			// event handlers assume that Store != null.
			DataGridView.CellValueNeeded += DataGridView_CellValueNeeded;
			DataGridView.ColumnHeaderMouseClick += DataGridView_ColumnHeaderMouseClick;
			DataGridView.CellContentClick += DataGridView_CellContentClick;
			DataGridView.CellPainting += DataGridViewCellPainting;
			DataGridView.ColumnWidthChanged += DataGridView_ColumnWidthChanged;
			DataGridView.HandleCreated += DataGridView_HandleCreated;

			WaitCursor.Changed += HandleWaitCursorHidden;

			DataGridView.Disposed += (sender, args) => Dispose();
			DataSource.Comparer = _sortComparison;

			RefreshRequired();
			return this;
		}

		public void Dispose()
		{
			_isDisposed = true;
			WaitCursor.Changed -= HandleWaitCursorHidden;
			ListChanged = null;
		}

		public bool IsDisposed
		{
			get { return _isDisposed; }
		}

		public T Current
		{
			get
			{
				if (DataSource == null || DataGridView.CurrentRow == null)
				{
					return null;
				}
				return DataSource.GetAt(DataGridView.CurrentRow.Index);
			}
		}

		public VirtualDataGridViewAdapter<T> SetDefaultSortOrder(params DataGridViewColumn[] columns)
		{
			_defaultSortOrder = (DataGridViewColumn[]) columns.Clone();
			return this;
		}

		public VirtualDataGridViewAdapter<T> DefineCellValue(DataGridViewColumn column,
		                                                     DataGridViewCellValueCallback<T> callback)
		{
			Verify.ArgumentNotNull(column, "column");
			Verify.ArgumentNotNull(callback, "callback");
			VerifyColumnBelongsToGridView(column);
			GetColumnInfo(column.Index).CellValueCallback = callback;
			return this;
		}

		public VirtualDataGridViewAdapter<T> SetCheckColumn(DataGridViewColumn column,
		                                                    DataGridViewCellCheckCallback<T> callback)
		{
			Verify.ArgumentNotNull(column, "column");
			Verify.ArgumentNotNull(callback, "callback");
			VerifyColumnBelongsToGridView(column);
			var columnInfo = GetColumnInfo(column.Index);
			_checkColumn = columnInfo;
			columnInfo.CellCheckCallback = callback;
			columnInfo.HeaderCheckState = CheckState.Unchecked;
			column.SortMode = DataGridViewColumnSortMode.NotSortable;
			return this;
		}

		public VirtualDataGridViewAdapter<T> SetLinkColumn(DataGridViewColumn column)
		{
			Verify.ArgumentNotNull(column, "column");
			VerifyColumnBelongsToGridView(column);
			var columnInfo = GetColumnInfo(column.Index);
			columnInfo.IsLinkColumn = true;
			return this;
		}

		public VirtualDataGridViewAdapter<T> WithDisplaySettings(DisplaySettings displaySettings)
		{
			DisplaySettings = displaySettings;
			return this;
		}

		public VirtualDataGridViewAdapter<T> SortBy(DataGridViewColumn sortColumn)
		{
			Verify.ArgumentNotNull(sortColumn, "sortColumn");
			VerifyColumnBelongsToGridView(sortColumn);
			var columnInfo = GetColumnInfo(sortColumn.Index);
			if (columnInfo.CellValueCallback == null)
			{
				// cannot sort if this column has not value callback
				return this;
			}

			var sortCallbacks = new List<DataGridViewCellValueCallback<T>> {columnInfo.CellValueCallback};
			if (_defaultSortOrder != null)
			{
				foreach (var column in _defaultSortOrder)
				{
					if (column.Visible && column != sortColumn)
					{
						var ci = GetColumnInfo(column.Index);
						if (ci.CellValueCallback != null)
						{
							sortCallbacks.Add(ci.CellValueCallback);
						}
					}
				}
			}

			var comparer = new Comparer(sortCallbacks);

			SortOrder sortOrder;
			if (sortColumn == _sortColumn)
			{
				// the same column has been pressed twice, so toggle the sort order
				sortOrder = _sortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
			}
			else
			{
				sortOrder = SortOrder.Ascending;

				// clear out any sort glyph in the previous sort column
				if (_sortColumn != null)
				{
					_sortColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
				}
			}
			_sortOrder = sortOrder;
			_sortColumn = sortColumn;
			_sortColumn.HeaderCell.SortGlyphDirection = _sortOrder;

			if (sortOrder == SortOrder.Ascending)
			{
				_sortComparison = comparer.CompareAscending;
			}
			else
			{
				_sortComparison = comparer.CompareDescending;
			}

			if (DataSource != null)
			{
				DataSource.Comparer = _sortComparison;
				RefreshRequired();
			}
			return this;
		}

		private void RefreshRequired()
		{
			if (!DataGridView.IsHandleCreated)
			{
				// If the data grid view is not created yet it is going to draw itself
				// when it is first created.
				return;
			}

			// Many calls to this method will result in one refresh request occuring.
			if (_refreshPending)
			{
				return;
			}
			_refreshPending = true;

			DataGridView.BeginInvoke(new Action(DoRefresh));
		}

		private void DoRefresh()
		{
			_refreshPending = false;

			if (DataSource == null)
			{
				DataGridView.RowCount = 0;
			}
			else if (DataGridView.RowCount != DataSource.Count)
			{
				int currentRowIndex = -1;
				int currentColumnIndex = -1;

				// Need to do this to get around a suspected bug in DataGridView.
				// If number of rows is reduced, need to set rows to zero first
				// and then subsequently set to the correct value, otherwise DGV
				// throws an exception.
				if (DataGridView.RowCount > DataSource.Count)
				{
					if (DataGridView.CurrentCell != null)
					{
						currentRowIndex = DataGridView.CurrentCell.RowIndex;
						currentColumnIndex = DataGridView.CurrentCell.ColumnIndex;
					}
					try
					{
						DataGridView.RowCount = 0;
					}
					catch (ArgumentOutOfRangeException)
					{}
				}
				try
				{
					DataGridView.RowCount = DataSource.Count;
				}
				catch (ArgumentOutOfRangeException)
				{}

				// Have a go at finding the current cell again.
				if (currentRowIndex >= DataGridView.RowCount)
				{
					currentRowIndex = DataGridView.RowCount - 1;
				}

				if (currentRowIndex >= 0)
				{
					DataGridView.CurrentCell = DataGridView.Rows[currentRowIndex].Cells[currentColumnIndex];
				}
			}

			foreach (var columnInfo in _columnInfos.Values)
			{
				if (columnInfo.CellCheckCallback != null)
				{
					SetHeaderCheckbox(columnInfo);
				}
			}

			DataGridView.Invalidate();

			if (ListChanged != null)
			{
				if (DataGridView.IsHandleCreated)
				{
					DataGridView.BeginInvoke(new Action(RaiseListChanged));
				}
				else
				{
					RaiseListChanged();
				}
			}
		}

		private void RaiseListChanged()
		{
			if (ListChanged != null)
			{
				ListChanged(this, EventArgs.Empty);
			}
		}

		private void VerifyColumnBelongsToGridView(DataGridViewColumn column)
		{
			if (column.DataGridView != DataGridView)
			{
				throw new ArgumentException("Column does not belong to the DataGridView");
			}
		}

		private void SetHeaderCheckbox(ColumnInfo columnInfo)
		{
			columnInfo.CheckCount = 0;
			columnInfo.UncheckCount = 0;

			var callback = columnInfo.CellValueCallback;

			// Build a delegate that can be called for every visible item in the store
			Action<T> action = delegate(T item) {
				bool value = Convert.ToBoolean(callback(item));

				if (value)
				{
					columnInfo.CheckCount++;
				}
				else
				{
					columnInfo.UncheckCount++;
				}
			};

			DataSource.ForEach(action);

			CheckState checkState = CheckState.Unchecked;

			if (columnInfo.CheckCount > 0 && columnInfo.UncheckCount > 0)
			{
				checkState = CheckState.Indeterminate;
			}
			else if (columnInfo.CheckCount > 0)
			{
				checkState = CheckState.Checked;
			}

			columnInfo.HeaderCheckState = checkState;
			DataGridView.InvalidateCell(columnInfo.ColumnIndex, -1);
		}

		private void DataGridView_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
		{
			if (DisplaySettings != null)
			{
				DisplaySettings.SaveColumnWidth(e.Column);
			}
		}

		/// <summary>
		///     Toggle the checkbox in the specified column at the specified row
		/// </summary>
		/// <param name="columnInfo"></param>
		/// <param name="rowIndex"></param>
		private void ToggleCheckBox(ColumnInfo columnInfo, int rowIndex)
		{
			var item = DataSource.GetAt(rowIndex);
			if (item != null)
			{
				var getValueCallback = columnInfo.CellValueCallback;
				var setValueCallback = columnInfo.CellCheckCallback;
				bool checkValue = Convert.ToBoolean(getValueCallback(item));

				if (checkValue)
				{
					columnInfo.CheckCount--;
					columnInfo.UncheckCount++;
					setValueCallback(item, false);
				}
				else
				{
					columnInfo.UncheckCount--;
					columnInfo.CheckCount++;
					setValueCallback(item, true);
				}

				CheckState checkState = CheckState.Unchecked;

				if (columnInfo.CheckCount > 0 && columnInfo.UncheckCount > 0)
				{
					checkState = CheckState.Indeterminate;
				}
				else if (columnInfo.CheckCount > 0)
				{
					checkState = CheckState.Checked;
				}

				if (columnInfo.HeaderCheckState != checkState)
				{
					columnInfo.HeaderCheckState = checkState;
					// need to redraw the header cell
					DataGridView.InvalidateCell(columnInfo.ColumnIndex, -1);
				}

				DataGridView.BeginInvoke(new Action(() => DataGridView.InvalidateCell(columnInfo.ColumnIndex, rowIndex)));
			}
		}

		private void DataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			var columnInfo = GetColumnInfo(e.ColumnIndex);
			if (columnInfo != null)
			{
				if (columnInfo.IsCheckBoxColumn)
				{
					ToggleCheckBox(columnInfo, e.RowIndex);
				}
				else
				{
					if (_checkColumn != null && !columnInfo.IsLinkColumn)
					{
						ToggleCheckBox(_checkColumn, e.RowIndex);
					}
				}
			}
		}

		private void DataGridViewCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
		{
			if (e.RowIndex == -1)
			{
				var columnInfo = GetColumnInfo(e.ColumnIndex);
				if (columnInfo.CellCheckCallback != null)
				{
					try
					{
						//Determine paint coordinates
						int x = e.CellBounds.Left +
						        ((e.CellBounds.Width - CheckedImage.Width)) - 4;
						int y = e.CellBounds.Top +
						        ((e.CellBounds.Height - CheckedImage.Height)/2) - 1;

						switch (columnInfo.HeaderCheckState)
						{
							case CheckState.Checked:
								e.Graphics.DrawImage(CheckedImage, x, y);
								break;
							case CheckState.Unchecked:
								e.Graphics.DrawImage(UncheckedImage, x, y);
								break;
							case CheckState.Indeterminate:
								e.Graphics.DrawImage(IndeterminateImage, x, y);
								break;
						}

						e.Handled = true;
					}
					catch
					{
						// TODO: Handle exception
					}
				}
			}
		}

		private void DataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			var columnInfo = GetColumnInfo(e.ColumnIndex);
			if (columnInfo.CellValueCallback == null)
			{
				// if we don't know how to get the value, we cannot sort
				// or manipulate check boxes
				return;
			}

			if (columnInfo.IsCheckBoxColumn)
			{
				bool newCheckValue = columnInfo.HeaderCheckState == CheckState.Unchecked;
				Action<T> checkAction = obj => columnInfo.CellCheckCallback(obj, newCheckValue);
				DataSource.ForEach(checkAction);
				if (newCheckValue)
				{
					columnInfo.CheckCount = DataSource.Count;
					columnInfo.UncheckCount = 0;
				}
				else
				{
					columnInfo.CheckCount = 0;
					columnInfo.UncheckCount = DataSource.Count;
				}
				columnInfo.HeaderCheckState = newCheckValue ? CheckState.Checked : CheckState.Unchecked;
				DataGridView.BeginInvoke(new Action(() => DataGridView.InvalidateColumn(e.ColumnIndex)));

				foreach (DataGridViewCell cell in DataGridView.SelectedCells)
				{
					cell.Selected = false;
				}

				DataGridView.CurrentCell = null;
			}
			else
			{
				var sortColumn = DataGridView.Columns[e.ColumnIndex];
				if (sortColumn.SortMode != DataGridViewColumnSortMode.NotSortable)
				{
					SortBy(sortColumn);
				}
			}
		}

		private void DataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
		{
			var columnInfo = GetColumnInfo(e.ColumnIndex);
			if (columnInfo.CellValueCallback != null)
			{
				T obj;

				if (DataSource.BuildListRequired)
				{
					using (WaitCursor.Show())
					{
						obj = DataSource.GetAt(e.RowIndex);
					}
				}
				else
				{
					obj = DataSource.GetAt(e.RowIndex);
				}
				if (obj != null)
				{
					e.Value = columnInfo.CellValueCallback(obj);
				}
			}
		}

		private void DataGridView_HandleCreated(object sender, EventArgs e)
		{
			// This callback is necessary to kick off the redraw process in the event
			// that the store is populated with data prior to the view being loaded.
			RefreshRequired();
		}

		// Gets around a known limitation of the DataGridView, where it can get stuck with a wait cursor.
		private void HandleWaitCursorHidden(object sender, EventArgs args)
		{
			if (DataGridView != null && !DataGridView.IsDisposed && !WaitCursor.Visible)
			{
				Control parent = DataGridView.Parent;
				if (parent != null && !parent.IsDisposed)
					DataGridView.Cursor = parent.Cursor;
			}
		}

		private ColumnInfo GetColumnInfo(int columnIndex)
		{
			ColumnInfo columnInfo;
			if (!_columnInfos.TryGetValue(columnIndex, out columnInfo))
			{
				columnInfo = new ColumnInfo(columnIndex);
				_columnInfos.Add(columnIndex, columnInfo);
			}
			return columnInfo;
		}

		private class ColumnInfo
		{
			public ColumnInfo(int columnIndex)
			{
				ColumnIndex = columnIndex;
			}

			public readonly int ColumnIndex;
			public DataGridViewCellValueCallback<T> CellValueCallback;
			public DataGridViewCellCheckCallback<T> CellCheckCallback;
			public CheckState HeaderCheckState;
			public int CheckCount;
			public int UncheckCount;
			public bool IsLinkColumn;

			public bool IsCheckBoxColumn
			{
				get { return CellCheckCallback != null; }
			}
		}

		private class Comparer
		{
			private readonly IEnumerable<DataGridViewCellValueCallback<T>> _callbacks;

			public Comparer(IEnumerable<DataGridViewCellValueCallback<T>> callbacks)
			{
				_callbacks = callbacks;
			}

			public int CompareAscending(T obj1, T obj2)
			{
				foreach (var callback in _callbacks)
				{
					object v1 = callback(obj1);
					object v2 = callback(obj2);
					int result = CompareValues(v1, v2);
					if (result != 0)
					{
						return result;
					}
				}
				return 0;
			}

			public int CompareDescending(T obj1, T obj2)
			{
				return -CompareAscending(obj1, obj2);
			}

			private static int CompareValues(object v1, object v2)
			{
				if (v1 == null)
				{
					if (v2 == null)
					{
						return 0;
					}
					return -1;
				}

				if (v2 == null)
				{
					return 1;
				}

				if (!v1.GetType().IsInstanceOfType(v2))
				{
					v1 = v1.ToString();
					v2 = v2.ToString();
				}

				IComparable c1 = (v1 as IComparable) ?? v1.ToString();
				IComparable c2 = (v2 as IComparable) ?? v2.ToString();

				int result = c1.CompareTo(c2);
				return result;
			}
		}
	}
}