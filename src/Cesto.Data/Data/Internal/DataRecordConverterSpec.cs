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
	/// 	Provides the specification for building a type derived from <see cref = "DataRecordConverter" />. Any two
	/// 	<see cref = "SqlQuery" /> classes that generate equivalent <see cref = "DataRecordConverterSpec" /> objects
	/// 	can use the same <see cref = "DataRecordConverter" /> derived type.
	/// </summary>
	/// <remarks>
	/// 	This class is designed to act as a type because it is immutable, and two instances with identical 
	/// 	values are considered equal. It is used as the key for dictionary collections.
	/// </remarks>
	internal class DataRecordConverterSpec
	{
		private readonly IList<DataRecordFieldInfo> _fields;
		private readonly Type _recordType;

		public DataRecordConverterSpec(IDataRecord dataRecord, Type recordType)
		{
			var fields = new List<DataRecordFieldInfo>(dataRecord.FieldCount);
			for (int index = 0; index < dataRecord.FieldCount; ++index)
			{
				fields.Add(new DataRecordFieldInfo(index, dataRecord.GetName(index), dataRecord.GetFieldType(index)));
			}

			_recordType = recordType;
			_fields = fields.AsReadOnly();
		}

		public Type RecordType
		{
			get { return _recordType; }
		}

		public IList<DataRecordFieldInfo> Fields
		{
			get { return _fields; }
		}

		public override bool Equals(object obj)
		{
			var other = obj as DataRecordConverterSpec;
			if (other == null)
			{
				return false;
			}

			if (other.RecordType != RecordType)
			{
				return false;
			}

			if (other.Fields.Count != Fields.Count)
			{
				return false;
			}

			for (int index = Fields.Count - 1; index >= 0; --index)
			{
				if (other.Fields[index] != Fields[index])
				{
					return false;
				}
			}

			return true;
		}

		public override int GetHashCode()
		{
			return RecordType.GetHashCode();
		}
	}
}