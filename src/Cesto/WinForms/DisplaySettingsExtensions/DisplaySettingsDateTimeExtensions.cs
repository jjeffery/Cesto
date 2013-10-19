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

using System;

// ReSharper disable CheckNamespace

namespace Cesto.WinForms
{
	/// <summary>
	/// Extension methods for <see cref="DisplaySettings"/>
	/// </summary>
	public static class DisplaySettingsDateTimeExtenstions
	{
		/// <summary>
		/// Create a <see cref="DisplaySetting{DateTime}"/>
		/// </summary>
		/// <param name="displaySettings">The <see cref="DisplaySettings"/>.</param>
		/// <param name="name">Name of the value.</param>
		/// <param name="defaultValue">Default value not present.</param>
		/// <returns></returns>
		public static DisplaySetting<DateTime> DateTimeSetting(this DisplaySettings displaySettings, string name,
		                                                       DateTime defaultValue = default(DateTime))
		{
			Verify.ArgumentNotNull(displaySettings, "displaySettings");
			return new DisplaySettingDateTime(displaySettings, name, defaultValue);
		}

		private class DisplaySettingDateTime : DisplaySetting<DateTime>
		{
			public DisplaySettingDateTime(DisplaySettings displaySettings, string name, DateTime defaultValue)
				: base(displaySettings, name, defaultValue)
			{}

			public override DateTime GetValue()
			{
				return DisplaySettings.GetDateTime(Name, DefaultValue);
			}

			public override void SetValue(DateTime value)
			{
				if (value == DefaultValue)
				{
					DisplaySettings.Delete(Name);
				}
				else
				{
					DisplaySettings.SetDateTime(Name, value);
				}
			}
		}
	}
}