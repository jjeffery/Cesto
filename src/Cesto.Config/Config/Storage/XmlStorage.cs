using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using Cesto.Internal;

namespace Cesto.Config.Storage
{
	/// <summary>
	/// Provides a file based configuration parameter storage. Parameters are stored
	/// using a simple XML file format.
	/// </summary>
	/// <remarks>
	/// This class inherits from <see cref="MemoryStorage"/>, which provides a caching
	/// mechanism. If the configuration does not change, configuration parameters are
	/// cached in memory.
	/// </remarks>
	public class XmlStorage : MemoryStorage
	{
		public XmlStorage(string filePath) : base(new XmlStorageHelper(filePath))
		{
		}
	}

	internal class XmlStorageHelper : IConfigStorage
	{
		public readonly string FilePath;
		private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

		/// <summary>
		/// Creates a configuration storage that persists itself as an XML file.
		/// </summary>
		/// <param name="filePath">File path of the XML configuration file</param>
		public XmlStorageHelper(string filePath)
		{
			FilePath = Verify.ArgumentNotNull(filePath, "filePath");
			IsReadOnly = false;
		}

		public ConfigValue GetValue(ConfigParameter parameter)
		{
			_lock.EnterReadLock();
			try
			{
				var xmlConfig = LoadData();
				var item = xmlConfig.GetItem(parameter.Name);
				if (item != null)
				{
					return new ConfigValue(parameter, item.Value, true);
				}
				return new ConfigValue(parameter, null, false);
			}
			finally
			{
				_lock.ExitReadLock();
			}
		}

		public ConfigValue[] GetValues(params ConfigParameter[] parameters)
		{
			_lock.EnterReadLock();
			try
			{
				var xmlConfig = LoadData();
				var values = new ConfigValue[parameters.Length];
				for (var index = 0; index < parameters.Length; ++index)
				{
					var parameter = parameters[index];
					var item = xmlConfig.GetItem(parameter.Name);
					ConfigValue value;
					if (item != null)
					{
						value = new ConfigValue(parameter, item.Value, true);
					}
					else
					{
						value = new ConfigValue(parameter, null, false);
					}
					values[index] = value;
				}
				return values;
			}
			finally
			{
				_lock.ExitReadLock();
			}
		}

		public bool IsReadOnly { get; private set; }

		public void SetValue(ConfigParameter parameter, string value)
		{
			_lock.EnterWriteLock();
			try
			{
				var xmlConfig = LoadData();
				var item = xmlConfig.GetItem(parameter.Name, create: true);
				item.Value = value;
				item.Type = parameter.ParameterType;
				SaveData(xmlConfig);
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}

		public void Refresh()
		{
		}

		private XmlConfig LoadData()
		{
			if (!File.Exists(FilePath))
			{
				return new XmlConfig { Items = new List<XmlConfigItem>()};
			}

			using (var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				var serializer = new XmlSerializer(typeof (XmlConfig));
				var xmlConfig = (XmlConfig)serializer.Deserialize(stream);
				return xmlConfig;
			}
		}

		private void SaveData(XmlConfig xmlConfig)
		{
			xmlConfig.Items.Sort((x1, x2) => StringComparer.OrdinalIgnoreCase.Compare(x1.Name, x2.Name));
			using (var stream = new FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				var serializer = new XmlSerializer(typeof (XmlConfig));
				var xmlWriterSettings = new XmlWriterSettings
				{
					Indent = true,
				};
				using (var xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
				{
					serializer.Serialize(xmlWriter, xmlConfig);
				}
			}
		}
	}
}
