using System;
using Cesto.WinForms;
using NUnit.Framework;

namespace Cesto.Tests.WinForms
{
	[TestFixture]
	public class RegistryUtilsTests
	{
		[Test]
		public void GetDefaultBasePath()
		{
			try
			{
				ApplicationInfo.ProductName = Guid.NewGuid().ToString();
				ApplicationInfo.CompanyName = "CompanyName";

				Assert.AreEqual(string.Format(@"SOFTWARE\{0}\{1}", ApplicationInfo.CompanyName, ApplicationInfo.ProductName),
				                RegistryUtils.GetDefaultBasePath());
			}
			finally
			{
				ApplicationInfo.ProductName = null;
				ApplicationInfo.CompanyName = null;
			}
		}
	}
}