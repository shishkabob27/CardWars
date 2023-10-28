using System;
using System.Collections.Generic;
using com.amazon.device.iap.cpt.json;

namespace com.amazon.device.iap.cpt
{
	public sealed class AmazonUserData : Jsonable
	{
		public string UserId { get; set; }

		public string Marketplace { get; set; }

		public string ToJson()
		{
			try
			{
				Dictionary<string, object> objectDictionary = GetObjectDictionary();
				return Json.Serialize(objectDictionary);
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while Jsoning", inner);
			}
		}

		public override Dictionary<string, object> GetObjectDictionary()
		{
			try
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("userId", UserId);
				dictionary.Add("marketplace", Marketplace);
				return dictionary;
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while getting object dictionary", inner);
			}
		}

		public static AmazonUserData CreateFromDictionary(Dictionary<string, object> jsonMap)
		{
			try
			{
				if (jsonMap == null)
				{
					return null;
				}
				AmazonUserData amazonUserData = new AmazonUserData();
				if (jsonMap.ContainsKey("userId"))
				{
					amazonUserData.UserId = (string)jsonMap["userId"];
				}
				if (jsonMap.ContainsKey("marketplace"))
				{
					amazonUserData.Marketplace = (string)jsonMap["marketplace"];
				}
				return amazonUserData;
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while creating Object from dicionary", inner);
			}
		}

		public static AmazonUserData CreateFromJson(string jsonMessage)
		{
			try
			{
				Dictionary<string, object> jsonMap = Json.Deserialize(jsonMessage) as Dictionary<string, object>;
				Jsonable.CheckForErrors(jsonMap);
				return CreateFromDictionary(jsonMap);
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while UnJsoning", inner);
			}
		}

		public static Dictionary<string, AmazonUserData> MapFromJson(Dictionary<string, object> jsonMap)
		{
			Dictionary<string, AmazonUserData> dictionary = new Dictionary<string, AmazonUserData>();
			foreach (KeyValuePair<string, object> item in jsonMap)
			{
				AmazonUserData value = CreateFromDictionary(item.Value as Dictionary<string, object>);
				dictionary.Add(item.Key, value);
			}
			return dictionary;
		}

		public static List<AmazonUserData> ListFromJson(List<object> array)
		{
			List<AmazonUserData> list = new List<AmazonUserData>();
			foreach (object item in array)
			{
				list.Add(CreateFromDictionary(item as Dictionary<string, object>));
			}
			return list;
		}
	}
}
