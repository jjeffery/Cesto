namespace Cesto.WinForms
{
	partial class ListConfigView
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.TopPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.SearchLabel = new System.Windows.Forms.Label();
			this.ClearSearchTextButton = new System.Windows.Forms.Button();
			this.EditButton = new System.Windows.Forms.Button();
			this.RefreshButton = new System.Windows.Forms.Button();
			this.DataGridView = new Quokka.WinForms.Config.ListConfigGridView();
			this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ParameterTypeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DescriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SearchTextBox = new Quokka.WinForms.Config.SearchTextBox();
			this.TopPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.DataGridView)).BeginInit();
			this.SuspendLayout();
			// 
			// TopPanel
			// 
			this.TopPanel.AutoSize = true;
			this.TopPanel.Controls.Add(this.SearchLabel);
			this.TopPanel.Controls.Add(this.SearchTextBox);
			this.TopPanel.Controls.Add(this.ClearSearchTextButton);
			this.TopPanel.Controls.Add(this.EditButton);
			this.TopPanel.Controls.Add(this.RefreshButton);
			this.TopPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.TopPanel.Location = new System.Drawing.Point(0, 0);
			this.TopPanel.Margin = new System.Windows.Forms.Padding(6);
			this.TopPanel.MinimumSize = new System.Drawing.Size(200, 48);
			this.TopPanel.Name = "TopPanel";
			this.TopPanel.Size = new System.Drawing.Size(1432, 56);
			this.TopPanel.TabIndex = 2;
			// 
			// SearchLabel
			// 
			this.SearchLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.SearchLabel.AutoSize = true;
			this.SearchLabel.Location = new System.Drawing.Point(12, 15);
			this.SearchLabel.Margin = new System.Windows.Forms.Padding(12, 0, 6, 0);
			this.SearchLabel.Name = "SearchLabel";
			this.SearchLabel.Size = new System.Drawing.Size(80, 25);
			this.SearchLabel.TabIndex = 2;
			this.SearchLabel.Text = "Search";
			// 
			// ClearSearchTextButton
			// 
			this.ClearSearchTextButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.ClearSearchTextButton.AutoSize = true;
			this.ClearSearchTextButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ClearSearchTextButton.Location = new System.Drawing.Point(622, 6);
			this.ClearSearchTextButton.Margin = new System.Windows.Forms.Padding(6);
			this.ClearSearchTextButton.MinimumSize = new System.Drawing.Size(150, 44);
			this.ClearSearchTextButton.Name = "ClearSearchTextButton";
			this.ClearSearchTextButton.Size = new System.Drawing.Size(150, 44);
			this.ClearSearchTextButton.TabIndex = 3;
			this.ClearSearchTextButton.Text = "Clear";
			this.ClearSearchTextButton.UseVisualStyleBackColor = true;
			this.ClearSearchTextButton.Click += new System.EventHandler(this.ClearSearchTextButton_Click);
			// 
			// EditButton
			// 
			this.EditButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.EditButton.AutoSize = true;
			this.EditButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.EditButton.Location = new System.Drawing.Point(842, 6);
			this.EditButton.Margin = new System.Windows.Forms.Padding(64, 6, 6, 6);
			this.EditButton.MinimumSize = new System.Drawing.Size(150, 44);
			this.EditButton.Name = "EditButton";
			this.EditButton.Size = new System.Drawing.Size(150, 44);
			this.EditButton.TabIndex = 0;
			this.EditButton.Text = "Edit";
			this.EditButton.UseVisualStyleBackColor = true;
			this.EditButton.Click += new System.EventHandler(this.EditButton_Click);
			// 
			// RefreshButton
			// 
			this.RefreshButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.RefreshButton.AutoSize = true;
			this.RefreshButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.RefreshButton.Location = new System.Drawing.Point(1004, 6);
			this.RefreshButton.Margin = new System.Windows.Forms.Padding(6);
			this.RefreshButton.MinimumSize = new System.Drawing.Size(150, 44);
			this.RefreshButton.Name = "RefreshButton";
			this.RefreshButton.Size = new System.Drawing.Size(150, 44);
			this.RefreshButton.TabIndex = 4;
			this.RefreshButton.Text = "Refresh";
			this.RefreshButton.UseVisualStyleBackColor = true;
			this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
			// 
			// DataGridView
			// 
			this.DataGridView.AllowUserToAddRows = false;
			this.DataGridView.AllowUserToDeleteRows = false;
			this.DataGridView.AllowUserToResizeRows = false;
			this.DataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
			this.DataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this.DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.DataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameColumn,
            this.ValueColumn,
            this.ParameterTypeColumn,
            this.DescriptionColumn});
			this.DataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.DataGridView.Location = new System.Drawing.Point(0, 56);
			this.DataGridView.Margin = new System.Windows.Forms.Padding(6);
			this.DataGridView.Name = "DataGridView";
			this.DataGridView.ReadOnly = true;
			this.DataGridView.RowHeadersVisible = false;
			this.DataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.DataGridView.Size = new System.Drawing.Size(1432, 1042);
			this.DataGridView.TabIndex = 3;
			this.DataGridView.MoveUp += new System.EventHandler(this.DataGridView_MoveUp);
			this.DataGridView.EnterKeyPressed += new System.EventHandler(this.DataGridView_EnterKeyPressed);
			this.DataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_CellDoubleClick);
			// 
			// NameColumn
			// 
			this.NameColumn.HeaderText = "Name";
			this.NameColumn.Name = "NameColumn";
			this.NameColumn.ReadOnly = true;
			// 
			// ValueColumn
			// 
			this.ValueColumn.HeaderText = "Value";
			this.ValueColumn.Name = "ValueColumn";
			this.ValueColumn.ReadOnly = true;
			// 
			// ParameterTypeColumn
			// 
			this.ParameterTypeColumn.HeaderText = "Type";
			this.ParameterTypeColumn.Name = "ParameterTypeColumn";
			this.ParameterTypeColumn.ReadOnly = true;
			// 
			// DescriptionColumn
			// 
			this.DescriptionColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.DescriptionColumn.HeaderText = "Description";
			this.DescriptionColumn.Name = "DescriptionColumn";
			this.DescriptionColumn.ReadOnly = true;
			// 
			// SearchTextBox
			// 
			this.SearchTextBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.SearchTextBox.Location = new System.Drawing.Point(104, 12);
			this.SearchTextBox.Margin = new System.Windows.Forms.Padding(6);
			this.SearchTextBox.Name = "SearchTextBox";
			this.SearchTextBox.Size = new System.Drawing.Size(506, 31);
			this.SearchTextBox.TabIndex = 1;
			this.SearchTextBox.DownKeyPressed += new System.EventHandler(this.SearchTextBox_DownKeyPressed);
			this.SearchTextBox.TextChanged += new System.EventHandler(this.SearchTextBox_TextChanged);
			// 
			// ListConfigView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.DataGridView);
			this.Controls.Add(this.TopPanel);
			this.Name = "ListConfigView";
			this.Size = new System.Drawing.Size(1432, 1098);
			this.TopPanel.ResumeLayout(false);
			this.TopPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.DataGridView)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.FlowLayoutPanel TopPanel;
		public System.Windows.Forms.Label SearchLabel;
		public Quokka.WinForms.Config.SearchTextBox SearchTextBox;
		public System.Windows.Forms.Button ClearSearchTextButton;
		public System.Windows.Forms.Button EditButton;
		public System.Windows.Forms.Button RefreshButton;
		public Quokka.WinForms.Config.ListConfigGridView DataGridView;
		public System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
		public System.Windows.Forms.DataGridViewTextBoxColumn ValueColumn;
		public System.Windows.Forms.DataGridViewTextBoxColumn ParameterTypeColumn;
		public System.Windows.Forms.DataGridViewTextBoxColumn DescriptionColumn;
	}
}
