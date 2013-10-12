using System.Globalization;

namespace Cesto.WinForms
{
	public static class DisplaySettingsSingleExtensions
	{
		public static DisplaySetting<float> SingleSetting(this DisplaySettings displaySettings, string name,
		                                                  float defaultValue)
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
				var stringValue = DisplaySettings.GetString(Name, null);
				if (string.IsNullOrWhiteSpace(stringValue))
				{
					return DefaultValue;
				}

				float value;
				if (!float.TryParse(stringValue, out value))
				{
					return DefaultValue;
				}

				return value;
			}

			public override void SetValue(float value)
			{
				// Comparison of floating point with equality, the default value is probably going to be zero,
				// which compares exactly. If not compared exactly, it does not matter much here.
				// ReSharper disable CompareOfFloatsByEqualityOperator
				if (value == DefaultValue)
					// ReSharper restore CompareOfFloatsByEqualityOperator
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