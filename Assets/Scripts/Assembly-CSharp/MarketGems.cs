using System.Collections.Generic;
using UnityEngine;

public class MarketGems : MonoBehaviour
{
	public IAPButtonScript[] IAPScripts;

	private void Start()
	{
	}

	private void OnEnable()
	{
		PurchaseManager instance = Singleton<PurchaseManager>.Instance;
		bool allowPurchase = true;
		if (!instance.HasServerProductData)
		{
			allowPurchase = false;
			instance.GetProductData(ProductDataCallback);
		}
		IAPButtonScript[] iAPScripts = IAPScripts;
		foreach (IAPButtonScript iAPButtonScript in iAPScripts)
		{
			iAPButtonScript.Setup(allowPurchase);
		}
		Singleton<AnalyticsManager>.Instance.LogEnterShop();
	}

	private void ProductDataCallback(bool a_Success, List<PurchaseManager.ProductData> a_ProductList, string error)
	{
		if (!a_Success || a_ProductList == null || a_ProductList.Count <= 0)
		{
			return;
		}
		foreach (PurchaseManager.ProductData a_Product in a_ProductList)
		{
		}
		IAPButtonScript[] iAPScripts = IAPScripts;
		foreach (IAPButtonScript iAPButtonScript in iAPScripts)
		{
			iAPButtonScript.Setup();
		}
	}
}
