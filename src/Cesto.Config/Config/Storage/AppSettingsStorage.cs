using System;
using System.Configuration;

namespace Cesto.Config.Storage
{
	public class AppSettingsStorage : IConfigStorage
	{
		public ConfigValue GetValue(ConfigParameter parameter)
		{
			var setting = ConfigurationManager.AppSettings[parameter.Name];
			if (setting == null)
			{
				return new ConfigValue(parameter, null, false);
			}
			return new ConfigValue(parameter, setting, true);
		}

		public ConfigValue[] GetValues(params ConfigParameter[] parameters)
		{
			var values = new ConfigValue[parameters.Length];
			for (var index = 0; index < parameters.Length; ++index)
			{
				values[index] = GetValue(parameters[index]);
			}
			return values;
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public void SetValue(ConfigParameter parameter, string value)
		{
			throw new NotSupportedException();
		}

		public void Refresh()
		{
		}
	}
}
