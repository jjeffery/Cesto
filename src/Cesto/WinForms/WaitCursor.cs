using System;
using System.Threading;
using System.Windows.Forms;

namespace Cesto.WinForms
{
	/// <summary>
	///     Simple mechanism for displaying the wait cursor
	/// </summary>
	/// <example>
	/// <code>
	/// public void LongOperation() {
	///     // about to start a long operation
	///     using (WaitCursor.Show()) {
	///         // do long operation here
	///         .
	///         .
	///         .
	///     }
	/// }
	/// </code>
	/// </example>
	/// <remarks>
	///     Multiple calls to <c>WaitCursor.Show</c> can be nested.
	/// </remarks>
	public static class WaitCursor
	{
		private static readonly IDisposable Disposable = new DisposableAction(RestoreCursor2);
		private static int _referenceCount;

		public static event EventHandler Changed;

		public static bool Visible { get; private set; }

		public static IDisposable Show()
		{
			if (Interlocked.Increment(ref _referenceCount) == 1)
			{
				Cursor.Current = Cursors.WaitCursor;
				Visible = true;
				var changed = Changed;
				if (changed != null)
				{
					changed(null, EventArgs.Empty);
				}
			}
			return Disposable;
		}

		private static void RestoreCursor2()
		{
			if (Interlocked.Decrement(ref _referenceCount) == 0)
			{
				Cursor.Current = Cursors.Default;
				Visible = false;
				var changed = Changed;
				if (changed != null)
				{
					changed(null, EventArgs.Empty);
				}
				
			}
		}
	}
}