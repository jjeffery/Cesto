using System;

// ReSharper disable CheckNamespace

namespace Cesto.WinForms
{
	public static class DisplaySettingsDateTimeExtenstions
	{
		public static DisplaySetting<DateTime> DateTimeSetting(this DisplaySettings displaySettings, string name,
		                                                       DateTime defaultValue = default(DateTime))
		{
			Verify.ArgumentNotNull(displaySettings, "displaySettings");
			return new DisplaySettingDateTime(displaySettings, name, defaultValue);
		}

		private class DisplaySettingDateTime : DisplaySetting<DateTime>
		{
			public DisplaySettingDateTime(DisplaySettings displaySettings, string name, DateTime defaultValue)
				: base(displaySettings, name, defaultValue)
			{}

			public override DateTime GetValue()
			{
				return DisplaySettings.GetDateTime(Name, DefaultValue);
			}

			public override void SetValue(DateTime value)
			{
				if (value == DefaultValue)
				{
					DisplaySettings.Delete(Name);
				}
				else
				{
					DisplaySettings.SetDateTime(Name, value);
				}
			}
		}
	}
}