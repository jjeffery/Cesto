using System;

namespace Cesto.Threading
{
    /// <summary>
    /// Event args associated with the <see cref="WorkThread.ExceptionThrown"/> event.
    /// </summary>
    public class WorkThreadExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// Exception thrown on the <see cref="WorkThread"/>.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Set to <c>true</c> if the exception is handled.
        /// </summary>
        public bool Handled { get; set; }

        public WorkThreadExceptionEventArgs(Exception exception)
        {
            Exception = Verify.ArgumentNotNull(exception, "exception");
        }
    }
}