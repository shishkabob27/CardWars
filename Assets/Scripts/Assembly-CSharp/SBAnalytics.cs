using System.Collections.Generic;
using UnityEngine;

public class SBAnalytics
{
	private const string PLAYER_ID = "PlayerId";

	private const string DEVICE_ID = "DeviceId";

	private const string DEVICE_INFO = "DeviceInfo";

	private const string OFFLINE = "Offline";

	private const string SUBTYPE_1 = "subtype1";

	private const string SUBTYPE_2 = "subtype2";

	private const string SUBTYPE_3 = "subtype3";

	private const string LEVEL = "level";

	private const string VALUE = "value";

	private const string MTU_TYPE = "mtutype";

	private const string CATEGORY_MONETIZATION = "MonetizationByLevel";

	private const string CATEGORY_ACQUISITION = "Acquisition";

	private const string CATEGORY_PROGRESSION = "Progression";

	private const string CATEGORY_RETENTION = "Retention";

	private const string CATEGORY_PLAYER = "PlayerInfo";

	private const string CATEGORY_GAMEPLAY = "GamePlay";

	private const string CATEGORY_UPGRADE = "Upgrade";

	private const string CATEGORY_IAP = "IAP";

	private const string CATEGORY_ECONOMY = "Economy";

	private const string EVENT_IN_APP_PURCHASE = "IAP";

	private const string EVENT_TUTORIAL = "Tutorial";

	private const string EVENT_FIRSTTIME = "FirstTime";

	private const string EVENT_USER_ENGAGEMENT = "Engagement";

	private const string EVENT_DEVICE_TYPE = "iOSDeviceType";

	private const string EVENT_INTERACTION = "Interaction";

	private const string EVENT_CHARACTERS = "Characters";

	private const string EVENT_RUN_METRICS = "RunMetrics";

	private const string EVENT_TOTAL = "Totals";

	private const string EVENT_TURTLE_TIME = "TurtleTime";

	private const string EVENT_RUN_COUNT = "RunCount";

	private const string EVENT_TOTAL_DISTANCE = "TotalDistance";

	private const string EVENT_TOTAL_COINS = "TotalCoins";

	private const string EVENT_SOURCE = "Source";

	private const string EVENT_SINK = "Sink";

	private const string EVENT_STATUS = "Status";

	private const string EVENT_PIZZAWHEEL = "PizzaWheel";

	private const string EVENT_QUEST_COMPLETE_KEYS = "QuestCompleteKeys";

	private const string EVENT_QUEST_COMPLETE_RANK = "QuestCompleteRank";

	private const string EVENT_RANKUP = "RankUp";

	private const string SUBEVENT_ITEM_GADGET = "Gadget";

	private string deviceId;

	private string deviceInfo;

	private string playerId;

	private bool isOffline;

	private int turtleID;

	public string PlayerId
	{
		get
		{
			return playerId;
		}
		set
		{
			playerId = value;
		}
	}

	public bool IsOffline
	{
		get
		{
			return isOffline;
		}
		set
		{
			isOffline = value;
		}
	}

	public string iOSDeviceType
	{
		get
		{
			return TFUtils.GetiOSDeviceTypeString();
		}
	}

	public SBAnalytics()
	{
		deviceId = SystemInfo.deviceUniqueIdentifier;
		deviceInfo = string.Format("{0} {1} {2}", SystemInfo.deviceModel, SystemInfo.processorType, SystemInfo.operatingSystem);
	}

	public void AddCommon(Dictionary<string, object> eventData)
	{
		eventData["DeviceId"] = deviceId;
		eventData["DeviceInfo"] = deviceInfo;
	}

	public void AddSubtypes(Dictionary<string, object> eventData, string subtype1, string subtype2 = null, string subtype3 = null)
	{
		if (subtype1 != null)
		{
			eventData["subtype1"] = subtype1;
		}
		if (subtype2 != null)
		{
			eventData["subtype2"] = subtype2;
		}
		if (subtype3 != null)
		{
			eventData["subtype3"] = subtype3;
		}
	}

