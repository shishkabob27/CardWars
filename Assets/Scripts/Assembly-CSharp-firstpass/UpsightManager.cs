using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MiniJSON;
using UnityEngine;

public class UpsightManager : MonoBehaviour
{
	[method: MethodImpl(32)]
	public static event Action<Dictionary<string, object>> openRequestSucceededEvent;

	[method: MethodImpl(32)]
	public static event Action<string> openRequestFailedEvent;

	[method: MethodImpl(32)]
	public static event Action<string> contentWillDisplayEvent;

	[method: MethodImpl(32)]
	public static event Action<string> contentDidDisplayEvent;

	[method: MethodImpl(32)]
	public static event Action<string> contentRequestLoadedEvent;

	[method: MethodImpl(32)]
	public static event Action<string, string> contentRequestFailedEvent;

	[method: MethodImpl(32)]
	public static event Action<string> contentPreloadSucceededEvent;

	[method: MethodImpl(32)]
	public static event Action<string, string> contentPreloadFailedEvent;

	[method: MethodImpl(32)]
	public static event Action<int> badgeCountRequestSucceededEvent;

	[method: MethodImpl(32)]
	public static event Action<string> badgeCountRequestFailedEvent;

	[method: MethodImpl(32)]
	public static event Action trackInAppPurchaseSucceededEvent;

	[method: MethodImpl(32)]
	public static event Action<string> trackInAppPurchaseFailedEvent;

	[method: MethodImpl(32)]
	public static event Action reportCustomEventSucceededEvent;

	[method: MethodImpl(32)]
	public static event Action<string> reportCustomEventFailedEvent;

	[method: MethodImpl(32)]
	public static event Action<string, string> contentDismissedEvent;

	[method: MethodImpl(32)]
	public static event Action<UpsightPurchase> makePurchaseEvent;

	[method: MethodImpl(32)]
	public static event Action<Dictionary<string, object>> dataOptInEvent;

	[method: MethodImpl(32)]
	public static event Action<UpsightReward> unlockedRewardEvent;

	[method: MethodImpl(32)]
	public static event Action<string, string, string> pushNotificationWithContentReceivedEvent;

	[method: MethodImpl(32)]
	public static event Action<string> pushNotificationWithUrlReceivedEvent;

