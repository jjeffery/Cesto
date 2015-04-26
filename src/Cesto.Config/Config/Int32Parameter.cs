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

namespace Cesto.Config
{
	/// <summary>
	/// A Parameter with an integer value.
	/// </summary>
	public class Int32Parameter : ConfigParameter<int, Int32Parameter>
	{
		public Int32Parameter(string paramName)
			: base(paramName, ConfigParameterType.Int32) {}

		protected override object ConvertFromString(string text)
		{
			return int.Parse(text);
		}

		protected override string ConvertToString(object value)
		{
			return value.ToString();
		}
	}

	public static class Int32ParameterExtensions
	{
		public static void ValidRange(this IConfigParameterBuilder<int> @this, int lowerInclusive, int upperInclusive)
		{
			@this.Validation(n => n >= lowerInclusive && n <= upperInclusive
				? null
				: string.Format("Value should be in the range {0} to {1} inclusive",
					lowerInclusive,
					upperInclusive));
		}
	}
}
