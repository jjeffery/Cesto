using System;

namespace Cesto.Logging
{
	/// <summary>
	/// Internal logger for use by Cesto library code. Not intended for use outside the
	/// Cesto library. This logger is used for infrequent warning and error conditions, and is not
	/// optimized for high performance.
	/// </summary>
	public interface IInternalLogger
	{
		/// <summary>
		/// Log a warning message
		/// </summary>
		/// <param name="message">Warning message</param>
		void Warn(string message);

		/// <summary>
		/// Log an error message.
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="exception">Exception associated with the error message</param>
		/// <remarks>
		/// This method involves calling methods on the underlying log4net or NLog logger
		/// using reflection, and is not optimized for performance.
		/// </remarks>
		void Error(string message, Exception exception);
	}
}
