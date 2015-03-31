using System;
using System.ComponentModel;
using System.Globalization;
using Cesto.WinForms;
using NUnit.Framework;

namespace Cesto.Tests.WinForms
{
	[TestFixture]
	public class DisplaySettingsTests
	{
		public const string CompanyName = "Cesto";
		public const string ProductName = "DisplaySettingsUnitTests";
		public DisplaySettings DisplaySettings;

		[SetUp]
		public void SetUp()
		{
			DisplaySettings = new DisplaySettings("UnitTest");
			DisplaySettings.DeleteAll();
		}

		[TearDown]
		public void TearDown()
		{
			Dispose();
		}

		private void Dispose()
		{
			if (DisplaySettings != null)
			{
				if (!DisplaySettings.IsDisposed)
				{
					DisplaySettings.DeleteAll();
					DisplaySettings.Dispose();
					DisplaySettings = null;
				}
			}
		}

		[Test]
		public void Construct_from_name()
		{
			Assert.AreEqual("UnitTest", DisplaySettings.Name);
		}

		[Test]
		public void Construct_from_type()
		{
			Dispose();
			DisplaySettings = new DisplaySettings(GetType());
			Assert.AreEqual(GetType().FullName, DisplaySettings.Name);
		}

		[Test]
		public void Construct_from_component()
		{
			Dispose();
			using (var component = new Component())
			{
				DisplaySettings = new DisplaySettings(component);
				Assert.AreEqual(component.GetType().FullName, DisplaySettings.Name);
				Assert.IsFalse(DisplaySettings.IsDisposed);
			}
			Assert.IsTrue(DisplaySettings.IsDisposed);
		}

		[Test]
		public void DefaultValues()
		{
			Assert.AreEqual("xyz", DisplaySettings.GetString("XXX", "xyz"));
			Assert.IsNull(DisplaySettings.GetString("XXX"));
			Assert.AreEqual(12, DisplaySettings.GetInt32("XXX", 12));
			Assert.AreEqual(0, DisplaySettings.GetDouble("XXX"));
			Assert.AreEqual(0.7f, DisplaySettings.GetSingle("XXX", 0.7f));
			Assert.AreEqual(-6.5m, DisplaySettings.GetDecimal("XXX", -6.5m));
			Assert.AreEqual(true, DisplaySettings.GetBoolean("XXX", true));
		}

		[Test]
		public void GetSetString()
		{
			Assert.IsNull(DisplaySettings.GetString("XXX"));
			Assert.AreEqual("xyz", DisplaySettings.GetString("XXX", "xyz"));
			DisplaySettings.SetString("XXX", "yyy");
			Assert.AreEqual("yyy", DisplaySettings.GetString("XXX"));
			for (var i = 1; i < 20; ++i)
			{
				DisplaySettings.SetString("XYZ" + i, i.ToString(CultureInfo.InvariantCulture));
				Assert.AreEqual(i.ToString(CultureInfo.InvariantCulture), DisplaySettings.GetString("XYZ" + i));
			}
		}

		[Test]
		public void StringSetting()
		{
			var setting = DisplaySettings.StringSetting("XXX");
			Assert.AreEqual(null, setting.GetValue());
			setting.SetValue("VALUE");
			Assert.AreEqual("VALUE", setting.GetValue());
			Assert.AreEqual("VALUE", DisplaySettings.GetString("XXX"));
			setting.SetValue(setting.DefaultValue);
			Assert.AreEqual(setting.DefaultValue, setting.GetValue());
		}

		[Test]
		public void GetSetInt32()
		{
			Assert.AreEqual(0, DisplaySettings.GetInt32("ZZZ"));
			Assert.AreEqual(24, DisplaySettings.GetInt32("XXXy", 24));
			DisplaySettings.SetInt32("XXX", 32);
			Assert.AreEqual(32, DisplaySettings.GetInt32("XXX"));
			for (var i = 1; i < 20; ++i)
			{
				DisplaySettings.SetInt32("XYZ" + i, i);
				Assert.AreEqual(i, DisplaySettings.GetInt32("XYZ" + i));
			}
		}

