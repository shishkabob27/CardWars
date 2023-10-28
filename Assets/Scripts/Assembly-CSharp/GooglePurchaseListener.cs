using System;
using System.Collections.Generic;
using UnityEngine;

public class GooglePurchaseListener : IPurchaseListener
{
	private const string IAP_RECEIPT_VERIFICATION_URL = "verify_purchase_android.php";

	private const string DEBUGKEY = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAvN8Ffh0NThjV4eWSnWJBMKibaSKPIlUUfvR5+stoEt3a1QDLMY2sr7X9WiIB1asxtdBtgXmE+djcYkUHGxj2RWUZ8VUMYduDYJKqrY3TSFJVA/oVUrpXs58Fd2Cvb/khLMoxYS02oWDv95AghY2+fM045aMU+cONGYWDz/GGok57ZejF2TwqhOINblBIiTK58NXgKAq88tlk9JaC3BorwJKp07mxiZeKFJIw1J94HkUunmxRqjizITgAZy72IPjjiCl6pTWMv21nMA30j/L6jJqHACfNsBsW4JHQeDgV1Cf4HbQFSUB5DYDbHFLeMVITtlXGJEtvjaW556XkUAexhQIDAQAB";

	private const string RELEASEKEY = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA4pgdc0iqpZ95p6jfrTMblkg/W6Rdgqel0chhkl9DkPKOyQ0ZTekdIyNu+eejEVtM01qygimdt/I5qu0ubvl8LFMs3N/5ihBnZfWA3yPQIbW2Ec3M+nUOT/SiQoFiNxHrPk6awDdVj/7sP2yJs5Efu/z5pXAO1P4OpYVzcZrzEDgOy5rq7QW+yAqU+iJks74JI84kAXsGlS7TNGSj5bx3FR4DgMG9Zjxad360mrNki6C7wUZ2dQCPVyyw1Vf9H92+v/QKWFvVkelmX3ZH9uQn6CejRdWKkFcZrNB/kyWi99en/Jhjyk2hnEU0n7Zu9Bm8sccQ4/DhZ6UA+dWQ7ZX+kwIDAQAB";

	private bool BillingSupported = true;

	private bool productDataQueryActive;

	private PurchaseManager.ReceivedProductDataCallback receivedProductDataCallback;

	private PurchaseManager.ProductPurchaseCallback productPurchaseCallback;

	private PurchaseManager.VerifyIAPReceiptCallback verifyIAPReceiptCallback;

	private List<GooglePurchase> oldPurchases = new List<GooglePurchase>();

	public void OnEnable()
	{
		GoogleIABManager.billingSupportedEvent += AndBillingSupportedEvent;
		GoogleIABManager.billingNotSupportedEvent += AndBillingNotSupportedEvent;
		GoogleIABManager.queryInventorySucceededEvent += AndQueryInventorySucceededEvent;
		GoogleIABManager.queryInventoryFailedEvent += AndQueryInventoryFailedEvent;
		GoogleIABManager.purchaseCompleteAwaitingVerificationEvent += AndPurchaseCompleteAwaitingVerificationEvent;
		GoogleIABManager.purchaseSucceededEvent += AndPurchaseSucceededEvent;
		GoogleIABManager.purchaseFailedEvent += AndPurchaseFailedEvent;
		GoogleIABManager.consumePurchaseSucceededEvent += AndConsumePurchaseSucceededEvent;
		GoogleIABManager.consumePurchaseFailedEvent += AndConsumePurchaseFailedEvent;
		InitIAB();
	}

	private void InitIAB()
	{
		GoogleIAB.init("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA4pgdc0iqpZ95p6jfrTMblkg/W6Rdgqel0chhkl9DkPKOyQ0ZTekdIyNu+eejEVtM01qygimdt/I5qu0ubvl8LFMs3N/5ihBnZfWA3yPQIbW2Ec3M+nUOT/SiQoFiNxHrPk6awDdVj/7sP2yJs5Efu/z5pXAO1P4OpYVzcZrzEDgOy5rq7QW+yAqU+iJks74JI84kAXsGlS7TNGSj5bx3FR4DgMG9Zjxad360mrNki6C7wUZ2dQCPVyyw1Vf9H92+v/QKWFvVkelmX3ZH9uQn6CejRdWKkFcZrNB/kyWi99en/Jhjyk2hnEU0n7Zu9Bm8sccQ4/DhZ6UA+dWQ7ZX+kwIDAQAB");
		GoogleIAB.enableLogging(false);
		GoogleIAB.setAutoVerifySignatures(false);
	}

