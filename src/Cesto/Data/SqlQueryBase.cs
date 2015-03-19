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
using System.IO;
using System.Reflection;
using Cesto.Data.DataReader;
using Cesto.Data.Internal;

namespace Cesto.Data
{
	/// <summary>
	/// 	Common base class for <see cref = "SqlQuery{T}" /> and <see cref = "SqlNonQuery" />
	/// </summary>
	public class SqlQueryBase
	{
		protected SqlQueryBase(IDbCommand cmd)
		{
			Command = cmd;
		}

		/// <summary>
		/// 	The <see cref = "IDbCommand" /> object used to send the query to the database.
		/// </summary>
		[IgnoreParameter]
		public IDbCommand Command { get; set; }

		/// <summary>
		/// 	Override this method to set the command text for the query.
		/// </summary>
		/// <param name = "cmd">
		/// 	The <see cref = "IDbCommand" /> object that will be used to execute the query.
		/// </param>
		protected virtual void SetCommandText(IDbCommand cmd)
		{
			cmd.CommandText = GetCommandTextFromResource();
			cmd.CommandType = CommandType.Text;
		}

		/// <summary>
		/// 	Override this method to set the parameters in the <see cref = "IDbCommand" />
		/// 	object prior to executing the query.
		/// </summary>
		/// <param name = "cmd">
		/// 	The <see cref = "IDbCommand" /> object that will be used to execute the query.
		/// </param>
		protected virtual void SetParameters(IDbCommand cmd)
		{
			DataParameterBuilder parameterBuilder = DataParameterBuilder.GetInstance(GetType());
			parameterBuilder.PopulateCommand(cmd, this);
		}

		/// <summary>
		/// 	Retrieve the SQL command text from an embedded resource file. The file name
		/// 	has the same name as the query class with the suffix ".sql".
		/// </summary>
		/// <returns></returns>
		protected string GetCommandTextFromResource()
		{
			return GetCommandTextFromResource(null);
		}

		/// <summary>
		/// 	Retrieve the SQL command text from an embedded resource file relative to the query type.
		/// </summary>
		/// <param name = "fileName">
		/// 	The file name of the embedded resource file. If this parameter is <c>null</c> or
		/// 	empty, then the file name defaults to the name of the query class with the suffix ".sql".
		/// 	For example if the query class is <c>MyQuery</c>, then the default file name is
		/// 	<c>MyQuery.sql</c>.
		/// </param>
		/// <returns>
		/// 	SQL command text.
		/// </returns>
		protected string GetCommandTextFromResource(string fileName)
		{
			Type type = GetType();
			if (string.IsNullOrEmpty(fileName))
			{
				fileName = type.Name + ".sql";
			}
			Assembly assembly = type.Assembly;
			using (Stream stream = assembly.GetManifestResourceStream(type, fileName))
			{
				if (stream == null)
				{
					throw new InvalidOperationException("Cannot find embedded query file: " + fileName);
				}

				using (TextReader reader = new StreamReader(stream))
				{
					return SqlCommandText.Sanitize(reader.ReadToEnd());
				}
			}
		}

		protected void CheckCommand()
		{
			if (Command == null)
			{
				throw new InvalidOperationException("Cannot execute query. Command must be non-null");
			}
		}

		protected void PopulateCommand(IDbCommand cmd)
		{
			SetCommandText(cmd);
			SetParameters(cmd);
		}

		/// <summary>
		/// Allows the derived class to customse how the command is executed
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns>
		/// Returns an <see cref="IDataReader"/> object that contains the result of the query.
		/// </returns>
		protected virtual IDataReader CommandExecuteReader(IDbCommand cmd)
		{
			return cmd.ExecuteReader();
		}

		///<summary>
		///	Provides an opportunity to decorate the <see cref = "IDataReader" /> object
		///</summary>
		///<param name = "reader">
		///	The <see cref = "IDataReader" /> returned by the SQL query
		///</param>
		///<returns>
		///	Returns an <see cref = "IDataReader" /> object that will be used to process the query.
		///</returns>
		///<remarks>
		///	<para>
		///		The default implementation simply returns the <paramref name = "reader" /> object. 
		///	</para>
		///	<para>
		///		This override can be useful if you wish to place a decorator object around the <see cref = "IDataReader" />.
		///		For example, you may choose to use the <see cref = "StringTrimDataReader" /> to ensure that all string fields
		///		have trailing spaces trimmed. You can easily create your own decorator by subclassing 
		///		<see cref = "DataReaderDecorator" />.
		///	</para>
		///</remarks>
		protected virtual IDataReader DecorateDataReader(IDataReader reader)
		{
			return reader;
		}
	}
}