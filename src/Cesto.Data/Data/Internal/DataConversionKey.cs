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

namespace Cesto.Data.Internal
{
	/// <summary>
	/// 	Tuple class used as a key to dictionary collections keyed on the combination of type of data record field
	/// 	and type of property.
	/// </summary>
	internal class DataConversionKey
	{
		public Type FieldType { get; private set; }
		public Type PropertyType { get; private set; }

		public DataConversionKey(Type fieldType, Type propertyType)
		{
			FieldType = Verify.ArgumentNotNull(fieldType, "fieldType");
			PropertyType = Verify.ArgumentNotNull(propertyType, "propertyType");
		}

		public override bool Equals(object obj)
		{
			var other = obj as DataConversionKey;
			if (other == null)
			{
				return false;
			}

			return other.FieldType == FieldType && other.PropertyType == PropertyType;
		}

		public override int GetHashCode()
		{
			return (FieldType.GetHashCode() << 16) ^ PropertyType.GetHashCode();
		}
	}
}