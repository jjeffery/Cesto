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
using System.Collections.Generic;
using System.Data;

namespace Cesto.Data.Internal
{
	/// <summary>
	/// 	Provides a mapping from a CLR type as used by a property of the
	/// 	<see cref = "SqlQuery{T}" /> class into a <see cref = "DbType" /> that
	/// 	can be used with an <see cref = "IDataParameter" />
	/// </summary>
	public class DataParameterTypeMapping
	{
		private static readonly Dictionary<Type, DataParameterTypeMapping> MapTypeToHelper =
			new Dictionary<Type, DataParameterTypeMapping>();

		static DataParameterTypeMapping()
		{
			DefineNullable<bool>(DbType.Boolean);
			DefineNullable<byte>(DbType.Byte);
			DefineNullable<char>(DbType.String);
			DefineNullable<DateTime>(DbType.DateTime);
			DefineNullable<decimal>(DbType.Decimal);
			DefineNullable<double>(DbType.Double);
			DefineNullable<float>(DbType.Single);
			DefineNullable<Guid>(DbType.Guid);
			DefineNullable<short>(DbType.Int16);
			DefineNullable<int>(DbType.Int32);
			DefineNullable<long>(DbType.Int64);
			DefineHelper<string>(DbType.String);
		}

		protected DataParameterTypeMapping(Type type, DbType dbType)
		{
			Type = type;
			DbType = dbType;
		}

		public Type Type { get; private set; }
		public DbType DbType { get; private set; }

		public virtual object GetValue(object obj)
		{
			return obj ?? DBNull.Value;
		}

		public static DataParameterTypeMapping ForType(Type type)
		{
			DataParameterTypeMapping helper;
			MapTypeToHelper.TryGetValue(type, out helper);
			return helper;
		}

		private static void DefineNullable<T>(DbType dbType) where T : struct
		{
			DefineHelper<T>(dbType);
			DefineHelper(new NullableHelper<T>(dbType));
		}

		private static void DefineHelper<T>(DbType dbType)
		{
			DefineHelper(new DataParameterTypeMapping(typeof (T), dbType));
		}

		private static void DefineHelper(DataParameterTypeMapping helper)
		{
			MapTypeToHelper.Add(helper.Type, helper);
		}

		#region Nested type: NullableHelper

		private class NullableHelper<T> : DataParameterTypeMapping where T : struct
		{
			public NullableHelper(DbType dbType) : base(typeof (T?), dbType)
			{
			}

			public override object GetValue(object obj)
			{
				var nullable = (T?) obj;
				if (nullable.HasValue)
				{
					return nullable.Value;
				}
				return DBNull.Value;
			}
		}

		#endregion
	}
}