	public void OnDisable()
	{
		GoogleIABManager.billingSupportedEvent -= AndBillingSupportedEvent;
		GoogleIABManager.billingNotSupportedEvent -= AndBillingNotSupportedEvent;
		GoogleIABManager.queryInventorySucceededEvent -= AndQueryInventorySucceededEvent;
		GoogleIABManager.queryInventoryFailedEvent -= AndQueryInventoryFailedEvent;
		GoogleIABManager.purchaseCompleteAwaitingVerificationEvent -= AndPurchaseCompleteAwaitingVerificationEvent;
		GoogleIABManager.purchaseSucceededEvent -= AndPurchaseSucceededEvent;
		GoogleIABManager.purchaseFailedEvent -= AndPurchaseFailedEvent;
		GoogleIABManager.consumePurchaseSucceededEvent -= AndConsumePurchaseSucceededEvent;
		GoogleIABManager.consumePurchaseFailedEvent -= AndConsumePurchaseFailedEvent;
	}

	public void Fetch()
	{
	}

	public PurchaseManager.ProductDataRequestResult GetProductData(string[] a_ProductIDs, PurchaseManager.ReceivedProductDataCallback a_Callback)
	{
		receivedProductDataCallback = a_Callback;
		return DoGetProductData(a_ProductIDs, true);
	}

	private PurchaseManager.ProductDataRequestResult DoGetProductData(string[] a_ProductIDs, bool initIfNecessary)
	{
		if (productDataQueryActive)
		{
			return PurchaseManager.ProductDataRequestResult.Success;
		}
		productDataQueryActive = true;
		if (BillingSupported)
		{
			GoogleIAB.queryInventory(a_ProductIDs);
			return PurchaseManager.ProductDataRequestResult.Success;
		}
		if (initIfNecessary)
		{
			Action billingSupportCb = null;
			Action<string> billingNotSupportCb = null;
			billingSupportCb = delegate
			{
				productDataQueryActive = false;
				GoogleIABManager.billingSupportedEvent -= billingSupportCb;
				GoogleIABManager.billingNotSupportedEvent -= billingNotSupportCb;
				if (DoGetProductData(a_ProductIDs, false) != 0 && receivedProductDataCallback != null)
				{
					receivedProductDataCallback(false, null, "billing not supported");
				}
			};
			billingNotSupportCb = delegate(string err)
			{
				productDataQueryActive = false;
				GoogleIABManager.billingSupportedEvent -= billingSupportCb;
				GoogleIABManager.billingNotSupportedEvent -= billingNotSupportCb;
				if (receivedProductDataCallback != null)
				{
					receivedProductDataCallback(false, null, err);
				}
			};
			GoogleIABManager.billingSupportedEvent += billingSupportCb;
			GoogleIABManager.billingNotSupportedEvent += billingNotSupportCb;
			InitIAB();
			return PurchaseManager.ProductDataRequestResult.Success;
		}
		return PurchaseManager.ProductDataRequestResult.CannotMakePayment;
	}

	public void PurchaseProduct(string a_ProductID, PurchaseManager.ProductPurchaseCallback a_Callback)
	{
		TFUtils.DebugLog("Calling Google PurchaseListener.PurchaseProduct for product: " + a_ProductID, "iap");
		productPurchaseCallback = a_Callback;
		GoogleIAB.purchaseProduct(a_ProductID);
	}

	public void ConsumeProduct(string a_productID)
	{
		TFUtils.DebugLog("Consuming product " + a_productID, "iap");
		GoogleIAB.consumeProduct(a_productID);
	}

	public void RestorePurchases(PurchaseManager.RestorePurchasesCallback callback = null)
	{
		if (callback != null)
		{
			callback(true);
		}
	}

	public KFFNetwork.WWWInfo VerifyIAPReceipt(PurchaseManager.TransactionData transaction, PurchaseManager.VerifyIAPReceiptCallback callback)
	{
		verifyIAPReceiptCallback = callback;
		return _VerifyIAPReceipt(((GooglePurchase)transaction.NativeTransaction).originalJson, ((GooglePurchase)transaction.NativeTransaction).signature, ((GooglePurchase)transaction.NativeTransaction).productId);
	}

	private KFFNetwork.WWWInfo _VerifyIAPReceipt(string a_OriginalJson, string a_Signature, string a_ProductID)
	{
		string text = ((!DebugFlagsScript.GetInstance().failIAP) ? string.Empty : "fail");
		string text2 = PurchaseManager.IAP_VERIFICATION_SERVER_URL + "/" + text + "verify_purchase_android.php?";
		if (PurchaseManager.SLOT_IAP_SANDBOX)
		{
			text2 += "sandbox=1&";
		}
		text2 = text2 + "language=" + KFFCSUtils.GetLanguageCode();
		text2 += "&subdirectory=data_1.01";
		text2 += "&json=1";
		string playerCode = PlayerInfoScript.GetInstance().GetPlayerCode();
		text2 = text2 + "&userid=" + playerCode;
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("signedData", a_OriginalJson);
		wWWForm.AddField("signature", a_Signature);
		return KFFNetwork.GetInstance().SendWWWRequestWithForm(wWWForm, text2, WWWVerifyReceiptCallback, a_ProductID);
	}

