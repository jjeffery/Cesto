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
using System.Drawing;
using System.Windows.Forms;

namespace Cesto.WinForms
{
    /// <summary>
    ///     This is an adapter that links a <see cref="DataGridView" /> with a <see cref="IVirtualDataSource{T}" />.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of objects in the <see cref="IVirtualDataSource{T}" />.
    /// </typeparam>
    public class VirtualDataGridViewAdapter<T> : IDisposable where T : class
    {
        private readonly Dictionary<int, ColumnInfo> _columnInfos = new Dictionary<int, ColumnInfo>();
        private ColumnInfo _checkColumn;
        private IVirtualDataSource<T> _dataSource;

        private DataGridViewColumn[] _defaultSortOrder;
        private bool _isDisposed;
        private bool _refreshPending;
        private DataGridViewColumn _sortColumn;
        private Comparison<T> _sortComparison;
        private SortOrder _sortOrder = SortOrder.None;

        /// <summary>
        ///     Creates a virtual data grid view adapter.
        /// </summary>
        /// <param name="dataGridView">
        ///     The <see cref="DataGridView" /> that this class controls.
        /// </param>
        public VirtualDataGridViewAdapter(DataGridView dataGridView)
        {
            DataGridView = Verify.ArgumentNotNull(dataGridView, "dataGridView");
            DataGridView.VirtualMode = true;
        }

        /// <summary>
        ///     This event is raised whenever the underlying virtual data grid view
        ///     has changed.
        /// </summary>
        public event EventHandler ListChanged;

        /// <summary>
        ///     The <see cref="DataGridView" /> associated with this adapter.
        /// </summary>
        public DataGridView DataGridView { get; private set; }

        /// <summary>
        ///     The <see cref="DisplaySettings" /> used to store persistent properties, such
        ///     as column widths.
        /// </summary>
        public DisplaySettings DisplaySettings { get; set; }

        /// <summary>
        ///     If the <see cref="DataGridView" /> is to display one or more check box columns,
        ///     this is the image displayed for a checked checkbox.
        /// </summary>
        public Image CheckedImage { get; set; }

        /// <summary>
        ///     If the <see cref="DataGridView" /> is to display one or more check box columns,
        ///     this is the image displayed for an unchecked checkbox.
        /// </summary>
        public Image UncheckedImage { get; set; }

        /// <summary>
        ///     If the <see cref="DataGridView" /> is to display one or more check box columns,
        ///     this is the image displayed for an indeterminate checkbox (ie neither checked
        ///     nor unchecked). This image is used in the column header when some rows are
        ///     checked, and some rows are unchecked.
        /// </summary>
        public Image IndeterminateImage { get; set; }

        /// <summary>
        ///     The virtual data source that supplies the data grid view.
        /// </summary>
        public IVirtualDataSource<T> DataSource
        {
            get { return _dataSource; }
            set
            {
                UnsubscribeEvents();
                _dataSource = value;
                SubscribeEvents();
                if (_dataSource != null)
                {
                    if (DisplaySettings != null)
                    {
                        DisplaySettings.LoadColumnWidths(DataGridView);
                    }
                    DataSource.Comparer = _sortComparison;
                }
                RefreshRequired();
            }
        }

        /// <summary>
        ///     As this object been disposed via the <see cref="Dispose" /> method.
        /// </summary>
        public bool IsDisposed
        {
            get { return _isDisposed; }
        }

        /// <summary>
        ///     The current object selected in the <see cref="DataGridView" />.
        /// </summary>
        /// <remarks>
        ///     Even if the data grid view is multi-select, one of the rows is the current row. This
        ///     property returns the associated object in the virtual data source associated with that
        ///     current row.
        /// </remarks>
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

        /// <summary>
        ///     <see cref="IDisposable.Dispose" />.
        /// </summary>
        public void Dispose()
        {
            _isDisposed = true;
            WaitCursor.Changed -= HandleWaitCursorHidden;
            ListChanged = null;
        }

        /// <summary>
        ///     Initialize the adapter with the data store.
        /// </summary>
        [Obsolete("Use DataSource property instead of Init(dataStore)")]
        public VirtualDataGridViewAdapter<T> Init(IVirtualDataSource<T> store)
        {
            DataSource = store;
            return this;
        }

        private void DataGridView_ListChanged(object sender, EventArgs args)
        {
            RefreshRequired();
        }

        private void DataGridView_Disposed(object sender, EventArgs args)
        {
            Dispose();
        }