		[Test]
		public void Int32Setting()
		{
			var setting = DisplaySettings.Int32Setting("XXX");
			Assert.AreEqual(0, setting.GetValue());
			setting.SetValue(10);
			Assert.AreEqual(10, setting.GetValue());
			Assert.AreEqual(10, DisplaySettings.GetInt32("XXX"));
			setting.SetValue(setting.DefaultValue);
			Assert.AreEqual(setting.DefaultValue, setting.GetValue());
		}

		[Test]
		public void GetSetSingle()
		{
			Assert.AreEqual(0.0f, DisplaySettings.GetSingle("ZZZ"));
			Assert.AreEqual(24.2f, DisplaySettings.GetSingle("XXXy", 24.2f));
			DisplaySettings.SetSingle("XXX", 32.5f);
			Assert.AreEqual(32.5f, DisplaySettings.GetSingle("XXX"));
			for (var value = 1.0f; value < 200000.0f; value += 1021.5f)
			{
				DisplaySettings.SetSingle("XYZ" + value, value);
				Assert.AreEqual(value, DisplaySettings.GetSingle("XYZ" + value));
			}

			// test invalid values
			DisplaySettings.SetString("XYXY", "yeyeyey");
			Assert.AreEqual(0.0, DisplaySettings.GetSingle("XYXY"));
		}

		[Test]
		public void SingleSetting()
		{
			var setting = DisplaySettings.SingleSetting("XXX");
			Assert.AreEqual(0.0f, setting.GetValue());
			setting.SetValue(10.0f);
			Assert.AreEqual(10.0f, setting.GetValue());
			Assert.AreEqual(10.0f, DisplaySettings.GetSingle("XXX"));
			setting.SetValue(setting.DefaultValue);
			Assert.AreEqual(setting.DefaultValue, setting.GetValue());
		}

		[Test]
		public void GetSetDouble()
		{
			Assert.AreEqual(0.0, DisplaySettings.GetDouble("ZZZ"));
			Assert.AreEqual(24.212, DisplaySettings.GetDouble("XXXy", 24.212));
			DisplaySettings.SetDouble("XXX", 32.51515);
			Assert.AreEqual(32.51515, DisplaySettings.GetDouble("XXX"));
			for (var value = -10110.0; value < 200000.0; value += 1021.5)
			{
				DisplaySettings.SetDouble("XYZ" + value, value);
				Assert.AreEqual(value, DisplaySettings.GetDouble("XYZ" + value));
			}

			// test invalid values
			DisplaySettings.SetString("XYXY", "yeyeyey");
			Assert.AreEqual(0.0, DisplaySettings.GetDouble("XYXY"));
		}

		[Test]
		public void DoubleSetting()
		{
			var setting = DisplaySettings.DoubleSetting("XXX");
			Assert.AreEqual(0.0, setting.GetValue());
			setting.SetValue(10.0);
			Assert.AreEqual(10.0, setting.GetValue());
			Assert.AreEqual(10.0, DisplaySettings.GetDouble("XXX"));
			setting.SetValue(setting.DefaultValue);
			Assert.AreEqual(setting.DefaultValue, setting.GetValue());
		}

		[Test]
		public void GetSetDecimal()
		{
			Assert.AreEqual(0.0m, DisplaySettings.GetDecimal("ZZZ"));
			Assert.AreEqual(24.212m, DisplaySettings.GetDecimal("XXXy", 24.212m));
			DisplaySettings.SetDecimal("XXX", 32.51515m);
			Assert.AreEqual(32.51515m, DisplaySettings.GetDecimal("XXX"));
			for (var value = -10110.0m; value < 200000.0m; value += 1021.5m)
			{
				DisplaySettings.SetDecimal("XYZ" + value, value);
				Assert.AreEqual(value, DisplaySettings.GetDecimal("XYZ" + value));
			}

			// test invalid values
			DisplaySettings.SetString("XYXY", "yeyeyey");
			Assert.AreEqual(0.0m, DisplaySettings.GetDecimal("XYXY"));
		}

