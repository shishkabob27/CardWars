using System.Collections;
using System.Collections.Generic;

namespace Prime31
{
	public static class JsonExtensions
	{
		public static string toJson(this IList obj)
		{
			return Json.encode(obj);
		}

		public static string toJson(this IDictionary obj)
		{
			return Json.encode(obj);
		}

		public static List<object> listFromJson(this string json)
		{
			return Json.decode(json) as List<object>;
		}

		public static Dictionary<string, object> dictionaryFromJson(this string json)
		{
			return Json.decode(json) as Dictionary<string, object>;
		}
	}
}