        /// <summary>
        ///     Set the default sort order for the <see cref="DataGridView" />.
        /// </summary>
        /// <param name="columns">
        ///     The column or columns to be in the default sort order.
        /// </param>
        /// <returns>
        ///     This object. Useful for chaining methods.
        /// </returns>
        public VirtualDataGridViewAdapter<T> SetDefaultSortOrder(params DataGridViewColumn[] columns)
        {
            _defaultSortOrder = (DataGridViewColumn[]) columns.Clone();
            return this;
        }

        /// <summary>
        ///     Define a function to call to obtain the value to display in a cell of the <see cref="DataGridView" />.
        /// </summary>
        /// <param name="column">
        ///     The <see cref="DataGridViewColumn" /> in the <see cref="DataGridView" />.
        /// </param>
        /// <param name="callback">
        ///     A delegate to call to return the value to display in the data grid view cell.
        /// </param>
        /// <returns>
        ///     This object. Useful for chaining methods.
        /// </returns>
        public VirtualDataGridViewAdapter<T> DefineCellValue(DataGridViewColumn column,
                                                             DataGridViewCellValueCallback<T> callback)
        {
            Verify.ArgumentNotNull(column, "column");
            Verify.ArgumentNotNull(callback, "callback");
            VerifyColumnBelongsToGridView(column);
            GetColumnInfo(column.Index).CellValueCallback = callback;
            return this;
        }

        /// <summary>
        ///     Identifies a <see cref="DataGridViewColumn" /> as a check column.
        /// </summary>
        /// <param name="column">
        ///     The <see cref="DataGridViewColumn" /> in the <see cref="DataGridView" />.
        /// </param>
        /// <param name="callback">
        ///     A delegate callback that determines whether the check box in the
        ///     column should be checked or not.
        /// </param>
        /// <returns></returns>
        public VirtualDataGridViewAdapter<T> SetCheckColumn(DataGridViewColumn column,
                                                            DataGridViewCellCheckCallback<T> callback)
        {
            Verify.ArgumentNotNull(column, "column");
            Verify.ArgumentNotNull(callback, "callback");
            VerifyColumnBelongsToGridView(column);
            ColumnInfo columnInfo = GetColumnInfo(column.Index);
            _checkColumn = columnInfo;
            columnInfo.CellCheckCallback = callback;
            columnInfo.HeaderCheckState = CheckState.Unchecked;
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
            return this;
        }

        /// <summary>
        ///     This identifes the <see cref="DataGridViewColumn" /> as a linked column.
        ///     This doesn't do anything much yet. It might get removed.
        /// </summary>
        /// <returns>
        ///     This object. Useful for method chaining.
        /// </returns>
        public VirtualDataGridViewAdapter<T> SetLinkColumn(DataGridViewColumn column)
        {
            Verify.ArgumentNotNull(column, "column");
            VerifyColumnBelongsToGridView(column);
            ColumnInfo columnInfo = GetColumnInfo(column.Index);
            columnInfo.IsLinkColumn = true;
            return this;
        }

        /// <summary>
        ///     Convenience method for assigning the <see cref="DisplaySettings" /> property.
        ///     Useful for method chaining.
        /// </summary>
        /// <returns>
        ///     This object. Useful for method chaining.
        /// </returns>
        public VirtualDataGridViewAdapter<T> WithDisplaySettings(DisplaySettings displaySettings)
        {
            DisplaySettings = displaySettings;
            return this;
        }

