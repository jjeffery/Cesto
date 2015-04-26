using System;
using System.Drawing;
using System.Windows.Forms;
using Cesto.Config;

namespace Cesto.WinForms
{
	public partial class DirectoryEditor : UserControl, IConfigParameterEditor
	{
		public DirectoryEditor()
		{
			InitializeComponent();
		}

		private void DirectoryEditor_Load(object sender, EventArgs e)
		{
			MinimumSize = new Size(BrowseButton.Width*2, TextBox.Height + TextBox.Margin.Top + TextBox.Margin.Bottom);
			Width = Parent.ClientSize.Width;

			// Adjust width of text box to suit maximum width
			Parent.SizeChanged += Parent_SizeChanged;
			SizeChanged += UserControl_SizeChanged;
		}

		public Control Control
		{
			get { return this; }
		}

		public IConfigParameter Parameter { get; private set; }

		public void Initialize(IConfigParameter parameter)
		{
			Verify.ArgumentNotNull(parameter, "parameter");
			Parameter = parameter;
			TextBox.Text = parameter.GetValueText();
			if (parameter is PasswordParameter)
			{
				TextBox.PasswordChar = '*';
			}
		}

		public string TextValue
		{
			get { return TextBox.Text; }
		}

		private void UserControl_SizeChanged(object sender, EventArgs e)
		{
			TextBox.Width = Width - TextBox.Margin.Left - TextBox.Margin.Right
			                - BrowseButton.Width - BrowseButton.Margin.Left - BrowseButton.Margin.Right;
		}

		private void Parent_SizeChanged(object sender, EventArgs eventArgs)
		{
			Width = Parent.ClientSize.Width;
		}

		protected virtual void BrowseButton_Click(object sender, EventArgs e)
		{
			var dialog = new FolderBrowserDialog();
			dialog.Description = Parameter.Name;
			dialog.ShowNewFolderButton = true;
			dialog.SelectedPath = Parameter.GetValueText();
			if (dialog.ShowDialog(TopLevelControl) == DialogResult.OK)
			{
				TextBox.Text = dialog.SelectedPath;
			}
		}
	}
}
