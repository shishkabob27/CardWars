using System;
using Multiplayer;
using UnityEngine;

public class NotificationCheckScript : MonoBehaviour
{
	private const float UPDATE_INTERVAL = 600f;

	private const string CLEAR_NOTIFICATIONS_RANK = "\t";

	private static Color tickerTitleColor = Color.yellow;

	private static Color tickerBodyColor = Color.white;

	private static Color tournamentTickerColor = Color.white;

	public UITweener partyNotification;

	public GameObject showNotificationsObj;

	public GameObject hideNotificationsObj;

	public UILabel partyTitle;

	public UILabel partyDescription;

	public UILabel expiration;

	public DateTime? end;

	private NotificationTicker ticker;

	private NotificationUI notificationUI;

	private bool tickerShown;

	private bool notificationShown;

	private RecentNotification recentNotification;

	private string newRank;

	private string prevRank;

	private float lastUpdateTime;

	private bool clearNotifications = true;

	private TournamentData tournamentData;

	private PartyInfo partyInfo;

	private float lastTickerUpdateTime;

	private void Awake()
	{
		if (base.transform.parent != null)
		{
			ticker = SLOTGame.GetComponentInChildren<NotificationTicker>(base.transform.parent.gameObject, true);
			notificationUI = SLOTGame.GetComponentInChildren<NotificationUI>(base.transform.parent.gameObject, true);
		}
	}

