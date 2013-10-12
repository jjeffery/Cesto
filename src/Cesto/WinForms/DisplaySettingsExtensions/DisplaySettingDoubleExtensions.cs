using System.Globalization;

namespace Cesto.WinForms
{
	public static class DisplaySettingsDoubleExtensions
	{
		public static DisplaySetting<double> SingleSetting(this DisplaySettings displaySettings, string name,
		                                                   double defaultValue = 0.0)
		{
			Verify.ArgumentNotNull(displaySettings, "displaySettings");
			return new DisplaySettingDouble(displaySettings, name, defaultValue);
		}


		private class DisplaySettingDouble : DisplaySetting<double>
		{
			public DisplaySettingDouble(DisplaySettings displaySettings, string name, double defaultValue)
				: base(displaySettings, name, defaultValue)
			{}

			public override double GetValue()
			{
				var stringValue = DisplaySettings.GetString(Name, null);
				if (string.IsNullOrWhiteSpace(stringValue))
				{
					return DefaultValue;
				}

				double value;
				if (!double.TryParse(stringValue, out value))
				{
					return DefaultValue;
				}

				return value;
			}

			public override void SetValue(double value)
			{
				// Comparison of floating point with equality, the default value is probably going to be zero,
				// which compares exactly. If not compared exactly, it does not matter much here.
				// ReSharper disable CompareOfFloatsByEqualityOperator
				if (value == DefaultValue)
				{
					DisplaySettings.Remove(Name);
				}
				else
				{
					DisplaySettings.SetString(Name, value.ToString(CultureInfo.InvariantCulture));
				}
				// ReSharper restore CompareOfFloatsByEqualityOperator
			}
		}
	}
}