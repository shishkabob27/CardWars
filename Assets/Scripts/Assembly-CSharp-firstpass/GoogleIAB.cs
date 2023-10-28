using UnityEngine;

public class GoogleIAB
{
	private static AndroidJavaObject _plugin;

	static GoogleIAB()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.prime31.GoogleIABPlugin"))
		{
			_plugin = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
		}
	}

	public static void enableLogging(bool shouldEnable)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			if (shouldEnable)
			{
			}
			_plugin.Call("enableLogging", shouldEnable);
		}
	}

	public static void setAutoVerifySignatures(bool shouldVerify)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("setAutoVerifySignatures", shouldVerify);
		}
	}

	public static void init(string publicKey)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("init", publicKey);
		}
	}

	public static void unbindService()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("unbindService");
		}
	}

	public static bool areSubscriptionsSupported()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return false;
		}
		return _plugin.Call<bool>("areSubscriptionsSupported", new object[0]);
	}

	public static void queryInventory(string[] skus)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("queryInventory", new object[1] { skus });
		}
	}

	public static void purchaseProduct(string sku)
	{
		purchaseProduct(sku, string.Empty);
	}

	public static void purchaseProduct(string sku, string developerPayload)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("purchaseProduct", sku, developerPayload);
		}
	}

	public static void consumeProduct(string sku)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("consumeProduct", sku);
		}
	}

	public static void consumeProducts(string[] skus)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("consumeProducts", new object[1] { skus });
		}
	}
}
