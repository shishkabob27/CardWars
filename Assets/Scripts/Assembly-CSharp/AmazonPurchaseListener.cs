using System.Collections.Generic;
using com.amazon.device.iap.cpt;
using UnityEngine;

public class AmazonPurchaseListener : IPurchaseListener
{
	private static string amazonUserID;

	private static string IAP_RECEIPT_VERIFICATION_URL = "verify_purchase_amazon2.php";

	private bool isBillingSupported = true;

	private bool isInitialProductDataUpdate = true;

	private int processPurchaseUpdates;

	private PurchaseManager.ReceivedProductDataCallback receivedProductDataCallback;

	private PurchaseManager.ProductPurchaseCallback productPurchaseCallback;

	public static PurchaseManager.VerifyIAPReceiptCallback verifyIAPReceiptCallback;

	private List<PurchaseReceipt> purchases = new List<PurchaseReceipt>();

	private static string GetAmazonUserID()
	{
		return amazonUserID;
	}

	public void OnEnable()
	{
		IAmazonIapV2 instance = AmazonIapV2Impl.Instance;
		instance.AddGetProductDataResponseListener(AmazonProductDataRequestCallback);
		instance.AddGetPurchaseUpdatesResponseListener(AmazonPurchaseUpdatesResponseCallback);
		instance.AddGetUserDataResponseListener(AmazonUserDataResponseCallback);
		instance.AddPurchaseResponseListener(AmazonPurchaseResponseCallback);
	}

	public void OnDisable()
	{
		IAmazonIapV2 instance = AmazonIapV2Impl.Instance;
		instance.RemoveGetProductDataResponseListener(AmazonProductDataRequestCallback);
		instance.RemoveGetPurchaseUpdatesResponseListener(AmazonPurchaseUpdatesResponseCallback);
		instance.RemoveGetUserDataResponseListener(AmazonUserDataResponseCallback);
		instance.RemovePurchaseResponseListener(AmazonPurchaseResponseCallback);
	}

	public void Fetch()
	{
	}

	protected void SendRecieptVerification(string userID, PurchaseReceipt receipt)
	{
		string receiptId = receipt.ReceiptId;
		string text = ((!DebugFlagsScript.GetInstance().failIAP) ? string.Empty : "fail");
		string text2 = PurchaseManager.IAP_VERIFICATION_SERVER_URL + "/" + text + IAP_RECEIPT_VERIFICATION_URL + "?";
		if (PurchaseManager.SLOT_IAP_SANDBOX)
		{
			text2 += "sandbox=1&";
		}
		text2 = text2 + "language=" + KFFCSUtils.GetLanguageCode();
		text2 += "&subdirectory=data_1.01";
		text2 += "&json=1";
		text2 = text2 + "&userid=" + userID;
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("receiptid", WWW.EscapeURL(receiptId));
		KFFNetwork.GetInstance().SendWWWRequestWithForm(wWWForm, text2, WWWVerifyReceiptCallback, receipt);
	}

	public PurchaseManager.ProductDataRequestResult GetProductData(string[] a_ProductIDs, PurchaseManager.ReceivedProductDataCallback a_Callback)
	{
		if (isBillingSupported)
		{
			IAmazonIapV2 iapService = AmazonIapV2Impl.Instance;
			receivedProductDataCallback = a_Callback;
			GetProductDataResponseDelegate productDataCallback = null;
            productDataCallback = delegate(GetProductDataResponse response)
			{
				iapService.RemoveGetProductDataResponseListener(productDataCallback);
				if (response.Status != "SUCCESSFUL")
				{
					if (a_Callback != null)
					{
						a_Callback(false, null, response.Status);
					}
				}
				else
				{
					processPurchaseUpdates++;
					iapService.GetPurchaseUpdates(new ResetInput
					{
						Reset = isInitialProductDataUpdate
					});
					isInitialProductDataUpdate = false;
				}
			};
			iapService.GetUserData();
			iapService.AddGetProductDataResponseListener(productDataCallback);
			SkusInput skusInput = new SkusInput();
			skusInput.Skus = new List<string>(a_ProductIDs);
			iapService.GetProductData(skusInput);
			return PurchaseManager.ProductDataRequestResult.Success;
		}
		return PurchaseManager.ProductDataRequestResult.CannotMakePayment;
	}

	public void PurchaseProduct(string a_ProductID, PurchaseManager.ProductPurchaseCallback a_Callback)
	{
		TFUtils.DebugLog("Calling Amazon PurchaseListener.PurchaseProduct for product: " + a_ProductID, "iap");
		productPurchaseCallback = a_Callback;
		IAmazonIapV2 instance = AmazonIapV2Impl.Instance;
		SkuInput skuInput = new SkuInput();
		skuInput.Sku = a_ProductID;
		SkuInput skuInput2 = skuInput;
		instance.Purchase(skuInput2);
	}

	public void ConsumeProduct(string a_productID)
	{
	}

