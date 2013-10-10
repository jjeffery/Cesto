using System;
using NUnit.Framework;

namespace Cesto.Tests
{
	[TestFixture]
	public class VerifyTests
	{
		[Test]
		public void ArgumentNotNull_with_null_parameter()
		{
			string s = null;

			try
			{
				Verify.ArgumentNotNull(s, "s");
				Assert.Fail("Expected ArgumentNullException");
			}
			catch (ArgumentNullException ex)
			{
				Assert.AreEqual("s", ex.ParamName);
			}
		}

		[Test]
		public void ArgumentNotNull_with_non_null_parameter()
		{
			Assert.AreEqual("xyz", Verify.ArgumentNotNull("xyz", "s"));
		}
	}
}