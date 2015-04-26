using System;

namespace Cesto.Logging
{
	/// <summary>
	/// Interface for factory classes that can create loggers
	/// of type <see cref="IInternalLogger"/>. This interface is
	/// not intended for use outside the Cesto library. It is made
	/// public, however, so calling programs can set a logger
	/// factory on the <see cref="InternalLogManager.LoggerFactory"/> field
	/// if necessary.
	/// </summary>
	public interface IInternalLoggerFactory
	{
		/// <summary>
		/// Returns an <see cref="IInternalLogger"/> instance appropriate
		/// for the calling type.
		/// </summary>
		/// <param name="type">Type that intends to log one or more messages.</param>
		/// <returns>
		/// Returns an instance of <see cref="IInternalLogger"/> appropriate for
		/// the calling type and the logging library used by the calling program.
		/// </returns>
		IInternalLogger GetLogger(Type type);
	}
}
