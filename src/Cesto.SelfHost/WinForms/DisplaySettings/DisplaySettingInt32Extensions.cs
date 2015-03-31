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

// ReSharper disable CheckNamespace
namespace Cesto.WinForms
{
    /// <summary>
    /// Extension methods for <see cref="DisplaySettings"/>
    /// </summary>
    public static class DisplaySettingsInt32Extensions
    {
        /// <summary>
        /// Create a <see cref="DisplaySetting{Int32}"/>
        /// </summary>
        /// <param name="displaySettings">The <see cref="DisplaySettings"/>.</param>
        /// <param name="name">Name of the value.</param>
        /// <param name="defaultValue">Default value not present.</param>
        /// <returns></returns>
        public static DisplaySetting<int> Int32Setting(this DisplaySettings displaySettings, string name,
            int defaultValue = 0)
        {
            return new DisplaySettingInt32(displaySettings, name, defaultValue);
        }

        private class DisplaySettingInt32 : DisplaySetting<int>
        {
            public DisplaySettingInt32(DisplaySettings displaySettings, string name, int defaultValue)
                : base(displaySettings, name, defaultValue)
            {
            }

            public override int GetValue()
            {
                return DisplaySettings.GetInt32(Name, DefaultValue);
            }

            public override void SetValue(int value)
            {
                if (value == DefaultValue)
                {
                    DisplaySettings.Delete(Name);
                }
                else
                {
                    DisplaySettings.SetInt32(Name, value);
                }
            }
        }
    }
}