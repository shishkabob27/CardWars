using System.Collections.Generic;
using UnityEngine;

public class UpsightDemoUI : MonoBehaviour
{
	public string androidAppToken;

	public string androidAppSecret;

	public string gcmProjectNumber;

	public string iosAppToken;

	public string iosAppSecret;

	private int _moreGamesBadgeCount = -1;

	private void Start()
	{
		Upsight.init(androidAppToken, androidAppSecret, gcmProjectNumber);
		UpsightManager.badgeCountRequestSucceededEvent += delegate(int badgeCount)
		{
			_moreGamesBadgeCount = badgeCount;
		};
		Upsight.registerForPushNotifications();
		Upsight.requestAppOpen();
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (!pauseStatus)
		{
			Upsight.requestAppOpen();
		}
	}

	private void OnGUI()
	{
		beginGuiColomn();
		if (GUILayout.Button("Enable Verbose Logs (Android only)"))
		{
			Upsight.setLogLevel(UpsightLogLevel.VERBOSE);
		}
		if (GUILayout.Button("Request App Open"))
		{
			Upsight.requestAppOpen();
		}
		string text = "Send Content Request (more_games_only)";
		if (_moreGamesBadgeCount >= 0)
		{
			text += string.Format(" ({0})", _moreGamesBadgeCount);
		}
		if (GUILayout.Button(text))
		{
			Upsight.sendContentRequest("more_games_only", true);
		}
		if (GUILayout.Button("Send Content Request (interstitial)"))
		{
			Upsight.sendContentRequest("interstitial", true);
		}
		if (GUILayout.Button("Send Content Request (optin)"))
		{
			Upsight.sendContentRequest("optin", true);
		}
		if (GUILayout.Button("Send Content Request (rewarded)"))
		{
			Upsight.sendContentRequest("rewarded", false, false);
		}
		if (GUILayout.Button("Send Content Request (vg_test)"))
		{
			Upsight.sendContentRequest("vg_test", true);
		}
		if (GUILayout.Button("Send Content Request (vg_test) with dimensions"))
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("ua_source", "PlayHaven");
			dictionary.Add("gold_balance", 2170);
			dictionary.Add("registered", true);
			Upsight.sendContentRequest("vg_test", true, false, dictionary);
		}
		if (GUILayout.Button("Toggle Opt Out Status"))
		{
			Upsight.setOptOutStatus(!Upsight.getOptOutStatus());
		}
		endGuiColumn(true);
		if (GUILayout.Button("Preload Content Request (announce)"))
		{
			Upsight.preloadContentRequest("announce");
		}
		if (GUILayout.Button("Send Preloaded Content Request (announce)"))
		{
			Upsight.sendContentRequest("announce", false);
		}
		if (GUILayout.Button("Get Content Badge Number (more_games_only)"))
		{
			Upsight.getContentBadgeNumber("more_games_only");
		}
		if (GUILayout.Button("Track In App Purchase"))
		{
			Upsight.trackInAppPurchase("com.playhaven.unityexample.plasmagun", 1, UpsightAndroidPurchaseResolution.Bought, 45.55, "the-order-id", "Play");
		}
		if (GUILayout.Button("Report Custom Event"))
		{
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary2.Add("first_key", "first_value");
			dictionary2.Add("second_key", 38);
			Upsight.reportCustomEvent(dictionary2);
		}
		GUILayout.Label("Push Notifications");
		if (GUILayout.Button("Register for Push Notifications"))
		{
			Upsight.registerForPushNotifications();
		}
		if (GUILayout.Button("Deregister for Push Notifications"))
		{
			Upsight.deregisterForPushNotifications();
		}
		endGuiColumn();
	}

	private void beginGuiColomn()
	{
		int num = ((Screen.width < 960 && Screen.height < 960) ? 30 : 70);
		GUI.skin.label.fixedHeight = num;
		GUI.skin.label.margin = new RectOffset(0, 0, 10, 0);
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		GUI.skin.button.margin = new RectOffset(0, 0, 10, 0);
		GUI.skin.button.fixedHeight = num;
		GUI.skin.button.fixedWidth = Screen.width / 2 - 20;
		GUI.skin.button.wordWrap = true;
		GUILayout.BeginArea(new Rect(10f, 10f, Screen.width / 2, Screen.height));
		GUILayout.BeginVertical();
	}

	private void endGuiColumn(bool hasSecondColumn = false)
	{
		GUILayout.EndVertical();
		GUILayout.EndArea();
		if (hasSecondColumn)
		{
			GUILayout.BeginArea(new Rect(Screen.width - Screen.width / 2, 10f, Screen.width / 2, Screen.height));
			GUILayout.BeginVertical();
		}
	}
}
