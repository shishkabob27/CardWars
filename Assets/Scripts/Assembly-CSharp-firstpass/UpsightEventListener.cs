using System.Collections.Generic;
using UnityEngine;

public class UpsightEventListener : MonoBehaviour
{
	private void OnEnable()
	{
		UpsightManager.openRequestSucceededEvent += openRequestSucceededEvent;
		UpsightManager.openRequestFailedEvent += openRequestFailedEvent;
		UpsightManager.contentWillDisplayEvent += contentWillDisplayEvent;
		UpsightManager.contentDidDisplayEvent += contentDidDisplayEvent;
		UpsightManager.contentRequestLoadedEvent += contentRequestLoadedEvent;
		UpsightManager.contentRequestFailedEvent += contentRequestFailedEvent;
		UpsightManager.contentPreloadSucceededEvent += contentPreloadSucceededEvent;
		UpsightManager.contentPreloadFailedEvent += contentPreloadFailedEvent;
		UpsightManager.badgeCountRequestSucceededEvent += badgeCountRequestSucceededEvent;
		UpsightManager.badgeCountRequestFailedEvent += badgeCountRequestFailedEvent;
		UpsightManager.trackInAppPurchaseSucceededEvent += trackInAppPurchaseSucceededEvent;
		UpsightManager.trackInAppPurchaseFailedEvent += trackInAppPurchaseFailedEvent;
		UpsightManager.reportCustomEventSucceededEvent += reportCustomEventSucceededEvent;
		UpsightManager.reportCustomEventFailedEvent += reportCustomEventFailedEvent;
		UpsightManager.contentDismissedEvent += contentDismissedEvent;
		UpsightManager.makePurchaseEvent += makePurchaseEvent;
		UpsightManager.dataOptInEvent += dataOptInEvent;
		UpsightManager.unlockedRewardEvent += unlockedRewardEvent;
		UpsightManager.pushNotificationWithContentReceivedEvent += pushNotificationWithContentReceivedEvent;
		UpsightManager.pushNotificationWithUrlReceivedEvent += pushNotificationWithUrlReceivedEvent;
	}

	private void OnDisable()
	{
		UpsightManager.openRequestSucceededEvent -= openRequestSucceededEvent;
		UpsightManager.openRequestFailedEvent -= openRequestFailedEvent;
		UpsightManager.contentWillDisplayEvent -= contentWillDisplayEvent;
		UpsightManager.contentDidDisplayEvent -= contentDidDisplayEvent;
		UpsightManager.contentRequestLoadedEvent -= contentRequestLoadedEvent;
		UpsightManager.contentRequestFailedEvent -= contentRequestFailedEvent;
		UpsightManager.contentPreloadSucceededEvent -= contentPreloadSucceededEvent;
		UpsightManager.contentPreloadFailedEvent -= contentPreloadFailedEvent;
		UpsightManager.badgeCountRequestSucceededEvent -= badgeCountRequestSucceededEvent;
		UpsightManager.badgeCountRequestFailedEvent -= badgeCountRequestFailedEvent;
		UpsightManager.trackInAppPurchaseSucceededEvent -= trackInAppPurchaseSucceededEvent;
		UpsightManager.trackInAppPurchaseFailedEvent -= trackInAppPurchaseFailedEvent;
		UpsightManager.reportCustomEventSucceededEvent -= reportCustomEventSucceededEvent;
		UpsightManager.reportCustomEventFailedEvent -= reportCustomEventFailedEvent;
		UpsightManager.contentDismissedEvent -= contentDismissedEvent;
		UpsightManager.makePurchaseEvent -= makePurchaseEvent;
		UpsightManager.dataOptInEvent -= dataOptInEvent;
		UpsightManager.unlockedRewardEvent -= unlockedRewardEvent;
		UpsightManager.pushNotificationWithContentReceivedEvent -= pushNotificationWithContentReceivedEvent;
		UpsightManager.pushNotificationWithUrlReceivedEvent -= pushNotificationWithUrlReceivedEvent;
	}

	private void openRequestSucceededEvent(Dictionary<string, object> dict)
	{
	}

	private void openRequestFailedEvent(string error)
	{
	}

	private void contentWillDisplayEvent(string placementID)
	{
	}

	private void contentDidDisplayEvent(string placementID)
	{
	}

	private void contentRequestLoadedEvent(string placement)
	{
	}

	private void contentRequestFailedEvent(string placement, string error)
	{
	}

	private void contentPreloadSucceededEvent(string placement)
	{
	}

	private void contentPreloadFailedEvent(string placement, string error)
	{
	}

	private void badgeCountRequestSucceededEvent(int badgeCount)
	{
	}

	private void badgeCountRequestFailedEvent(string error)
	{
	}

	private void trackInAppPurchaseSucceededEvent()
	{
	}

	private void trackInAppPurchaseFailedEvent(string error)
	{
	}

	private void reportCustomEventSucceededEvent()
	{
	}

	private void reportCustomEventFailedEvent(string error)
	{
	}

	private void contentDismissedEvent(string placement, string dismissType)
	{
	}

	private void makePurchaseEvent(UpsightPurchase purchase)
	{
	}

	private void dataOptInEvent(Dictionary<string, object> dict)
	{
	}

	private void unlockedRewardEvent(UpsightReward reward)
	{
	}

	private void pushNotificationWithContentReceivedEvent(string messageID, string contentUnitID, string campaignID)
	{
	}

	private void pushNotificationWithUrlReceivedEvent(string url)
	{
	}
}
