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
			{ }

			public override DateTime GetValue()
			{
				var stringValue = DisplaySettings.GetString(Name, null);
				if (string.IsNullOrWhiteSpace(stringValue))
				{
					return DefaultValue;
				}

				DateTime value;
				if (!DateTime.TryParse(stringValue, out value))
				{
					return DefaultValue;
				}

				return value;
			}

			public override void SetValue(DateTime value)
			{
				if (value == DefaultValue)
				{
					DisplaySettings.Remove(Name);
				}
				else
				{
					string stringValue;
					if (value.Hour == 0 && value.Minute == 0 && value.Second == 0 && value.Millisecond == 0)
					{
						// Just a date
						stringValue = value.ToString("yyyy-MM-dd");
					}
					else
					{
						stringValue = value.ToString("yyyy-MM-dd HH:mm:ss.fffff");
					}
					DisplaySettings.SetString(Name, stringValue);
				}
			}
		}
	}
}