using System.Collections.Generic;

public class TFAnalytics
{
	private const bool LOG_MISSING_ANALYTICS = true;

	public static void LogEvent(string eventName)
	{
		KontagentBinding.customEvent(eventName, null);
	}

	public static void LogEvent(string eventName, Dictionary<string, object> eventData)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (KeyValuePair<string, object> eventDatum in eventData)
		{
			dictionary["n"] = eventName;
			if (eventDatum.Key == "level")
			{
				dictionary["l"] = eventDatum.Value.ToString();
			}
			else if (eventDatum.Key == "value")
			{
				dictionary["v"] = eventDatum.Value.ToString();
			}
			else if (eventDatum.Key == "subtype1")
			{
				dictionary["st1"] = eventDatum.Value.ToString();
			}
			else if (eventDatum.Key == "subtype2")
			{
				dictionary["st2"] = eventDatum.Value.ToString();
			}
			else if (eventDatum.Key == "subtype3")
			{
				dictionary["st3"] = eventDatum.Value.ToString();
			}
			else
			{
				dictionary[eventDatum.Key] = eventDatum.Value.ToString();
			}
		}
		KontagentBinding.customEvent(eventName, dictionary);
	}

	public static void LogIAP(string eventName, Dictionary<string, object> eventData)
	{
		if (KFFUpsightManager.UpsightIAPLoggingEnabled)
		{
			return;
		}
		TFUtils.DebugLog("H: LogIAP is called : " + eventName);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		int value = 0;
		foreach (KeyValuePair<string, object> eventDatum in eventData)
		{
			dictionary["n"] = eventName;
			if (eventDatum.Key == "level")
			{
				dictionary["l"] = eventDatum.Value.ToString();
			}
			else if (eventDatum.Key == "value")
			{
				value = (int)eventDatum.Value;
				dictionary["v"] = eventDatum.Value.ToString();
			}
			else if (eventDatum.Key == "subtype1")
			{
				dictionary["st1"] = eventDatum.Value.ToString();
			}
			else if (eventDatum.Key == "subtype2")
			{
				dictionary["st2"] = eventDatum.Value.ToString();
			}
			else if (eventDatum.Key == "subtype3")
			{
				dictionary["st3"] = eventDatum.Value.ToString();
			}
			else
			{
				dictionary[eventDatum.Key] = eventDatum.Value.ToString();
			}
		}
		KontagentBinding.revenueTracking(value, dictionary);
	}
}
