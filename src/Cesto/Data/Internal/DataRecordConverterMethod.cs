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
using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace Cesto.Data.Internal
{
	/// <summary>
	/// 	This helper class is responsible for identifying which method of <see cref = "DataRecordConverter" />
	/// 	can be used to convert from one type in the <see cref = "IDataRecord" /> to a (possibly different) type
	/// 	in the record returned by the <see cref = "SqlQuery{T}" /> class.
	/// </summary>
	internal static class DataRecordConverterMethod
	{
		private static readonly Dictionary<DataConversionKey, MethodInfo> MapTypeToMethod =
			new Dictionary<DataConversionKey, MethodInfo>();

		public static readonly MethodInfo GetBoolean;
		public static readonly MethodInfo GetByte;
		public static readonly MethodInfo GetBytes;
		public static readonly MethodInfo GetChar;
		public static readonly MethodInfo GetDateTime;
		public static readonly MethodInfo GetDecimal;
		public static readonly MethodInfo GetDouble;
		public static readonly MethodInfo GetFloat;
		public static readonly MethodInfo GetGuid;
		public static readonly MethodInfo GetInt16;
		public static readonly MethodInfo GetInt32;
		public static readonly MethodInfo GetInt64;
		public static readonly MethodInfo GetNullableBoolean;
		public static readonly MethodInfo GetNullableByte;
		public static readonly MethodInfo GetNullableChar;
		public static readonly MethodInfo GetNullableDateTime;
		public static readonly MethodInfo GetNullableDecimal;
		public static readonly MethodInfo GetNullableDouble;
		public static readonly MethodInfo GetNullableFloat;
		public static readonly MethodInfo GetNullableGuid;
		public static readonly MethodInfo GetNullableInt32;
		public static readonly MethodInfo GetNullableInt16;
		public static readonly MethodInfo GetNullableInt64;
		public static readonly MethodInfo GetString;
		public static readonly MethodInfo GetEnumFromString;
		public static readonly MethodInfo GetNullableEnumFromString;
		public static readonly MethodInfo GetEnumFromInt32;
		public static readonly MethodInfo GetNullableEnumFromInt32;
		public static readonly MethodInfo GetEnumFromInteger;
		public static readonly MethodInfo GetNullableEnumFromInteger;

		static DataRecordConverterMethod()
		{
			Type type = typeof (DataRecordConverter);

			foreach (MethodInfo method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
			{
				switch (method.Name)
				{
						// The GetEnumXXXX methods are generic, and are not added to the conversion dictionary until
						// necessary
					case "GetEnumFromString":
						GetEnumFromString = method;
						break;

					case "GetNullableEnumFromString":
						GetNullableEnumFromString = method;
						break;

					case "GetEnumFromInt32":
						GetEnumFromInt32 = method;
						break;

					case "GetNullableEnumFromInt32":
						GetNullableEnumFromInt32 = method;
						break;

					case "GetEnumFromInteger":
						GetEnumFromInteger = method;
						break;

					case "GetNullableEnumFromInteger":
						GetNullableEnumFromInteger = method;
						break;

					case "GetBoolean":
						GetBoolean = method;
						AddConversion(typeof (bool), method);
						break;

					case "GetNullableBoolean":
						GetNullableBoolean = method;
						AddConversion(typeof (bool), typeof (bool?), method);
						break;

					case "GetByte":
						GetByte = method;
						AddConversion(typeof (byte), method);
						break;

					case "GetBytes":
						GetBytes = method;
						AddConversion(typeof (byte[]), typeof (byte[]), method);
						break;

					case "GetNullableByte":
						GetNullableByte = method;
						AddConversion(typeof (byte), typeof (byte?), method);
						break;

					case "GetChar":
						GetChar = method;
						AddConversion(typeof (char), method);
						break;

					case "GetNullableChar":
						GetNullableChar = method;
						AddConversion(typeof (char), typeof (char?), method);
						break;

					case "GetDateTime":
						GetDateTime = method;
						AddConversion(typeof (DateTime), method);
						break;

					case "GetNullableDateTime":
						GetNullableDateTime = method;
						AddConversion(typeof (DateTime), typeof (DateTime?), method);
						break;

					case "GetDecimal":
						GetDecimal = method;
						AddConversion(typeof (decimal), method);
						break;

					case "GetNullableDecimal":
						GetNullableDecimal = method;
						AddConversion(typeof (decimal), typeof (decimal?), method);
						break;

					case "GetDouble":
						GetDouble = method;
						AddConversion(typeof (double), method);
						break;

					case "GetNullableDouble":
						GetNullableDouble = method;
						AddConversion(typeof (double), typeof (double?), method);
						break;

					case "GetFloat":
						GetFloat = method;
						AddConversion(typeof (float), method);
						break;

					case "GetNullableFloat":
						GetNullableFloat = method;
						AddConversion(typeof (float), typeof (float?), method);
						break;

					case "GetGuid":
						GetGuid = method;
						AddConversion(typeof (Guid), method);
						break;

					case "GetNullableGuid":
						GetNullableGuid = method;
						AddConversion(typeof (Guid), typeof (Guid?), method);
						break;

					case "GetInt16":
						GetInt16 = method;
						AddConversion(typeof (short), method);
						break;

					case "GetNullableInt16":
						GetNullableInt16 = method;
						AddConversion(typeof (short), typeof (short?), method);
						break;

					case "GetInt32":
						GetInt32 = method;
						AddConversion(typeof (int), method);
						break;

					case "GetInt32FromNumeric":
						AddConversion(typeof (decimal), typeof (int), method);
						AddConversion(typeof (double), typeof (int), method);
						AddConversion(typeof (float), typeof (int), method);
						AddConversion(typeof (short), typeof (int), method);
						AddConversion(typeof (byte), typeof (int), method);
						break;

					case "GetNullableInt32":
						GetNullableInt32 = method;
						AddConversion(typeof (int), typeof (int?), method);
						break;

					case "GetNullableInt32FromNumeric":
						AddConversion(typeof (decimal), typeof (int?), method);
						AddConversion(typeof (double), typeof (int?), method);
						AddConversion(typeof (float), typeof (int?), method);
						AddConversion(typeof (short), typeof (int?), method);
						AddConversion(typeof (byte), typeof (int?), method);
						break;

					case "GetInt64":
						GetInt64 = method;
						AddConversion(typeof (long), method);
						break;

					case "GetNullableInt64":
						GetNullableInt64 = method;
						AddConversion(typeof (long), typeof (long?), method);
						break;

					case "GetString":
						GetString = method;
						AddConversion(typeof (string), method);
						break;
				}
			}
		}

		public static bool CanHandleConversion(Type dataRecordType, Type propertyType)
		{
			return GetMethod(dataRecordType, propertyType) != null;
		}

		public static MethodInfo GetMethod(Type dataRecordType, Type propertyType)
		{
			DataConversionKey key = new DataConversionKey(dataRecordType, propertyType);
			MethodInfo method;

			if (MapTypeToMethod.TryGetValue(key, out method))
			{
				return method;
			}

			if (propertyType.IsEnum)
			{
				return CreateEnumMethod(dataRecordType, propertyType);
			}

			if (propertyType.IsGenericType
			    && propertyType.GetGenericTypeDefinition().Equals(typeof (Nullable<>)))
			{
				NullableConverter nc = new NullableConverter(propertyType);
				if (nc.UnderlyingType.IsEnum)
				{
					return CreateNullableEnumMethod(dataRecordType, propertyType, nc.UnderlyingType);
				}
			}

			// TODO throw exception?
			return null;
		}

		private static MethodInfo CreateEnumMethod(Type dataRecordType, Type propertyType)
		{
			MethodInfo method = null;

			if (dataRecordType == typeof (string))
			{
				method = GetEnumFromString;
			}
			else if (dataRecordType == typeof (int))
			{
				method = GetEnumFromInt32;
			}
			else if (dataRecordType == typeof (byte)
			         || dataRecordType == typeof (char)
			         || dataRecordType == typeof (short)
			         || dataRecordType == typeof (long))
			{
				method = GetEnumFromInteger;
			}

			if (method == null)
			{
				return null;
			}

			MethodInfo genericMethod = method.MakeGenericMethod(propertyType);
			AddConversion(dataRecordType, propertyType, genericMethod);
			return genericMethod;
		}

		private static MethodInfo CreateNullableEnumMethod(Type dataRecordType, Type propertyType, Type propertyUnderlyingType)
		{
			MethodInfo method = null;

			if (dataRecordType == typeof (string))
			{
				method = GetNullableEnumFromString;
			}
			else if (dataRecordType == typeof (int))
			{
				method = GetNullableEnumFromInt32;
			}
			else if (dataRecordType == typeof (byte)
			         || dataRecordType == typeof (char)
			         || dataRecordType == typeof (short)
			         || dataRecordType == typeof (long))
			{
				method = GetNullableEnumFromInteger;
			}

			if (method == null)
			{
				return null;
			}

			MethodInfo genericMethod = method.MakeGenericMethod(propertyUnderlyingType);
			AddConversion(dataRecordType, propertyType, genericMethod);
			return genericMethod;
		}

		private static void AddConversion(Type type, MethodInfo method)
		{
			MapTypeToMethod.Add(new DataConversionKey(type, type), method);
		}

		private static void AddConversion(Type dataRecordType, Type propertyType, MethodInfo method)
		{
			MapTypeToMethod.Add(new DataConversionKey(dataRecordType, propertyType), method);
		}
	}
}