	public void SetupNotificationUI(PartyInfo info, bool clearPreviousNotifications)
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (instance.IsReady() && instance.NotificationEnabled)
		{
			Session theSession = SessionManager.GetInstance().theSession;
			partyInfo = info;
			string text = null;
			string text2 = null;
			if (info != null)
			{
				text = KFFLocalization.Get(info.title);
				text2 = KFFLocalization.Get(info.description);
			}
			if (ticker != null && clearPreviousNotifications)
			{
				ticker.SetText(string.Empty, true);
			}
			tournamentData = null;
			global::Multiplayer.Multiplayer.GetTournamentEndDate(theSession, instance.Cheater, GetTournamentEndDateCallback);
			bool flag = !clearPreviousNotifications && tickerShown;
			bool flag2 = !clearPreviousNotifications && notificationShown;
			if (flag || flag2)
			{
				ShowTicker(flag);
				ShowNotificationUI(flag2);
			}
			clearNotifications = clearPreviousNotifications;
			global::Multiplayer.Multiplayer.RecentBattles(theSession, RecentBattlesCallback);
		}
	}

	private void GetTournamentEndDateCallback(TournamentData data, ResponseFlag flag)
	{
		if (flag == ResponseFlag.Success)
		{
			tournamentData = data;
		}
	}

	private void RecentBattlesCallback(RecentNotification recent)
	{
		if (recent != null)
		{
			recentNotification = recent;
			return;
		}
		Session theSession = SessionManager.GetInstance().theSession;
		global::Multiplayer.Multiplayer.GetRank(theSession, false, GetRankCallback);
	}

	private void GetRankCallback(string data, ResponseFlag flag)
	{
		bool flag2 = false;
		if (flag == ResponseFlag.Success)
		{
			newRank = data;
			prevRank = PlayerInfoScript.GetInstance().MultiplayerRank;
			PlayerInfoScript.GetInstance().MultiplayerRank = data;
			flag2 = true;
		}
		if (!flag2 && clearNotifications)
		{
			newRank = "\t";
		}
	}

	private void ShowPlayerAttackedNotification(RecentNotification recent)
	{
		ShowNotificationUI(true);
		if (notificationUI != null)
		{
			if (clearNotifications)
			{
				notificationUI.ClearNotifications();
			}
			bool flag = notificationUI.IsEmpty();
			if (clearNotifications || flag)
			{
				notificationUI.ResetPosition();
			}
			notificationUI.AddPlayerAttackedNotification(recent);
			if (clearNotifications || flag)
			{
				notificationUI.ResetPosition();
			}
			else
			{
				notificationUI.ScrollToBottom();
			}
			PlayerInfoScript.GetInstance().MultiplayerRank = string.Empty + recent.rank;
		}
	}

	private void ShowRankChangeNotification(string newrank, string prevrank)
	{
		ShowNotificationUI(true);
		if (notificationUI != null)
		{
			if (clearNotifications)
			{
				notificationUI.ClearNotifications();
			}
			bool flag = notificationUI.IsEmpty();
			if (clearNotifications || flag)
			{
				notificationUI.ResetPosition();
			}
			notificationUI.AddRankChangedNotification(newrank, prevrank);
			if (clearNotifications || flag)
			{
				notificationUI.ResetPosition();
			}
			else
			{
				notificationUI.ScrollToBottom();
			}
		}
	}

	public void Update()
	{
		float num = 0.9f;
		if (Time.realtimeSinceStartup - lastTickerUpdateTime > num)
		{
			lastTickerUpdateTime = Time.realtimeSinceStartup;
			string text = string.Empty;
			string[] @params = null;
			if (partyInfo != null)
			{
				DateTime? dateTime = end;
				if (!dateTime.HasValue || (end.HasValue && DateTime.UtcNow.CompareTo(end.Value) <= 0))
				{
					string text2 = KFFLocalization.Get(partyInfo.title);
					string text3 = KFFLocalization.Get(partyInfo.description);
					text += ((!string.IsNullOrEmpty(text2)) ? (GetColorString(tickerTitleColor) + text2 + "     " + ((!string.IsNullOrEmpty(text3)) ? (GetColorString(tickerBodyColor) + text3) : string.Empty) + GetColorString(tournamentTickerColor) + "                                        ") : string.Empty);
				}
			}
			if (tournamentData != null)
			{
				TournamentManager.Tournament currentTournament = Singleton<TournamentManager>.Instance.GetCurrentTournament(tournamentData.tournamentId);
				if (currentTournament != null)
				{
					string text4 = currentTournament.Name;
					DateTime dateTime2 = Convert.ToDateTime(tournamentData.endDate);
					DateTime serverTime = TFUtils.ServerTime;
					if (serverTime < dateTime2)
					{
						TimeSpan timeSpan = dateTime2 - serverTime;
						text += KFFLocalization.Get("!!FORMAT_TICKER_TOURNAMENT");
						@params = new string[2]
						{
							text4,
							timeSpan.Days + "d " + timeSpan.Hours + "h " + timeSpan.Minutes + "m"
						};
					}
				}
			}
			SetTickerMessage(text, @params);
		}
		if (recentNotification != null)
		{
			ShowPlayerAttackedNotification(recentNotification);
			recentNotification = null;
		}
		else if (newRank != null)
		{
			if (newRank == "\t")
			{
				if (notificationUI != null)
				{
					notificationUI.ClearNotifications();
					notificationUI.ResetPosition();
				}
			}
			else
			{
				ShowRankChangeNotification(newRank, prevRank);
			}
			newRank = null;
		}
		if (Time.time - lastUpdateTime >= 600f)
		{
			CheckForNotifications();
		}
	}

	public void OnEnable()
	{
		GachaManager instance = GachaManager.Instance;
		PartyInfo currentPartyInfo = instance.GetCurrentPartyInfo();
		SetupNotificationUI(currentPartyInfo, true);
		lastUpdateTime = Time.time;
	}

	public void OnDisable()
	{
		end = null;
		hideNotificationsObj.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		tickerShown = false;
		notificationShown = false;
	}

	private string GetColorString(Color c)
	{
		return "[" + NGUITools.EncodeColor(c) + "]";
	}

	private void ShowTicker(bool show)
	{
		if (!(ticker != null))
		{
			return;
		}
		if (show)
		{
			if (!tickerShown && !notificationShown)
			{
				showNotificationsObj.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
				if (notificationUI != null)
				{
					notificationUI.gameObject.SetActive(false);
				}
			}
		}
		else if (tickerShown && !notificationShown)
		{
			hideNotificationsObj.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
		ticker.gameObject.SetActive(show);
		tickerShown = show;
	}

	private void ShowNotificationUI(bool show)
	{
		if (!base.gameObject.activeInHierarchy || !base.enabled || !(notificationUI != null))
		{
			return;
		}
		if (show)
		{
			if (!tickerShown && !notificationShown)
			{
				showNotificationsObj.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
				if (ticker != null)
				{
					ticker.gameObject.SetActive(false);
				}
			}
		}
		else if (!tickerShown && notificationShown)
		{
			hideNotificationsObj.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
		notificationUI.gameObject.SetActive(show);
		notificationShown = show;
	}

	private void CheckForNotifications()
	{
		GachaManager instance = GachaManager.Instance;
		PartyInfo currentPartyInfo = instance.GetCurrentPartyInfo();
		SetupNotificationUI(currentPartyInfo, false);
		lastUpdateTime = Time.time;
	}

	private void AddTickerMessage(string msg)
	{
		ShowTicker(true);
		if (ticker != null)
		{
			string text = ticker.GetText();
			if (!string.IsNullOrEmpty(text))
			{
				msg = text + "          " + msg;
			}
			ticker.SetText(msg, false);
		}
	}

	private void SetTickerMessage(string msg, params string[] Params)
	{
		ShowTicker(!string.IsNullOrEmpty(msg));
		if (ticker != null)
		{
			ticker.SetText(msg, false, Params);
		}
	}
}
