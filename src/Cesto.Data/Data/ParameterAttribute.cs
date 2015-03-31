#region License

// Copyright 2004-2012 John Jeffery <john@jeffery.id.au>
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

namespace Cesto.Data
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class ParameterAttribute : Attribute
	{
		public string ParameterName { get; protected set; }
		public int ParameterOrder { get; protected set; }
		public bool Ignore { get; protected set; }

		public ParameterAttribute(string parameterName)
		{
			ParameterName = Verify.ArgumentNotNull(parameterName, "parameterName");
		}

		public ParameterAttribute(int parameterOrder)
		{
			ParameterOrder = parameterOrder;
		}

		protected ParameterAttribute()
		{
		}
	}
}