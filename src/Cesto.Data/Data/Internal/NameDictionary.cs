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
using System.Collections;
using System.Collections.Generic;

namespace Cesto.Data.Internal
{
	/// <summary>
	/// A collection class that is case insensitive, but will prefer a case-sensitive match if one exists.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class NameDictionary<T> : IEnumerable<KeyValuePair<string, T>>
	{
		private readonly IDictionary<string, T> _mapCaseSensitive = new Dictionary<string, T>();

		private readonly IDictionary<string, T> _mapIgnoreCase =
			new Dictionary<string, T>(StringComparer.InvariantCultureIgnoreCase);

		public int Count
		{
			get { return _mapIgnoreCase.Count; }
		}

		public T this[string key]
		{
			get
			{
				T value;
				if (!TryGetValue(key, out value))
				{
					throw new KeyNotFoundException();
				}
				return value;
			}
		}

		#region IEnumerable<KeyValuePair<string,T>> Members

		public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
		{
			return _mapCaseSensitive.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _mapCaseSensitive.GetEnumerator();
		}

		#endregion

		public void Clear()
		{
			_mapIgnoreCase.Clear();
			_mapCaseSensitive.Clear();
		}

		public bool Contains(KeyValuePair<string, T> item)
		{
			return _mapCaseSensitive.Contains(item) || _mapIgnoreCase.Contains(item);
		}

		public bool ContainsKey(string key)
		{
			return _mapIgnoreCase.ContainsKey(key);
		}

		public void Add(string key, T value)
		{
			_mapIgnoreCase.Add(key, value);
			_mapCaseSensitive.Add(key, value);
		}

		public bool TryGetValue(string key, out T value)
		{
			return _mapCaseSensitive.TryGetValue(key, out value) || _mapIgnoreCase.TryGetValue(key, out value);
		}
	}
}