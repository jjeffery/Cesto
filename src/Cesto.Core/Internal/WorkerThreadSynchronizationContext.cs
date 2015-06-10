using System.Threading;
using Cesto.Threading;

// XML comments not required
#pragma warning disable 1591

namespace Cesto.Internal
{
	public class WorkerThreadSynchronizationContext : SynchronizationContext
	{
		private readonly WorkThread _workThread;

		public WorkerThreadSynchronizationContext(WorkThread workThread)
		{
			_workThread = Verify.ArgumentNotNull(workThread, "workerThread");
		}

		public override SynchronizationContext CreateCopy()
		{
			return new WorkerThreadSynchronizationContext(_workThread);
		}

		public override void Post(SendOrPostCallback d, object state)
		{
			_workThread.Post(() => d(state));
		}

		public override void Send(SendOrPostCallback d, object state)
		{
			_workThread.Send(() => d(state));
		}
	}
}
