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
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Cesto.Data.Internal
{
	/// <summary>
	/// Builds subtypes of <see cref="DataRecordConverter"/> using System.Reflection.Emit.
	/// </summary>
	internal class DataRecordConverterTypeBuilder
	{
		private readonly List<string> _errors = new List<string>();

		private readonly Dictionary<DataRecordFieldInfo, PropertyList> _mapFieldToProperty =
			new Dictionary<DataRecordFieldInfo, PropertyList>();

		private readonly Dictionary<PropertyInfo, DataRecordFieldInfo> _mapPropertyToField =
			new Dictionary<PropertyInfo, DataRecordFieldInfo>();

		private readonly DataRecordConverterSpec _queryInfo;

		private class PropertyList
		{
			public readonly List<PropertyInfo> GetProperties = new List<PropertyInfo>();
			public readonly PropertyInfo SetProperty;
			public readonly Type RecordType;
			public readonly string Expression;
			public readonly List<string> Errors = new List<string>();
			private static readonly char[] SplitChars = new[] {'.'};

			public PropertyList(Type recordType, string expression)
			{
				RecordType = Verify.ArgumentNotNull(recordType, "recordType");
				Expression = Verify.ArgumentNotNull(expression, "expression");

				var propertyNames = Expression.Split(SplitChars);

				var type = RecordType;
				for (int index = 0; index < propertyNames.Length - 1; ++index)
				{
					var propertyName = propertyNames[index].Trim();
					var property = type.GetPropertyCaseInsensitive(propertyName);
					if (property == null)
					{
						Errors.Add(string.Format("Type {0} does not have a property named '{1}'", type.FullName, propertyName));
						return;
					}

					GetProperties.Add(property);
					type = property.PropertyType;
				}

				var setPropertyName = propertyNames[propertyNames.Length - 1].Trim();
				SetProperty = type.GetPropertyCaseInsensitive(setPropertyName);
				if (SetProperty == null)
				{
					Errors.Add(string.Format("Type {0} does not have a property named '{1}'", type.FullName, setPropertyName));
				}

				foreach (var property in GetProperties)
				{
					if (!property.CanRead)
					{
						Errors.Add(string.Format("Property is not readable: {0} ({1})", property.Name, property.DeclaringType.FullName));
					}
				}

				if (SetProperty != null && !SetProperty.HasPublicSetter())
				{
					Errors.Add(string.Format("Property {0} is not publicly writable ({1})", SetProperty.Name, SetProperty.DeclaringType.FullName));
				}
			}

			public bool HasErrors
			{
				get { return Errors != null && Errors.Count > 0; }
			}

			public Type PropertyType
			{
				get
				{
					if (SetProperty == null)
					{
						return null;
					}
					return SetProperty.PropertyType;
				}
			}
		}

		public DataRecordConverterTypeBuilder(DataRecordConverterSpec queryInfo)
		{
			_queryInfo = Verify.ArgumentNotNull(queryInfo, "queryInfo");

			PropertyInfo[] properties = queryInfo.RecordType.GetProperties();

			var propertyMap = new NameDictionary<PropertyInfo>();
			var dataRecordMap = new NameDictionary<DataRecordFieldInfo>();

			foreach (DataRecordFieldInfo field in queryInfo.Fields)
			{
				dataRecordMap.Add(field.FieldName, field);
			}

			foreach (PropertyInfo property in properties)
			{
				if (property.HasPublicSetter())
				{
					propertyMap.Add(property.Name, property);
				}
			}

			foreach (DataRecordFieldInfo field in queryInfo.Fields)
			{
				var propertyList = new PropertyList(queryInfo.RecordType, field.FieldName);
				if (propertyList.HasErrors)
				{
					_errors.AddRange(propertyList.Errors);
				}
				else if (DataRecordConverterMethod.CanHandleConversion(field.FieldType, propertyList.PropertyType))
				{
					_mapFieldToProperty.Add(field, propertyList);
				}
				else
				{
					_errors.Add(string.Format("Cannot convert from {0} to {1} for {2}",
					                          field.FieldType.Name, propertyList.PropertyType.Name, field.FieldName));
				}
			}

			foreach (PropertyInfo property in properties)
			{
				if (property.HasPublicSetter())
				{
					DataRecordFieldInfo field;
					if (dataRecordMap.TryGetValue(property.Name, out field))
					{
						_mapPropertyToField.Add(property, field);
					}
					else
					{
						_errors.Add(string.Format("Query result does not have a column named '{0}'",
						                          property.Name));
					}
				}
			}
		}

		public IList<string> Errors
		{
			get { return _errors; }
		}

		public bool CanBuildConverter
		{
			get { return _errors.Count == 0; }
		}

		public Type BuildConverter()
		{
			const TypeAttributes typeAttributes = TypeAttributes.Public |
			                                      TypeAttributes.Class |
			                                      TypeAttributes.AutoClass |
			                                      TypeAttributes.AnsiClass |
			                                      TypeAttributes.BeforeFieldInit |
			                                      TypeAttributes.AutoLayout;

			TypeBuilder typeBuilder = DynamicAssembly.Instance.DefineType("DataRecordConverter", typeAttributes,
			                                                              typeof(DataRecordConverter));

			if (!CanBuildConverter)
			{
				var sb = new StringBuilder("Cannot build converter for ");
				sb.Append(_queryInfo.RecordType.FullName);
				sb.AppendLine(":");
				foreach (string errorMessage in _errors)
				{
					sb.AppendLine(errorMessage);
				}
				throw new InvalidOperationException(sb.ToString());
			}

			BuildConstructor(typeBuilder);
			BuildDoCopyMethod(typeBuilder);
			return typeBuilder.CreateType();
		}

		private static void BuildConstructor(TypeBuilder typeBuilder)
		{
			Type parentType = typeof(DataRecordConverter);
			var parameters = new[] {typeof(IDataReader)};
			ConstructorInfo parentConstructor = parentType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
			                                                              parameters, null);
			const MethodAttributes methodAttributes = MethodAttributes.Public
			                                          | MethodAttributes.SpecialName
			                                          | MethodAttributes.RTSpecialName
			                                          | MethodAttributes.HideBySig;

			ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(methodAttributes,
			                                                                      CallingConventions.Standard,
			                                                                      new[] {typeof(IDataReader)});

			ILGenerator generator = constructorBuilder.GetILGenerator();
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Call, parentConstructor);
			generator.Emit(OpCodes.Ret);
		}

		private void BuildDoCopyMethod(TypeBuilder typeBuilder)
		{
			const MethodAttributes methodAttributes = MethodAttributes.Family
			                                          | MethodAttributes.HideBySig
			                                          | MethodAttributes.Virtual;


			MethodBuilder methodBuilder = typeBuilder.DefineMethod("DoCopy", methodAttributes, CallingConventions.Standard,
			                                                       typeof(void), new[] {typeof(object)});

			ILGenerator generator = methodBuilder.GetILGenerator();
			generator.DeclareLocal(_queryInfo.RecordType);

			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Castclass, _queryInfo.RecordType);
			generator.Emit(OpCodes.Stloc_0);

			foreach (DataRecordFieldInfo fieldInfo in _queryInfo.Fields)
			{
				PropertyList propertyList = _mapFieldToProperty[fieldInfo];
				generator.Emit(OpCodes.Ldloc_0);

				foreach (var getProperty in propertyList.GetProperties)
				{
					generator.Emit(OpCodes.Callvirt, getProperty.GetGetMethod());
				}

				generator.Emit(OpCodes.Ldarg_0);

				switch (fieldInfo.Index)
				{
				case 0:
					generator.Emit(OpCodes.Ldc_I4_0);
					break;
				case 1:
					generator.Emit(OpCodes.Ldc_I4_1);
					break;
				case 2:
					generator.Emit(OpCodes.Ldc_I4_2);
					break;
				case 3:
					generator.Emit(OpCodes.Ldc_I4_3);
					break;
				case 4:
					generator.Emit(OpCodes.Ldc_I4_4);
					break;
				case 5:
					generator.Emit(OpCodes.Ldc_I4_5);
					break;
				case 6:
					generator.Emit(OpCodes.Ldc_I4_6);
					break;
				case 7:
					generator.Emit(OpCodes.Ldc_I4_7);
					break;
				case 8:
					generator.Emit(OpCodes.Ldc_I4_8);
					break;
				default:
					generator.Emit(OpCodes.Ldc_I4, fieldInfo.Index);
					break;
				}

				generator.Emit(OpCodes.Call, DataRecordConverterMethod.GetMethod(fieldInfo.FieldType, propertyList.PropertyType));
				generator.Emit(OpCodes.Callvirt, propertyList.SetProperty.GetSetMethod());
			}

			generator.Emit(OpCodes.Ret);
		}
	}
}