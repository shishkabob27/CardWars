using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using JsonFx.Json;

namespace JsonFx.Xml
{
	public class XmlDataWriter : IDataWriter
	{
		public const string XmlMimeType = "application/xml";

		public const string XmlFileExtension = ".xml";

		private readonly XmlWriterSettings Settings;

		private readonly XmlSerializerNamespaces Namespaces;

		public Encoding ContentEncoding
		{
			get
			{
				return Settings.Encoding ?? Encoding.UTF8;
			}
		}

		public string ContentType
		{
			get
			{
				return "application/xml";
			}
		}

		public string FileExtension
		{
			get
			{
				return ".xml";
			}
		}

		public XmlDataWriter(XmlWriterSettings settings, XmlSerializerNamespaces namespaces)
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

		public void Serialize(TextWriter output, object data)
		{
			if (data != null)
			{
				if (Settings.Encoding == null)
				{
					Settings.Encoding = ContentEncoding;
				}
				XmlWriter writer = XmlWriter.Create(output, Settings);
				XmlSerializer xmlSerializer = new XmlSerializer(data.GetType());
				xmlSerializer.Serialize(writer, data, Namespaces);
			}
		}

		public static XmlWriterSettings CreateSettings(Encoding encoding, bool prettyPrint)
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.CheckCharacters = true;
			xmlWriterSettings.CloseOutput = false;
			xmlWriterSettings.ConformanceLevel = ConformanceLevel.Auto;
			xmlWriterSettings.Encoding = encoding;
			xmlWriterSettings.OmitXmlDeclaration = true;
			if (prettyPrint)
			{
				xmlWriterSettings.Indent = true;
				xmlWriterSettings.IndentChars = "\t";
			}
			else
			{
				xmlWriterSettings.Indent = false;
				xmlWriterSettings.NewLineChars = string.Empty;
			}
			xmlWriterSettings.NewLineHandling = NewLineHandling.Replace;
			return xmlWriterSettings;
		}
	}
}
