using UnityEngine;

public class KFFNotificationManager : MonoBehaviour
{
	private int _DayToSec = 86400;

	private bool RanOnce;

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	private void NotifyStamina()
	{
		if ((bool)CWUpdatePlayerStats.GetInstance())
		{
			int staminaCountdown = CWUpdatePlayerStats.GetInstance().GetStaminaCountdown();
			if (staminaCountdown > 0)
			{
				scheduleLocalNotification(staminaCountdown, KFFLocalization.Get("!!NOTIFY_FULL_HEART"), string.Empty);
			}
		}
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if ((bool)instance && instance.NumMPGamesPlayed > 0)
		{
			int secondsFromNow = 86400;
			scheduleLocalNotification(secondsFromNow, KFFLocalization.Get("!!LOCAL_NOTIFICATION_BEING_ATTACKED"), string.Empty);
		}
	}

	private void NotifyDailyGift()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (!(instance == null) && instance.IsReady())
		{
			int num = DailyGiftController.TimeToNextDailyGift();
			if (num > 0)
			{
				scheduleLocalNotification(num, KFFLocalization.Get("!!LOCAL_NOTIFICATION_FREE_DAILYGIFT"), string.Empty);
			}
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			cancelAllLocalNotifications();
			NotifyStamina();
			NotifyDailyGift();
		}
	}

	public static void scheduleLocalNotification(int secondsFromNow, string text, string action)
	{
		if (PlayerInfoScript.GetInstance().NotificationEnabled)
		{
		}
	}

	private void cancelAllLocalNotifications()
	{
	}
}
