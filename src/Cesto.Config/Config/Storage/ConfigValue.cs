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

namespace Cesto.Config.Storage
{
	/// <summary>
	/// Represents a value associated with a <see cref="ConfigParameter"/>, and whether
	/// that value is defined or not.
	/// </summary>
	public struct ConfigValue
	{
		/// <summary>
		/// Associated <see cref="ConfigParameter"/>.
		/// </summary>
		public readonly ConfigParameter Parameter;

		/// <summary>
		/// Value associated with the <see cref="ConfigParameter"/>
		/// </summary>
		/// <remarks>
		/// Depending on the storage mechanism, a value might legitimately be <c>null</c>,
		/// in which case it is not clear whether the value is not defined, or whether
		/// the value has been deliberately set to <c>null</c>. In this case, refer to
		/// the <see cref="HasValue"/> field.
		/// </remarks>
		public readonly string Value;

		/// <summary>
		/// When the <see cref="Value"/> is <c>null</c>, this property indicates whether the
		/// value was explicitly set as <c>null</c>, or is missing from the configuration
		/// storage all together. This field should always be set to <c>true</c> if <see cref="Value"/>
		/// is not <c>null</c>.
		/// </summary>
		public readonly bool HasValue;

		public ConfigValue(ConfigParameter param, string value, bool hasValue)
		{
			Parameter = param;
			Value = value;
			HasValue = hasValue;
		}
	}
}
