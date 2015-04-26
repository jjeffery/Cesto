using System;
using Cesto.Internal;

namespace Cesto.Logging
{
	/// <summary>
	/// Internal log manager for use by Cesto library code. Not intended for use outside
	/// the Cesto library. This log manager will detect use of the log4net or NLog library
	/// in the application, and will support whichever of these two libraries is in use.
	/// </summary>
	public static class InternalLogManager
	{
		/// <summary>
		/// If the calling program sets this field to a non-null value,
		/// then this will be used as the factory for creating <see cref="IInternalLogger"/>
		/// instances. If this field is <c>null</c> (the default value), then the appropriate
		/// logger factory will be used depending on whether the calling program uses
		/// log4net or NLog.
		/// </summary>
		public static IInternalLoggerFactory LoggerFactory;

		/// <summary>
		/// Return an <see cref="IInternalLogger"/> for the specified type.
		/// </summary>
		/// <param name="type">Class type that wants to log the message.</param>
		/// <returns>
		/// Returns an <see cref="IInternalLogger"/> that will work with the program's
		/// log4net or NLog configuration. Note, however, that the returned logger works
		/// using reflection, and is not optimized for performance.
		/// </returns>
		public static IInternalLogger GetLogger(Type type)
		{
			var loggerFactory = LoggerFactory
			                    ?? Log4netInternalLoggerFactory.Instance
			                    ?? NLogInternalLoggerFactory.Instance
			                    ?? NullInternalLoggerFactory.Instance;

			return loggerFactory.GetLogger(type);
		}
	}
}
