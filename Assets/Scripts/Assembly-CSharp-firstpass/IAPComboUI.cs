using System.Collections.Generic;
using Prime31;
using UnityEngine;

public class IAPComboUI : MonoBehaviourGUI
{
	private void OnGUI()
	{
		beginColumn();
		if (GUILayout.Button("Init IAP System"))
		{
			string text = "your public key from the Android developer portal here";
			text = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAmffbbQPr/zqRjP3vkxr1601/eKsXm5kO2NzQge8m7PeUj5V+saeounyL34U8WoZ3BvCRKbw6DrRLs2DMoVuCLq7QtJggBHT/bBSHGczEXGIPjWpw6OQb24EWM0PaTRTH2x2mC/X6RwIKcPLJFmy68T38Eh0DXnF4jjiIoaD0W8AYLjLzv0WvbIfgtJlvmmwvI2/Kta1LRnW3/Ggi5jb9UmXZAUIBz8kQtSH5FUCmFOQHMzekfg8rQ4VO1nlWhnB58UPwsxWt/DNyDfqv2VMeA2+VJG0fkiMl/6vWA7+ianVTU3owXcvxJHseEDUVYo1wEKfhK7ErGB7sxDJx5wHXAwIDAQAB";
			IAP.init(text);
		}
		if (GUILayout.Button("Request Product Data"))
		{
			string[] androidSkus = new string[5] { "com.prime31.testproduct", "android.test.purchased", "android.test.purchased2", "com.prime31.managedproduct", "com.prime31.testsubscription" };
			string[] iosProductIdentifiers = new string[5] { "anotherProduct", "tt", "testProduct", "sevenDays", "oneMonthSubsciber" };
			IAP.requestProductData(iosProductIdentifiers, androidSkus, delegate(List<IAPProduct> productList)
			{
				Utils.logObject(productList);
			});
		}
		if (GUILayout.Button("Restore Transactions (iOS only)"))
		{
			IAP.restoreCompletedTransactions(delegate
			{
			});
		}
		if (GUILayout.Button("Purchase Consumable"))
		{
			string productId3 = "android.test.purchased";
			IAP.purchaseConsumableProduct(productId3, delegate(bool didSucceed, string error)
			{
				if (didSucceed)
				{
				}
			});
		}
		if (GUILayout.Button("Purchase Non-Consumable"))
		{
			string productId2 = "android.test.purchased2";
			IAP.purchaseNonconsumableProduct(productId2, delegate(bool didSucceed, string error)
			{
				if (didSucceed)
				{
				}
			});
		}
		endColumn();
	}
}
