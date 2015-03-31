using System;
using NUnit.Framework;

namespace Cesto.Tests
{
	[TestFixture]
	public class DisposableActionTests
	{
		public void Null_action()
		{
			// This is a pretty lame test -- really just verifies that
			// nothing happens. No NullReferenceException, for example
			using (new DisposableAction(null))
			{
			}
		}

		[Test]
		public void Non_null_action()
		{
			bool didSomething = false;
			var action = new Action(() => didSomething = true);
			using (new DisposableAction(action))
			{
				Assert.IsFalse(didSomething);
			}

			Assert.IsTrue(didSomething);
		}
	}
}