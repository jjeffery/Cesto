using System.ComponentModel;
using NUnit.Framework;

namespace Cesto.Tests
{
	[TestFixture]
	public class DisposableExtensionsTests
	{
		[Test]
		public void DisposeWith()
		{
			var disposed = false;
			var disposable = new DisposableAction(() => disposed = true);
			var component = new Component();
			disposable.DisposeWith(component);
			Assert.AreEqual(false, disposed);
			component.Dispose();
			Assert.AreEqual(true, disposed);
		}
	}
}