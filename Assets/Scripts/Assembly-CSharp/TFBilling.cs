using System.Collections.Generic;

public class TFBilling
{
	public const string PRODUCT_INFO_REQUEST = "productInfo";

	public const string PURCHASE_UPDATE = "purchaseUpdate";

	public const string PURCHASE_RESPONSE = "purchaseResponse";

	public const string PURCHASE_COMPLETE = "purchaseComplete";

	public const string PURCHASE_INFO = "purchaseInfo";

	public const string PURCHASE_COMPLETED = "completed";

	public const string PURCHASE_FAILED = "failed";

	public const string PURCHASE_STARTED = "started";

	public const string TECHNICAL_FAILURE = "technicalFailure";

	public const string USER_CANCEL = "userCancelled";

	public static bool BillingIsAvailable()
	{
		return InternalBillingIsAvailable();
	}

	public static void InitializeStore()
	{
		InternalInitializeStore();
	}

	public static void FetchProductBillingInfo(List<string> productIds)
	{
		InternalFetchBillingInfo(productIds);
	}

	public static void StartPremiumPurchase(string productId)
	{
		InternalStartPremiumPurchase(productId);
	}

	public static void CompletePremiumPurchase(string transactionId)
	{
		InternalCompletePremiumPurchase(transactionId);
	}

	public static void ResumePurchase()
	{
		InternalResumePurchase();
	}

	private static bool InternalBillingIsAvailable()
	{
		return false;
	}

	private static void InternalInitializeStore()
	{
	}

	private static void InternalFetchBillingInfo(List<string> productIds)
	{
	}

	private static void InternalStartPremiumPurchase(string productId)
	{
	}

	private static void InternalCompletePremiumPurchase(string transactionId)
	{
	}

	private static void InternalResumePurchase()
	{
	}
}
