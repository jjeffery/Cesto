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
using Cesto.Config;
using Cesto.Config.Storage;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace Cesto.Tests
{
	[TestFixture]
	public class ConfigTests
	{
		[TearDown]
		public void TearDown()
		{
			// clear out any parameters from previous tests
			ConfigParameter.Storage = new MemoryStorage();
		}

		[Test]
		public void Name_defaultValue_and_description()
		{
			var param = new StringParameter("name")
				.With(with => with.DefaultValue("default-value")
				                  .Description("The description"));

			Assert.AreEqual("name", param.Name);
			Assert.AreEqual("default-value", param.Extra.DefaultValue);
			Assert.AreEqual("The description", param.Description);
		}

		[Test]
		public void Validation()
		{
			var param1 = new Int32Parameter("p1")
				.With(p => p.Validation(value => {
					if (value < 1 || value > 10)
					{
						return "valid values are in the range [1, 10]";
					}
					return null;
				}));

			var param2 = new Int32Parameter("p2");

			Assert.IsNull(param1.Extra.Validate(1));
			Assert.AreEqual("valid values are in the range [1, 10]", ((IConfigParameter<int>)param1).Validate(11));

			Assert.IsNull(param2.Extra.Validate(0));
			Assert.IsNull(param2.Extra.Validate(int.MaxValue));
		}

		[Test]
		public void ChangeValues()
		{
			IConfigParameter<int> param1 = new Int32Parameter("int32");
			Assert.AreEqual(0, param1.Value);
			param1.SetValue(2);
			Assert.AreEqual(2, param1.Value);

			IConfigParameter<string> param2 = new StringParameter("string");
			Assert.AreEqual(null, param2.Value);
			param2.SetValue("yyy");
			Assert.AreEqual("yyy", param2.Value);

			IConfigParameter<TimeSpan> param3 = new TimeSpanParameter("timespan");
			Assert.AreEqual(TimeSpan.Zero, param3.Value);
			param3.SetValue(TimeSpan.FromDays(1.5));
			Assert.AreEqual(TimeSpan.FromDays(1.5), param3.Value);
		}

		[Test]
		public void WhenChanged()
		{
			var changed = false;
			var param = new Int32Parameter("test1")
				.With(with => with.ChangeAction(() => changed = true));
			Assert.AreEqual(0, param.Value);

			((IConfigParameter<int>)param).SetValue(2);
			Assert.IsTrue(changed);
			Assert.AreEqual(2, param.Value);

			changed = false;
			((IConfigParameter<int>)param).SetValue(2);
			Assert.IsFalse(changed);
			Assert.AreEqual(2, param.Value);
		}

		[Test]
		public void ImplicitConversion()
		{
			var intp = new Int32Parameter("intp").With(p => p.DefaultValue(22));
			var intValue = intp*4;
			Assert.AreEqual(88, intValue);

			var stringp = new StringParameter("stringp").With(p => p.DefaultValue("Hello world"));
			var stringValue = stringp + "!";
			Assert.AreEqual("Hello world!", stringValue);
		}

		[Test]
		public void DerivedValue()
		{
			var derivedValue = new Uri("http://www.google.com");

			var param = new UrlParameter("test")
				.With(with => with.DerivedValue(() => derivedValue));

			Assert.AreEqual(new Uri("http://www.google.com"), param.Value);
			Assert.IsTrue(param.Extra.IsDerived);
			Assert.IsTrue(param.Extra.IsReadOnly);

			Assert.Throws<ReadOnlyConfigException>(() => param.Extra.SetValueText("http://example.com/"));

			var anotherParam = new UrlParameter("test2");
			Assert.IsFalse(anotherParam.Extra.IsDerived);
			Assert.IsFalse(anotherParam.Extra.IsReadOnly);
			anotherParam.Extra.SetValueText("http://example.com/");
		}

		public void ScratchPad()
		{
			Int32Parameter param = new Int32Parameter("name")
				.With(p => p.Description("Some description")
				            .ValidRange(1, 10)
				            .DefaultValue(4)
							.ChangeAction(DoSomething));			
		}

		public static readonly Int32Parameter testParam = new Int32Parameter("test-param")
			.With(with => with.Description("Test parameter in Quokka.Core")
			                  .DefaultValue(1)
			                  .ValidRange(1, 10)
			                  .ChangeAction(() => Console.WriteLine("{0} changed to {1}", testParam.Name, testParam.Value)));

		private void DoSomething() {}
	}
}