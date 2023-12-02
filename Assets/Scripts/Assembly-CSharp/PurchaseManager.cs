using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class PurchaseManager : Singleton<PurchaseManager>
{
	public enum ProductDataRequestResult
	{
		Success,
		CannotMakePayment,
		Busy
	}

	public enum ProductPurchaseResult
	{
		Success,
		Failed,
		Cancelled
	}

	public class ProductData
	{
		public string ProductIdentifier;

		public string Title;

		public string Description;

		public string Price;

		public string CurrencySymbol;

		public string CurrencyCode;

		public string FormattedPrice;

		public override string ToString()
		{
			return string.Format("<ProductData>\nID: {0}\nTitle: {1}\nDescription: {2}\nPrice: {3}\nCurrency Symbol: {4}\nFormatted Price: {5}\nCurrency Code: {6}", ProductIdentifier, Title, Description, Price, CurrencySymbol, FormattedPrice, CurrencyCode);
		}
	}

	public class TransactionData
	{
		public object NativeTransaction;

		public override string ToString()
		{
			GooglePurchase googlePurchase = (GooglePurchase)NativeTransaction;
			return string.Format("<GooglePurchase> ID: {0}, type: {1}, transactionIdentifier: {2}", googlePurchase.productId, googlePurchase.type, googlePurchase.orderId);
		}
	}

	public class DbProductInfo
	{
		public string productID { get; private set; }

		public string productType { get; private set; }

		public float price { get; private set; }

		public bool consumable { get; private set; }

		public int productCount { get; private set; }

		public int gems
		{
			get
			{
				return (productType == "Gem") ? productCount : 0;
			}
		}

		public int coins
		{
			get
			{
				return (productType == "Coin") ? productCount : 0;
			}
		}

		public DbProductInfo(string inProductId, string inProductType, int inProductCount, float inPrice, bool inConsumable)
		{
			productID = inProductId;
			productType = inProductType;
			productCount = inProductCount;
			price = inPrice;
			consumable = inConsumable;
		}
	}

	public delegate void ReceivedProductDataCallback(bool success, List<ProductData> list, string err);

	public delegate void ProductPurchaseCallback(ProductPurchaseResult result, TransactionData transaction, string err);

	public delegate void VerifyIAPReceiptCallback(KFFNetwork.WWWInfo wwwinfo, object resultObj, string err, object param);

	public delegate void RestorePurchasesCallback(bool success);

	private static readonly TimeSpan PRODUCTDATA_EXPIRY_DELAY = new TimeSpan(1, 0, 0);

	private DateTime productDataExpiryTime = DateTime.MinValue;

	private List<ProductData> m_Products = new List<ProductData>();

	private int busyCounter;

	private bool AmazonDevice;

	private IPurchaseListener m_Listener;

	public static string CHECK_CLIENT_VERSION_URL = "http://cardwars.retroretreat.net/DomoJump/IAPReceiptVerificationServer/check_client_version.php";

	public static string CHECK_ASSET_DOWNLOADS_URL = "http://cardwars.retroretreat.net/DomoJump/IAPReceiptVerificationServer/check_asset_downloads.php";

	private List<DbProductInfo> storeItems;

	private bool isRetrievingProductData;

	public bool HasServerProductData
	{
		get
		{
			return DateTime.Now - productDataExpiryTime < PRODUCTDATA_EXPIRY_DELAY;
		}
		private set
		{
			if (value)
			{
				productDataExpiryTime = DateTime.Now;
			}
			else
			{
				productDataExpiryTime = DateTime.MinValue;
			}
		}
	}

	public bool IsAmazon
	{
		get
		{
			return AmazonDevice;
		}
		private set
		{
		}
	}

	public static string IAP_VERIFICATION_SERVER_URL
	{
		get
		{
			return "http://cardwars.retroretreat.net/AdventureTime/CardWars/IAPReceiptVerificationServer";
		}
	}

	public static bool SLOT_IAP_SANDBOX
	{
		get
		{
			RuntimePlatform platform = Application.platform;
			if (platform == RuntimePlatform.IPhonePlayer)
			{
				return true;
			}
			return false;
		}
	}

	public void GetPriceInfo(string ProductID, out float Price, out string CurrencyType)
	{
		foreach (ProductData product in m_Products)
		{
			if (product.ProductIdentifier == ProductID)
			{
				Price = float.Parse(product.Price, CultureInfo.InvariantCulture.NumberFormat);
				CurrencyType = product.CurrencyCode;
				return;
			}
		}
		DbProductInfo dbProductInfoById = GetDbProductInfoById(ProductID);
		Price = ((dbProductInfoById == null) ? 0f : dbProductInfoById.price);
		CurrencyType = "US";
	}

	public void GetformattedPrice(string ProductID, out string Price)
	{
		foreach (ProductData product in m_Products)
		{
			if (product.ProductIdentifier == ProductID)
			{
				Price = product.FormattedPrice;
				return;
			}
		}
		DbProductInfo dbProductInfoById = GetDbProductInfoById(ProductID);
		Price = ((dbProductInfoById == null) ? "0.00" : dbProductInfoById.price.ToString() + "C");
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
		AmazonDevice = KFFCSUtils.IsAmazonDevice();
		m_Listener = new GooglePurchaseListener();
	}

	private void OnEnable()
	{
		if (m_Listener != null)
		{
			m_Listener.OnEnable();
		}
	}

	private void OnDisable()
	{
		if (m_Listener != null)
		{
			m_Listener.OnDisable();
		}
	}

	private void OnDestroy()
	{
		if (m_Listener != null)
		{
			m_Listener.OnDisable();
		}
	}

	private void Update()
	{
		if (m_Listener != null)
		{
			m_Listener.Fetch();
		}
	}

	public bool IsBusy()
	{
		return busyCounter > 0;
	}

	public void PurchaseProduct(string a_productID, ProductPurchaseCallback a_Callback)
	{
		TFUtils.DebugLog("calling KFF.PurchaseProduct", "iap");
		RedeemPurchase(a_productID);
		return;
		if (m_Listener != null)
		{
			m_Listener.PurchaseProduct(a_productID, a_Callback);
		}
	}

	public void ConsumeProduct(string a_productID)
	{
		TFUtils.DebugLog("calling KFF.ConsummeProduct", "iap");
		if (!Singleton<PurchaseManager>.Instance.IsConsumable(a_productID))
		{
			TFUtils.DebugLog("attempting to consume un-consumable " + a_productID + ". Ignoring request.", "iap");
		}
		else if (m_Listener != null)
		{
			m_Listener.ConsumeProduct(a_productID);
		}
	}

	public void RestorePurchases(RestorePurchasesCallback callback = null)
	{
		TFUtils.DebugLog("calling KFF.RestorePurchases", "iap");
		StartCoroutine(CoroutineRestorePurchase(callback));
	}

	private ProductDataRequestResult GetProductData(string[] a_StringIDs, ReceivedProductDataCallback a_Callback)
	{
		TFUtils.DebugLog("calling KFF.GetProductData", "iap");

		List<ProductData> productList = new List<ProductData>();

		foreach(DbProductInfo localproduct in storeItems)
		{
			ProductData newproduct = new ProductData();
			newproduct.ProductIdentifier = localproduct.productID;
			newproduct.Price = localproduct.price.ToString();

		}

		if (productList != null)
		{
			HasServerProductData = true;

			m_Products = productList;
		}
		if (a_Callback != null)
		{
			a_Callback(true, productList, "");
		}
		return ProductDataRequestResult.Success;
	}

	private IEnumerator CoroutineRestorePurchase(RestorePurchasesCallback callback)
	{
		yield return null;
		if (m_Listener != null)
		{
			m_Listener.RestorePurchases(callback);
		}
	}

	public KFFNetwork.WWWInfo VerifyIAPReceipt(TransactionData transaction, VerifyIAPReceiptCallback callback)
	{
		TFUtils.DebugLog("calling KFF.VerifyIAPReceipt", "iap");
		if (m_Listener != null)
		{
			return m_Listener.VerifyIAPReceipt(transaction, callback);
		}
		return null;
	}

	private void Start()
	{
		ReadTable();
	}

	private void ReadTable()
	{
		if (storeItems == null)
		{
			Dictionary<string, object>[] array = SQUtils.ReadJSONData("db_IAP.json");
			storeItems = new List<DbProductInfo>();
			string text = "ProductID";
			text = "ProductID_Android";
			Dictionary<string, object>[] array2 = array;
			foreach (Dictionary<string, object> dictionary in array2)
			{
				string inProductId = TFUtils.LoadString(dictionary, text);
				string inProductType = TFUtils.LoadString(dictionary, "Product", string.Empty);
				int inProductCount = TFUtils.LoadInt(dictionary, "CurrencyCount");
				float inPrice = TFUtils.LoadFloat(dictionary, "Price");
				bool inConsumable = TFUtils.LoadBool(dictionary, "Consumable", true);
				DbProductInfo item = new DbProductInfo(inProductId, inProductType, inProductCount, inPrice, inConsumable);
				storeItems.Add(item);
			}
		}
	}

	public int GetStoreItemCount()
	{
		ReadTable();
		if (storeItems != null)
		{
			return storeItems.Count;
		}
		return 0;
	}

	public int GetCoinStoreItemCount()
	{
		ReadTable();
		IEnumerable<DbProductInfo> source = storeItems.Where((DbProductInfo s) => s.coins > 0);
		return source.Count();
	}

	public int GetGemStoreItemCount()
	{
		ReadTable();
		IEnumerable<DbProductInfo> source = storeItems.Where((DbProductInfo s) => s.gems > 0);
		return source.Count();
	}

	public DbProductInfo GetStoreItemInfo(int itemIndex)
	{
		ReadTable();
		if (itemIndex >= 0 && itemIndex < storeItems.Count)
		{
			return storeItems[itemIndex];
		}
		return null;
	}

	public int GetCoinCountForProductID(string productID)
	{
		ReadTable();
		DbProductInfo dbProductInfo = storeItems.Where((DbProductInfo s) => s.productID == productID).FirstOrDefault();
		if (dbProductInfo != null)
		{
			return dbProductInfo.coins;
		}
		return 0;
	}

	public string GetTypeForProductID(string productID)
	{
		ReadTable();
		DbProductInfo dbProductInfo = storeItems.Where((DbProductInfo s) => s.productID == productID).FirstOrDefault();
		if (dbProductInfo != null)
		{
			return dbProductInfo.productType;
		}
		return string.Empty;
	}

	public int GetGemCountForProductID(string productID)
	{
		ReadTable();
		DbProductInfo dbProductInfo = storeItems.Where((DbProductInfo s) => s.productID == productID).FirstOrDefault();
		if (dbProductInfo != null)
		{
			return dbProductInfo.gems;
		}
		return 0;
	}

	public float GetPriceForProductID(string productID)
	{
		ReadTable();
		DbProductInfo dbProductInfo = storeItems.Where((DbProductInfo s) => s.productID == productID).FirstOrDefault();
		return (dbProductInfo == null) ? 0f : dbProductInfo.price;
	}

	public bool IsConsumable(string productID)
	{
		ReadTable();
		DbProductInfo dbProductInfo = storeItems.Where((DbProductInfo s) => s.productID == productID).FirstOrDefault();
		return dbProductInfo.consumable;
	}

	public ProductDataRequestResult GetProductData(ReceivedProductDataCallback a_Callback)
	{
		int storeItemCount = GetStoreItemCount();
		List<string> list2 = new List<string>();
		for (int i = 0; i < storeItemCount; i++)
		{
			DbProductInfo storeItemInfo = Singleton<PurchaseManager>.Instance.GetStoreItemInfo(i);
			if (storeItemInfo != null)
			{
				list2.Add(storeItemInfo.productID);
			}
		}
		string[] a_StringIDs = list2.ToArray();
		isRetrievingProductData = true;
        return GetProductData(a_StringIDs, delegate(bool success, List<ProductData> list, string error)
		{
			isRetrievingProductData = false;
            if (success || !string.IsNullOrEmpty(error))
			{
            }
			if (a_Callback != null)
			{
				a_Callback(success, list, error);
			}
		});
	}

	public bool RedeemPurchase(string productID)
	{
		DbProductInfo dbProductInfoById = GetDbProductInfoById(productID);
		if (dbProductInfoById == null)
		{
			return false;
		}
		TFUtils.DebugLog("START RedeemPurchase: " + productID + ", consumable: " + dbProductInfoById.consumable);
		if (!OnRedeemPurchase(dbProductInfoById))
		{
			return false;
		}
		TFUtils.DebugLog("SUCCESS RedeemPurchase: " + productID);
		Singleton<AnalyticsManager>.Instance.LogIAPByBattle(productID, dbProductInfoById.price);
		return true;
	}

	public void RevokePurchase(string productID)
	{
		TFUtils.DebugLog("RevokePurchase: " + productID);
		DbProductInfo dbProductInfoById = GetDbProductInfoById(productID);
		if (dbProductInfoById != null)
		{
			OnRevokePurchase(dbProductInfoById);
		}
	}

	protected virtual bool OnRedeemPurchase(DbProductInfo product)
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (instance == null)
		{
			return false;
		}
		bool flag = true;
		switch (product.productType.ToLower())
		{
		case "gem":
			if (instance.Coins >= (int)product.price)
			{
                TFUtils.DebugLog("RedeemPurchase granting " + product.productCount + " gems from " + product.productID, "iap");
                instance.Gems += product.productCount;
                instance.Coins -= (int)product.price;
            }
            break;
		case "coin":
			TFUtils.DebugLog("RedeemPurchase Granting " + product.productCount + " coins from " + product.productID, "iap");
			instance.Coins += product.productCount;
			break;
		case "fc1":
			TFUtils.DebugLog("RedeemPurchase Granting F&C 1 from " + product.productID, "iap");
			flag = instance.SetHasPurchasedFC(product.productID);
			if (flag && VirtualGoodsDataManager.Instance.GetVirtualGoods(product.productID) != null)
			{
				flag = AwardBundle(product.productID);
			}
			break;
		case "bundle":
			TFUtils.DebugLog("RedeemPurchase Granting bundle " + product.productID, "iap");
			flag = AwardBundle(product.productID);
			break;
		default:
			flag = false;
			break;
		}
		if (!flag)
		{
			return false;
		}
		try
		{
			instance.Save();
		}
		catch (Exception ex)
		{
			TFUtils.WarnLog("playerInfo.Save() failed. Exception: " + ex, "iap");
		}
		return true;
	}

	private bool AwardBundle(string productID)
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (instance == null || !instance.IsReady())
		{
			TFUtils.WarnLog("Virtual Goods " + productID + " cannot be consumed. PlayerInfo is not ready.", "iap");
			return false;
		}
		VirtualGoods virtualGoods = VirtualGoodsDataManager.Instance.GetVirtualGoods(productID);
		if (virtualGoods == null)
		{
			TFUtils.WarnLog("Virtual Goods " + productID + " not recognized.", "iap");
			return false;
		}
		foreach (string occuranceString in virtualGoods.OccuranceStrings)
		{
			instance.IncOccuranceCounter(occuranceString);
		}
		instance.Gems += virtualGoods.Gems;
		instance.Coins += virtualGoods.Coins;
		instance.Stamina += virtualGoods.Hearts;
		instance.MaxInventory += virtualGoods.Inventory;
		foreach (string card in virtualGoods.Cards)
		{
			string text = card.Trim();
			if (text.StartsWith("Leader_", StringComparison.CurrentCultureIgnoreCase))
			{
				LeaderManager.Instance.AddNewLeaderIfUnique(text);
			}
			else
			{
				instance.DeckManager.AddCardAward(text);
			}
		}
		return true;
	}

	protected virtual void OnRevokePurchase(DbProductInfo product)
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (instance == null)
		{
			return;
		}
		switch (product.productType.ToLower())
		{
		case "fc1":
			TFUtils.DebugLog("RevokePurchase Revoking F&C 1 from " + product.productID, "iap");
			instance.UnsetHasPurchasedFC(product.productID);
			break;
		}
		try
		{
			instance.Save();
		}
		catch (Exception ex)
		{
			TFUtils.WarnLog("playerInfo.Save() failed. Exception: " + ex, "iap");
		}
	}

	private DbProductInfo GetDbProductInfoById(string productID)
	{
		try
		{
			return storeItems.Where((DbProductInfo s) => s.productID == productID).First();
		}
		catch (InvalidOperationException)
		{
		}
		return null;
	}
}
