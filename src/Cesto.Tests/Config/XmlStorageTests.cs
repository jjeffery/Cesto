using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cesto.Config;
using Cesto.Config.Storage;
using NUnit.Framework;

namespace Cesto.Tests.Config
{
	[TestFixture]
	public class XmlStorageTests
	{
		private readonly Int32Parameter _int32Parameter = new Int32Parameter("Int32");
		private readonly string _filePath = Path.Combine(Path.GetTempPath(), "XmlStorage.config");

		[SetUp]
		public void SetUp()
		{
			if (File.Exists(_filePath))
			{
				File.Delete(_filePath);
			}
			ConfigParameter.DefaultStorage = new XmlStorage(_filePath);
		}

		[TearDown]
		public void TearDown()
		{
			ConfigParameter.DefaultStorage = new MemoryStorage();
		}

		[Test]
		public void Test1()
		{
			Assert.AreEqual(0, _int32Parameter.Value);
			_int32Parameter.Extra.SetValue(42);
			ConfigParameter.DefaultStorage.Refresh();
			Assert.AreEqual(42, _int32Parameter.Value);
		}
	}
}
