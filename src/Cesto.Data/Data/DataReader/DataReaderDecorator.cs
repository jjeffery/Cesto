#region License

// Copyright 2004-2014 John Jeffery
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

namespace Cesto.Data.DataReader
{
	/// <summary>
	///		Base class for decorating an <see cref="IDataReader"/>
	/// </summary>
	/// <remarks>
	///		Provides a convenient base class for creating classes that
	///		implement <see cref="IDataReader"/>, which can be used to
	///		'decorate' another data reader. One useful example is 
	///		<see cref="StringTrimDataReader"/>.
	/// </remarks>
	public class DataReaderDecorator : IDataReader
	{
		public DataReaderDecorator(IDataReader inner)
		{
			Inner = Verify.ArgumentNotNull(inner, "inner");
		}

		public IDataReader Inner { get; private set; }

		public virtual void Dispose()
		{
			Inner.Dispose();
		}

		public virtual string GetName(int i)
		{
			return Inner.GetName(i);
		}

		public virtual string GetDataTypeName(int i)
		{
			return Inner.GetDataTypeName(i);
		}

		public virtual Type GetFieldType(int i)
		{
			return Inner.GetFieldType(i);
		}

		public virtual object GetValue(int i)
		{
			return Inner.GetValue(i);
		}

		public virtual int GetValues(object[] values)
		{
			return Inner.GetValues(values);
		}

		public virtual int GetOrdinal(string name)
		{
			return Inner.GetOrdinal(name);
		}

		public virtual bool GetBoolean(int i)
		{
			return Inner.GetBoolean(i);
		}

		public virtual byte GetByte(int i)
		{
			return Inner.GetByte(i);
		}

		public virtual long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			return Inner.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
		}

		public virtual char GetChar(int i)
		{
			return Inner.GetChar(i);
		}

		public virtual long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			return Inner.GetChars(i, fieldoffset, buffer, bufferoffset, length);
		}

		public virtual Guid GetGuid(int i)
		{
			return Inner.GetGuid(i);
		}

		public virtual short GetInt16(int i)
		{
			return Inner.GetInt16(i);
		}

		public virtual int GetInt32(int i)
		{
			return Inner.GetInt32(i);
		}

		public virtual long GetInt64(int i)
		{
			return Inner.GetInt64(i);
		}

		public virtual float GetFloat(int i)
		{
			return Inner.GetFloat(i);
		}

		public virtual double GetDouble(int i)
		{
			return Inner.GetDouble(i);
		}

		public virtual string GetString(int i)
		{
			return Inner.GetString(i);
		}

		public virtual decimal GetDecimal(int i)
		{
			return Inner.GetDecimal(i);
		}

		public virtual DateTime GetDateTime(int i)
		{
			return Inner.GetDateTime(i);
		}

		public virtual IDataReader GetData(int i)
		{
			return Inner.GetData(i);
		}

		public virtual bool IsDBNull(int i)
		{
			return Inner.IsDBNull(i);
		}

		public virtual int FieldCount
		{
			get { return Inner.FieldCount; }
		}

		object IDataRecord.this[int i]
		{
			get { return Inner[i]; }
		}

		object IDataRecord.this[string name]
		{
			get { return Inner[name]; }
		}

		public virtual void Close()
		{
			Inner.Close();
		}

		public virtual DataTable GetSchemaTable()
		{
			return Inner.GetSchemaTable();
		}

		public virtual bool NextResult()
		{
			return Inner.NextResult();
		}

		public virtual bool Read()
		{
			return Inner.Read();
		}

		public virtual int Depth
		{
			get { return Inner.Depth; }
		}

		public virtual bool IsClosed
		{
			get { return Inner.IsClosed; }
		}

		public virtual int RecordsAffected
		{
			get { return Inner.RecordsAffected; }
		}
	}
}