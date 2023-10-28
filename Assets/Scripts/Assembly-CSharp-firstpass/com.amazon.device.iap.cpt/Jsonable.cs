using System.Collections.Generic;

namespace com.amazon.device.iap.cpt
{
	public abstract class Jsonable
	{
		public static Dictionary<string, object> unrollObjectIntoMap<T>(Dictionary<string, T> obj) where T : Jsonable
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (KeyValuePair<string, T> item in obj)
			{
				dictionary.Add(item.Key, item.Value.GetObjectDictionary());
			}
			return dictionary;
		}

		public static List<object> unrollObjectIntoList<T>(List<T> obj) where T : Jsonable
		{
			List<object> list = new List<object>();
			foreach (T item in obj)
			{
				list.Add(item.GetObjectDictionary());
			}
			return list;
		}

		public abstract Dictionary<string, object> GetObjectDictionary();

		public static void CheckForErrors(Dictionary<string, object> jsonMap)
		{
			object value;
			if (jsonMap.TryGetValue("error", out value))
			{
				throw new AmazonException(value as string);
			}
		}
	}
}
