using System;
using System.Reflection;

namespace Cesto.Logging
{
	internal class NLogInternalLoggerFactory : IInternalLoggerFactory
	{
		public static readonly IInternalLoggerFactory Instance;
		private readonly MethodInfo _getLoggerMethod;

		static NLogInternalLoggerFactory()
		{
			var logManagerType = Type.GetType("NLog.LogManager, NLog");
			if (logManagerType != null)
			{
				var getLoggerMethod = logManagerType.GetMethod("GetLogger",
					BindingFlags.Public | BindingFlags.Static,
					null,
					new[] {typeof (string)},
					null);

				if (getLoggerMethod != null)
				{
					Instance = new NLogInternalLoggerFactory(getLoggerMethod);
				}
			}
		}

		private NLogInternalLoggerFactory(MethodInfo getLoggerMethod)
		{
			_getLoggerMethod = getLoggerMethod;
		}

		public IInternalLogger GetLogger(Type type)
		{
			var logger = _getLoggerMethod.Invoke(null, new object[] {type.FullName});
			return new NLogInternalLogger(logger);
		}
	}
}
