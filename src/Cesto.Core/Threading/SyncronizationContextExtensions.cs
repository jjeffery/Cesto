using System;
using System.Threading;

namespace Cesto.Threading
{
	/// <summary>
	/// Extension methods for <see cref="SynchronizationContext"/>
	/// </summary>
	public static class SyncronizationContextExtensions
	{
		/// <summary>
		/// Post an <see cref="Action"/> to a <see cref="SynchronizationContext"/>.
		/// It is often more convenient to post an <see cref="Action"/> than a <see cref="SendOrPostCallback"/>.
		/// </summary>
		/// <param name="synchronizationContext">Synchronization context</param>
		/// <param name="action">Action to perform</param>
		public static void Post(this SynchronizationContext synchronizationContext, Action action)
		{
			Verify.ArgumentNotNull(synchronizationContext, "synchronizationContext");
			synchronizationContext.Post(SendOrPostCallback, action);
		}

		/// <summary>
		/// Send an <see cref="Action"/> to a <see cref="SynchronizationContext"/>.
		/// It is often more convenient to post an <see cref="Action"/> than a <see cref="SendOrPostCallback"/>.
		/// </summary>
		/// <param name="synchronizationContext">Synchronization context</param>
		/// <param name="action">Action to perform</param>
		public static void Send(this SynchronizationContext synchronizationContext, Action action)
		{
			Verify.ArgumentNotNull(synchronizationContext, "synchronizationContext");
			synchronizationContext.Send(SendOrPostCallback, action);
		}

		private static void SendOrPostCallback(object state)
		{
			var a = (Action) state;
			if (a != null)
			{
				a();
			}
		}
	}
}
