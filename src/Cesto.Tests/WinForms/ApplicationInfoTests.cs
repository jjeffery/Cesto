using System;
using System.Windows.Forms;
using Cesto.WinForms;
using NUnit.Framework;

namespace Cesto.Tests.WinForms
{
	[TestFixture]
	public class ApplicationInfoTests
	{
		[Test]
		public void ProductName()
		{
			var val = Guid.NewGuid().ToString();
			Assert.AreEqual(Application.ProductName, ApplicationInfo.ProductName);
			ApplicationInfo.ProductName = val;
			Assert.AreEqual(val, ApplicationInfo.ProductName);
			ApplicationInfo.ProductName = null;
		}

		[Test]
		public void CompanyName()
		{
			var val = Guid.NewGuid().ToString();
			Assert.AreEqual(Application.CompanyName, ApplicationInfo.CompanyName);
			ApplicationInfo.CompanyName = val;
			Assert.AreEqual(val, ApplicationInfo.CompanyName);
			ApplicationInfo.CompanyName = null;
		}

		[Test]
		public void CopyrightText()
		{
			// This is a pretty lame test.
			var val = Guid.NewGuid().ToString();
			ApplicationInfo.CopyrightText = val;
			Assert.AreEqual(val, ApplicationInfo.CopyrightText);
			ApplicationInfo.CopyrightText = null;
		}

		[Test]
		public void WindowTitle()
		{
			var val1 = Guid.NewGuid().ToString();
			var val2 = Guid.NewGuid().ToString();
			Assert.AreEqual(Application.ProductName, ApplicationInfo.WindowTitle);
			ApplicationInfo.ProductName = val1;
			Assert.AreEqual(val1, ApplicationInfo.WindowTitle);
			ApplicationInfo.WindowTitle = val2;
			Assert.AreEqual(val2, ApplicationInfo.WindowTitle);
			ApplicationInfo.ProductName = null;
			ApplicationInfo.WindowTitle = null;
		}

		[Test]
		public void MessageBoxCaption()
		{
			var val1 = Guid.NewGuid().ToString();
			var val2 = Guid.NewGuid().ToString();
			var val3 = Guid.NewGuid().ToString();
			Assert.AreEqual(Application.ProductName, ApplicationInfo.MessageBoxCaption);
			ApplicationInfo.ProductName = val1;
			Assert.AreEqual(val1, ApplicationInfo.MessageBoxCaption);
			ApplicationInfo.WindowTitle = val2;
			Assert.AreEqual(val2, ApplicationInfo.MessageBoxCaption);
			ApplicationInfo.MessageBoxCaption = val3;
			Assert.AreEqual(val3, ApplicationInfo.MessageBoxCaption);
			ApplicationInfo.ProductName = null;
			ApplicationInfo.WindowTitle = null;
			ApplicationInfo.MessageBoxCaption = null;
		}
	}
}