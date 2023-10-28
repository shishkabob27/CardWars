using System;
using System.Collections.Generic;
using com.amazon.device.iap.cpt.json;

namespace com.amazon.device.iap.cpt
{
	public sealed class PurchaseReceipt : Jsonable
	{
		public string ReceiptId { get; set; }

		public long CancelDate { get; set; }

		public long PurchaseDate { get; set; }

		public string Sku { get; set; }

		public string ProductType { get; set; }

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
				dictionary.Add("receiptId", ReceiptId);
				dictionary.Add("cancelDate", CancelDate);
				dictionary.Add("purchaseDate", PurchaseDate);
				dictionary.Add("sku", Sku);
				dictionary.Add("productType", ProductType);
				return dictionary;
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while getting object dictionary", inner);
			}
		}

		public static PurchaseReceipt CreateFromDictionary(Dictionary<string, object> jsonMap)
		{
			try
			{
				if (jsonMap == null)
				{
					return null;
				}
				PurchaseReceipt purchaseReceipt = new PurchaseReceipt();
				if (jsonMap.ContainsKey("receiptId"))
				{
					purchaseReceipt.ReceiptId = (string)jsonMap["receiptId"];
				}
				if (jsonMap.ContainsKey("cancelDate"))
				{
					purchaseReceipt.CancelDate = (long)jsonMap["cancelDate"];
				}
				if (jsonMap.ContainsKey("purchaseDate"))
				{
					purchaseReceipt.PurchaseDate = (long)jsonMap["purchaseDate"];
				}
				if (jsonMap.ContainsKey("sku"))
				{
					purchaseReceipt.Sku = (string)jsonMap["sku"];
				}
				if (jsonMap.ContainsKey("productType"))
				{
					purchaseReceipt.ProductType = (string)jsonMap["productType"];
				}
				return purchaseReceipt;
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while creating Object from dicionary", inner);
			}
		}

		public static PurchaseReceipt CreateFromJson(string jsonMessage)
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

		public static Dictionary<string, PurchaseReceipt> MapFromJson(Dictionary<string, object> jsonMap)
		{
			Dictionary<string, PurchaseReceipt> dictionary = new Dictionary<string, PurchaseReceipt>();
			foreach (KeyValuePair<string, object> item in jsonMap)
			{
				PurchaseReceipt value = CreateFromDictionary(item.Value as Dictionary<string, object>);
				dictionary.Add(item.Key, value);
			}
			return dictionary;
		}

		public static List<PurchaseReceipt> ListFromJson(List<object> array)
		{
			List<PurchaseReceipt> list = new List<PurchaseReceipt>();
			foreach (object item in array)
			{
				list.Add(CreateFromDictionary(item as Dictionary<string, object>));
			}
			return list;
		}
	}
}
