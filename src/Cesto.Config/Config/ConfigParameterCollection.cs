#region License

// Copyright 2004-2013 John Jeffery <john@jeffery.id.au>
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cesto.Logging;

namespace Cesto.Config
{
	public class ConfigParameterCollection : ICollection<ConfigParameter>
	{
		private readonly HashSet<ConfigParameter> _set = new HashSet<ConfigParameter>();

		public IEnumerator<ConfigParameter> GetEnumerator()
		{
			return _set.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _set.GetEnumerator();
		}

		public void Add(ConfigParameter item)
		{
			_set.Add(item);
		}

		public void Clear()
		{
			_set.Clear();
		}

		public bool Contains(ConfigParameter item)
		{
			return _set.Contains(item);
		}

		public void CopyTo(ConfigParameter[] array, int arrayIndex)
		{
			_set.CopyTo(array, arrayIndex);
		}

		public bool Remove(ConfigParameter item)
		{
			return _set.Remove(item);
		}

		public int Count
		{
			get { return _set.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public void AddRange(IEnumerable<ConfigParameter> configParameters)
		{
			foreach (var configParameter in configParameters)
			{
				_set.Add(configParameter);
			}
		}

		public void AddFromAssembly(Assembly assembly = null)
		{
			if (assembly == null)
			{
				assembly = Assembly.GetCallingAssembly();
			}

			var fields = from t in assembly.GetTypes()
				from f in t.GetFields()
				where f.IsStatic
				where typeof (ConfigParameter).IsAssignableFrom(f.FieldType)
				select f;

			List<string> warnings = null;
			var configParams = new List<ConfigParameter>();

			foreach (var field in fields)
			{
				var parameter = field.GetValue(null) as ConfigParameter;
				if (parameter == null)
				{
					if (warnings == null)
					{
						warnings = new List<string>();
					}
					warnings.Add(string.Format("Ignoring parameter defined in {0}.{1} because its value is null",
						GetFullName(field),
						field.Name));
					continue;
				}

				if (!field.IsInitOnly)
				{
					if (warnings == null)
					{
						warnings = new List<string>();
					}
					warnings.Add(string.Format("Ignoring parameter {0} defined in {1}.{2} because it is not defined as readonly",
						parameter.Name,
						GetFullName(field),
						field.Name));
					continue;
				}

				configParams.Add(parameter);
			}

			if (warnings != null)
			{
				var logger = InternalLogManager.GetLogger(typeof (ConfigParameterCollection));
				foreach (var warning in warnings)
				{
					logger.Warn(warning);
				}
			}

			AddRange(configParams);
		}

		private static string GetFullName(FieldInfo field)
		{
			if (field.DeclaringType == null)
			{
				// shouldn't happen
				return "(null)";
			}
			return field.DeclaringType.FullName;
		}
	}
}
