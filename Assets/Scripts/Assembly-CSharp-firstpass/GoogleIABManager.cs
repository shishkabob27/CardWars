using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Prime31;

public class GoogleIABManager : AbstractManager
{
	[method: MethodImpl(32)]
	public static event Action billingSupportedEvent;

	[method: MethodImpl(32)]
	public static event Action<string> billingNotSupportedEvent;

	[method: MethodImpl(32)]
	public static event Action<List<GooglePurchase>, List<GoogleSkuInfo>> queryInventorySucceededEvent;

	[method: MethodImpl(32)]
	public static event Action<string> queryInventoryFailedEvent;

	[method: MethodImpl(32)]
	public static event Action<string, string> purchaseCompleteAwaitingVerificationEvent;

	[method: MethodImpl(32)]
	public static event Action<GooglePurchase> purchaseSucceededEvent;

	[method: MethodImpl(32)]
	public static event Action<string, int> purchaseFailedEvent;

	[method: MethodImpl(32)]
	public static event Action<GooglePurchase> consumePurchaseSucceededEvent;

	[method: MethodImpl(32)]
	public static event Action<string> consumePurchaseFailedEvent;

	static GoogleIABManager()
	{
		AbstractManager.initialize(typeof(GoogleIABManager));
	}

	public void billingSupported(string empty)
	{
		GoogleIABManager.billingSupportedEvent.fire();
	}

	public void billingNotSupported(string error)
	{
		GoogleIABManager.billingNotSupportedEvent.fire(error);
	}

	public void queryInventorySucceeded(string json)
	{
		if (GoogleIABManager.queryInventorySucceededEvent != null)
		{
			Dictionary<string, object> dictionary = json.dictionaryFromJson();
			GoogleIABManager.queryInventorySucceededEvent(GooglePurchase.fromList(dictionary["purchases"] as List<object>), GoogleSkuInfo.fromList(dictionary["skus"] as List<object>));
		}
	}

	public void queryInventoryFailed(string error)
	{
		GoogleIABManager.queryInventoryFailedEvent.fire(error);
	}

	public void purchaseCompleteAwaitingVerification(string json)
	{
		if (GoogleIABManager.purchaseCompleteAwaitingVerificationEvent != null)
		{
			Dictionary<string, object> dictionary = json.dictionaryFromJson();
			string arg = dictionary["purchaseData"].ToString();
			string arg2 = dictionary["signature"].ToString();
			GoogleIABManager.purchaseCompleteAwaitingVerificationEvent(arg, arg2);
		}
	}

	public void purchaseSucceeded(string json)
	{
		GoogleIABManager.purchaseSucceededEvent.fire(new GooglePurchase(json.dictionaryFromJson()));
	}

	public void purchaseFailed(string json)
	{
		if (GoogleIABManager.purchaseFailedEvent != null)
		{
			Dictionary<string, object> dictionary = Json.decode<Dictionary<string, object>>(json);
			GoogleIABManager.purchaseFailedEvent(dictionary["result"].ToString(), int.Parse(dictionary["response"].ToString()));
		}
	}

	public void consumePurchaseSucceeded(string json)
	{
		if (GoogleIABManager.consumePurchaseSucceededEvent != null)
		{
			GoogleIABManager.consumePurchaseSucceededEvent.fire(new GooglePurchase(json.dictionaryFromJson()));
		}
	}

	public void consumePurchaseFailed(string error)
	{
		GoogleIABManager.consumePurchaseFailedEvent.fire(error);
	}
}
