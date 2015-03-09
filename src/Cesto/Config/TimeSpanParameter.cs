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
	/// A Parameter with a time span value
	/// </summary>
	public class TimeSpanParameter : ConfigParameter<TimeSpan, TimeSpanParameter>
	{
		public TimeSpanParameter(string paramName) : base(paramName, ConfigParameterType.TimeSpan)
		{
		}

		protected override object ConvertFromString(string text)
		{
			TimeSpan value;
			if (!TimeSpanExtensions.TryParse(text, out value))
			{
				throw new FormatException("Invalid timespan value");
			}
			return value;
		}

		protected override string ConvertToString(object value)
		{
			return ((TimeSpan) value).ToReadableString();
		}
	}
}