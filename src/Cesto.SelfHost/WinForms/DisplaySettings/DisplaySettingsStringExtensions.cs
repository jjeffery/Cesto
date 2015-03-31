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
    public static class DisplaySettingsStringExtensions
    {
        /// <summary>
        /// Create a <see cref="DisplaySetting{String}"/>
        /// </summary>
        /// <param name="displaySettings">The <see cref="DisplaySettings"/>.</param>
        /// <param name="name">Name of the value.</param>
        /// <param name="defaultValue">Default value not present.</param>
        /// <returns></returns>
        public static DisplaySetting<string> StringSetting(this DisplaySettings displaySettings, string name,
            string defaultValue = null)
        {
            return new DisplaySettingString(displaySettings, name, defaultValue);
        }

        private class DisplaySettingString : DisplaySetting<string>
        {
            public DisplaySettingString(DisplaySettings displaySettings, string name, string defaultValue)
                : base(displaySettings, name, defaultValue)
            {
            }

            public override string GetValue()
            {
                return DisplaySettings.GetString(Name, DefaultValue);
            }

            public override void SetValue(string value)
            {
                if (value == DefaultValue)
                {
                    DisplaySettings.Delete(Name);
                }
                else
                {
                    DisplaySettings.SetString(Name, value);
                }
            }
        }
    }
}