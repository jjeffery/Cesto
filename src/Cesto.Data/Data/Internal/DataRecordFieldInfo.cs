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
using System.Data;
using System.Text;

namespace Cesto.Data.Internal
{
	/// <summary>
	/// 	Contains information about a field in an <see cref = "IDataRecord" />.
	/// </summary>
	/// <remarks>
	/// 	This type can be used as a value type in the sense that it is immutable, and two 
	/// 	instances with identical property values are considered equal.
	/// </remarks>
	internal class DataRecordFieldInfo
	{
		private readonly int _index;
		private readonly string _fieldName;
		private readonly Type _fieldType;

		public DataRecordFieldInfo(int index, string fieldName, Type fieldType)
		{
			_index = index;
			_fieldName = Verify.ArgumentNotNull(fieldName, "fieldName");
			_fieldType = Verify.ArgumentNotNull(fieldType, "fieldType");
		}

		public int Index
		{
			get { return _index; }
		}

		public string FieldName
		{
			get { return _fieldName; }
		}

		public Type FieldType
		{
			get { return _fieldType; }
		}

		public override bool Equals(object obj)
		{
			var other = obj as DataRecordFieldInfo;
			if (other == null)
			{
				return false;
			}
			return Index == other.Index
			       && FieldName == other.FieldName
			       && FieldType == other.FieldType;
		}

		public override int GetHashCode()
		{
			return (FieldName.GetHashCode() << 16) ^ FieldType.GetHashCode();
		}

		public override string ToString()
		{
			var sb = new StringBuilder(FieldName);
			sb.Append(" (");
			sb.Append(FieldType.Name);
			sb.Append(')');
			return sb.ToString();
		}
	}
}