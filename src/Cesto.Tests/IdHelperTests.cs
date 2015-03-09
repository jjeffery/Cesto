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
using Cesto.Internal;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace Cesto.Tests
{
	[TestFixture]
	public class IdHelperTests
	{
		[Test]
		public void Value_type_short()
		{
			var helper = new IdHelperForValueType<short>();

			Assert.AreEqual(false, helper.IsNull(0));
			Assert.AreEqual(true, helper.IsDefaultValue(0));
			Assert.AreEqual(false, helper.IsDefaultValue(1));
			Assert.AreEqual(true, helper.AreEqual(0, 0));
			Assert.AreEqual(true, helper.AreEqual(23, 23));
			Assert.AreEqual(-1, helper.Compare(0, 1));
			Assert.AreEqual(+1, helper.Compare(0, -1));
			Assert.That(helper.Compare(0, 0), Is.EqualTo(0));
			Assert.AreEqual(0, helper.Compare(23, 23));
			Assert.AreEqual(0.GetHashCode(), helper.GetHashCode(0));
			Assert.AreEqual(((short)23).GetHashCode(), helper.GetHashCode(23));
		}

		[Test]
		public void Value_type_long()
		{
			var helper = new IdHelperForInt64();

			Assert.AreEqual(false, helper.IsNull(0));
			Assert.AreEqual(true, helper.IsDefaultValue(0));
			Assert.AreEqual(false, helper.IsDefaultValue(1));
			Assert.AreEqual(true, helper.AreEqual(0, 0));
			Assert.AreEqual(true, helper.AreEqual(23, 23));
			Assert.AreEqual(-1, helper.Compare(0, 1));
			Assert.AreEqual(+1, helper.Compare(0, -1));
			Assert.That(helper.Compare(0, 0), Is.EqualTo(0));
			Assert.AreEqual(0, helper.Compare(23, 23));
			Assert.AreEqual(0.GetHashCode(), helper.GetHashCode(0));
			Assert.AreEqual(((long)23).GetHashCode(), helper.GetHashCode(23));
		}


		[Test]
		public void Value_type_int()
		{
			var helper = new IdHelperForInt32();

			Assert.AreEqual(false, helper.IsNull(0));
			Assert.AreEqual(true, helper.IsDefaultValue(0));
			Assert.AreEqual(false, helper.IsDefaultValue(1));
			Assert.AreEqual(true, helper.AreEqual(0, 0));
			Assert.AreEqual(true, helper.AreEqual(23, 23));
			Assert.AreEqual(-1, helper.Compare(0, 1));
			Assert.AreEqual(+1, helper.Compare(0, -1));
			Assert.That(helper.Compare(0, 0), Is.EqualTo(0));
			Assert.AreEqual(0, helper.Compare(23, 23));
			Assert.AreEqual(0.GetHashCode(), helper.GetHashCode(0));
			Assert.AreEqual(23.GetHashCode(), helper.GetHashCode(23));
		}

		[Test]
		public void Value_type_Guid()
		{
			var helper = new IdHelperForValueType<Guid>();

			var guid1 = new Guid("005faa88-a529-4065-9d04-72a96316988b");
			var guid2 = new Guid("64b9dd7a-f46e-448b-b6e5-ddd3f83ec508");

			Assert.AreEqual(false, helper.IsNull(Guid.Empty));
			Assert.AreEqual(true, helper.IsDefaultValue(Guid.Empty));
			Assert.AreEqual(false, helper.IsDefaultValue(guid1));
			Assert.AreEqual(true, helper.AreEqual(Guid.Empty, Guid.Empty));
			Assert.AreEqual(true, helper.AreEqual(guid1, guid1));
			Assert.AreEqual(-1, helper.Compare(Guid.Empty, guid1));
			Assert.AreEqual(+1, helper.Compare(guid2, Guid.Empty));
			Assert.That(helper.Compare(Guid.Empty, Guid.Empty), Is.EqualTo(0));
			Assert.AreEqual(0, helper.Compare(guid2, guid2));
			Assert.AreEqual(Guid.Empty.GetHashCode(), helper.GetHashCode(Guid.Empty));
			Assert.AreEqual(guid1.GetHashCode(), helper.GetHashCode(guid1));
		}

		[Test]
		public void Value_type_string()
		{
			var helper = new IdHelperForString();

			Assert.AreEqual(true, helper.IsNull(null));
			Assert.AreEqual(true, helper.IsDefaultValue(null));
			Assert.AreEqual(false, helper.IsDefaultValue("XX"));
			Assert.AreEqual(true, helper.AreEqual(null, null));
			Assert.AreEqual(false, helper.AreEqual(null, "X"));
			Assert.AreEqual(true, helper.AreEqual("ABCD", "abcd"));
			Assert.AreEqual(true, helper.AreEqual("ABCDE", "ABCDE"));
			Assert.AreEqual(-1, helper.Compare(null, "AAA"));
			Assert.AreEqual(+1, helper.Compare("BBB", null));
			Assert.That(helper.Compare(null, null), Is.EqualTo(0));
			Assert.AreEqual(0, helper.Compare("X", "x"));
			Assert.AreEqual(string.Empty.GetHashCode(), helper.GetHashCode(string.Empty));
			Assert.AreEqual(0, helper.GetHashCode(null));
			Assert.AreEqual("XXX".GetHashCode(), helper.GetHashCode("XXX"));
			Assert.AreEqual(helper.GetHashCode("XXX"), helper.GetHashCode("xxx"));
		}

#if NET40
		[Test]
		public void Value_type_tuple()
		{
			// This is a bit contrived, just need an easy class (ie non-value type)
			var helper = new IdHelperForClassType<Tuple<int, int>>();

			var tuple1 = new Tuple<int, int>(1, 2);
			var tuple2 = new Tuple<int, int>(1, 3);

			var tuple1Again = new Tuple<int, int>(1, 2);

			Assert.AreEqual(true, helper.IsNull(null));
			Assert.AreEqual(true, helper.IsDefaultValue(null));
			Assert.AreEqual(false, helper.IsDefaultValue(tuple1));
			Assert.AreEqual(true, helper.AreEqual(null, null));
			Assert.AreEqual(true, helper.AreEqual(tuple1, tuple1Again));
			Assert.AreEqual(-1, helper.Compare(null, tuple1));
			Assert.AreEqual(-1, helper.Compare(tuple1, tuple2));
			Assert.AreEqual(+1, helper.Compare(tuple2, null));
			Assert.AreEqual(+1, helper.Compare(tuple2, tuple1));
			Assert.That(helper.Compare(null, null), Is.EqualTo(0));
			Assert.AreEqual(0, helper.Compare(tuple1, tuple1Again));
			Assert.AreEqual(tuple1.GetHashCode(), helper.GetHashCode(tuple1));
			Assert.AreEqual(0, helper.GetHashCode(null));
		}
#endif
	}
}
