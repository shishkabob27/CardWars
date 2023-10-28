using System.Collections.Generic;
using MiniJSON;

public class UpsightPurchase
{
	public string placement { get; private set; }

	public int quantity { get; private set; }

	public string productIdentifier { get; private set; }

	public string store { get; private set; }

	public string receipt { get; private set; }

	public string title { get; private set; }

	public double price { get; private set; }

	public static UpsightPurchase purchaseFromJson(string json)
	{
		UpsightPurchase upsightPurchase = new UpsightPurchase();
		Dictionary<string, object> dictionary = Json.Deserialize(json) as Dictionary<string, object>;
		if (dictionary != null)
		{
			if (dictionary.ContainsKey("placement") && dictionary["placement"] != null)
			{
				upsightPurchase.placement = dictionary["placement"].ToString();
			}
			if (dictionary.ContainsKey("quantity") && dictionary["quantity"] != null)
			{
				upsightPurchase.quantity = int.Parse(dictionary["quantity"].ToString());
			}
			if (dictionary.ContainsKey("productIdentifier") && dictionary["productIdentifier"] != null)
			{
				upsightPurchase.productIdentifier = dictionary["productIdentifier"].ToString();
			}
			if (dictionary.ContainsKey("store") && dictionary["store"] != null)
			{
				upsightPurchase.store = dictionary["store"].ToString();
			}
			if (dictionary.ContainsKey("receipt") && dictionary["receipt"] != null)
			{
				upsightPurchase.receipt = dictionary["receipt"].ToString();
			}
			if (dictionary.ContainsKey("title") && dictionary["title"] != null)
			{
				upsightPurchase.title = dictionary["title"].ToString();
			}
			if (dictionary.ContainsKey("price") && dictionary["price"] != null)
			{
				upsightPurchase.price = double.Parse(dictionary["price"].ToString());
			}
		}
		return upsightPurchase;
	}

	public override string ToString()
	{
		return string.Format("[UpsightPurchase: placement={0}, quantity={1}, productIdentifier={2}, store={3}, receipt={4}, title={5}, price={6}]", placement, quantity, productIdentifier, store, receipt, title, price);
	}
}
