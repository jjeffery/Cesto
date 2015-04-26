using System;

namespace Cesto.Logging
{
	internal class NullInternalLogger : IInternalLogger
	{
		public void Warn(string message) {}

		public void Error(string message, Exception exception) {}
	}
}
