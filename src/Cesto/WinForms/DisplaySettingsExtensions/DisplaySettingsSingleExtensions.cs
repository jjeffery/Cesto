// ReSharper disable CheckNamespace
namespace Cesto.WinForms
{
	public static class DisplaySettingsSingleExtensions
	{
		public static DisplaySetting<float> SingleSetting(this DisplaySettings displaySettings, string name,
		                                                  float defaultValue = 0)
		{
			Verify.ArgumentNotNull(displaySettings, "displaySettings");
			return new DisplaySettingSingle(displaySettings, name, defaultValue);
		}


		private class DisplaySettingSingle : DisplaySetting<float>
		{
			public DisplaySettingSingle(DisplaySettings displaySettings, string name, float defaultValue)
				: base(displaySettings, name, defaultValue)
			{}

			public override float GetValue()
			{
				return DisplaySettings.GetSingle(Name, DefaultValue);
			}

			public override void SetValue(float value)
			{
				// Comparison of floating point with equality, the default value is probably going to be zero,
				// which compares exactly. If not compared exactly, it does not matter much here.
				// ReSharper disable CompareOfFloatsByEqualityOperator
				if (value == DefaultValue)
				{
					DisplaySettings.Delete(Name);
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