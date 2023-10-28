using System;
using System.Collections.Generic;

public static class IAP
{
	private const string CONSUMABLE_PAYLOAD = "consume";

	private const string NON_CONSUMABLE_PAYLOAD = "nonconsume";

	public static List<GooglePurchase> androidPurchasedItems;

	private static Action<List<IAPProduct>> _productListReceivedAction;

	private static Action<bool, string> _purchaseCompletionAction;

	private static Action<string> _purchaseRestorationAction;

	static IAP()
	{
		androidPurchasedItems = new List<GooglePurchase>();
		GoogleIABManager.queryInventorySucceededEvent += delegate(List<GooglePurchase> purchases, List<GoogleSkuInfo> skus)
		{
			androidPurchasedItems = purchases;
			List<IAPProduct> list = new List<IAPProduct>();
			foreach (GoogleSkuInfo sku in skus)
			{
				list.Add(new IAPProduct(sku));
			}
			if (_productListReceivedAction != null)
			{
				_productListReceivedAction(list);
			}
		};
		GoogleIABManager.queryInventoryFailedEvent += delegate
		{
			if (_productListReceivedAction != null)
			{
				_productListReceivedAction(null);
			}
		};
		GoogleIABManager.purchaseSucceededEvent += delegate(GooglePurchase purchase)
		{
			if (purchase.developerPayload == "nonconsume")
			{
				if (_purchaseCompletionAction != null)
				{
					_purchaseCompletionAction(true, null);
				}
			}
			else
			{
				GoogleIAB.consumeProduct(purchase.productId);
			}
		};
		GoogleIABManager.purchaseFailedEvent += delegate(string error, int response)
		{
			if (_purchaseCompletionAction != null)
			{
				_purchaseCompletionAction(false, error);
			}
		};
		GoogleIABManager.consumePurchaseSucceededEvent += delegate
		{
			if (_purchaseCompletionAction != null)
			{
				_purchaseCompletionAction(true, null);
			}
		};
		GoogleIABManager.consumePurchaseFailedEvent += delegate
		{
			if (_purchaseCompletionAction != null)
			{
				_purchaseCompletionAction(false, null);
			}
		};
	}

	public static void init(string androidPublicKey)
	{
		GoogleIAB.init(androidPublicKey);
	}

	public static void requestProductData(string[] iosProductIdentifiers, string[] androidSkus, Action<List<IAPProduct>> completionHandler)
	{
		_productListReceivedAction = completionHandler;
		GoogleIAB.queryInventory(androidSkus);
	}

	public static void purchaseConsumableProduct(string productId, Action<bool, string> completionHandler)
	{
		_purchaseCompletionAction = completionHandler;
		_purchaseRestorationAction = null;
		GoogleIAB.purchaseProduct(productId, "consume");
	}

	public static void purchaseNonconsumableProduct(string productId, Action<bool, string> completionHandler)
	{
		_purchaseCompletionAction = completionHandler;
		_purchaseRestorationAction = null;
		GoogleIAB.purchaseProduct(productId, "nonconsume");
	}

	public static void restoreCompletedTransactions(Action<string> completionHandler)
	{
		_purchaseCompletionAction = null;
		_purchaseRestorationAction = completionHandler;
	}
}
