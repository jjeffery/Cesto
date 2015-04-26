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

using System.Collections.Generic;
using System.Threading;

namespace Cesto.Config.Storage
{
	/// <summary>
	/// Configuration storage where all configuration is stored in memory.
	/// Can have an optional parent storage, in which case this acts as a
	/// caching storage.
	/// </summary>
	public class MemoryStorage : IConfigStorage
	{
		/// <summary>
		/// Dictionary containing values for config parameters. If this memory storage
		/// has a parent, then this dictionary acts as a cache.
		/// </summary>
		protected readonly Dictionary<ConfigParameter, ConfigValue> Values
			= new Dictionary<ConfigParameter, ConfigValue>();

		/// <summary>
		/// Lock for accessing the <see cref="Values"/> dictionary.
		/// </summary>
		protected readonly ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();

		/// <summary>
		/// Parent storage. If this is non-null, then this memory storage is a caching
		/// for the parent storage. This improves performance if the parent storage
		/// is slow (for example, storage in a database).
		/// </summary>
		public IConfigStorage Parent { get; private set; }

		public ConfigValue GetValue(ConfigParameter parameter)
		{
			Lock.EnterReadLock();
			try
			{
				return GetValueWithoutLock(parameter);
			}
			finally
			{
				Lock.ExitReadLock();
			}
		}

		public ConfigValue[] GetValues(params ConfigParameter[] parameters)
		{
			if (Parent != null)
			{
				// delegate entirely to the parent
				var parentValues = Parent.GetValues(parameters);

				// update the dictionary with values from the parent
				Lock.EnterWriteLock();
				try
				{
					foreach (var parentValue in parentValues)
					{
						Values[parentValue.Parameter] = parentValue;
					}
				}
				finally
				{
					Lock.ExitWriteLock();
				}

				return parentValues;
			}

			// No parent, so our values are the single point of truth.
			var values = new ConfigValue[parameters.Length];
			Lock.EnterReadLock();
			try
			{
				for (int index = 0; index < values.Length; ++index)
				{
					values[index] = GetValueWithoutLock(parameters[index]);
				}
			}
			finally
			{
				Lock.ExitReadLock();
			}
			return values;
		}

		public bool IsReadOnly
		{
			get
			{
				if (Parent != null)
				{
					return Parent.IsReadOnly;
				}
				return false;
			}
		}

		public void SetValue(ConfigParameter parameter, string value)
		{
			if (Parent != null)
			{
				Parent.SetValue(parameter, value);
			}
			Lock.EnterWriteLock();
			try
			{
				Values[parameter] = new ConfigValue(parameter, value, true);
			}
			finally
			{
				Lock.ExitWriteLock();
			}
		}

		public void Refresh()
		{
			if (Parent != null)
			{
				// Only clear if the parent exists, because if there is no parent
				// we are the only source of configuration information (which should
				// only happen during unit testing).
				Lock.EnterWriteLock();
				try
				{
					Values.Clear();
				}
				finally
				{
					Lock.ExitWriteLock();
				}
			}
		}

		public MemoryStorage(IConfigStorage parent = null)
		{
			Parent = parent;
		}

		private ConfigValue GetValueWithoutLock(ConfigParameter parameter)
		{
			ConfigValue value;
			if (Values.TryGetValue(parameter, out value))
			{
				return value;
			}

			if (Parent == null)
			{
				// No parent and no value
				return new ConfigValue(parameter, null, false);
			}

			value = Parent.GetValue(parameter);
			Values.Add(parameter, value);
			return value;
		}
	}
}
