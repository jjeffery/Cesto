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

namespace Cesto.Data
{
	/// <summary>
	/// 	SQL command that is not strongly typed.
	/// </summary>
	public class SqlQuery : SqlQueryBase
	{
		public SqlQuery() : this(null)
		{
		}

		public SqlQuery(IDbCommand cmd) : base(cmd)
		{
		}

		public int ExecuteNonQuery()
		{
			CheckCommand();
			PopulateCommand(Command);
			return CommandExecuteNonQuery(Command);
		}

		public IDataReader ExecuteReader()
		{
			CheckCommand();
			PopulateCommand(Command);
			return DecorateDataReader(CommandExecuteReader(Command));
		}

		/// <summary>
		/// 	Allows the derived class to customise how the command is executed
		/// </summary>
		/// <param name = "cmd">Command populated with command text and parameters</param>
		/// <returns>Number of rows affected by the command</returns>
		protected virtual int CommandExecuteNonQuery(IDbCommand cmd)
		{
			return cmd.ExecuteNonQuery();
		}
	}
}