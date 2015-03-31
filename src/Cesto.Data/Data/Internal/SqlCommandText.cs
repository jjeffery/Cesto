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
using System.Text.RegularExpressions;

namespace Cesto.Data.Internal
{
	/// <summary>
	/// 	Used for preprocessing the SQL text prior to sending it to the data source
	/// </summary>
	internal static class SqlCommandText
	{
		private static readonly Regex StartQueryRegex = new Regex(@"^\s*--\s*start\s*query\s*",
		                                                          RegexOptions.IgnoreCase | RegexOptions.Multiline);

		public static string Sanitize(string rawSql)
		{
			if (String.IsNullOrEmpty(rawSql))
			{
				return string.Empty;
			}

			var match = StartQueryRegex.Match(rawSql);
			if (match.Success)
			{
				return rawSql.Substring(match.Index);
			}

			// match not found, return original SQL
			return rawSql;
		}
	}
}