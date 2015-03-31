namespace Cesto.WinForms
{
	partial class LogView
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.kryptonPanel1 = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.showDebugCheckBox = new System.Windows.Forms.CheckBox();
            this.showLoggerCheckBox = new System.Windows.Forms.CheckBox();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.timestampColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.levelColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.loggerColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.messageColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.kryptonPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.AutoSize = true;
            this.kryptonPanel1.Controls.Add(this.flowLayoutPanel1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Margin = new System.Windows.Forms.Padding(6);
            this.kryptonPanel1.MinimumSize = new System.Drawing.Size(200, 48);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(1576, 50);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.flowLayoutPanel1.Controls.Add(this.showDebugCheckBox);
            this.flowLayoutPanel1.Controls.Add(this.showLoggerCheckBox);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(6);
            this.flowLayoutPanel1.MinimumSize = new System.Drawing.Size(200, 48);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1576, 50);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // showDebugCheckBox
            // 
            this.showDebugCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.showDebugCheckBox.Location = new System.Drawing.Point(6, 6);
            this.showDebugCheckBox.Margin = new System.Windows.Forms.Padding(6);
            this.showDebugCheckBox.Name = "showDebugCheckBox";
            this.showDebugCheckBox.Size = new System.Drawing.Size(300, 38);
            this.showDebugCheckBox.TabIndex = 0;
            this.showDebugCheckBox.Text = "Show Debug Messages";
            this.showDebugCheckBox.CheckedChanged += new System.EventHandler(this.ShowDebugCheckBoxCheckedChanged);
            // 
            // showLoggerCheckBox
            // 
            this.showLoggerCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.showLoggerCheckBox.Location = new System.Drawing.Point(318, 6);
            this.showLoggerCheckBox.Margin = new System.Windows.Forms.Padding(6);
            this.showLoggerCheckBox.Name = "showLoggerCheckBox";
            this.showLoggerCheckBox.Size = new System.Drawing.Size(190, 38);
            this.showLoggerCheckBox.TabIndex = 1;
            this.showLoggerCheckBox.Text = "Show Logger";
            this.showLoggerCheckBox.CheckedChanged += new System.EventHandler(this.ShowLoggerCheckBoxCheckedChanged);
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.timestampColumn,
            this.levelColumn,
            this.loggerColumn,
            this.messageColumn});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView.Location = new System.Drawing.Point(0, 50);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.RowTemplate.Height = 33;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(1576, 1165);
            this.dataGridView.TabIndex = 1;
            // 
            // timestampColumn
            // 
            dataGridViewCellStyle1.Format = "HH:mm:ss";
            dataGridViewCellStyle1.NullValue = null;
            this.timestampColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.timestampColumn.HeaderText = "Time";
            this.timestampColumn.Name = "timestampColumn";
            this.timestampColumn.ReadOnly = true;
            this.timestampColumn.Width = 200;
            // 
            // levelColumn
            // 
            this.levelColumn.HeaderText = "Level";
            this.levelColumn.Name = "levelColumn";
            this.levelColumn.ReadOnly = true;
            this.levelColumn.Width = 200;
            // 
            // loggerColumn
            // 
            this.loggerColumn.HeaderText = "Logger";
            this.loggerColumn.Name = "loggerColumn";
            this.loggerColumn.ReadOnly = true;
            this.loggerColumn.Width = 300;
            // 
            // messageColumn
            // 
            this.messageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.messageColumn.HeaderText = "Message";
            this.messageColumn.Name = "messageColumn";
            this.messageColumn.ReadOnly = true;
            // 
            // LogView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.kryptonPanel1);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "LogView";
            this.Size = new System.Drawing.Size(1576, 1215);
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.Panel kryptonPanel1;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.CheckBox showDebugCheckBox;
        private System.Windows.Forms.CheckBox showLoggerCheckBox;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn timestampColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn levelColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn loggerColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn messageColumn;

	}
}
