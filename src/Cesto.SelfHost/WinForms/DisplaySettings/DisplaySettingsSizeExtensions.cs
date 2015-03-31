#region License

// Copyright 2004-2013 John Jeffery <john@jeffery.id.au>
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System.Drawing;

namespace Cesto.WinForms.DisplaySettingsExtensions
{
    /// <summary>
    ///     Extension methods for <see cref="DisplaySettings" />
    /// </summary>
    public static class DisplaySettingsSizeExtensions
    {
        /// <summary>
        ///     Set a <see cref="Size" /> value.
        /// </summary>
        /// <param name="settings">
        ///     The associated <see cref="DisplaySettings" />.
        /// </param>
        /// <param name="name">
        ///     The name of the value to set.
        /// </param>
        /// <param name="value">
        ///     The value to set.
        /// </param>
        public static void SetSize(this DisplaySettings settings, string name, Size value)
        {
            Verify.ArgumentNotNull(settings, "settings");
            Verify.ArgumentNotNull(name, "name");
            settings.SetInt32(name + ".Width", value.Width);
            settings.SetInt32(name + ".Height", value.Height);
        }

        /// <summary>
        ///     Get a <see cref="Size" /> value.
        /// </summary>
        /// <param name="settings">
        ///     The associated <see cref="DisplaySettings" />.
        /// </param>
        /// <param name="name">
        ///     The name of the value to return as a <see cref="Size" />.
        /// </param>
        /// <param name="defaultValue">
        ///     The default value to return of the value does not exist.
        /// </param>
        /// <returns>
        ///     A <see cref="Size" /> value.
        /// </returns>
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

        /// <summary>
        ///     Create a <see cref="DisplaySetting{Size}" />
        /// </summary>
        /// <param name="displaySettings">
        ///     The <see cref="DisplaySettings" />.
        /// </param>
        /// <param name="name">Name of the value.</param>
        /// <param name="defaultValue">Default value not present.</param>
        /// <returns></returns>
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
            {
            }

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