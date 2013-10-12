using System.Drawing;

namespace Cesto.WinForms.DisplaySettingsExtensions
{
	public static class DisplaySettingsSizeExtensions
	{
		public static void SetSize(this DisplaySettings settings, string name, Size size)
		{
			Verify.ArgumentNotNull(settings, "settings");
			Verify.ArgumentNotNull(name, "name");
			settings.SetInt32(name + ".Width", size.Width);
			settings.SetInt32(name + ".Height", size.Height);
		}

		public static Size GetSize(this DisplaySettings settings, string name, Size defaultValue)
		{
			Verify.ArgumentNotNull(settings, "settings");
			Verify.ArgumentNotNull(name, "name");
			int height = settings.GetInt32(name + ".Height", -1);
			int width = settings.GetInt32(name + ".Width", -1);
			if (height < 0 || width < 0)
			{
				return defaultValue;
			}
			return new Size(width, height);
		}

		public static DisplaySetting<Size> SizeSetting(this DisplaySettings displaySettings, string name,
		                                               Size defaultValue = default(Size))
		{
			Verify.ArgumentNotNull(displaySettings, "displaySettings");
			Verify.ArgumentNotNull(name, "name");
			return new DisplaySettingSize(displaySettings, name, defaultValue);
		}

		private class DisplaySettingSize : DisplaySetting<Size>
		{
			public DisplaySettingSize(DisplaySettings displaySettings, string name, Size defaultValue)
				: base(displaySettings, name, defaultValue)
			{}

			public override Size GetValue()
			{
				return DisplaySettings.GetSize(Name, DefaultValue);
			}

			public override void SetValue(Size value)
			{
				// Comparison of floating point with equality, the default value is probably going to be zero,
				// which compares exactly. If not compared exactly, it does not matter much here.
				if (value == DefaultValue)
				{
					DisplaySettings.Delete(Name);
				}
				else
				{
					DisplaySettings.SetSize(Name, value);
				}
			}
		}
	}
}