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

namespace Cesto
{
	/// <summary>
	///     Methods for consistency checking.
	/// </summary>
	public static class Verify
	{
		/// <summary>
		///     Checks whether an argument is null.
		/// </summary>
		/// <typeparam name="T">
		///     The argument type, must be a class. This type parameter can usually be inferred
		///     and so it does not need to be explicitly set.
		/// </typeparam>
		/// <param name="param">The parameter, which should not be null.</param>
		/// <param name="paramName">The name of the parameter</param>
		/// <returns>
		///     Returns <paramref name="param" />, if it is not null.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///     Thrown if <paramref name="param" /> is null.
		/// </exception>
		public static T ArgumentNotNull<T>(T param, string paramName) where T : class
		{
			if (param == null)
			{
				throw new ArgumentNullException(paramName);
			}
			return param;
		}
	}
}