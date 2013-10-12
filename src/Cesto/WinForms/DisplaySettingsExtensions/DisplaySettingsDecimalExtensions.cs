using System.Globalization;

namespace Cesto.WinForms
{
	public static class DisplaySettingsDecimalExtensions
	{
		public static DisplaySetting<decimal> DecimalSetting(this DisplaySettings displaySettings, string name,
		                                                     decimal defaultValue = 0)
		{
			Verify.ArgumentNotNull(displaySettings, "displaySettings");
			return new DisplaySettingDecimal(displaySettings, name, defaultValue);
		}


		private class DisplaySettingDecimal : DisplaySetting<decimal>
		{
			public DisplaySettingDecimal(DisplaySettings displaySettings, string name, decimal defaultValue)
				: base(displaySettings, name, defaultValue)
			{}

			public override decimal GetValue()
			{
				var stringValue = DisplaySettings.GetString(Name, null);
				if (string.IsNullOrWhiteSpace(stringValue))
				{
					return DefaultValue;
				}

				decimal value;
				if (!decimal.TryParse(stringValue, out value))
				{
					return DefaultValue;
				}

				return value;
			}

			public override void SetValue(decimal value)
			{
				// Comparison of floating point with equality, the default value is probably going to be zero,
				// which compares exactly. If not compared exactly, it does not matter much here.
				if (value == DefaultValue)
				{
					DisplaySettings.Remove(Name);
				}
				else
				{
					DisplaySettings.SetString(Name, value.ToString(CultureInfo.InvariantCulture));
				}
			}
		}
	}
}