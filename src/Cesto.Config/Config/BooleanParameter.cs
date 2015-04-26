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
	/// A Parameter with a boolean value.
	/// </summary>
	public class BooleanParameter : ConfigParameter<bool, BooleanParameter>
	{
		public BooleanParameter(string paramName)
			: base(paramName, ConfigParameterType.Boolean)
		{
		}

		protected override object ConvertFromString(string s)
		{
			// check expected values first
			if (s == "1")
			{
				return true;
			}
			if (s == "0")
			{
				return false;
			}

			// attempt to parse using runtime library
			{
				bool result;
				if (bool.TryParse(s, out result))
				{
					return result;
				}
			}

			if (string.IsNullOrWhiteSpace(s))
			{
				// Blank value is considered false
				return false;
			}

			// Could not parse, but the runtime library only checks for values "True" and "False".
			// Look for a few other non-ambiguous values.

			s = s.Trim().ToLowerInvariant();

			if (s == "y" || s == "yes" || s == "1" || s == "t")
			{
				return true;
			}

			if (s == "n" || s == "no" || s == "0" || s == "f")
			{
				return false;
			}

			// don't know what it is, return false
			return false;
		}


		protected override string ConvertToString(object value)
		{
			return ((bool) value) ? "1" : "0";
		}
	}
}
