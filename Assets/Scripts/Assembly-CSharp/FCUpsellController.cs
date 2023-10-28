using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FCUpsellController : MonoBehaviour
{
	public UIButtonTween FCUpsellTweenShow;

	public UIButtonTween FCUpsellTweenHide;

	public Text FCPrice;

	public bool AllowPurchase = true;

	private string PurchaseID = "at_pack_fionnacake_1";

	private KFFNetwork.WWWInfo wwwVerifyPurchase;

	public UIButtonTween SuccessfulPurchase;

	private GameObject FCButton;

	private void OnEnable()
	{
		AllowPurchase = true;
		PurchaseManager instance = Singleton<PurchaseManager>.Instance;
		if (!instance.HasServerProductData)
		{
			AllowPurchase = false;
			instance.GetProductData(delegate(bool success, List<PurchaseManager.ProductData> productInfo, string error)
			{
				AllowPurchase = success;
				UpdatePrice();
			});
		}
		UpdatePrice();
	}

	public void SetFCButton(GameObject go)
	{
		FCButton = go;
	}

	public void ShowPanel()
	{
		if (null != FCUpsellTweenShow)
		{
			FCUpsellTweenShow.Play(true);
		}
		Singleton<AnalyticsManager>.Instance.LogFCSellOpen();
	}

	public void HidePanel()
	{
		if (null != FCUpsellTweenHide)
		{
			FCUpsellTweenHide.Play(true);
		}
	}

	public void RestartDemo()
	{
		PlayerInfoScript.GetInstance().ResetFCDemo();
		if (null != FCButton)
		{
			FCButton.SendMessage("LaunchFCDemo");
		}
		Singleton<AnalyticsManager>.Instance.LogFCDemoReplay();
	}

	private void UpdatePrice()
	{
		if (null == FCPrice)
		{
			return;
		}
		if (!AllowPurchase)
		{
			FCPrice.text = "...";
			return;
		}
		string Price = null;
		Singleton<PurchaseManager>.Instance.GetformattedPrice(PurchaseID, out Price);
		if (Price != null)
		{
			FCPrice.text = Price;
		}
	}

	public void PurchaseFC()
	{
		if (AllowPurchase)
		{
			TFUtils.DebugLog("Clicked on FC Purchase button, starting purchase of " + PurchaseID, GetType().ToString());
			Singleton<AnalyticsManager>.Instance.LogFCSellClick();
			Singleton<PurchaseManager>.Instance.PurchaseProduct(PurchaseID, PurchaseCallback);
		}
	}

	public void DebugUnlockFC()
	{
	}

	public void CloseFC()
	{
		Collider component = GetComponent<Collider>();
		if (component != null)
		{
			component.enabled = false;
		}
	}

	private void PurchaseCallback(PurchaseManager.ProductPurchaseResult result, PurchaseManager.TransactionData transaction, string err)
	{
		TFUtils.DebugLog("PurchaseCallback called", GetType().ToString());
		switch (result)
		{
		case PurchaseManager.ProductPurchaseResult.Success:
			wwwVerifyPurchase = Singleton<PurchaseManager>.Instance.VerifyIAPReceipt(transaction, VerifyIAPReceiptCallback);
			break;
		case PurchaseManager.ProductPurchaseResult.Failed:
			TFUtils.DebugLog("Error purchasing", GetType().ToString());
			break;
		case PurchaseManager.ProductPurchaseResult.Cancelled:
			TFUtils.DebugLog("Purchase was cancelled.", GetType().ToString());
			break;
		}
	}

	private void VerifyIAPReceiptCallback(KFFNetwork.WWWInfo wwwinfo, object resultObj, string err, object para)
	{
		TFUtils.DebugLog("VerifyIAPReceiptCallback called", GetType().ToString());
		if (wwwVerifyPurchase == wwwinfo || Singleton<PurchaseManager>.Instance.IsAmazon)
		{
			wwwVerifyPurchase = null;
			KFFNetwork.WWWRequestResult wWWRequestResult = resultObj as KFFNetwork.WWWRequestResult;
			if (wWWRequestResult == null || !wWWRequestResult.IsValid())
			{
				TFUtils.WarnLog((err != null) ? err : ((wWWRequestResult == null) ? "Error verifying purchase" : wWWRequestResult.GetValueAsString("ERROR_MSG")), GetType().ToString());
				return;
			}
			CompletePurchase();
			Singleton<PurchaseManager>.Instance.ConsumeProduct(PurchaseID);
		}
		else
		{
			TFUtils.WarnLog("Purchase failed.", GetType().ToString());
		}
	}

	private void CompletePurchase()
	{
		TFUtils.DebugLog("Purchase completed: " + PurchaseID, GetType().ToString());
		PlayerInfoScript.GetInstance().SetHasPurchasedFC(PurchaseID);
		if ((bool)SuccessfulPurchase)
		{
			SuccessfulPurchase.Play(true);
		}
		FCMapController.SetupFCDeck();
		HidePanel();
		LaunchFCMap();
	}

	private void LaunchFCMap()
	{
		StartCoroutine(LaunchFCMapHelper());
	}

	private IEnumerator LaunchFCMapHelper()
	{
		if (null != FCButton)
		{
			yield return new WaitForSeconds(1f);
			FCButton.SendMessage("OnClick");
		}
		yield return null;
	}
}
