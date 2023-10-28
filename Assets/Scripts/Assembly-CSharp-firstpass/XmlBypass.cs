using System.IO;
using System.Xml;

public static class XmlBypass
{
	public static float GetFloat(XmlAttribute attrib)
	{
		return GetFloat(attrib, 0f);
	}

	public static float GetFloat(XmlAttribute attrib, float @default = 0f)
	{
		float.TryParse((attrib != null) ? attrib.InnerText : "0", out @default);
		return @default;
	}

	public static string GetString(this XmlNode node, string @default = "")
	{
		return (node != null) ? node.InnerText : @default;
	}

	public static int GetInt(this XmlNode node, int @default = 0)
	{
		int.TryParse((node != null) ? node.InnerText : "0", out @default);
		return @default;
	}

	public static float GetFloat(this XmlNode node, float @default = 0f)
	{
		float.TryParse((node != null) ? node.InnerText : "0", out @default);
		return @default;
	}

	public static bool GetBoolean(this XmlNode node, bool @default = false)
	{
		string text = ((node != null) ? node.InnerText : "0");
		int result = 0;
		if (int.TryParse(text, out result))
		{
			return result != 0;
		}
		switch (text.ToLower())
		{
		case "yes":
		case "true":
			@default = true;
			break;
		case "no":
		case "false":
			@default = false;
			break;
		}
		return @default;
	}

	public static XmlDocument ParseString(string xml)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.Load(new StringReader(xml));
		return xmlDocument;
	}
}
