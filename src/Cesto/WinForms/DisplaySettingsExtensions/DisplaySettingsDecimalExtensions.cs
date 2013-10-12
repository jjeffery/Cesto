// ReSharper disable CheckNamespace
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
				return DisplaySettings.GetDecimal(Name, DefaultValue);
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
					DisplaySettings.SetDecimal(Name, value);
				}
			}
		}
	}
}