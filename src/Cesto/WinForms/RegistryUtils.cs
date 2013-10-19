#region Copyright notice

//
// Authors: 
//  John Jeffery <john@jeffery.id.au>
//
// Copyright (C) John Jeffery. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

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