namespace Cesto.WinForms
{
	public static class DisplaySettingsStringExtensions
	{
		public static DisplaySetting<string> StringSetting(this DisplaySettings displaySettings, string name,
		                                                   string defaultValue = null)
		{
			return new DisplaySettingString(displaySettings, name, defaultValue);
		}

		private class DisplaySettingString : DisplaySetting<string>
		{
			public DisplaySettingString(DisplaySettings displaySettings, string name, string defaultValue)
				: base(displaySettings, name, defaultValue)
			{}

			public override string GetValue()
			{
				return DisplaySettings.GetString(Name, DefaultValue);
			}

			public override void SetValue(string value)
			{
				if (value == DefaultValue)
				{
					DisplaySettings.Remove(Name);
				}
				else
				{
					DisplaySettings.SetString(Name, value);
				}
			}
		}
	}
}