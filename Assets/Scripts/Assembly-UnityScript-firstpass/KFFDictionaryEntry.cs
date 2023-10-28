using System;
using System.Xml.Serialization;

[Serializable]
public class KFFDictionaryEntry
{
	[XmlAttribute("Key")]
	public string key;

	[XmlAttribute("Value")]
	public string value;
}
