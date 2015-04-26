using System.ComponentModel;
using NUnit.Framework;

namespace Cesto.Tests
{
	[TestFixture]
	public class DisposableExtensionsTests
	{
		[Test]
		public void AddTo()
		{
			var collection = new DisposableCollection();
			var disposable = new Component();
			disposable.AddTo(collection);
			Assert.AreEqual(1, collection.Count);
			Assert.IsTrue(collection.Contains(disposable));
		}
	}
}
