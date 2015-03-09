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

namespace Cesto.Config
{
	/// <summary>
	/// A Parameter with a date value (no time component -- only the date)
	/// </summary>
	public class DateParameter : ConfigParameter<DateTime, DateParameter>
	{
		public DateParameter(string paramName)
			: base(paramName, ConfigParameterType.Date)
		{
		}

		protected override object ConvertFromString(string text)
		{
			return DateTime.Parse(text);
		}

		protected override string ConvertToString(object value)
		{
			return ((DateTime) value).ToString("yyyy-MM-dd");
		}
	}
}