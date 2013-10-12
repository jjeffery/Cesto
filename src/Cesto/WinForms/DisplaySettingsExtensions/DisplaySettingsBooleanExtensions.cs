// ReSharper disable CheckNamespace
namespace Cesto.WinForms
{
	public static class DisplaySettingsBooleanExtensions
	{
		public static DisplaySetting<bool> BooleanSetting(this DisplaySettings displaySettings, string name, bool defaultValue)
		{
			return new DisplaySettingBoolean(displaySettings, name, defaultValue);
		}

		private class DisplaySettingBoolean : DisplaySetting<bool>
		{
			public DisplaySettingBoolean(DisplaySettings displaySettings, string name, bool defaultValue)
				: base(displaySettings, name, defaultValue)
			{}

			public override bool GetValue()
			{
				return DisplaySettings.GetBoolean(Name, DefaultValue);
			}

			public override void SetValue(bool value)
			{
				if (value == DefaultValue)
				{
					DisplaySettings.Remove(Name);
				}
				else
				{
					DisplaySettings.SetBoolean(Name, value);
				}
			}
		}
	}
}