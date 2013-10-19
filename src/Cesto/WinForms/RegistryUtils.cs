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
using System.Text;

namespace Cesto.WinForms
{
	/// <summary>
	///     Some simple Registry utility functions.
	/// </summary>
	public static class RegistryUtils
	{
		private static string _basePath;

		/// <summary>
		///     The base path. Defaults to the value returned by <see cref="GetDefaultBasePath" />,
		///     but can be overridden.
		/// </summary>
		public static string BasePath
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(_basePath))
				{
					return _basePath;
				}

				return GetDefaultBasePath();
			}
			set { _basePath = value; }
		}

		/// <summary>
		///     Returns the path underneath the <see cref="BasePath" /> with the
		///     specified <paramref name="subKeyName" />.
		/// </summary>
		/// <param name="subKeyName">Name of the sub key.</param>
		/// <returns>
		///     If <see cref="BasePath" /> is "SOFTWARE\Company\Product" then the sub key
		///     with name "SubKey" would have a path "SOFTWARE\Company\Product\SubKey".
		/// </returns>
		public static string SubKeyPath(string subKeyName)
		{
			var sb = new StringBuilder(BasePath);
			if (!String.IsNullOrEmpty(subKeyName))
			{
				sb.Append('\\');
				sb.Append(subKeyName);
			}
			return sb.ToString();
		}

		/// <summary>
		///     Returns the path underneath the <see cref="BasePath" /> with the
		///     specified <paramref name="subKeyNames" />.
		/// </summary>
		/// <param name="subKeyNames">Names of the sub keys.</param>
		/// <returns>
		///     If <see cref="BasePath" /> is "SOFTWARE\Company\Product" then the sub key
		///     with names "SubKey1", "SubKey2" and "SubKey3" would have a path
		///     "SOFTWARE\Company\Product\SubKey1\SubKey2\SubKey3".
		/// </returns>
		public static string SubKeyPath(params string[] subKeyNames)
		{
			var sb = new StringBuilder(BasePath);
			foreach (string subKeyName in subKeyNames)
			{
				if (!String.IsNullOrEmpty(subKeyName))
				{
					sb.Append('\\');
					sb.Append(subKeyName);
				}
			}
			return sb.ToString();
		}

		/// <summary>
		///     Default base path: SOFTWARE\CompanyName\ProductName -- if CompanyName or ProductName are
		///     omitted then they are not included.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		///     Thrown when <see cref="ApplicationInfo.CompanyName" /> and <see cref="ApplicationInfo.ProductName" />
		///     are both null or empty.
		/// </exception>
		public static string GetDefaultBasePath()
		{
			if (string.IsNullOrWhiteSpace(ApplicationInfo.CompanyName)
			    && string.IsNullOrWhiteSpace(ApplicationInfo.ProductName))
			{
				throw new InvalidOperationException(
					"CompanyName and ProductName are both null. Cannot infer RegistryUtils.BasePath");
			}

			var sb = new StringBuilder(@"SOFTWARE");
			if (!string.IsNullOrWhiteSpace(ApplicationInfo.CompanyName))
			{
				sb.Append('\\');
				sb.Append(ApplicationInfo.CompanyName);
			}
			if (!string.IsNullOrWhiteSpace(ApplicationInfo.ProductName))
			{
				sb.Append('\\');
				sb.Append(ApplicationInfo.ProductName);
			}
			return sb.ToString();
		}
	}
}