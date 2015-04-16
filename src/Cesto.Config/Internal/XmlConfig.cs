using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Cesto.Config.Storage;

namespace Cesto.Internal
{
	/// <summary>
	/// Used by <see cref="XmlStorage"/> for serializing configuration settings
	/// </summary>
	[XmlRoot("config")]
	public class XmlConfig
	{
		[XmlElement("item")]
		public List<XmlConfigItem> Items;

		public XmlConfigItem GetItem(string name, bool create = false)
		{
			if (Items == null)
			{
				Items = new List<XmlConfigItem>();
			}

			var item = Items.FirstOrDefault(i => name.Equals(i.Name, StringComparison.OrdinalIgnoreCase));
			if (item == null && create)
			{
				item = new XmlConfigItem {Name = name};
				Items.Add(item);
			}

			return item;
		}
	}

	public class XmlConfigItem
	{
		[XmlAttribute("name")]
		public string Name;

		[XmlAttribute("type")]
		public string Type;

		[XmlText]
		public string Value;
	}
}
