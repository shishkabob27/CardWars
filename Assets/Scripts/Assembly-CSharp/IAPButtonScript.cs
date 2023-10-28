using System;
using UnityEngine;

public class IAPButtonScript : MonoBehaviour
{
	public string PurchaseID;

	public string PurchaseAndroidID;

	public bool AllowPurchase;

	private string AbstractPurchaseID;

	private KFFNetwork.WWWInfo wwwVerifyPurchase;

	private UIButtonTween SuccessfulPurchase;

	private void Start()
	{
		if (SuccessfulPurchase == null)
		{
			GameObject gameObject = GameObject.Find("G_PlayerStats_GemsPurchased");
			if (gameObject != null)
			{
				SuccessfulPurchase = gameObject.GetComponent<UIButtonTween>();
			}
		}
	}

	public void Setup(bool allowPurchase = true)
	{
		AllowPurchase = allowPurchase;
		AbstractPurchaseID = PurchaseAndroidID;
		GameObject gameObject = base.gameObject.transform.parent.Find("Gem_Amount").gameObject;
		if ((bool)gameObject)
		{
			UILabel component = gameObject.GetComponent<UILabel>();
			if ((bool)component)
			{
				int gemCountForProductID = Singleton<PurchaseManager>.Instance.GetGemCountForProductID(AbstractPurchaseID);
				if (gemCountForProductID == 1)
				{
					component.text = gemCountForProductID + " " + KFFLocalization.Get("!!GEM");
				}
				else
				{
					component.text = gemCountForProductID + " " + KFFLocalization.Get("!!GEMS");
				}
			}
		}
		gameObject = base.gameObject.transform.Find("Gem_Cost").gameObject;
		if ((bool)gameObject)
		{
			UILabel component2 = gameObject.GetComponent<UILabel>();
			if (component2 != null)
			{
				string Price;
				Singleton<PurchaseManager>.Instance.GetformattedPrice(AbstractPurchaseID, out Price);
				component2.text = ((!AllowPurchase) ? "..." : Price);
			}
		}
	}

	private void OnClick()
	{
		if (AllowPurchase)
		{
			Setup();
			Purchase();
		}
	}

	private void Purchase()
	{
		if (AbstractPurchaseID != null)
		{
			Singleton<PurchaseManager>.Instance.PurchaseProduct(AbstractPurchaseID, PurchaseCallback);
		}
	}

	private void PurchaseCallback(PurchaseManager.ProductPurchaseResult result, PurchaseManager.TransactionData transaction, string err)
	{
		switch (result)
		{
		case PurchaseManager.ProductPurchaseResult.Success:
			wwwVerifyPurchase = Singleton<PurchaseManager>.Instance.VerifyIAPReceipt(transaction, VerifyIAPReceiptCallback);
			break;
		case PurchaseManager.ProductPurchaseResult.Failed:
			break;
		case PurchaseManager.ProductPurchaseResult.Cancelled:
			break;
		}
	}

	private void VerifyIAPReceiptCallback(KFFNetwork.WWWInfo wwwinfo, object resultObj, string err, object para)
	{
		if (wwwVerifyPurchase == wwwinfo || Singleton<PurchaseManager>.Instance.IsAmazon)
		{
			wwwVerifyPurchase = null;
			KFFNetwork.WWWRequestResult wWWRequestResult = resultObj as KFFNetwork.WWWRequestResult;
			if (wWWRequestResult != null && wWWRequestResult.IsValid())
			{
				CompletePurchase();
				Singleton<PurchaseManager>.Instance.ConsumeProduct(AbstractPurchaseID);
			}
		}
	}

	private void CompletePurchase()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (instance != null)
		{
			instance.Gems += Singleton<PurchaseManager>.Instance.GetGemCountForProductID(AbstractPurchaseID);
			instance.Coins += Singleton<PurchaseManager>.Instance.GetCoinCountForProductID(AbstractPurchaseID);
			float priceForProductID = Singleton<PurchaseManager>.Instance.GetPriceForProductID(AbstractPurchaseID);
			Singleton<AnalyticsManager>.Instance.LogIAPByBattle(AbstractPurchaseID, priceForProductID);
			try
			{
				instance.Save();
			}
			catch (Exception)
			{
			}
		}
		if ((bool)SuccessfulPurchase)
		{
			SuccessfulPurchase.Play(true);
		}
	}
}
