using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

public class Upsight
{
	private static AndroidJavaObject _plugin;

	static Upsight()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		UpsightManager.noop();
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.upsight.unity.UpsightPlugin"))
		{
			_plugin = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
		}
	}

	public static void setLogLevel(UpsightLogLevel logLevel)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("setLogLevel", logLevel.ToString());
		}
	}

	public static string getPluginVersion()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return "UnityEditor";
		}
		return _plugin.Call<string>("getPluginVersion", new object[0]);
	}

	public static void init(string appToken, string appSecret, string gcmProjectNumber = null)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("init", appToken, appSecret, gcmProjectNumber);
		}
	}

	public static void requestAppOpen()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("requestAppOpen");
		}
	}

	public static void sendContentRequest(string placementID, bool showsOverlayImmediately, bool shouldAnimate = true, Dictionary<string, object> dimensions = null)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			AndroidJavaObject androidJavaObject = dictionaryToJavaHashMap(dimensions);
			_plugin.Call("sendContentRequest", placementID, showsOverlayImmediately, shouldAnimate, androidJavaObject);
		}
	}

	public static AndroidJavaObject dictionaryToJavaHashMap(Dictionary<string, object> dictionary)
	{
		AndroidJavaObject result = null;
		if (dictionary != null)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("net.minidev.json.parser.JSONParser");
			int @static = androidJavaClass.GetStatic<int>("MODE_JSON_SIMPLE");
			string text = Json.Serialize(dictionary);
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("net.minidev.json.parser.JSONParser", @static);
			result = androidJavaObject.Call<AndroidJavaObject>("parse", new object[1] { text });
		}
		return result;
	}

	public static void preloadContentRequest(string placementID, Dictionary<string, object> dimensions = null)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			AndroidJavaObject androidJavaObject = dictionaryToJavaHashMap(dimensions);
			_plugin.Call("preloadContentRequest", placementID, androidJavaObject);
		}
	}

	public static void getContentBadgeNumber(string placementID)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("sendMetadataRequest", placementID);
		}
	}

	public static bool getOptOutStatus()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return false;
		}
		return _plugin.Call<bool>("getOptOutStatus", new object[0]);
	}

	public static void setOptOutStatus(bool optOutStatus)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("setOptOutStatus", optOutStatus);
		}
	}

	public static void trackInAppPurchase(string sku, int quantity, UpsightAndroidPurchaseResolution resolutionType, double price, string orderId, string store)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("trackInAppPurchase", sku, quantity, (int)resolutionType, price, orderId, store);
		}
	}

	public static void reportCustomEvent(Dictionary<string, object> properties)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("reportCustomEvent", Json.Serialize(properties));
		}
	}

	public static void registerForPushNotifications()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("registerForPushNotifications");
		}
	}

	public static void deregisterForPushNotifications()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("deregisterForPushNotifications");
		}
	}

	public static void setShouldOpenContentRequestsFromPushNotifications(bool shouldOpen)
	{
	}

	public static void setShouldOpenUrlsFromPushNotifications(bool shouldOpen)
	{
	}
}