        /// <summary>
        ///     Sort the <see cref="DataGridView" /> by the specified the <see cref="DataGridViewColumn" />.
        ///     The specified column will be the first column in the sort order. The remaining columns
        ///     in the sort order are defined by the <see cref="SetDefaultSortOrder" /> method.
        /// </summary>
        /// <returns>
        ///     This object. Useful for method chaining.
        /// </returns>
        public VirtualDataGridViewAdapter<T> SortBy(DataGridViewColumn sortColumn)
        {
            Verify.ArgumentNotNull(sortColumn, "sortColumn");
            VerifyColumnBelongsToGridView(sortColumn);
            ColumnInfo columnInfo = GetColumnInfo(sortColumn.Index);
            if (columnInfo.CellValueCallback == null)
            {
                // cannot sort if this column has not value callback
                return this;
            }

            var sortCallbacks = new List<DataGridViewCellValueCallback<T>> {columnInfo.CellValueCallback};
            if (_defaultSortOrder != null)
            {
                foreach (DataGridViewColumn column in _defaultSortOrder)
                {
                    if (column.Visible && column != sortColumn)
                    {
                        ColumnInfo ci = GetColumnInfo(column.Index);
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

        private void SubscribeEvents()
        {
            if (DataSource != null)
            {
                // Subscribe for events only after the store has been set, because these
                // event handlers assume that DataStore != null.
                DataSource.ListChanged += DataGridView_ListChanged;
                DataGridView.CellValueNeeded += DataGridView_CellValueNeeded;
                DataGridView.ColumnHeaderMouseClick += DataGridView_ColumnHeaderMouseClick;
                DataGridView.CellContentClick += DataGridView_CellContentClick;
                DataGridView.CellPainting += DataGridViewCellPainting;
                DataGridView.ColumnWidthChanged += DataGridView_ColumnWidthChanged;
                DataGridView.HandleCreated += DataGridView_HandleCreated;
                DataGridView.Disposed += DataGridView_Disposed;
                WaitCursor.Changed += HandleWaitCursorHidden;
            }
        }

        private void UnsubscribeEvents()
        {
            if (DataSource != null)
            {
                DataSource.ListChanged -= DataGridView_ListChanged;
                DataGridView.CellValueNeeded -= DataGridView_CellValueNeeded;
                DataGridView.ColumnHeaderMouseClick -= DataGridView_ColumnHeaderMouseClick;
                DataGridView.CellContentClick -= DataGridView_CellContentClick;
                DataGridView.CellPainting -= DataGridViewCellPainting;
                DataGridView.ColumnWidthChanged -= DataGridView_ColumnWidthChanged;
                DataGridView.HandleCreated -= DataGridView_HandleCreated;
                DataGridView.Disposed -= DataGridView_Disposed;
                WaitCursor.Changed -= HandleWaitCursorHidden;
            }
        }

        private void RefreshRequired()
        {
            if (!DataGridView.IsHandleCreated || DataGridView.IsDisposed)
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

                // Need to do this to get around a bug in DataGridView.
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

            foreach (ColumnInfo columnInfo in _columnInfos.Values)
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

        // ReSharper disable UnusedParameter.Local
        private void VerifyColumnBelongsToGridView(DataGridViewColumn column)
        {
            if (column.DataGridView != DataGridView)
            {
                throw new ArgumentException("Column does not belong to the DataGridView");
            }
        }

        // ReSharper restore UnusedParameter.Local

        private void SetHeaderCheckbox(ColumnInfo columnInfo)
        {
            columnInfo.CheckCount = 0;
            columnInfo.UncheckCount = 0;

            DataGridViewCellValueCallback<T> callback = columnInfo.CellValueCallback;

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

            var checkState = CheckState.Unchecked;

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
            T item = DataSource.GetAt(rowIndex);
            if (item != null)
            {
                DataGridViewCellValueCallback<T> getValueCallback = columnInfo.CellValueCallback;
                DataGridViewCellCheckCallback<T> setValueCallback = columnInfo.CellCheckCallback;
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

                var checkState = CheckState.Unchecked;

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
            ColumnInfo columnInfo = GetColumnInfo(e.ColumnIndex);
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
                ColumnInfo columnInfo = GetColumnInfo(e.ColumnIndex);
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
                        // Not sure if this is still required. When this class was first developed,
                        // there was the odd time when this method would raise an exception. I no
                        // longer know if this should remain here.
                    }
                }
            }
        }

        private void DataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            ColumnInfo columnInfo = GetColumnInfo(e.ColumnIndex);
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
                DataGridViewColumn sortColumn = DataGridView.Columns[e.ColumnIndex];
                if (sortColumn.SortMode != DataGridViewColumnSortMode.NotSortable)
                {
                    SortBy(sortColumn);
                }
            }
        }

        private void DataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            ColumnInfo columnInfo = GetColumnInfo(e.ColumnIndex);
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
            public readonly int ColumnIndex;
            public DataGridViewCellCheckCallback<T> CellCheckCallback;
            public DataGridViewCellValueCallback<T> CellValueCallback;
            public int CheckCount;
            public CheckState HeaderCheckState;
            public bool IsLinkColumn;
            public int UncheckCount;

            public ColumnInfo(int columnIndex)
            {
                ColumnIndex = columnIndex;
            }

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
                    var v1 = callback(obj1);
                    var v2 = callback(obj2);
                    var result = CompareValues(v1, v2);
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