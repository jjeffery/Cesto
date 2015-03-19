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
using System.Text;
using System.Text.RegularExpressions;

namespace Cesto.Data.Internal
{
	/// <summary>
	/// 	This class is responsible for populating an <see cref = "IDbCommand" /> object with the
	/// 	necessary <see cref = "IDataParameter" /> parameter objects that match the property values
	/// 	in an <see cref = "SqlQuery{T}" /> object.
	/// </summary>
	/// <remarks>
	/// 	The current implementation uses the System.Reflection namespace to query the property values
	/// 	in the <see cref = "SqlQuery{T}" /> object. A future implementation may improve performance by
	/// 	generating dynamic code using the System.Reflection.Emit namespace.
	/// </remarks>
	internal class DataParameterBuilder
	{
		private static readonly Dictionary<Type, DataParameterBuilder> Instances =
			new Dictionary<Type, DataParameterBuilder>();

		private static readonly Regex ParameterNameRegex = new Regex(@"^[a-z_][a-z0-9_]*$", RegexOptions.IgnoreCase);
		private readonly List<string> _errors = new List<string>();
		private readonly Dictionary<string, DataParameterInfo> _namedParameters;
		private readonly DataParameterInfo[] _orderedParameters;
		private readonly Type _queryType;

		public DataParameterBuilder(Type queryType)
		{
			_queryType = Verify.ArgumentNotNull(queryType, "queryType");

			PropertyInfo[] properties = queryType.GetProperties();
			Dictionary<string, DataParameterInfo> namedParameters = null;
			Dictionary<int, DataParameterInfo> orderedParameters = null;

			foreach (PropertyInfo property in properties)
			{
				ParameterAttribute attribute = GetParameterAttribute(property);
				if (attribute.Ignore)
				{
					continue;
				}

				if (DataParameterTypeMapping.ForType(property.PropertyType) == null)
				{
					_errors.Add("Cannot handle property types of " + property.PropertyType.Name);
				}

				if (attribute.ParameterOrder > 0)
				{
					if (orderedParameters == null)
					{
						orderedParameters = new Dictionary<int, DataParameterInfo>();
					}

					if (orderedParameters.ContainsKey(attribute.ParameterOrder))
					{
						string message = string.Format("Properties {0} and {1} both have a parameter order of {2}",
						                               orderedParameters[attribute.ParameterOrder].Property.Name,
						                               property.Name,
						                               attribute.ParameterOrder);
						_errors.Add(message);
					}
					orderedParameters[attribute.ParameterOrder] = new DataParameterInfo(attribute, property);
				}
				else
				{
					if (namedParameters == null)
					{
						namedParameters = new Dictionary<string, DataParameterInfo>();
					}
					string parameterName = SanitizeParameterName(attribute.ParameterName);
					if (namedParameters.ContainsKey(parameterName))
					{
						_errors.Add("Parameter name is defined more than once: " + parameterName);
					}
					namedParameters[parameterName] = new DataParameterInfo(attribute, property);
				}
			}

			if (namedParameters != null && orderedParameters != null)
			{
				_errors.Add("Cannot have a mix of named properties and ordered properties");
			}

			if (namedParameters != null)
			{
				_namedParameters = namedParameters;
			}
			else if (orderedParameters != null)
			{
				var array = new DataParameterInfo[orderedParameters.Count];
				for (int index = 0; index < array.Length; ++index)
				{
					int propertyOrder = index + 1;
					DataParameterInfo property;
					if (orderedParameters.TryGetValue(propertyOrder, out property))
					{
						array[index] = property;
					}
					else
					{
						_errors.Add("Missing property with order of " + propertyOrder);
					}
				}

				_orderedParameters = array;
			}
		}

		public Type QueryType
		{
			get { return _queryType; }
		}

		public bool IsValid
		{
			get { return _errors.Count == 0; }
		}

		public IList<string> Errors
		{
			get { return _errors.AsReadOnly(); }
		}

		public static DataParameterBuilder GetInstance(Type type)
		{
			DataParameterBuilder instance;
			lock (Instances)
			{
				if (!Instances.TryGetValue(type, out instance))
				{
					instance = new DataParameterBuilder(type);
					Instances.Add(type, instance);
				}
			}
			return instance;
		}

		public void PopulateCommand(IDbCommand cmd, object query)
		{
			if (!IsValid)
			{
				var sb = new StringBuilder("Cannot populate command from query object: ");
				foreach (string message in _errors)
				{
					sb.AppendLine(message);
				}
				throw new InvalidOperationException(sb.ToString());
			}
			cmd.Parameters.Clear();
			if (_namedParameters != null)
			{
				foreach (var keyValuePair in _namedParameters)
				{
					string prefix = GetParameterNamePrefix(cmd, keyValuePair.Value);
					string parameterName = prefix + keyValuePair.Key;
					DataParameterInfo parameterInfo = keyValuePair.Value;
					AddParameter(cmd, parameterName, parameterInfo.Property, query);
				}
			}
			else if (_orderedParameters != null)
			{
				foreach (DataParameterInfo parameterInfo in _orderedParameters)
				{
					AddParameter(cmd, string.Empty, parameterInfo.Property, query);
				}
			}
		}

		private static void AddParameter(IDbCommand cmd, string parameterName, PropertyInfo property, object query)
		{
			IDbDataParameter parameter = cmd.CreateParameter();
			parameter.ParameterName = parameterName;
			parameter.Direction = ParameterDirection.Input;
			DataParameterTypeMapping helper = DataParameterTypeMapping.ForType(property.PropertyType);
			parameter.DbType = helper.DbType;
			parameter.Value = helper.GetValue(property.GetValue(query, null));
			cmd.Parameters.Add(parameter);
		}

		private static string GetParameterNamePrefix(IDbCommand cmd, DataParameterInfo parameterInfo)
		{
			string typeName = cmd.GetType().FullName;
			if (typeName.StartsWith("System.Data.SqlClient") ||
			    typeName.StartsWith("System.Data.SqlServerCe"))
			{
				return "@";
			}
			if (typeName.StartsWith("MySql"))
			{
				return "?";
			}

			if (!Char.IsLetterOrDigit(parameterInfo.Attribute.ParameterName[0]))
			{
				return parameterInfo.Attribute.ParameterName[0].ToString();
			}

			// TODO: probably need to look into postgres, firebird, etc
			return ":";
		}

		/// <summary>
		/// 	Gets the <see cref = "ParameterAttribute" /> associated with a property, and if there
		/// 	is none a default attribute is constructed.
		/// </summary>
		private static ParameterAttribute GetParameterAttribute(PropertyInfo property)
		{
			object[] attributes = property.GetCustomAttributes(typeof (ParameterAttribute), true);
			if (attributes != null && attributes.Length > 0)
			{
				return (ParameterAttribute) attributes[0];
			}
			return new ParameterAttribute(property.Name);
		}

		private string SanitizeParameterName(string parameterName)
		{
			if (parameterName.StartsWith("@") || parameterName.StartsWith("?") || parameterName.StartsWith(":"))
			{
				parameterName = parameterName.Substring(1);
			}

			if (!ParameterNameRegex.IsMatch(parameterName))
			{
				string message = string.Format("'{0}' is not a valid parameter name", parameterName);
				_errors.Add(message);
			}

			return parameterName;
		}
	}
}