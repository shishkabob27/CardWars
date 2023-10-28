public class iOSPurchaseListener : IPurchaseListener
{
	public void OnEnable()
	{
	}

	public void OnDisable()
	{
	}

	public void Fetch()
	{
	}

	public PurchaseManager.ProductDataRequestResult GetProductData(string[] a_ProductIDs, PurchaseManager.ReceivedProductDataCallback a_Callback)
	{
		return PurchaseManager.ProductDataRequestResult.CannotMakePayment;
	}

	public void PurchaseProduct(string a_ProductID, PurchaseManager.ProductPurchaseCallback a_Callback)
	{
	}

	public void ConsumeProduct(string a_productID)
	{
	}

	public void RestorePurchases(PurchaseManager.RestorePurchasesCallback callback = null)
	{
	}

	public KFFNetwork.WWWInfo VerifyIAPReceipt(PurchaseManager.TransactionData transaction, PurchaseManager.VerifyIAPReceiptCallback callback)
	{
		return null;
	}
}
