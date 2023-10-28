using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

[RequireComponent(typeof(Kochava))]
public class KFFKochavaAnalytics : MonoBehaviour
{
	public string kochavaDevAppId = string.Empty;

	public string kochavaDevAppIdIOS = string.Empty;

	public string kochavaDevAppIdAndroid = string.Empty;

	public string kochavaDevAppIdKindle = string.Empty;

	public static void LogEvent(string eventName, string eventData)
	{
		Kochava.FireEvent(eventName, eventData);
	}

	public static void LogEvent(string eventName, Dictionary<string, object> eventData)
	{
		string eventData2 = Json.Serialize(eventData);
		Kochava.FireEvent(eventName, eventData2);
	}

	private void Awake()
	{
		Kochava component = GetComponent<Kochava>();
		component.appCurrency = KFFCSUtils.GetCurrencyCode();
		TFUtils.DebugLog("Override Kochava app currency: " + component.appCurrency);
		if (KFFCSUtils.IsAmazonDevice())
		{
			component.adidSupressed = true;
		}
	}
}
