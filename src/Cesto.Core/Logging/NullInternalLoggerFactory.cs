using System;

namespace Cesto.Logging
{
    internal class NullInternalLoggerFactory : IInternalLoggerFactory
    {
        public static readonly IInternalLoggerFactory Instance = new NullInternalLoggerFactory();

        public IInternalLogger GetLogger(Type type)
        {
            return new NullInternalLogger();
        }
    }
}