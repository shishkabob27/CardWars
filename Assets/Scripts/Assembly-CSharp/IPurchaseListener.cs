public interface IPurchaseListener
{
	void OnEnable();

	void OnDisable();

	void Fetch();

	PurchaseManager.ProductDataRequestResult GetProductData(string[] a_ProductIDs, PurchaseManager.ReceivedProductDataCallback a_Callback);

	void PurchaseProduct(string a_ProductID, PurchaseManager.ProductPurchaseCallback a_Callback);

	void ConsumeProduct(string a_productID);

	void RestorePurchases(PurchaseManager.RestorePurchasesCallback callback = null);

	KFFNetwork.WWWInfo VerifyIAPReceipt(PurchaseManager.TransactionData a_Transaction, PurchaseManager.VerifyIAPReceiptCallback a_Callback);
}
