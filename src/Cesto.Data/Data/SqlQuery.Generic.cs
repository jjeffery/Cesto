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

using System.Collections.Generic;
using System.Data;
using Cesto.Data.Internal;

namespace Cesto.Data
{
	/// <summary>
	/// Base class for types that represent an SQL query against a database.
	/// </summary>
	/// <typeparam name="T">
	/// Each record returned from the SQL query is mapped to an instance of this type.
	/// </typeparam>
	public class SqlQuery<T> : SqlQueryBase where T : class, new()
	{
		public SqlQuery() : this(null) {}

		public SqlQuery(IDbCommand cmd) : base(cmd) {}

		/// <summary>
		/// Execute the query and return a single record object.
		/// </summary>
		/// <returns>
		/// A record object, or <c>null</c> if the query resulted in no rows.
		/// </returns>
		public T ExecuteSingle()
		{
			CheckCommand();
			T record = null;
			using (ISqlQueryReader<T> reader = ExecuteReader())
			{
				if (reader.Read())
				{
					record = reader.Record;
				}
			}
			return record;
		}

		/// <summary>
		/// Execute the query and return a list of record objects.
		/// </summary>
		/// <returns>
		/// Returns an <see cref="List{T}"/> collection containing one object for
		/// each row returned by the query. If no rows are returned by the query, then
		/// the list has no items in it.
		/// </returns>
		/// <remarks>
		/// Although it is often considered good practice for methods like this to return a 
		/// collection interface such as <see cref="IList{T}"/> or <see cref="ICollection{T}"/>, 
		/// this method returns <see cref="List{T}"/> on purpose, as a common requirement by a calling 
		/// program is to sort the list or filter it further. Returning a <see cref="List{T}"/> collection
		/// means that the calling program can easily do this without having to make a copy of the 
		/// collection.
		/// </remarks>
		public List<T> ExecuteList()
		{
			CheckCommand();
			PopulateCommand(Command);
			using (IDataReader dataReader = DecorateDataReader(CommandExecuteReader(Command)))
			{
				var list = new List<T>();
				DataRecordConverter converter = DataRecordConverter.CreateConverterFor(typeof(T), dataReader);

				while (dataReader.Read())
				{
					var record = new T();
					converter.CopyTo(record);
					list.Add(record);
				}

				return list;
			}
		}

		/// <summary>
		/// Execute the query and return the result as an <see cref="ISqlQueryReader{T}"/>.
		/// </summary>
		/// <returns>
		/// Returns an <see cref="ISqlQueryReader{T}"/>, which can be used to iterate through the
		/// results. It is the responsibility of the calling program to dispose of this object.
		/// </returns>
		/// <remarks>
		/// This method is useful if the query will return many rows, and it is not necessary to
		/// keep the result as a collection. The <see cref="ISqlQueryReader{T}"/> implementation
		/// saves memory by copying the data from each row into the same record object in turn.
		/// </remarks>
		public ISqlQueryReader<T> ExecuteReader()
		{
			CheckCommand();
			PopulateCommand(Command);
			IDataReader reader = DecorateDataReader(CommandExecuteReader(Command));
			return new QueryReader<T>(reader);
		}
	}
}