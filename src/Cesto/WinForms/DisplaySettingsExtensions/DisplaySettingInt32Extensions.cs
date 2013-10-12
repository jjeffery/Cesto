// ReSharper disable CheckNamespace
namespace Cesto.WinForms
{
	public static class DisplaySettingsInt32Extensions
	{
		public static DisplaySetting<int> Int32Setting(this DisplaySettings displaySettings, string name, int defaultValue = 0)
		{
			return new DisplaySettingInt32(displaySettings, name, defaultValue);
		}

		private class DisplaySettingInt32 : DisplaySetting<int>
		{
			public DisplaySettingInt32(DisplaySettings displaySettings, string name, int defaultValue)
				: base(displaySettings, name, defaultValue)
			{}

			public override int GetValue()
			{
				return DisplaySettings.GetInt32(Name, DefaultValue);
			}

			public override void SetValue(int value)
			{
				if (value == DefaultValue)
				{
					DisplaySettings.Remove(Name);
				}
				else
				{
					DisplaySettings.SetInt32(Name, value);
				}
			}
		}
	}
}