public static class StringExtensions
{
	public static string UnescapeXML(this string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return s;
		}
		string text = s;
		text = text.Replace("&apos;", "'");
		text = text.Replace("&quot;", "\"");
		text = text.Replace("&gt;", ">");
		text = text.Replace("&lt;", "<");
		return text.Replace("&amp;", "&");
	}
}
