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
using System.Reflection;

namespace Cesto.Data.Internal
{
	internal static class TypeExtensions
	{
		public static PropertyInfo GetPropertyCaseInsensitive(this Type type, string propertyName)
		{
			var properties = type.GetProperties();
			propertyName = (propertyName ?? string.Empty).Trim();

			// attempt a case-sensitive match
			foreach (var property in properties)
			{
				if (property.Name == propertyName)
				{
					return property;
				}
			}

			// second attempt at case-insensitive match
			foreach (var property in properties)
			{
				if (StringComparer.OrdinalIgnoreCase.Compare(property.Name, propertyName) == 0)
				{
					return property;
				}
			}

			return null;
		}
	}

	internal static class PropertyInfoExtensions
	{
		public static bool HasPublicSetter(this PropertyInfo propertyInfo)
		{
			if (!propertyInfo.CanWrite)
			{
				return false;
			}

			var methodInfo = propertyInfo.GetSetMethod();
			if (methodInfo == null || !methodInfo.IsPublic)
			{
				return false;
			}

			return true;
		}
	}
}
