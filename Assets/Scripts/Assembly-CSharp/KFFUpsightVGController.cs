using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KFFUpsightVGController : MonoBehaviour
{
	private const float SHOW_PLACEMENT_TIMEOUT_SECS = 30f;

	public GameObject YouGotThis;

	private string purchaseID;

	private KFFNetwork.WWWInfo wwwVerifyPurchase;

	private bool purchaseInProgress;

	private static KFFUpsightVGController instance;

	private static int purchaseIdIndex = 9;

	public bool IsPlacementInProgress
	{
		get
		{
			KFFRequestorController kFFRequestorController = KFFRequestorController.GetInstance();
			if (purchaseInProgress || (kFFRequestorController != null && kFFRequestorController.IsPlacementInProgress))
			{
				return true;
			}
			return false;
		}
	}

	public static KFFUpsightVGController GetInstance()
	{
		return instance;
	}

	private void OnEnable()
	{
		TFUtils.DebugLog("KFFUpsightVGController.RegisterUpsightCallbacks", "upsight");
		KFFRequestorController kFFRequestorController = KFFRequestorController.GetInstance();
		if (kFFRequestorController != null)
		{
			kFFRequestorController.onPurchaseEvent = (KFFRequestorController.PurchaseEvent)Delegate.Combine(kFFRequestorController.onPurchaseEvent, new KFFRequestorController.PurchaseEvent(makePurchase));
		}
	}

	private void OnDisable()
	{
		TFUtils.DebugLog("KFFUpsightVGController.DeregisterUpsightCallbacks", "upsight");
		KFFRequestorController kFFRequestorController = KFFRequestorController.GetInstance();
		if (kFFRequestorController != null)
		{
			kFFRequestorController.onPurchaseEvent = (KFFRequestorController.PurchaseEvent)Delegate.Remove(kFFRequestorController.onPurchaseEvent, new KFFRequestorController.PurchaseEvent(makePurchase));
		}
	}

	private void purchaseFailed(string error)
	{
		if (IsPlacementInProgress)
		{
			TFUtils.DebugLog("purchase failed with error: " + error, "upsight");
		}
		purchaseInProgress = false;
	}

	private void purchaseCancelled(string msg)
	{
		if (IsPlacementInProgress)
		{
			TFUtils.DebugLog("purchase cancelled with message: " + msg, "upsight");
		}
		purchaseInProgress = false;
	}

	private void makePurchase(UpsightPurchase purchase)
	{
		purchaseID = purchase.productIdentifier;
		purchaseInProgress = true;
		Singleton<PurchaseManager>.Instance.PurchaseProduct(purchaseID, PurchaseCallback);
		TFUtils.DebugLog("KFFUpsightVGController.UpsightPurchase Placement: " + purchase.placement + ", Purchased: " + purchase.productIdentifier + ", Quantity: " + purchase.quantity, "upsight");
	}

	private void PurchaseCallback(PurchaseManager.ProductPurchaseResult result, PurchaseManager.TransactionData transaction, string err)
	{
		TFUtils.DebugLog("KFFUpsightVGController.PurchaseCallback called, transaction data: " + transaction.ToString(), "upsight");
		switch (result)
		{
		case PurchaseManager.ProductPurchaseResult.Success:
			wwwVerifyPurchase = Singleton<PurchaseManager>.Instance.VerifyIAPReceipt(transaction, VerifyIAPReceiptCallback);
			TFUtils.DebugLog("KFFUpsightVGController.ProductPurchaseResult.Success", "upsight");
			break;
		case PurchaseManager.ProductPurchaseResult.Failed:
			TFUtils.WarnLog("KFFUpsightVGController.ProductPurchaseResult.Failed: " + err);
			purchaseInProgress = false;
			break;
		case PurchaseManager.ProductPurchaseResult.Cancelled:
			TFUtils.DebugLog("KFFUpsightVGController.ProductPurchaseResult.Cancelled", "upsight");
			purchaseInProgress = false;
			break;
		}
	}

	private void VerifyIAPReceiptCallback(KFFNetwork.WWWInfo wwwinfo, object resultObj, string err, object para)
	{
		if (wwwVerifyPurchase == wwwinfo || Singleton<PurchaseManager>.Instance.IsAmazon)
		{
			wwwVerifyPurchase = null;
			KFFNetwork.WWWRequestResult wWWRequestResult = resultObj as KFFNetwork.WWWRequestResult;
			if (wWWRequestResult == null || !wWWRequestResult.IsValid())
			{
				TFUtils.WarnLog((err != null) ? err : ((wWWRequestResult == null) ? "Error verifying purchase" : wWWRequestResult.GetValueAsString("ERROR_MSG")));
				purchaseInProgress = false;
				return;
			}
			CompletePurchase();
			PurchaseManager purchaseManager = Singleton<PurchaseManager>.Instance;
			if (purchaseManager.IsConsumable(purchaseID))
			{
				purchaseManager.ConsumeProduct(purchaseID);
			}
		}
		else
		{
			TFUtils.WarnLog("Purchase failed.");
			purchaseInProgress = false;
		}
	}

	private void CompletePurchase()
	{
		if (Singleton<PurchaseManager>.Instance.GetTypeForProductID(purchaseID).ToLower() == "fc1")
		{
			PlayerInfoScript.GetInstance().SetHasPurchasedFC(purchaseID);
		}
		VirtualGoods virtualGoods = VirtualGoodsDataManager.Instance.GetVirtualGoods(purchaseID);
		if (virtualGoods != null)
		{
			StartCoroutine(RedeemVirtualGoods(virtualGoods));
		}
		else
		{
			purchaseInProgress = false;
		}
	}

	public void DebugRedeemVirtualGoods()
	{
		StartCoroutine(DebugRedeemVirtualGoodsHelper());
	}

	private IEnumerator DebugRedeemVirtualGoodsHelper()
	{
		List<string> purchaseIds = VirtualGoodsDataManager.Instance.GetPurchaseIds();
		if (purchaseIds == null || purchaseIds.Count == 0)
		{
			TFUtils.DebugLog("No purchase ids -- make sure the json data exists and is valid.", "upsight");
			yield break;
		}
		if (purchaseIdIndex > purchaseIds.Count)
		{
			purchaseIdIndex = 0;
		}
		string purchaseID = purchaseIds[purchaseIdIndex++];
		VirtualGoods vg = VirtualGoodsDataManager.Instance.GetVirtualGoods(purchaseID);
		if (vg != null)
		{
			yield return new WaitForSeconds(0.5f);
			yield return StartCoroutine(RedeemVirtualGoods(vg));
		}
		yield return null;
	}

	private IEnumerator RedeemVirtualGoods(VirtualGoods vg)
	{
		if (vg == null)
		{
			yield break;
		}
		PlayerInfoScript pInfo = PlayerInfoScript.GetInstance();
		if (null != pInfo)
		{
			foreach (string occuranceString in vg.OccuranceStrings)
			{
				pInfo.IncOccuranceCounter(occuranceString);
			}
			pInfo.Gems += vg.Gems;
			pInfo.Coins += vg.Coins;
			pInfo.Stamina += vg.Hearts;
			pInfo.MaxInventory += vg.Inventory;
			TFUtils.DebugLog(string.Format("RedeemVirtualGoods: pID: {0} gems: {1} coins: {2} stamina: {3} inventory: {4}", purchaseID, vg.Gems, vg.Coins, vg.Hearts, vg.Inventory), "upsight");
			if (vg.Cards.Any())
			{
				yield return StartCoroutine(AwardCards(vg));
				yield return new WaitForSeconds(3f);
			}
			pInfo.Save();
		}
		purchaseInProgress = false;
		yield return null;
	}

	private IEnumerator AwardCards(VirtualGoods vg)
	{
		if (null == YouGotThis)
		{
			yield break;
		}
		YouGotThisController youGotThisController = YouGotThis.GetComponent<YouGotThisController>();
		if (null == youGotThisController)
		{
			yield break;
		}
		foreach (string cardId in vg.Cards)
		{
			if (cardId.StartsWith("Leader_", StringComparison.CurrentCultureIgnoreCase))
			{
				yield return StartCoroutine(youGotThisController.AwardLeader(cardId));
			}
			else
			{
				yield return StartCoroutine(youGotThisController.AwardCard(cardId));
			}
		}
		yield return null;
	}
}
