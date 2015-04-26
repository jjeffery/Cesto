using System;
using System.Drawing;
using System.Windows.Forms;
using Cesto.Config;

namespace Cesto.WinForms
{
	public partial class StringEditor : UserControl, IConfigParameterEditor
	{
		/// <summary>
		/// Desired width in characters.
		/// </summary>
		public int CharCount { get; set; }

		public bool IsReadOnly
		{
			get { return textBox.ReadOnly; }
			set
			{
				textBox.ReadOnly = value;
				textBox.BackColor = SystemColors.Window;
			}
		}

		public StringEditor()
		{
			InitializeComponent();
			Load += OnLoad;
		}

		private void OnLoad(object sender, EventArgs eventArgs)
		{
			MinimumSize = new Size(0, textBox.Size.Height + textBox.Margin.Top + textBox.Margin.Bottom);
			Width = Parent.ClientSize.Width;

			if (CharCount > 0)
			{
				textBox.Width = TextRenderer.MeasureText(new string('m', CharCount), textBox.Font).Width;
			}
			else
			{
				// Adjust width of text box to suit maximum width
				Parent.SizeChanged += Parent_SizeChanged;
				SizeChanged += UserControl_SizeChanged;
			}
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
			textBox.Text = parameter.GetValueText();
			if (parameter is PasswordParameter)
			{
				textBox.PasswordChar = '*';
			}
		}

		public string TextValue
		{
			get { return textBox.Text; }
		}

		private void UserControl_SizeChanged(object sender, EventArgs e)
		{
			textBox.Width = Width - textBox.Margin.Left - textBox.Margin.Right;
		}

		private void Parent_SizeChanged(object sender, EventArgs eventArgs)
		{
			Width = Parent.ClientSize.Width;
		}
	}
}
