using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Cesto.Tests
{
    [TestFixture]
    public class DisposableCollectionTests
    {
        [Test]
        public void Add_and_dispose()
        {
            var collection = new DisposableCollection();
            var d1 = new Component();
            var d2 = new DodgyDisposable();
            var d3 = new Component();

            collection.Add(d1);
            collection.Add(d2);
            collection.Add(d3);

            var d1Disposed = false;
            d1.Disposed += (sender, args) => d1Disposed = true;
            var d3Disposed = false;
            d3.Disposed += (sender, args) => d3Disposed = true;

            Assert.IsFalse(collection.IsDisposed);
            collection.Dispose();
            Assert.IsTrue(collection.IsDisposed);
            Assert.IsTrue(d1Disposed);
            Assert.IsTrue(d3Disposed);
            Assert.AreEqual(1, d2.DisposeCount);
        }

        [Test]
        public void Throws_ObjectDisposedException()
        {
            var d1 = new Component();
            var collection = new DisposableCollection {d1};
            collection.Dispose();

            Assert.Throws<ObjectDisposedException>(() => collection.Add(d1));
            Assert.Throws<ObjectDisposedException>(() => collection.Insert(0, d1));

            // Cannot test object disposed exception for removal, as the collection
            // is empty after it has been disposed.
            Assert.AreEqual(0, collection.Count);
        }

        [Test]
        public void ConstructorFromEnumerable()
        {
            var list = new List<SomeDisposable>
            {
                new SomeDisposable(),
                new SomeDisposable()
            };

            var coll = new DisposableCollection(list);
            Assert.AreEqual(2, coll.Count);
        }

        private class DodgyDisposable : IDisposable
        {
            public int DisposeCount = 0;

            public void Dispose()
            {
                DisposeCount++;

                // Simulate an object that throws an exception when it is disposed.
                throw new ObjectDisposedException("Test");
            }
        }

        private class SomeDisposable : IDisposable
        {
            public void Dispose() { }
        }
    }
}
