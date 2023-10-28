using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using JsonFx.Json;

namespace JsonFx.Xml
{
	public class XmlDataReader : IDataReader
	{
		public const string XmlMimeType = "application/xml";

		private readonly XmlReaderSettings Settings;

		private readonly XmlSerializerNamespaces Namespaces;

		public string ContentType
		{
			get
			{
				return "application/xml";
			}
		}

		public XmlDataReader(XmlReaderSettings settings, XmlSerializerNamespaces namespaces)
		{
			if (settings == null)
			{
				throw new ArgumentNullException("settings");
			}
			Settings = settings;
			if (namespaces == null)
			{
				namespaces = new XmlSerializerNamespaces();
				namespaces.Add(string.Empty, string.Empty);
			}
			Namespaces = namespaces;
		}

		public object Deserialize(TextReader input, Type type)
		{
			XmlReader xmlReader = XmlReader.Create(input, Settings);
			xmlReader.MoveToContent();
			XmlSerializer xmlSerializer = new XmlSerializer(type);
			return xmlSerializer.Deserialize(xmlReader);
		}

		public static XmlReaderSettings CreateSettings()
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.CloseInput = false;
			xmlReaderSettings.ConformanceLevel = ConformanceLevel.Auto;
			xmlReaderSettings.IgnoreComments = true;
			xmlReaderSettings.IgnoreWhitespace = true;
			xmlReaderSettings.IgnoreProcessingInstructions = true;
			return xmlReaderSettings;
		}
	}
}