		[Test]
		public void DecimalSetting()
		{
			var setting = DisplaySettings.DecimalSetting("XXX");
			Assert.AreEqual(0.0m, setting.GetValue());
			setting.SetValue(10.0m);
			Assert.AreEqual(10.0m, setting.GetValue());
			Assert.AreEqual(10.0m, DisplaySettings.GetDecimal("XXX"));
			setting.SetValue(setting.DefaultValue);
			Assert.AreEqual(setting.DefaultValue, setting.GetValue());
		}

		[Test]
		public void GetSetBoolean()
		{
			Assert.AreEqual(false, DisplaySettings.GetBoolean("ZZZ"));
			Assert.AreEqual(true, DisplaySettings.GetBoolean("XXXy", true));
			DisplaySettings.SetBoolean("XXX", true);
			Assert.AreEqual(true, DisplaySettings.GetBoolean("XXX"));
			DisplaySettings.SetBoolean("XXX", false);
			Assert.AreEqual(false, DisplaySettings.GetBoolean("XXX"));

			// test integer values
			DisplaySettings.SetString("XYZ", "0");
			Assert.AreEqual(false, DisplaySettings.GetBoolean("XYZ", true));
			DisplaySettings.SetString("XYZ", "1");
			Assert.AreEqual(true, DisplaySettings.GetBoolean("XYZ"));

			// test invalid values
			DisplaySettings.SetString("XYZ", "ghg");
			Assert.AreEqual(false, DisplaySettings.GetBoolean("XYZ"));
		}

		[Test]
		public void BooleanSetting()
		{
			var setting = DisplaySettings.BooleanSetting("XXX");
			Assert.AreEqual(false, setting.GetValue());
			setting.SetValue(true);
			Assert.AreEqual(true, setting.GetValue());
			Assert.AreEqual(true, DisplaySettings.GetBoolean("XXX"));
			setting.SetValue(false);
			Assert.AreEqual(false, setting.GetValue());
		}

		[Test]
		public void GetSetDateTime()
		{
			Assert.AreEqual(DateTime.MinValue, DisplaySettings.GetDateTime("ZZZ"));
			Assert.AreEqual(DateTime.Parse("2099-12-16 11:47"),
			                DisplaySettings.GetDateTime("XXXy", new DateTime(2099, 12, 16, 11, 47, 0)));
			DisplaySettings.SetDateTime("XXX", new DateTime(2098, 12, 16, 11, 47, 45));
			Assert.AreEqual(new DateTime(2098, 12, 16, 11, 47, 45), DisplaySettings.GetDateTime("XXX"));
			for (var value = new DateTime(1992, 12, 16, 11, 47, 0); value < new DateTime(2099, 12, 31, 23, 59, 59); value += new TimeSpan(699, 12, 15, 31, 500))
			{
				DisplaySettings.SetDateTime("XYZ", value);
				Assert.AreEqual(value, DisplaySettings.GetDateTime("XYZ"));
			}

			// test invalid values
			DisplaySettings.SetString("XYXY", "yeyeyey");
			Assert.AreEqual(DateTime.MinValue, DisplaySettings.GetDateTime("XYXY"));
		}

		[Test]
		public void DateTimeSetting()
		{
			var setting = DisplaySettings.DateTimeSetting("XXX");
			Assert.AreEqual(DateTime.MinValue, setting.GetValue());
			var value = new DateTime(1997, 1, 30, 22, 02, 0);
			setting.SetValue(value);
			Assert.AreEqual(value, setting.GetValue());
			Assert.AreEqual(value, DisplaySettings.GetDateTime("XXX"));
			setting.SetValue(setting.DefaultValue);
			Assert.AreEqual(setting.DefaultValue, DisplaySettings.GetDateTime("XXX"));
		}
	}
}