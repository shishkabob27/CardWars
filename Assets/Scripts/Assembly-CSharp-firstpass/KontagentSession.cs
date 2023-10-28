using System.Collections.Generic;
using UnityEngine;

public class KontagentSession
{
	private AndroidJavaObject jSession;

	public KontagentSession(AndroidJavaObject jSession)
	{
		this.jSession = jSession;
	}

	public bool start()
	{
		return jSession.Call<bool>("start", new object[0]);
	}

	public void stop()
	{
		jSession.Call("stop");
	}

	public void setSecureHttpConnectionEnabled(bool isEnabled)
	{
		jSession.Call("setSecureHttpConnectionEnabled", isEnabled);
	}

	public bool isSecureHttpConnectionEnabled()
	{
		return jSession.Call<bool>("isSecureHttpConnectionEnabled", new object[0]);
	}

	public void startHeartbeatTimer()
	{
		jSession.Call("startHeartbeatTimer");
	}

	public void stopHeartbeatTimer()
	{
		jSession.Call("stopHeartbeatTimer");
	}

	public void applicationAdded(Dictionary<string, string> optionalParams)
	{
		jSession.Call("applicationAdded", KontagentBinding.mapFromDictionary(optionalParams));
	}

	public void customEvent(string eventName, Dictionary<string, string> optionalParams)
	{
		jSession.Call("customEvent", eventName, KontagentBinding.mapFromDictionary(optionalParams));
	}

	public void revenueTracking(int value, Dictionary<string, string> optionalParams)
	{
		using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("java.lang.Integer", value))
		{
			jSession.Call("revenueTracking", androidJavaObject, KontagentBinding.mapFromDictionary(optionalParams));
		}
	}

	public void sendDeviceInformation(Dictionary<string, string> optionalParams)
	{
		jSession.Call("sendDeviceInformation", KontagentBinding.mapFromDictionary(optionalParams));
	}
}
