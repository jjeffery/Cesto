// ReSharper disable CheckNamespace
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
				return DisplaySettings.GetDouble(Name, DefaultValue);
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
					DisplaySettings.SetDouble(Name, value);
				}
				// ReSharper restore CompareOfFloatsByEqualityOperator
			}
		}
	}
}