	static UpsightManager()
	{
		try
		{
			GameObject gameObject = new GameObject("UpsightManager");
			gameObject.AddComponent<UpsightManager>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
		catch (UnityException)
		{
		}
	}

	public static void noop()
	{
	}

	private void openRequestSucceeded(string json)
	{
		if (UpsightManager.openRequestSucceededEvent != null)
		{
			UpsightManager.openRequestSucceededEvent(Json.Deserialize(json) as Dictionary<string, object>);
		}
	}

	private void openRequestFailed(string error)
	{
		if (UpsightManager.openRequestFailedEvent != null)
		{
			UpsightManager.openRequestFailedEvent(error);
		}
	}

	private void contentWillDisplay(string placementID)
	{
		if (UpsightManager.contentWillDisplayEvent != null)
		{
			UpsightManager.contentWillDisplayEvent(placementID);
		}
	}

	private void contentDidDisplay(string placementID)
	{
		if (UpsightManager.contentDidDisplayEvent != null)
		{
			UpsightManager.contentDidDisplayEvent(placementID);
		}
	}

	private void contentRequestLoaded(string placementID)
	{
		if (UpsightManager.contentRequestLoadedEvent != null)
		{
			UpsightManager.contentRequestLoadedEvent(placementID);
		}
	}

	private void contentRequestFailed(string json)
	{
		if (UpsightManager.contentRequestFailedEvent != null)
		{
			Dictionary<string, object> dictionary = Json.Deserialize(json) as Dictionary<string, object>;
			if (dictionary != null && dictionary.ContainsKey("error") && dictionary.ContainsKey("placement"))
			{
				UpsightManager.contentRequestFailedEvent(dictionary["placement"].ToString(), dictionary["error"].ToString());
			}
		}
	}

	private void contentPreloadSucceeded(string placementID)
	{
		if (UpsightManager.contentPreloadSucceededEvent != null)
		{
			UpsightManager.contentPreloadSucceededEvent(placementID);
		}
	}

	private void contentPreloadFailed(string json)
	{
		if (UpsightManager.contentPreloadFailedEvent != null)
		{
			Dictionary<string, object> dictionary = Json.Deserialize(json) as Dictionary<string, object>;
			if (dictionary != null && dictionary.ContainsKey("error") && dictionary.ContainsKey("placement"))
			{
				UpsightManager.contentPreloadFailedEvent(dictionary["placement"].ToString(), dictionary["error"].ToString());
			}
		}
	}

	private void metadataRequestSucceeded(string json)
	{
		if (UpsightManager.badgeCountRequestSucceededEvent != null)
		{
			Dictionary<string, object> dictionary = Json.Deserialize(json) as Dictionary<string, object>;
			if (dictionary != null && dictionary.ContainsKey("notification"))
			{
				Dictionary<string, object> dictionary2 = dictionary["notification"] as Dictionary<string, object>;
				if (dictionary2.ContainsKey("type") && dictionary2.ContainsKey("value"))
				{
					UpsightManager.badgeCountRequestSucceededEvent(int.Parse(dictionary2["value"].ToString()));
					return;
				}
			}
		}
		UpsightManager.badgeCountRequestFailedEvent("No badge count could be found for the placement");
	}

	private void metadataRequestFailed(string error)
	{
		if (UpsightManager.badgeCountRequestFailedEvent != null)
		{
			UpsightManager.badgeCountRequestFailedEvent(error);
		}
	}

	private void trackInAppPurchaseSucceeded(string empty)
	{
		if (UpsightManager.trackInAppPurchaseSucceededEvent != null)
		{
			UpsightManager.trackInAppPurchaseSucceededEvent();
		}
	}

	private void trackInAppPurchaseFailed(string error)
	{
		if (UpsightManager.trackInAppPurchaseFailedEvent != null)
		{
			UpsightManager.trackInAppPurchaseFailedEvent(error);
		}
	}

	private void reportCustomEventSucceeded(string empty)
	{
		if (UpsightManager.reportCustomEventSucceededEvent != null)
		{
			UpsightManager.reportCustomEventSucceededEvent();
		}
	}

	private void reportCustomEventFailed(string error)
	{
		if (UpsightManager.reportCustomEventFailedEvent != null)
		{
			UpsightManager.reportCustomEventFailedEvent(error);
		}
	}

	private void contentDismissed(string json)
	{
		if (UpsightManager.contentDismissedEvent != null)
		{
			Dictionary<string, object> dictionary = Json.Deserialize(json) as Dictionary<string, object>;
			if (dictionary != null && dictionary.ContainsKey("dismissType") && dictionary.ContainsKey("placement"))
			{
				UpsightManager.contentDismissedEvent(dictionary["placement"].ToString(), dictionary["dismissType"].ToString());
			}
		}
	}

	private void makePurchase(string json)
	{
		if (UpsightManager.makePurchaseEvent != null)
		{
			UpsightManager.makePurchaseEvent(UpsightPurchase.purchaseFromJson(json));
		}
	}

	private void dataOptIn(string json)
	{
		if (UpsightManager.dataOptInEvent != null)
		{
			UpsightManager.dataOptInEvent(Json.Deserialize(json) as Dictionary<string, object>);
		}
	}

	private void unlockedReward(string json)
	{
		if (UpsightManager.unlockedRewardEvent != null)
		{
			UpsightManager.unlockedRewardEvent(UpsightReward.rewardFromJson(json));
		}
	}

	private void pushNotificationWithContentReceived(string json)
	{
		if (UpsightManager.pushNotificationWithContentReceivedEvent == null)
		{
			return;
		}
		Dictionary<string, object> dictionary = Json.Deserialize(json) as Dictionary<string, object>;
		if (dictionary != null && dictionary.ContainsKey("messageID") && dictionary.ContainsKey("contentUnitID"))
		{
			string arg = string.Empty;
			if (dictionary.ContainsKey("campaignID") && dictionary["campaignID"] != null)
			{
				arg = dictionary["campaignID"].ToString();
			}
			UpsightManager.pushNotificationWithContentReceivedEvent(dictionary["messageID"].ToString(), dictionary["contentUnitID"].ToString(), arg);
		}
	}

	private void pushNotificationWithUrlReceived(string url)
	{
		if (UpsightManager.pushNotificationWithUrlReceivedEvent != null)
		{
			UpsightManager.pushNotificationWithUrlReceivedEvent(url);
		}
	}
}
