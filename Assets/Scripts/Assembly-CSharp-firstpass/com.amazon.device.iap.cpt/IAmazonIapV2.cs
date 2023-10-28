namespace com.amazon.device.iap.cpt
{
	public interface IAmazonIapV2
	{
		RequestOutput GetUserData();

		RequestOutput Purchase(SkuInput skuInput);

		RequestOutput GetProductData(SkusInput skusInput);

		RequestOutput GetPurchaseUpdates(ResetInput resetInput);

		void NotifyFulfillment(NotifyFulfillmentInput notifyFulfillmentInput);

		void UnityFireEvent(string jsonMessage);

		void AddGetUserDataResponseListener(GetUserDataResponseDelegate responseDelegate);

		void RemoveGetUserDataResponseListener(GetUserDataResponseDelegate responseDelegate);

		void AddPurchaseResponseListener(PurchaseResponseDelegate responseDelegate);

		void RemovePurchaseResponseListener(PurchaseResponseDelegate responseDelegate);

		void AddGetProductDataResponseListener(GetProductDataResponseDelegate responseDelegate);

		void RemoveGetProductDataResponseListener(GetProductDataResponseDelegate responseDelegate);

		void AddGetPurchaseUpdatesResponseListener(GetPurchaseUpdatesResponseDelegate responseDelegate);

		void RemoveGetPurchaseUpdatesResponseListener(GetPurchaseUpdatesResponseDelegate responseDelegate);
	}
}
