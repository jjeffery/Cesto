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

using System.Data;

namespace Cesto.Data.Internal
{
	/// <summary>
	/// 	Implementation of <see cref = "ISqlQueryReader{T}" />.
	/// </summary>
	/// <typeparam name = "T"></typeparam>
	internal class QueryReader<T> : ISqlQueryReader<T> where T : class, new()
	{
		private readonly DataRecordConverter _converter;
		private readonly IDataReader _dataReader;
		private readonly T _record;

		public QueryReader(IDataReader dataReader)
		{
			_dataReader = Verify.ArgumentNotNull(dataReader, "dataReader");
			_record = new T();
			_converter = DataRecordConverter.CreateConverterFor(typeof (T), dataReader);
		}

		#region ISqlQueryReader<T> Members

		public void Dispose()
		{
			_dataReader.Dispose();
		}

		public T Record
		{
			get { return _record; }
		}

		public bool Read()
		{
			if (!_dataReader.Read())
			{
				return false;
			}

			_converter.CopyTo(_record);
			return true;
		}

		#endregion
	}
}