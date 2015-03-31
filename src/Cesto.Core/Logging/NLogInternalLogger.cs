using System;

namespace Cesto.Logging
{
    internal class NLogInternalLogger : IInternalLogger
    {
        private readonly object _logger;

        public NLogInternalLogger(object logger)
        {
            _logger = Verify.ArgumentNotNull(logger, "logger");
        }

        public void Warn(string message)
        {
            var type = _logger.GetType();
            var method = type.GetMethod("Warn", new[] { typeof(string) });
            method.Invoke(_logger, new object[] { message });
        }

        public void Error(string message, Exception exception)
        {
            var type = _logger.GetType();
            var method = type.GetMethod("Error", new[] {typeof (string), typeof (Exception)});
            method.Invoke(_logger, new object[] {message, exception});
        }
    }
}