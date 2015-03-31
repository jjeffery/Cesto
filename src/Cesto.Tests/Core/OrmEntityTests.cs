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
using NUnit.Framework;

namespace Cesto.Tests.Core
{
    [TestFixture]
    public class OrmEntityTests
    {
        [Test]
        public void Int32Tests()
        {
            var id1A = new Int32Entity(1);
            var id1B = new Int32Entity(1);
            Assert.AreEqual(id1A, id1B);
            Assert.IsFalse(id1A != id1B);
            Assert.IsTrue(id1A == id1B);

            // default values mean entities are not equal
            var id0A = new Int32Entity(0);
            var id0B = new Int32Entity(0);
            Assert.AreNotEqual(id0A, id0B);
            Assert.IsTrue(id0A != id0B);
            Assert.IsFalse(id0A == id0B);

            var id2 = new Int32Entity(2);
            Assert.AreNotEqual(id1A, id2);
            Assert.IsTrue(id1A != id2);
            Assert.IsFalse(id1A == id2);

            Assert.That(id0A.CompareTo(id1A), Is.LessThan(0));
            Assert.That(id1A.CompareTo(id2), Is.LessThan(0));
            Assert.That(id2.CompareTo(id1B), Is.GreaterThan(0));
        }

        [Test]
        public void Int64Tests()
        {
            var id1A = new Int64Entity(1);
            var id1B = new Int64Entity(1);
            Assert.AreEqual(id1A, id1B);
            Assert.IsFalse(id1A != id1B);
            Assert.IsTrue(id1A == id1B);

            // default values mean entities are not equal
            var id0A = new Int64Entity(0);
            var id0B = new Int64Entity(0);
            Assert.AreNotEqual(id0A, id0B);
            Assert.IsTrue(id0A != id0B);
            Assert.IsFalse(id0A == id0B);

            var id2 = new Int64Entity(2);
            Assert.AreNotEqual(id1A, id2);
            Assert.IsTrue(id1A != id2);
            Assert.IsFalse(id1A == id2);

            Assert.That(id0A.CompareTo(id1A), Is.LessThan(0));
            Assert.That(id1A.CompareTo(id2), Is.LessThan(0));
            Assert.That(id2.CompareTo(id1B), Is.GreaterThan(0));
        }

        [Test]
        public void ValueTypeTests()
        {
            var id1A = new EnumEntity(EnumType.Enum1);
            var id1B = new EnumEntity(EnumType.Enum1);
            Assert.AreEqual(id1A, id1B);
            Assert.IsFalse(id1A != id1B);
            Assert.IsTrue(id1A == id1B);

            // default values mean entities are not equal
            var id0A = new EnumEntity(default(EnumType));
            var id0B = new EnumEntity(default(EnumType));
            Assert.AreNotEqual(id0A, id0B);
            Assert.IsTrue(id0A != id0B);
            Assert.IsFalse(id0A == id0B);

            var id2 = new EnumEntity(EnumType.Enum2);
            Assert.AreNotEqual(id1A, id2);
            Assert.IsTrue(id1A != id2);
            Assert.IsFalse(id1A == id2);

            Assert.That(id0A.CompareTo(id1A), Is.LessThan(0));
            Assert.That(id1A.CompareTo(id2), Is.LessThan(0));
            Assert.That(id2.CompareTo(id1B), Is.GreaterThan(0));
        }

        [Test]
        public void GuidTests()
        {
            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();

            // We want guid1 < guid2
            if (guid1.CompareTo(guid2) > 0)
            {
                var temp = guid1;
                guid1 = guid2;
                guid2 = temp;
            }
            Assert.IsTrue(guid1.CompareTo(guid2) < 0);

            var id1A = new GuidEntity(guid1);
            var id1B = new GuidEntity(guid1);
            Assert.AreEqual(id1A, id1B);
            Assert.IsFalse(id1A != id1B);
            Assert.IsTrue(id1A == id1B);

            // default values mean entities are not equal
            var id0A = new GuidEntity(default(Guid));
            var id0B = new GuidEntity(default(Guid));
            Assert.AreNotEqual(id0A, id0B);
            Assert.IsTrue(id0A != id0B);
            Assert.IsFalse(id0A == id0B);

            var id2 = new GuidEntity(guid2);
            Assert.AreNotEqual(id1A, id2);
            Assert.IsTrue(id1A != id2);
            Assert.IsFalse(id1A == id2);

            Assert.That(id0A.CompareTo(id1A), Is.LessThan(0));
            Assert.That(id1A.CompareTo(id2), Is.LessThan(0));
            Assert.That(id2.CompareTo(id1B), Is.GreaterThan(0));
        }

        [Test]
        public void StringTests()
        {
            var id1A = new StringEntity("XX");
            var id1B = new StringEntity("xx");
            Assert.AreEqual(id1A, id1B);
            Assert.IsFalse(id1A != id1B);
            Assert.IsTrue(id1A == id1B);

            // default values mean entities are not equal
            var id0A = new StringEntity(null);
            var id0B = new StringEntity(null);
            Assert.AreNotEqual(id0A, id0B);
            Assert.IsTrue(id0A != id0B);
            Assert.IsFalse(id0A == id0B);

            var id2 = new StringEntity("ZZ");
            Assert.AreNotEqual(id1A, id2);
            Assert.IsTrue(id1A != id2);
            Assert.IsFalse(id1A == id2);

            Assert.That(id0A.CompareTo(id1A), Is.LessThan(0));
            Assert.That(id1A.CompareTo(id2), Is.LessThan(0));
            Assert.That(id2.CompareTo(id1B), Is.GreaterThan(0));
        }

        [Test]
        public void GetHashCode_returns_same_value_if_id_changes()
        {
            var entity = new EntityWhoseIdCanChange {Id = 1};

            var hashCode1 = entity.GetHashCode();
            var hashCode2 = entity.GetHashCode();

            Assert.AreEqual(hashCode1, hashCode2);

            entity.Id = 2;
            Assert.AreEqual(hashCode1, entity.GetHashCode());
        }

        public class EntityWhoseIdCanChange : OrmEntity<EntityWhoseIdCanChange, int>
        {
            public int Id;

            protected override int GetId()
            {
                return Id;
            }
        }

        public class Int32Entity : OrmEntity<Int32Entity, int>
        {
            public readonly int Id;

            public Int32Entity(int id)
            {
                Id = id;
            }

            protected override int GetId()
            {
                return Id;
            }
        }

        public class Int64Entity : OrmEntity<Int64Entity, long>
        {
            public readonly long Id;

            public Int64Entity(long id)
            {
                Id = id;
            }

            protected override long GetId()
            {
                return Id;
            }
        }

        public class StringEntity : OrmEntity<StringEntity, string>
        {
            public readonly string Id;

            public StringEntity(string id)
            {
                Id = id;
            }

            protected override string GetId()
            {
                return Id;
            }
        }

        public enum EnumType
        {
            Enum1 = 1,
            Enum2 = 2,
        }

        public class EnumEntity : OrmEntity<EnumEntity, EnumType>
        {
            public readonly EnumType Id;

            public EnumEntity(EnumType id)
            {
                Id = id;
            }

            protected override EnumType GetId()
            {
                return Id;
            }
        }

        public class GuidEntity : OrmEntity<GuidEntity, Guid>
        {
            public readonly Guid Id;

            public GuidEntity(Guid id)
            {
                Id = id;
            }

            protected override Guid GetId()
            {
                return Id;
            }
        }
    }
}
