using System;
using System.Collections.Generic;
using UnityEngine;

public class KontagentBinding
{
	private static AndroidJavaObject _plugin;

	static KontagentBinding()
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.prime31.KontagentPlugin"))
		{
			_plugin = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
		}
	}

	public static AndroidJavaObject mapFromDictionary(Dictionary<string, string> dict)
	{
		if (dict == null)
		{
			dict = new Dictionary<string, string>();
		}
		AndroidJavaObject androidJavaObject = new AndroidJavaObject("java.util.HashMap");
		AndroidJNI.PushLocalFrame(50);
		IntPtr methodID = AndroidJNIHelper.GetMethodID(androidJavaObject.GetRawClass(), "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
		object[] array = new object[2];
		foreach (KeyValuePair<string, string> item in dict)
		{
			using (AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("java.lang.String", item.Key))
			{
				using (AndroidJavaObject androidJavaObject3 = new AndroidJavaObject("java.lang.String", item.Value))
				{
					array[0] = androidJavaObject2;
					array[1] = androidJavaObject3;
					AndroidJNI.CallObjectMethod(androidJavaObject.GetRawObject(), methodID, AndroidJNIHelper.CreateJNIArgArray(array));
				}
			}
		}
		AndroidJNI.PopLocalFrame(IntPtr.Zero);
		return androidJavaObject;
	}

	public static KontagentSession createSession(string apiKey, string senderId, bool isOnTestMode, bool doSendApplicationAdded, string facebookApplicationId)
	{
		AndroidJavaObject jSession = _plugin.Call<AndroidJavaObject>("createSession", new object[5] { apiKey, senderId, isOnTestMode, doSendApplicationAdded, facebookApplicationId });
		return new KontagentSession(jSession);
	}

	public static void enableDebug()
	{
		_plugin.Call("enableDebug", true);
	}

	public static void disableDebug()
	{
		_plugin.Call("enableDebug", false);
	}

	public static void startSession(string apiKey, bool enableTestMode)
	{
		startSession(apiKey, enableTestMode, null);
	}

	public static void startSession(string apiKey, bool enableTestMode, string senderId)
	{
		startSession(apiKey, enableTestMode, senderId, null);
	}

	public static void startSession(string apiKey, bool enableTestMode, string senderId, bool shouldSendAPA)
	{
		startSession(apiKey, enableTestMode, senderId, shouldSendAPA, null, null, null);
	}

	public static void startSession(string apiKey, bool enableTestMode, string senderId, string fbAppID)
	{
		startSession(apiKey, enableTestMode, senderId, true, null, null, null, fbAppID);
	}

	public static void startSession(string apiKey, bool enableTestMode, string senderId, bool shouldSendAPA, string apiKeyForTimezone, string apiKeyTimezoneOffset, string customID)
	{
		startSession(apiKey, enableTestMode, senderId, shouldSendAPA, apiKeyForTimezone, apiKeyTimezoneOffset, customID, null);
	}

	public static void startSession(string apiKey, bool enableTestMode, string senderId, bool shouldSendAPA, string apiKeyForTimezone, string apiKeyTimezoneOffset, string customID, string fbAppID)
	{
		startSession(apiKey, enableTestMode, senderId, shouldSendAPA, apiKeyForTimezone, apiKeyTimezoneOffset, customID, fbAppID, false);
	}

	public static void startSession(string apiKey, bool enableTestMode, string senderId, bool shouldSendAPA, string apiKeyForTimezone, string apiKeyTimezoneOffset, string customID, string fbAppID, bool enableAcquisitionTracking)
	{
		_plugin.Call("startSession", (apiKey != null) ? apiKey : string.Empty, enableTestMode ? 1 : 0, (senderId != null) ? senderId : string.Empty, shouldSendAPA ? 1 : 0, (apiKeyForTimezone != null) ? apiKeyForTimezone : string.Empty, (apiKeyTimezoneOffset != null) ? apiKeyTimezoneOffset : string.Empty, customID, (fbAppID != null) ? fbAppID : string.Empty);
	}

	public static void stopSession()
	{
		_plugin.Call("stopSession");
	}

	[Obsolete("Will be removed in next Plugin release")]
	public static void pageRequest(Dictionary<string, string> optionalParams)
	{
		_plugin.Call("pageRequest", mapFromDictionary(optionalParams));
	}

	[Obsolete("Will be removed in next Plugin release")]
	public static void messageSent(string message, string trackingId, Dictionary<string, string> optionalParams)
	{
		if (trackingId == null)
		{
			trackingId = string.Empty;
		}
		int num = Convert.ToInt32(message);
		_plugin.Call("messageSent", num, trackingId, mapFromDictionary(optionalParams));
	}

	[Obsolete("Will be removed in next Plugin release")]
	public static void messageResponse(bool applicationInstalled, string trackingId, Dictionary<string, string> optionalParams)
	{
		if (trackingId == null)
		{
			trackingId = string.Empty;
		}
		_plugin.Call("messageResponse", applicationInstalled ? 1 : 0, trackingId, mapFromDictionary(optionalParams));
	}

	public static void applicationAdded(Dictionary<string, string> optionalParams)
	{
		_plugin.Call("applicationAdded", mapFromDictionary(optionalParams));
	}

	public static void customEvent(string eventName, Dictionary<string, string> optionalParams)
	{
		_plugin.Call("customEvent", eventName, mapFromDictionary(optionalParams));
	}

	public static void goalCount(int goalCountId, int goalCountValue)
	{
		_plugin.Call("goalCount", goalCountId, goalCountValue);
	}

	[Obsolete("Will be removed in next Plugin release")]
	public static void inviteSent(string recipientUIDs, string trackingId, Dictionary<string, string> optionalParams)
	{
		int num = Convert.ToInt32(recipientUIDs);
		_plugin.Call("inviteSent", num, trackingId, mapFromDictionary(optionalParams));
	}

	[Obsolete("Will be removed in next Plugin release")]
	public static void inviteResponse(bool applicationInstalled, string trackingId, Dictionary<string, string> optionalParams)
	{
		_plugin.Call("inviteResponse", applicationInstalled ? 1 : 0, trackingId, mapFromDictionary(optionalParams));
	}

	public static void revenueTracking(int value, Dictionary<string, string> optionalParams)
	{
		_plugin.Call("revenueTracking", value, mapFromDictionary(optionalParams));
	}

	[Obsolete("Will be removed in next Plugin release")]
	public static void streamPost(string type, string trackingId, Dictionary<string, string> optionalParams)
	{
		if (trackingId == null)
		{
			trackingId = string.Empty;
		}
		_plugin.Call("streamPost", type, trackingId, mapFromDictionary(optionalParams));
	}

	[Obsolete("Will be removed in next Plugin release")]
	public static void streamPostResponse(bool applicationInstalled, string type, string trackingId, Dictionary<string, string> optionalParams)
	{
		if (trackingId == null)
		{
			trackingId = string.Empty;
		}
		_plugin.Call("streamPostResponse", applicationInstalled ? 1 : 0, type, trackingId, mapFromDictionary(optionalParams));
	}

	[Obsolete("Will be removed in next Plugin release")]
	public static void undirectedCommunicationClick(bool applicationInstalled, string type, Dictionary<string, string> optionalParams)
	{
		_plugin.Call("undirectedCommunicationClick", applicationInstalled ? 1 : 0, type, mapFromDictionary(optionalParams));
	}

	[Obsolete("Will be removed in next Plugin release")]
	public static void undirectedCommunicationClickWithTrackingTag(bool applicationInstalled, string type, string trackingTag, string trackingId, Dictionary<string, string> optionalParams)
	{
	}

	[Obsolete("Will be removed in next Plugin release")]
	public static void emailSent(string recipientUIDs, string trackingId, Dictionary<string, string> optionalParams)
	{
		if (trackingId == null)
		{
			trackingId = string.Empty;
		}
		_plugin.Call("notificationEmailSent", recipientUIDs, trackingId, mapFromDictionary(optionalParams));
	}

	[Obsolete("Will be removed in next Plugin release")]
	public static void emailResponse(bool applicationInstalled, string trackingId, Dictionary<string, string> optionalParams)
	{
		if (trackingId == null)
		{
			trackingId = string.Empty;
		}
		_plugin.Call("notificationEmailResponse", applicationInstalled ? 1 : 0, trackingId, mapFromDictionary(optionalParams));
	}

	[Obsolete("Will be removed in next Plugin release")]
	public static void userInformation(Dictionary<string, string> optionalParams)
	{
		_plugin.Call("userInformation", mapFromDictionary(optionalParams));
	}

	public static void sendDeviceInformation(Dictionary<string, string> optionalParams)
	{
		_plugin.Call("sendDeviceInformation", mapFromDictionary(optionalParams));
	}

	public static string libraryVersion()
	{
		string text = "n/a";
		return _plugin.Call<string>("libraryVersion", new object[0]);
	}

	public static int currentMaxQueueSizeForSessionApiKey(string apiKey)
	{
		int num = 0;
		return _plugin.Call<int>("currentMaxQueueSizeForSessionApiKey", new object[1] { apiKey });
	}

	public static void changeMaxQueueSizeForSessionApiKey(int newQueueSize, string apiKey)
	{
		_plugin.Call("changeMaxQueueSizeForSessionApiKey", newQueueSize, apiKey);
	}

	public static string defaultApiKey()
	{
		string text = "036bb229bacb4f7e953a7a31d2c88217";
		return _plugin.Call<string>("defaultApiKey", new object[0]);
	}

	public static bool isUnitTestsBuild()
	{
		bool flag = false;
		return _plugin.Call<int>("isUnitTestsBuild", new object[0]) == 1;
	}

	public static string getSenderID(string apiKey)
	{
		string text = null;
		return _plugin.Call<string>("getSenderId", new object[1] { apiKey });
	}

	public static void setSecureHttpConnectionEnabled(bool isEnabled)
	{
		_plugin.Call("setSecureHttpConnectionEnabled", isEnabled);
	}

	public static bool isSecureHttpConnectionEnabled()
	{
		bool flag = false;
		return _plugin.Call<bool>("isSecureHttpConnectionEnabled", new object[0]);
	}

	public static void startHeartbeatTimer()
	{
		_plugin.Call("startHeartbeatTimer");
	}

	public static void stopHeartbeatTimer()
	{
		_plugin.Call("stopHeartbeatTimer");
	}

	public static string dynamicValueForKey(string key, string backup)
	{
		return null;
	}
}
