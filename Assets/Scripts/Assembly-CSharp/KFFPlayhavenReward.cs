using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class KFFPlayhavenReward : MonoBehaviour
{
	private UpsightRewardParser _Parser;

	private KFFNetwork.WWWInfo wwwVerifyPurchase;

	private string PurchaseID;

	private UIButtonTween ShowReceivePopup;

	[CompilerGenerated]
	private static Dictionary<string, int> _003C_003Ef__switch_0024map22;

	private void OnEnable()
	{
		UpsightManager.unlockedRewardEvent += OnUpsightRewardGiven;
		UpsightManager.makePurchaseEvent += UpsightPurchase;
	}

	private void OnDisable()
	{
		UpsightManager.unlockedRewardEvent -= OnUpsightRewardGiven;
		UpsightManager.makePurchaseEvent -= UpsightPurchase;
	}

	private void UpsightPurchase(UpsightPurchase aPurchase)
	{
		Singleton<PurchaseManager>.Instance.PurchaseProduct(aPurchase.productIdentifier, PurchaseCallback);
		PurchaseID = aPurchase.productIdentifier;
		TFUtils.DebugLog("Placement: " + aPurchase.placement + ", Purchased: " + PurchaseID + ", Quantity: " + aPurchase.quantity, "upsight");
		string purchaseID = PurchaseID;
		if (purchaseID != null)
		{
			if (_003C_003Ef__switch_0024map22 == null)
			{
				_003C_003Ef__switch_0024map22 = new Dictionary<string, int>(0);
			}
			int value;
			if (!_003C_003Ef__switch_0024map22.TryGetValue(purchaseID, out value))
			{
			}
		}
	}

	private void PurchaseCallback(PurchaseManager.ProductPurchaseResult result, PurchaseManager.TransactionData transaction, string err)
	{
		TFUtils.DebugLog("PurchaseCallback called", "upsight");
		switch (result)
		{
		case PurchaseManager.ProductPurchaseResult.Success:
			TFUtils.DebugLog("ProductPurchaseResult.Success", "upsight");
			wwwVerifyPurchase = Singleton<PurchaseManager>.Instance.VerifyIAPReceipt(transaction, VerifyIAPReceiptCallback);
			break;
		case PurchaseManager.ProductPurchaseResult.Failed:
			TFUtils.WarnLog("Error purchfasing");
			break;
		case PurchaseManager.ProductPurchaseResult.Cancelled:
			TFUtils.WarnLog("Purchase was cancelled.");
			break;
		}
	}

	private void VerifyIAPReceiptCallback(KFFNetwork.WWWInfo wwwinfo, object resultObj, string err, object para)
	{
		TFUtils.DebugLog("VerifyIAPReceiptCallback called", "upsight");
		if (wwwVerifyPurchase == wwwinfo || Singleton<PurchaseManager>.Instance.IsAmazon)
		{
			wwwVerifyPurchase = null;
			KFFNetwork.WWWRequestResult wWWRequestResult = resultObj as KFFNetwork.WWWRequestResult;
			if (wWWRequestResult == null || !wWWRequestResult.IsValid())
			{
				TFUtils.WarnLog((err != null) ? err : ((wWWRequestResult == null) ? "Error verifying purchase" : wWWRequestResult.GetValueAsString("ERROR_MSG")));
				return;
			}
			CompletePurchase();
			Singleton<PurchaseManager>.Instance.ConsumeProduct(PurchaseID);
		}
		else
		{
			TFUtils.WarnLog("Purchase failed.");
		}
	}

	private void CompletePurchase()
	{
		TFUtils.DebugLog("CompletePurchase", "upsight");
	}

	private void OnUpsightRewardGiven(UpsightReward aReward)
	{
		TFUtils.DebugLog("OnUpsightRewardGiven: " + aReward, "upsight");
		_Parser.Parse(aReward.name);
	}
}
