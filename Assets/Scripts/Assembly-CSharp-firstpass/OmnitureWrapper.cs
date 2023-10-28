using System.Collections.Generic;
using UnityEngine;

public static class OmnitureWrapper
{
	private static AndroidJavaClass _jc;

	private static void ADBMobile()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_jc = new AndroidJavaClass("com.hibernum.OmnitureWrapper");
		}
	}

	public static void TrackCustomEvent(string eventName, Dictionary<string, string> keyValueHash)
	{
		string text = keyValueHash.toJson();
		if (text != null && Application.platform == RuntimePlatform.Android)
		{
			_jc.CallStatic("TrackCustomEvent", eventName, text);
		}
	}
}
