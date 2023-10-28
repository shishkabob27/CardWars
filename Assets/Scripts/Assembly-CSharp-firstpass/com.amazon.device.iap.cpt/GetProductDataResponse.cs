using System;
using System.Collections.Generic;
using System.Linq;
using com.amazon.device.iap.cpt.json;

namespace com.amazon.device.iap.cpt
{
	public sealed class GetProductDataResponse : Jsonable
	{
		public string RequestId { get; set; }

		public Dictionary<string, ProductData> ProductDataMap { get; set; }

		public List<string> UnavailableSkus { get; set; }

		public string Status { get; set; }

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
				dictionary.Add("requestId", RequestId);
				dictionary.Add("productDataMap", (ProductDataMap == null) ? null : Jsonable.unrollObjectIntoMap(ProductDataMap));
				dictionary.Add("unavailableSkus", UnavailableSkus);
				dictionary.Add("status", Status);
				return dictionary;
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while getting object dictionary", inner);
			}
		}

		public static GetProductDataResponse CreateFromDictionary(Dictionary<string, object> jsonMap)
		{
			try
			{
				if (jsonMap == null)
				{
					return null;
				}
				GetProductDataResponse getProductDataResponse = new GetProductDataResponse();
				if (jsonMap.ContainsKey("requestId"))
				{
					getProductDataResponse.RequestId = (string)jsonMap["requestId"];
				}
				if (jsonMap.ContainsKey("productDataMap"))
				{
					getProductDataResponse.ProductDataMap = ProductData.MapFromJson(jsonMap["productDataMap"] as Dictionary<string, object>);
				}
				if (jsonMap.ContainsKey("unavailableSkus"))
				{
					getProductDataResponse.UnavailableSkus = ((List<object>)jsonMap["unavailableSkus"]).Select((object element) => (string)element).ToList();
				}
				if (jsonMap.ContainsKey("status"))
				{
					getProductDataResponse.Status = (string)jsonMap["status"];
				}
				return getProductDataResponse;
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while creating Object from dicionary", inner);
			}
		}

		public static GetProductDataResponse CreateFromJson(string jsonMessage)
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

		public static Dictionary<string, GetProductDataResponse> MapFromJson(Dictionary<string, object> jsonMap)
		{
			Dictionary<string, GetProductDataResponse> dictionary = new Dictionary<string, GetProductDataResponse>();
			foreach (KeyValuePair<string, object> item in jsonMap)
			{
				GetProductDataResponse value = CreateFromDictionary(item.Value as Dictionary<string, object>);
				dictionary.Add(item.Key, value);
			}
			return dictionary;
		}

		public static List<GetProductDataResponse> ListFromJson(List<object> array)
		{
			List<GetProductDataResponse> list = new List<GetProductDataResponse>();
			foreach (object item in array)
			{
				list.Add(CreateFromDictionary(item as Dictionary<string, object>));
			}
			return list;
		}
	}
}
