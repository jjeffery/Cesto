using System;

namespace Cesto.Logging
{
    // ReSharper disable once InconsistentNaming
    internal class Log4netInternalLogger : IInternalLogger
    {
        private readonly object _logger;

        public Log4netInternalLogger(object logger)
        {
            _logger = Verify.ArgumentNotNull(logger, "logger");
        }

        public void Warn(string message)
        {
            var type = _logger.GetType();
            var method = type.GetMethod("Warn", new[] { typeof(string) });
            if (method != null)
            {
                method.Invoke(_logger, new object[] {message});
            }
        }

        public void Error(string message, Exception exception)
        {
            var type = _logger.GetType();
            var method = type.GetMethod("Error", new[] {typeof (string), typeof (Exception)});
            if (method != null)
            {
                method.Invoke(_logger, new object[] {message, exception});
            }
        }
    }
}