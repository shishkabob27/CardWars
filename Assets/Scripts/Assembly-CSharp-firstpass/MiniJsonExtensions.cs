using System.Collections;
using System.Collections.Generic;

public static class MiniJsonExtensions
{
	public static string toJson(this Hashtable obj)
	{
		return MiniJSON_Prime31.jsonEncode(obj);
	}

	public static string toJson(this Dictionary<string, string> obj)
	{
		return MiniJSON_Prime31.jsonEncode(obj);
	}

	public static ArrayList arrayListFromJson(this string json)
	{
		return MiniJSON_Prime31.jsonDecode(json) as ArrayList;
	}

	public static Hashtable hashtableFromJson(this string json)
	{
		return MiniJSON_Prime31.jsonDecode(json) as Hashtable;
	}
}
