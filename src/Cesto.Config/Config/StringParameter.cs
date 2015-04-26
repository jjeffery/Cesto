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
	/// A parameter with a string value.
	/// </summary>
	/// <remarks>
	/// There are a number of specializations of <see cref="StringParameter"/>, including
	/// <see cref="PasswordParameter"/>, <see cref="DirectoryParameter"/> and <see cref="FilePathParameter"/>.
	/// </remarks>
	public class StringParameter : ConfigParameter<string, StringParameter>
	{
		public StringParameter(string paramName) : this(paramName, ConfigParameterType.String) {}

		/// <summary>
		/// Constructor for derived types. These derived types add some semantic value to the string, but in the
		/// end they just return a string.
		/// </summary>
		protected StringParameter(string paramName, string paramType)
			: base(paramName, paramType) {}

		protected override object ConvertFromString(string text)
		{
			return text;
		}

		protected override string ConvertToString(object value)
		{
			return value == null ? null : value.ToString();
		}
	}

	/// <summary>
	/// String parameter that contains the full path of a directory in the filesystem. Allows the value to be displayed differently
	/// in the maintenance UI, but it is really just a string parameter.
	/// </summary>
	public class DirectoryParameter : ConfigParameter<string, DirectoryParameter>
	{
		public DirectoryParameter(string paramName) : base(paramName, ConfigParameterType.Directory) {}

		protected override object ConvertFromString(string text)
		{
			return text;
		}

		protected override string ConvertToString(object value)
		{
			return value == null ? null : value.ToString();
		}
	}

	/// <summary>
	/// String parameter that contains the full path of a directory in the filesystem. Allows the value to be displayed differently
	/// in the maintenance UI, but it is really just a string parameter.
	/// </summary>
	public class FilePathParameter : ConfigParameter<string, FilePathParameter>
	{
		public FilePathParameter(string paramName) : base(paramName, ConfigParameterType.FilePath) {}

		protected override object ConvertFromString(string text)
		{
			return text;
		}

		protected override string ConvertToString(object value)
		{
			return value == null ? null : value.ToString();
		}
	}

	/// <summary>
	/// String parameter that contains a password. Essentially just a string parameter, but displayed slightly differently when listing
	/// config parameters. (Value is elided).
	/// </summary>
	public class PasswordParameter : ConfigParameter<string, PasswordParameter>
	{
		public PasswordParameter(string paramName) : base(paramName, ConfigParameterType.Password) {}

		protected override object ConvertFromString(string text)
		{
			return text;
		}

		protected override string ConvertToString(object value)
		{
			return value == null ? null : value.ToString();
		}

		protected override string ConvertToDisplayString(object value)
		{
			if (value == null || string.IsNullOrEmpty(value.ToString()))
			{
				return null;
			}

			return "********";
		}
	}
}
