using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cesto.Config;
using Cesto.WinForms.DisplaySettingsExtensions;

namespace Cesto.WinForms
{
	public partial class EditParameterDialog : Form
	{
		private readonly DisplaySettings _displaySettings = new DisplaySettings(typeof (EditParameterDialog));
		private readonly DisplaySetting<Size> _sizeSetting;
		private IConfigParameterEditor _editor;
		private IConfigParameter _configParameter;

		public EditParameterDialog()
		{
			InitializeComponent();
			_sizeSetting = _displaySettings.SizeSetting("Size");
		}

		public IConfigParameter ConfigParameter
		{
			get { return _configParameter; }
			set
			{
				_configParameter = value;
				NameValueLabel.Text = _configParameter.Name;
				TypeValueLabel.Text = _configParameter.ParameterType;
				DescriptionTextBox.Text = _configParameter.Description;
				if (_configParameter.IsReadOnly)
				{
					Text = HeadingLabel.Text = "View read-only Configuration Parameter";
					SaveButton.Visible = false;
					CancelEditButton.Text = "Close";
				}
			}
		}

		private void CreateAndAddEditorControl()
		{
			SuspendLayout();
			EditValuePanel.Controls.Clear();
			_editor = null;
			if (_configParameter != null)
			{
				_editor = CreateEditor();
			}
			if (_editor != null)
			{
				var control = _editor.Control;
				EditValuePanel.Controls.Add(control);
				control.Anchor = AnchorStyles.Left;
				control.Dock = DockStyle.Fill;
				_editor.Initialize(_configParameter);
				if (!ConfigParameter.IsReadOnly)
				{
					BeginInvoke(new Action(() => control.Focus()));
				}
			}
			ResumeLayout();
		}

		private IConfigParameterEditor CreateEditor()
		{
			if (ConfigParameter.IsReadOnly)
			{
				return new StringEditor { IsReadOnly = true };
			}

			switch (ConfigParameter.ParameterType)
			{
				case ConfigParameterType.Int32:
					return new StringEditor
					{
						CharCount = 8,
					};
				case ConfigParameterType.Password:
					return new StringEditor
					{
						CharCount = 32,
					};
				case ConfigParameterType.Directory:
					return new DirectoryEditor();
				case ConfigParameterType.FilePath:
					// don't specify size and it will be as big as possible.
					// TODO: file editor with browse button
					return new StringEditor();
				default:
					return new StringEditor();
			}
		}


		private void EditParameterDialog_Load(object sender, EventArgs e)
		{
			Icon = ApplicationInfo.Icon;
			var size = _sizeSetting.GetValue();
			if (!size.IsEmpty)
			{
				Size = size;
			}
			CreateAndAddEditorControl();
		}

		private void EditParameterDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			_sizeSetting.SetValue(Size);
		}

		private void SaveButton_Click(object sender, EventArgs e)
		{
			var textValue = _editor.TextValue;
			var errorMessage = ConfigParameter.ValidateText(textValue);
			if (errorMessage != null)
			{
				MessageBox.Show(this, errorMessage, ApplicationInfo.MessageBoxCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				ConfigParameter.SetValueText(textValue);
				DialogResult = DialogResult.OK;
			}
		}
	}
}