	public void LogRunDistance(int distance, int runcount, string characterID)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "GamePlay", "Characters", characterID);
		dictionary["value"] = distance;
		dictionary["level"] = runcount;
		TFAnalytics.LogEvent("character_run", dictionary);
	}

	public void LogCharacterRuns(int runcount, int binRun, string characterID)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "GamePlay", "Characters", characterID);
		dictionary["value"] = runcount;
		dictionary["level"] = binRun;
		TFAnalytics.LogEvent("run_number_character", dictionary);
	}

	public void LogTurtleTime(int performance, string characterID)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "GamePlay", "Characters", characterID);
		dictionary["value"] = performance;
		dictionary["level"] = 0;
		TFAnalytics.LogEvent("turtle_time_achieved", dictionary);
	}

	public void LogTurtleTimeByRun(int coin, int runcount, string characterID)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "GamePlay", "TurtleTime", characterID);
		dictionary["value"] = runcount;
		dictionary["level"] = coin;
		TFAnalytics.LogEvent("turtle_time_by_run_number", dictionary);
	}

	public void LogTurtleTimeByDistance(int coin, int distance, string characterID)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "GamePlay", "TurtleTime", characterID);
		dictionary["value"] = distance;
		dictionary["level"] = coin;
		TFAnalytics.LogEvent("turtle_time_by_distance", dictionary);
	}

	public void LogRunDistanceByRunNum(int distance, int runcount, string characterID)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "GamePlay", "RunMetrics", characterID);
		dictionary["value"] = distance;
		dictionary["level"] = runcount;
		TFAnalytics.LogEvent("run_distance_by_run_number", dictionary);
	}

	public void LogCoinsEarnedByDist(int coin, int distance, string characterID)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "GamePlay", "RunMetrics", characterID);
		dictionary["value"] = coin;
		dictionary["level"] = distance;
		TFAnalytics.LogEvent("coins_per_meter", dictionary);
	}

	public void LogPizzaEarnedByDist(int pizza, int distance, string characterID)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "GamePlay", "RunMetrics", characterID);
		dictionary["value"] = pizza;
		dictionary["level"] = distance;
		TFAnalytics.LogEvent("pizza_per_meter", dictionary);
	}

	public void LogCoinsByRuns(int coin, int runcount, string characterID)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "GamePlay", "RunMetrics", characterID);
		dictionary["value"] = coin;
		dictionary["level"] = runcount;
		TFAnalytics.LogEvent("coins_by_run_number", dictionary);
	}

	public void LogCoinsByCharacter(int coin, int runcount, string characterID)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "GamePlay", "Characters", characterID);
		dictionary["value"] = coin;
		dictionary["level"] = runcount;
		TFAnalytics.LogEvent("coins_by_character", dictionary);
	}

	public void LogPowerupsByRun(string category, string pupName, int buyPrice, int runcount)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "Upgrade", "RunCount", category);
		dictionary["value"] = buyPrice;
		dictionary["level"] = runcount;
		TFAnalytics.LogEvent(pupName, dictionary);
	}

	public void LogPowerupsByDistance(string category, string pupName, int buyPrice, int distance)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "Upgrade", "TotalDistance", category);
		dictionary["value"] = buyPrice;
		dictionary["level"] = distance;
		TFAnalytics.LogEvent(pupName, dictionary);
	}

	public void LogPowerupsByTotalCoinsEarned(string category, string pupName, int buyPrice, int coin)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "Upgrade", "TotalCoins", category);
		dictionary["value"] = buyPrice;
		dictionary["level"] = coin;
		TFAnalytics.LogEvent(pupName, dictionary);
	}

	public void LogIAPByRun(string iapName, int buyPrice, int runcount)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "IAP", "RunCount");
		dictionary["level"] = runcount;
		dictionary["value"] = buyPrice;
		TFAnalytics.LogEvent(iapName, dictionary);
		TFAnalytics.LogIAP(iapName, dictionary);
	}

	public void LogIAPByDistance(string iapName, int buyPrice, int distance)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "IAP", "TotalDistance");
		dictionary["level"] = distance;
		dictionary["value"] = buyPrice;
		TFAnalytics.LogEvent(iapName, dictionary);
		TFAnalytics.LogIAP(iapName, dictionary);
	}

	public void LogIAPByCoin(string iapName, int buyPrice, int coin)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "IAP", "TotalCoins");
		dictionary["level"] = coin;
		dictionary["value"] = buyPrice;
		TFAnalytics.LogEvent(iapName, dictionary);
		TFAnalytics.LogIAP(iapName, dictionary);
	}

	public void LogIAPFirst(string iapName, int buyPrice, int coin)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "IAP", "TotalCoins");
		dictionary["level"] = coin;
		dictionary["value"] = buyPrice;
		TFAnalytics.LogEvent(iapName, dictionary);
		TFAnalytics.LogIAP(iapName, dictionary);
	}

	public void LogEconomyEarnsCoins(int coin, int runcount)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "Economy", "Source");
		dictionary["level"] = runcount;
		dictionary["value"] = coin;
		TFAnalytics.LogEvent("coins_earned", dictionary);
	}

	public void LogEconomyBuysCoins(int coin, int runcount)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "Economy", "Source");
		dictionary["level"] = runcount;
		dictionary["value"] = coin;
		TFAnalytics.LogEvent("coins_bought", dictionary);
	}

	public void LogEconomySpendsCoinsUpgrade(int coin, int runcount)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "Economy", "Sink");
		dictionary["level"] = runcount;
		dictionary["value"] = coin;
		TFAnalytics.LogEvent("coins_spent", dictionary);
	}

	public void LogDeathCause(string name, int dist, int runcount, string characterID)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "GamePlay", "RunMetrics", characterID);
		dictionary["level"] = runcount;
		dictionary["value"] = dist;
		TFAnalytics.LogEvent(name, dictionary);
	}

	public void LogEnemiesKilled(string name, int count, int runcount, string characterID)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "GamePlay", "RunMetrics", characterID);
		dictionary["level"] = runcount;
		dictionary["value"] = count;
		TFAnalytics.LogEvent(name, dictionary);
	}

	public void LogEnemiesMissed(string name, int count, int runcount, string characterID)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "GamePlay", "RunMetrics", characterID);
		dictionary["level"] = runcount;
		dictionary["value"] = count;
		TFAnalytics.LogEvent(name, dictionary);
	}

	public void LogBossFightsWon(int count, int dist, int runcount, string characterID)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "GamePlay", "RunMetrics", characterID);
		dictionary["level"] = runcount;
		dictionary["value"] = count;
		TFAnalytics.LogEvent("boss_win", dictionary);
	}

	public void LogBossFightsLost(int count, int dist, int runcount, string characterID)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "GamePlay", "RunMetrics", characterID);
		dictionary["level"] = runcount;
		dictionary["value"] = count;
		TFAnalytics.LogEvent("boss_lose", dictionary);
	}

	public void LogCoinsAtStart(int count, int runcount, string characterID)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "Economy", "Status", characterID);
		dictionary["level"] = runcount;
		dictionary["value"] = count;
		TFAnalytics.LogEvent("wallet_contents_at_run", dictionary);
	}

	public void LogCoinsEearnedBoss(int count, int runcount, string characterID)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "Economy", "Source", characterID);
		dictionary["level"] = runcount;
		dictionary["value"] = count;
		TFAnalytics.LogEvent("coins_earned_boss", dictionary);
	}

	public void LogCoinsEarnedEnemies(int count, int runcount, string characterID)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "Economy", "Source", characterID);
		dictionary["level"] = runcount;
		dictionary["value"] = count;
		TFAnalytics.LogEvent("coins_earned_enemies", dictionary);
	}

	public void LogCoinsEarnedTurtleTime(int count, int runcount, string characterID)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "Economy", "Source", characterID);
		dictionary["level"] = runcount;
		dictionary["value"] = count;
		TFAnalytics.LogEvent("coins_earned_turtletime", dictionary);
	}

	public void LogCoinsEarnedPizzaWheel(int count, int runcount, string characterID)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "Economy", "Source", characterID);
		dictionary["level"] = runcount;
		dictionary["value"] = count;
		TFAnalytics.LogEvent("coins_earned_pizza", dictionary);
	}

	public void LogPizzaPrize(string itemName, int count)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "Economy", "PizzaWheel", itemName);
		dictionary["value"] = count;
		dictionary["level"] = 0;
		TFAnalytics.LogEvent(itemName, dictionary);
	}

	public void LogGadgetUsed(string itemName, int count, int runcount)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "GamePlay", "RunMetrics", itemName);
		dictionary["value"] = count;
		dictionary["level"] = runcount;
		TFAnalytics.LogEvent(itemName, dictionary);
	}

	public void LogTutorialFlow(string step, int gold, int keys)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "Progression", "Tutorial");
		dictionary["value"] = gold;
		dictionary["level"] = keys;
		TFAnalytics.LogEvent(step, dictionary);
	}

	public void LogFirstTimePlay(string eventName)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "Progression", "FirstTime");
		dictionary["value"] = 0;
		dictionary["level"] = 0;
		int @int = PlayerPrefs.GetInt("FirstTimePlay", 0);
		if (@int > 0)
		{
			TFAnalytics.LogEvent(eventName, dictionary);
		}
	}

	public void LogQuestCompletion(string questname, int gold, int keys, int rank)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "Progression", "QuestCompleteKeys");
		dictionary["value"] = gold;
		dictionary["level"] = keys;
		TFAnalytics.LogEvent(questname, dictionary);
		AddSubtypes(dictionary, "Progression", "QuestCompleteRank");
		dictionary["value"] = gold;
		dictionary["level"] = rank;
		TFAnalytics.LogEvent(questname, dictionary);
	}

	public void LogRankUp(string rank, int gold, int keys)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AddCommon(dictionary);
		AddSubtypes(dictionary, "Progression", "RankUp");
		dictionary["value"] = gold;
		dictionary["level"] = keys;
		TFAnalytics.LogEvent(rank, dictionary);
		AddSubtypes(dictionary, "Progression", "Engagement");
		if (rank == "5")
		{
			TFAnalytics.LogEvent("CasualUser", dictionary);
		}
		if (rank == "8")
		{
			TFAnalytics.LogEvent("SeriousUser", dictionary);
		}
	}
}
