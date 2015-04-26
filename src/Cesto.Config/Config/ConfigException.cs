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
using System.Runtime.Serialization;

namespace Cesto.Config
{
	/// <summary>
	/// Exception thrown due to errors accessing configuration parameters.
	/// </summary>
	public class ConfigException : CestoException
	{
		public ConfigException() {}

		public ConfigException(string message) : base(message) {}

		public ConfigException(string message, Exception innerException) : base(message, innerException) {}

		protected ConfigException(SerializationInfo info, StreamingContext context) : base(info, context) {}

		public ConfigParameter ConfigParameter { get; internal set; }
	}

	/// <summary>
	/// Attempt to write to a read-only configuration parameter.
	/// </summary>
	public class ReadOnlyConfigException : ConfigException
	{
		public ReadOnlyConfigException() {}

		public ReadOnlyConfigException(string message) : base(message) {}

		public ReadOnlyConfigException(string message, Exception innerException) : base(message, innerException) {}

		protected ReadOnlyConfigException(SerializationInfo info, StreamingContext context) : base(info, context) {}
	}

}
