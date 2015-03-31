using System;
using System.Reflection;

namespace Cesto.Logging
{
    // ReSharper disable once InconsistentNaming
    internal class Log4netInternalLoggerFactory : IInternalLoggerFactory
    {
        public static readonly IInternalLoggerFactory Instance;
        private readonly MethodInfo _getLoggerMethod;

        static Log4netInternalLoggerFactory()
        {
            var logManagerType = Type.GetType("log4net.LogManager, log4net");
            if (logManagerType != null)
            {
                var getLoggerMethod = logManagerType.GetMethod("GetLogger",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new[] {typeof (Type)},
                    null);

                if (getLoggerMethod != null)
                {
                    Instance = new Log4netInternalLoggerFactory(getLoggerMethod);
                }
            }
        }

        private Log4netInternalLoggerFactory(MethodInfo getLoggerMethod)
        {
            _getLoggerMethod = getLoggerMethod;
        }

        public IInternalLogger GetLogger(Type type)
        {
            var logger = _getLoggerMethod.Invoke(null, new object[] {type});
            return new Log4netInternalLogger(logger);
        }
    }
}