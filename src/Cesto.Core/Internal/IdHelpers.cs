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

using System;

// disable warning about missing XML comments for public members
#pragma warning disable 1591


namespace Cesto.Internal
{
    /// <summary>
    /// Interface that provides helper methods for NHibernate identity values.
    /// </summary>
    /// <typeparam name="TId">
    /// Type of entity identifer, usually <c>int</c> or <c>string</c>.
    /// </typeparam>
    public interface IIdHelper<in TId>
    {
        bool IsDefaultValue(TId id);
        bool IsNull(TId id);
        int Compare(TId id1, TId id2);
        bool AreEqual(TId id1, TId id2);
        int GetHashCode(TId id);
    }

    #region IdHelperForClassType

    /// <summary>
    /// Implementation of <see cref="IIdHelper{T}"/> that works for any class type.
    /// </summary>
    /// <typeparam name="TId">
    /// Type of entity identifier.
    /// </typeparam>
    public class IdHelperForClassType<TId> : IIdHelper<TId> where TId : class, IComparable
    {
        public bool IsDefaultValue(TId id)
        {
            return id == null;
        }

        public bool IsNull(TId id)
        {
            return id == null;
        }

        public int Compare(TId id1, TId id2)
        {
            if (id1 == null) {
                if (id2 == null) {
                    return 0;
                }
                return -1;
            }
            if (id2 == null) {
                return 1;
            }
            return id1.CompareTo(id2);
        }

        public bool AreEqual(TId id1, TId id2)
        {
            if (id1 == null) {
                if (id2 == null) {
                    return true;
                }
                return false;
            }
            return id1.Equals(id2);
        }

        public int GetHashCode(TId id)
        {
            if (id == null) {
                return 0;
            }
            return id.GetHashCode();
        }
    }

    #endregion

    #region IdHelperForInt32

    /// <summary>
    /// Implementation of <see cref="IIdHelper{T}"/> that is optimised for <see cref="Int32"/>.
    /// </summary>
    public class IdHelperForInt32 : IIdHelper<int>
    {
        public bool IsDefaultValue(int id)
        {
            return id == 0;
        }

        public bool IsNull(int id)
        {
            return false;
        }

        public virtual int Compare(int id1, int id2)
        {
            return id1 - id2;
        }

        public bool AreEqual(int id1, int id2)
        {
            return id1 == id2;
        }

        public int GetHashCode(int id)
        {
            return id.GetHashCode();
        }
    }

    #endregion

    #region IdHelperForInt64

    /// <summary>
    /// Implementation of <see cref="IIdHelper{T}"/> that is optimised for <see cref="Int64"/>.
    /// </summary>
    public class IdHelperForInt64 : IIdHelper<long>
    {
        public bool IsDefaultValue(long id)
        {
            return id == 0;
        }

        public bool IsNull(long id)
        {
            return false;
        }

        public virtual int Compare(long id1, long id2)
        {
            return id1.CompareTo(id2);
        }

        public bool AreEqual(long id1, long id2)
        {
            return id1 == id2;
        }

        public int GetHashCode(long id)
        {
            return id.GetHashCode();
        }
    }

    #endregion

    #region IdHelperForString

    /// <summary>
    /// Implementation of <see cref="IIdHelper{T}"/> that works for a string.
    /// </summary>
    /// <remarks>
    /// TODO: String comparisons are invariant culture, case-insensitive. This works
    /// well for databases where string comparison is case-sensitive, but may not be 
    /// appropriate in all cases.
    /// </remarks>
    public class IdHelperForString : IIdHelper<string>
    {
        public bool IsDefaultValue(string id)
        {
            return id == null;
        }

        public bool IsNull(string id)
        {
            return id == null;
        }

        public int Compare(string id1, string id2)
        {
            if (id1 == null) {
                if (id2 == null) {
                    return 0;
                }
                return -1;
            }
            if (id2 == null) {
                return 1;
            }

            // Needs to be invariant culture to work with GetHashCode
            return StringComparer.InvariantCultureIgnoreCase.Compare(id1, id2);
        }

        public bool AreEqual(string id1, string id2)
        {
            if (id1 == null) {
                if (id2 == null) {
                    return true;
                }
                return false;
            }
            // Needs to be invariant culture to work with GetHashCode
            return StringComparer.InvariantCultureIgnoreCase.Compare(id1, id2) == 0;
        }

        public int GetHashCode(string id)
        {
            if (id == null) {
                return 0;
            }
            return id.ToUpperInvariant().GetHashCode();
        }
    }

    #endregion

    #region IdHelperForValueType

    /// <summary>
    /// Implementation of <see cref="IIdHelper{T}"/> that works for any value type.
    /// </summary>
    /// <typeparam name="TId">
    /// Type of entity identifier.
    /// </typeparam>
    public class IdHelperForValueType<TId> : IIdHelper<TId> where TId : struct, IComparable
    {
        // ReSharper disable StaticFieldInGenericType
        private static readonly object BoxedDefaultType = default(TId);
        // ReSharper restore StaticFieldInGenericType

        public bool IsDefaultValue(TId id)
        {
            return id.Equals(BoxedDefaultType);
        }

        public bool IsNull(TId id)
        {
            return false;
        }

        public virtual int Compare(TId id1, TId id2)
        {
            return id1.CompareTo(id2);
        }

        public bool AreEqual(TId id1, TId id2)
        {
            return id1.Equals(id2);
        }

        public int GetHashCode(TId id)
        {
            var x = id.GetHashCode();
            return x;
        }
    }

    #endregion

}