	private void ProcessOldPurchases()
	{
		if (oldPurchases.Count > 0)
		{
			if (oldPurchases[0].purchaseState == GooglePurchase.GooglePurchaseState.Purchased)
			{
				_VerifyIAPReceipt(oldPurchases[0].originalJson, oldPurchases[0].signature, oldPurchases[0].productId);
			}
			else if (Singleton<PurchaseManager>.Instance.IsConsumable(oldPurchases[0].productId))
			{
				ConsumeProduct(oldPurchases[0].productId);
			}
			oldPurchases.RemoveAt(0);
		}
	}

	private void WWWVerifyReceiptCallback(KFFNetwork.WWWInfo wwwinfo, object resultObj, string err, object param)
	{
		KFFNetwork.WWWRequestResult wWWRequestResult = resultObj as KFFNetwork.WWWRequestResult;
		bool flag = false;
		string text = (string)param;
		if (wWWRequestResult == null)
		{
		}
		if (wWWRequestResult != null && wWWRequestResult.IsValid())
		{
			string key = "RZrb({~IXRD{Oen[UXZA(a5my-5bZe17JeJMyR-DcuOI2";
			string text2 = "szTn-kDeI=niczdj8BIw{oNG=6|]{{EY%[Ym";
			string valueAsString = wWWRequestResult.GetValueAsString(key);
			if (valueAsString != text2)
			{
				resultObj = null;
				err = KFFLocalization.Get("!!ERROR_PURCHASE_FAILED_RESULT_INVALID");
			}
			else
			{
				flag = true;
			}
		}
		if (verifyIAPReceiptCallback != null)
		{
			verifyIAPReceiptCallback(wwwinfo, resultObj, err, param);
			verifyIAPReceiptCallback = null;
		}
		else if (flag)
		{
			if (Singleton<PurchaseManager>.Instance.RedeemPurchase(text) && Singleton<PurchaseManager>.Instance.IsConsumable(text))
			{
				ConsumeProduct(text);
			}
		}
		else if (Singleton<PurchaseManager>.Instance.IsConsumable(text))
		{
			ConsumeProduct(text);
		}
	}

	private void AndBillingSupportedEvent()
	{
		BillingSupported = true;
	}

	private void AndBillingNotSupportedEvent(string error)
	{
		BillingSupported = false;
	}

	private void AndQueryInventorySucceededEvent(List<GooglePurchase> a_Purchases, List<GoogleSkuInfo> a_ProductList)
	{
		productDataQueryActive = false;
		oldPurchases.Clear();
		foreach (GooglePurchase a_Purchase in a_Purchases)
		{
			oldPurchases.Add(a_Purchase);
		}
		ProcessOldPurchases();
		List<PurchaseManager.ProductData> list = new List<PurchaseManager.ProductData>();
		foreach (GoogleSkuInfo a_Product in a_ProductList)
		{
			PurchaseManager.ProductData productData = new PurchaseManager.ProductData();
			productData.ProductIdentifier = a_Product.productId;
			productData.Title = a_Product.title;
			productData.Price = a_Product.price;
			productData.Description = a_Product.description;
			productData.FormattedPrice = a_Product.price;
			list.Add(productData);
		}
		if (receivedProductDataCallback != null)
		{
			receivedProductDataCallback(true, list, null);
		}
	}

	private void AndQueryInventoryFailedEvent(string error)
	{
		productDataQueryActive = false;
		if (receivedProductDataCallback != null)
		{
			receivedProductDataCallback(false, null, error);
		}
	}

	private void AndPurchaseCompleteAwaitingVerificationEvent(string purchaseData, string signature)
	{
	}

	private void AndPurchaseSucceededEvent(GooglePurchase a_Transaction)
	{
		if (productPurchaseCallback != null)
		{
			PurchaseManager.TransactionData transactionData = new PurchaseManager.TransactionData();
			transactionData.NativeTransaction = a_Transaction;
			productPurchaseCallback(PurchaseManager.ProductPurchaseResult.Success, transactionData, null);
		}
	}

	private void AndPurchaseFailedEvent(string error, int errorId)
	{
		ProcessOldPurchases();
		if (productPurchaseCallback != null)
		{
			productPurchaseCallback(PurchaseManager.ProductPurchaseResult.Failed, null, error);
		}
	}

	private void AndConsumePurchaseSucceededEvent(GooglePurchase purchase)
	{
	}

	private void AndConsumePurchaseFailedEvent(string error)
	{
	}
}
