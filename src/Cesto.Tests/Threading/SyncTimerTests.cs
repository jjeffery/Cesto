using System;
using System.Threading;
using System.Threading.Tasks;
using Cesto.Threading;
using NUnit.Framework;

namespace Cesto.Tests.Threading
{
	[TestFixture]
	public class SyncTimerTests
	{
		private FakeSynchronizationContext _fakeSynchronizationContext;
		private SynchronizationContext _previousSynchronizationContext;

		[SetUp]
		public void Setup()
		{
			_previousSynchronizationContext = SynchronizationContext.Current;
			_fakeSynchronizationContext = new FakeSynchronizationContext();
			SynchronizationContext.SetSynchronizationContext(_fakeSynchronizationContext);
		}

		[TearDown]
		public void TearDown()
		{
			SynchronizationContext.SetSynchronizationContext(_previousSynchronizationContext);
		}

		[Test]
		public void Throws_if_no_SynchronizationContext()
		{
			SynchronizationContext.SetSynchronizationContext(null);
			// ReSharper disable once ObjectCreationAsStatement
			Assert.Throws<InvalidOperationException>(() => new SyncTimer(() => { }));
		}

		[Test]
		public async Task StartWithTimeSpan()
		{
			var timerExpired = false;
			SynchronizationContext sc = null;
			var syncTimer = new SyncTimer(() => {
				timerExpired = true;
				sc = SynchronizationContext.Current;
			});
			syncTimer.Start(TimeSpan.FromMilliseconds(50));
			await Task.Delay(120);
			Assert.AreEqual(true, timerExpired);
			Assert.AreSame(_fakeSynchronizationContext, sc);
		}

		[Test]
		public async Task StartWithInt32()
		{
			var timerExpired = false;
			SynchronizationContext sc = null;
			var syncTimer = new SyncTimer(() => {
				timerExpired = true;
				sc = SynchronizationContext.Current;
			});
			syncTimer.Start(50);
			await Task.Delay(120);
			Assert.AreEqual(true, timerExpired);
			Assert.AreSame(_fakeSynchronizationContext, sc);
		}

		[Test]
		public async Task Stop()
		{
			var timerExpired = false;
			var syncTimer = new SyncTimer(() => {
				timerExpired = true;
			});
			syncTimer.Start(120);
			await Task.Delay(50);
			syncTimer.Stop();
			await Task.Delay(120);
			Assert.AreEqual(false, timerExpired);
		}

		[Test]
		public async Task StopsWhenDisposed()
		{
			var timerExpired = false;
			var syncTimer = new SyncTimer(() => {
				timerExpired = true;
			});
			syncTimer.Start(500);
			syncTimer.Dispose();
			await Task.Delay(600);
			Assert.AreEqual(false, timerExpired);
		}


		public class FakeSynchronizationContext : SynchronizationContext
		{
			public override void Post(SendOrPostCallback d, object state)
			{
				SendOrPostCallback callback = s => {
					var sc = Current;
					SetSynchronizationContext(this);
					try
					{
						d(state);
					}
					finally
					{
						SetSynchronizationContext(sc);
					}
				};

				base.Post(callback, state);
			}
		}
	}
}