	public void RestorePurchases(PurchaseManager.RestorePurchasesCallback restorePurchaseCB = null)
	{
		if (restorePurchaseCB != null)
		{
			restorePurchaseCB(true);
		}
	}

	public KFFNetwork.WWWInfo VerifyIAPReceipt(PurchaseManager.TransactionData transaction, PurchaseManager.VerifyIAPReceiptCallback callback)
	{
		PurchaseResponse purchaseResponse = (PurchaseResponse)transaction.NativeTransaction;
		verifyIAPReceiptCallback = callback;
		SendRecieptVerification(purchaseResponse.AmazonUserData.UserId, purchaseResponse.PurchaseReceipt);
		return null;
	}

	private void WWWVerifyReceiptCallback(KFFNetwork.WWWInfo wwwinfo, object resultObj, string err, object param)
	{
		KFFNetwork.WWWRequestResult wWWRequestResult = resultObj as KFFNetwork.WWWRequestResult;
		if (wWWRequestResult == null)
		{
		}
		if (wWWRequestResult != null && wWWRequestResult.IsValid())
		{
			string key = "RZrb({~IXRD{Oen[UXZA(a5my-5bZe17JeJMyR-DcuOI6";
			string text = "azTn-kDeI=niczdj8BIw{oNG=6|]{{EY%[Ym";
			string valueAsString = wWWRequestResult.GetValueAsString(key);
			if (valueAsString != text)
			{
				resultObj = null;
				err = KFFLocalization.Get("!!ERROR_PURCHASE_FAILED_RESULT_INVALID");
			}
			else
			{
				NotifyFulfillment((PurchaseReceipt)param);
			}
		}
		if (verifyIAPReceiptCallback != null)
		{
			verifyIAPReceiptCallback(wwwinfo, resultObj, err, param);
			verifyIAPReceiptCallback = null;
		}
	}

	private void AmazonPurchaseResponseCallback(PurchaseResponse response)
	{
		if (response.Status == "SUCCESSFUL")
		{
			purchases.Add(response.PurchaseReceipt);
			if (productPurchaseCallback != null)
			{
				PurchaseManager.TransactionData transactionData = new PurchaseManager.TransactionData();
				transactionData.NativeTransaction = response;
				productPurchaseCallback(PurchaseManager.ProductPurchaseResult.Success, transactionData, null);
			}
		}
		else if (productPurchaseCallback != null)
		{
			productPurchaseCallback(PurchaseManager.ProductPurchaseResult.Failed, null, "failed");
		}
	}

	private void AmazonUserDataResponseCallback(GetUserDataResponse response)
	{
		if (response.Status == "SUCCESSFUL")
		{
			amazonUserID = response.AmazonUserData.UserId;
		}
	}

	private void AmazonProductDataRequestCallback(GetProductDataResponse response)
	{
		if (response.Status == "SUCCESSFUL")
		{
			Dictionary<string, ProductData> productDataMap = response.ProductDataMap;
			List<string> unavailableSkus = response.UnavailableSkus;
			List<PurchaseManager.ProductData> list = new List<PurchaseManager.ProductData>();
			foreach (KeyValuePair<string, ProductData> item in productDataMap)
			{
				PurchaseManager.ProductData productData = new PurchaseManager.ProductData();
				productData.ProductIdentifier = item.Value.Sku;
				productData.Title = item.Value.Title;
				productData.Price = item.Value.Price;
				productData.Description = item.Value.Description;
				productData.FormattedPrice = item.Value.Price;
				list.Add(productData);
			}
			if (receivedProductDataCallback != null)
			{
				receivedProductDataCallback(true, list, null);
			}
		}
		else if (receivedProductDataCallback != null)
		{
			receivedProductDataCallback(false, null, string.Empty);
		}
	}

	private void AmazonPurchaseUpdatesResponseCallback(GetPurchaseUpdatesResponse response)
	{
		if (processPurchaseUpdates <= 0)
		{
			return;
		}
		processPurchaseUpdates--;
		if (!(response.Status == "SUCCESSFUL"))
		{
			return;
		}
		List<PurchaseReceipt> receipts = response.Receipts;
		foreach (PurchaseReceipt item in receipts)
		{
			long cancelDate = item.CancelDate;
			if (cancelDate != 0L)
			{
				Singleton<PurchaseManager>.Instance.RevokePurchase(item.Sku);
				continue;
			}
			bool success = Singleton<PurchaseManager>.Instance.RedeemPurchase(item.Sku);
			NotifyFulfillment(item, success);
		}
		if (response.HasMore)
		{
			AmazonIapV2Impl.Instance.GetPurchaseUpdates(new ResetInput
			{
				Reset = false
			});
		}
	}

	private void NotifyFulfillment(PurchaseReceipt receipt, bool success = true)
	{
		if (receipt != null)
		{
			AmazonIapV2Impl.Instance.NotifyFulfillment(new NotifyFulfillmentInput
			{
				ReceiptId = receipt.ReceiptId,
				FulfillmentResult = ((!success) ? "UNAVAILABLE" : "FULFILLED")
			});
		}
	}
}
