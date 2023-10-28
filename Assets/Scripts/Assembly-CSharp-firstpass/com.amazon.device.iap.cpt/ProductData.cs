using System;
using System.Collections.Generic;
using com.amazon.device.iap.cpt.json;

namespace com.amazon.device.iap.cpt
{
	public sealed class ProductData : Jsonable
	{
		public string Sku { get; set; }

		public string ProductType { get; set; }

		public string Price { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public string SmallIconUrl { get; set; }

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
				dictionary.Add("sku", Sku);
				dictionary.Add("productType", ProductType);
				dictionary.Add("price", Price);
				dictionary.Add("title", Title);
				dictionary.Add("description", Description);
				dictionary.Add("smallIconUrl", SmallIconUrl);
				return dictionary;
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while getting object dictionary", inner);
			}
		}

		public static ProductData CreateFromDictionary(Dictionary<string, object> jsonMap)
		{
			try
			{
				if (jsonMap == null)
				{
					return null;
				}
				ProductData productData = new ProductData();
				if (jsonMap.ContainsKey("sku"))
				{
					productData.Sku = (string)jsonMap["sku"];
				}
				if (jsonMap.ContainsKey("productType"))
				{
					productData.ProductType = (string)jsonMap["productType"];
				}
				if (jsonMap.ContainsKey("price"))
				{
					productData.Price = (string)jsonMap["price"];
				}
				if (jsonMap.ContainsKey("title"))
				{
					productData.Title = (string)jsonMap["title"];
				}
				if (jsonMap.ContainsKey("description"))
				{
					productData.Description = (string)jsonMap["description"];
				}
				if (jsonMap.ContainsKey("smallIconUrl"))
				{
					productData.SmallIconUrl = (string)jsonMap["smallIconUrl"];
				}
				return productData;
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while creating Object from dicionary", inner);
			}
		}

		public static ProductData CreateFromJson(string jsonMessage)
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

		public static Dictionary<string, ProductData> MapFromJson(Dictionary<string, object> jsonMap)
		{
			Dictionary<string, ProductData> dictionary = new Dictionary<string, ProductData>();
			foreach (KeyValuePair<string, object> item in jsonMap)
			{
				ProductData value = CreateFromDictionary(item.Value as Dictionary<string, object>);
				dictionary.Add(item.Key, value);
			}
			return dictionary;
		}

		public static List<ProductData> ListFromJson(List<object> array)
		{
			List<ProductData> list = new List<ProductData>();
			foreach (object item in array)
			{
				list.Add(CreateFromDictionary(item as Dictionary<string, object>));
			}
			return list;
		}
	}
}
