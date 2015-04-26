namespace Cesto.WinForms
{
	partial class DirectoryEditor
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
			this.BrowseButton = new System.Windows.Forms.Button();
			this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.TextBox = new System.Windows.Forms.TextBox();
			this.TableLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// BrowseButton
			// 
			this.BrowseButton.AutoSize = true;
			this.BrowseButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BrowseButton.Location = new System.Drawing.Point(332, 6);
			this.BrowseButton.Margin = new System.Windows.Forms.Padding(6);
			this.BrowseButton.Name = "BrowseButton";
			this.BrowseButton.Size = new System.Drawing.Size(93, 35);
			this.BrowseButton.TabIndex = 1;
			this.BrowseButton.Text = "Browse";
			this.BrowseButton.UseVisualStyleBackColor = true;
			this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
			// 
			// TableLayoutPanel
			// 
			this.TableLayoutPanel.AutoSize = true;
			this.TableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.TableLayoutPanel.ColumnCount = 2;
			this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.TableLayoutPanel.Controls.Add(this.BrowseButton, 1, 0);
			this.TableLayoutPanel.Controls.Add(this.TextBox, 0, 0);
			this.TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TableLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this.TableLayoutPanel.Margin = new System.Windows.Forms.Padding(6);
			this.TableLayoutPanel.MinimumSize = new System.Drawing.Size(200, 44);
			this.TableLayoutPanel.Name = "TableLayoutPanel";
			this.TableLayoutPanel.RowCount = 1;
			this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TableLayoutPanel.Size = new System.Drawing.Size(431, 47);
			this.TableLayoutPanel.TabIndex = 2;
			// 
			// TextBox
			// 
			this.TextBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.TextBox.Location = new System.Drawing.Point(6, 8);
			this.TextBox.Margin = new System.Windows.Forms.Padding(6);
			this.TextBox.Name = "TextBox";
			this.TextBox.Size = new System.Drawing.Size(314, 31);
			this.TextBox.TabIndex = 0;
			// 
			// DirectoryEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.TableLayoutPanel);
			this.Name = "DirectoryEditor";
			this.Size = new System.Drawing.Size(431, 47);
			this.Load += new System.EventHandler(this.DirectoryEditor_Load);
			this.TableLayoutPanel.ResumeLayout(false);
			this.TableLayoutPanel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.Button BrowseButton;
		public System.Windows.Forms.TableLayoutPanel TableLayoutPanel;
		public System.Windows.Forms.TextBox TextBox;
	}
}
