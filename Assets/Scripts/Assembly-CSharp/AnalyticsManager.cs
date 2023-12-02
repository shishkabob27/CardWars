using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AnalyticsManager : Singleton<AnalyticsManager>
{
	private const string CATEGORY_PROGRESSION = "Progression";

	private const string CATEGORY_IAP = "IAP";

	private const string CATEGORY_BATTLE = "battle";

	private const string CATEGORY_LEADER = "leader";

	private const string CATEGORY_PAYWALL = "paywall";

	private const string CATEGORY_CARD = "card";

	private const string CATEGORY_META_GAME = "meta_game";

	private const string CATEGORY_PVP = "pvp";

	private const string CATEGORY_DUNGEON = "dungeon";

	private const string CATEGORY_DEBUG = "debug";

	private const string CATEGORY_FC = "fionnacake";

	private const string EVENT_FIRSTTIME = "FirstTime";

	private const string EVENT_IN_APP_PURCHASE = "IAP";

	private const string EVENT_RUN_COUNT = "RunCount";

	private const string EVENT_END_RESULT = "end_result";

	private const string EVENT_CRAFT = "craft";

	private const string EVENT_SELL = "sell";

	private const string EVENT_USED = "used";

	private const string EVENT_GEMS_PER_RANK = "GemsPerRank";

	private const string EVENT_ENTER_SHOP = "enter_shop";

	private const string EVENT_LEADER_UP = "leader_up";

	private const string EVENT_LEADER_EQUIPPED = "leader_equipped";

	private const string EVENT_STAMINA_CONSUMED = "stamina_consumed";

	private const string EVENT_PATH_UNLOCKED = "path_unlocked";

	private const string EVENT_FC_DEMO_START = "demo_start";

	private const string EVENT_FC_DEMO_COMPLETE = "demo_complete";

	private const string EVENT_FC_HEROES_AWARDED = "heroes_awarded";

	private const string EVENT_FC_SELL_OPEN = "sell_open";

	private const string EVENT_FC_SELL_CLICK = "sell_click";

	private const string EVENT_FC_DEMO_REPLAY = "demo_replay";

	private const string EXTRA_GAME_ID = "game_id";

	private const string EXTRA_NUM_ROUNDS = "num_rounds";

	private const string EXTRA_PATH_NAME = "path_name";

	private const string EXTRA_CARD_ID = "card_id";

	private const string EXTRA_SELECTED_DECK = "selected_deck";

	private const int NUM_BUCKETS = 255;

	private const string PLAYER_ID = "PlayerId";

	private const string DEVICE_ID = "DeviceId";

	private const string DEVICE_INFO = "DeviceInfo";

	private const string OFFLINE = "Offline";

	private const string SUBTYPE_1 = "subtype1";

	private const string SUBTYPE_2 = "subtype2";

	private const string SUBTYPE_3 = "subtype3";

	private const string LEVEL = "level";

	private const string VALUE = "value";

	private int BATTLE_COUNT_BUCKET_SIZE = 10;

	private int BattleCount;

	private int BR_HitCount;

	private int BR_MissCount;

	private int BR_CritCount;

	private int BR_BlockCount;

	private int BR_CounterCount;

	private string KontagentApiKey;

	private bool KontagentTestMode;

	private bool TurnOff = true;

	private string deviceId;

	private string deviceInfo;

	private string playerId;

	private bool isOffline;

	public void IncBattleCount()
	{
		BattleCount++;
		PlayerPrefs.SetInt("battle_count", BattleCount);
	}

	public void ResetBattleCount()
	{
		BattleCount = PlayerPrefs.GetInt("battle_count");
	}

	public void IncBR_HitCount()
	{
		BR_HitCount++;
	}

	public void IncBR_MissCount()
	{
		BR_MissCount++;
	}

	public void IncBR_CritCount()
	{
		BR_CritCount++;
	}

	public void IncBR_BlockCount()
	{
		BR_BlockCount++;
	}

	public void IncBR_CounterCount()
	{
		BR_CounterCount++;
	}

	public void LogDebug(string eventName, int dataValue = 0, int dataLevel = 0, Exception exception = null)
	{
		if (!TurnOff)
		{
			string[] categories = new string[3] { "debug", null, null };
			Dictionary<string, object> dictionary = null;
			if (exception != null)
			{
				string value = exception.ToString().Substring(0, 255);
				dictionary = new Dictionary<string, object>();
				dictionary.Add("info", value);
			}
			LogEvent(categories, eventName, dataValue, dataLevel, dictionary);
		}
	}

	public void LogFirstTimePlay(string eventName)
	{
		if (!TurnOff)
		{
			int @int = PlayerPrefs.GetInt("FirstTimePlay", 1);
			if (@int > 0)
			{
				string[] categories = new string[3] { "Progression", "FirstTime", null };
				LogEvent(categories, eventName, 0, 0);
				PlayerPrefs.SetInt("FirstTimePlay", 0);
			}
		}
	}

	public void LogIAPByBattle(string iapName, float buyPrice)
	{
		if (!TurnOff)
		{
			int num = (int)(buyPrice * 100f);
			string[] categories = new string[3] { "IAP", "RunCount", null };
			int dataLevel = CompressData(BattleCount, BATTLE_COUNT_BUCKET_SIZE);
			LogEvent(categories, iapName, num, dataLevel);
			LogIAP(categories, iapName, num, dataLevel);
		}
	}

	public void LogPVPGemsSpentByRank(int aGemCount, int aRank)
	{
		if (!TurnOff)
		{
			string[] categories = new string[3] { "pvp", "GemsPerRank", null };
			LogEvent(categories, "pvp_gems_spent", aGemCount, aRank);
		}
	}

	public void LogTotalXP()
	{
		if (TurnOff)
		{
			return;
		}
		int num = 0;
		foreach (LeaderItem leader in LeaderManager.Instance.leaders)
		{
			num += leader.XP;
		}
		if (num > 0)
		{
			string[] categories = new string[3] { "meta_game", null, null };
			LogEvent(categories, "Total_XP", num, 0);
		}
	}

	public void LogTotalCoins()
	{
		if (!TurnOff)
		{
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			if (!(instance == null))
			{
				string[] categories = new string[3] { "meta_game", null, null };
				LogEvent(categories, "Total_Coins", instance.CoinsAccumulated, 0);
				LogEvent(categories, "Coins", instance.Coins, 0);
			}
		}
	}

	public void LogStaminaConsumed(string consumeContext, int staminaCost, int staminaCurr, int staminaMax, int questId = 255)
	{
		if (!TurnOff)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["stamina_max"] = staminaMax;
			dictionary["stamina_cost"] = staminaCost;
			string[] categories = new string[3] { "meta_game", "stamina_consumed", null };
			LogEvent(categories, "Consume_" + consumeContext, staminaCurr, questId, dictionary);
		}
	}

	public void LogPathUnlocked(QuestData fromQuest, string pathName)
	{
		if (!TurnOff)
		{
			int dataLevel = 0;
			GameState instance = GameState.Instance;
			if (instance != null && instance.ActiveQuest != null)
			{
				dataLevel = instance.ActiveQuest.iQuestID;
			}
			string[] categories = new string[3]
			{
				"Progression",
				fromQuest.QuestType.ToString(),
				null
			};
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["path_name"] = pathName;
			LogEvent(categories, "path_unlocked", fromQuest.iQuestID, dataLevel, dictionary);
		}
	}

	public void LogQuestStart()
	{
		if (!TurnOff)
		{
			int num = 0;
			int num2 = 0;
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			if (instance != null)
			{
				num = instance.GetCurrentGameID();
				num2 = ((GlobalFlags.Instance == null || !GlobalFlags.Instance.InMPMode) ? instance.SelectedDeck : instance.SelectedMPDeck);
			}
			int num3 = 0;
			GameState instance2 = GameState.Instance;
			if (instance2 != null && instance2.ActiveQuest != null)
			{
				num3 = instance2.ActiveQuest.iQuestID;
			}
			string[] categories = new string[3] { "battle", null, null };
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["game_id"] = num;
			dictionary["selected_deck"] = num2;
			LogEvent(categories, "Quest_Start", num3, num3, dictionary);
		}
	}

	public void LogQuestWin()
	{
		if (!TurnOff)
		{
			int num = 0;
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			if (instance != null)
			{
				num = instance.GetCurrentGameID();
			}
			int dataLevel = 0;
			GameState instance2 = GameState.Instance;
			if (instance2 != null && instance2.ActiveQuest != null)
			{
				dataLevel = instance2.ActiveQuest.iQuestID;
			}
			string[] categories = new string[3] { "battle", "end_result", null };
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["game_id"] = num;
			if (GameDataScript.GetInstance() != null)
			{
				dictionary["num_rounds"] = (GameDataScript.GetInstance().Turn + 1) / 2;
			}
			LogEvent(categories, "Quest_Won", 1, dataLevel, dictionary);
		}
	}

	public void LogQuestLoss()
	{
		if (!TurnOff)
		{
			int num = 0;
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			if (instance != null)
			{
				num = instance.GetCurrentGameID();
			}
			int dataLevel = 0;
			GameState instance2 = GameState.Instance;
			if (instance2 != null && instance2.ActiveQuest != null)
			{
				dataLevel = instance2.ActiveQuest.iQuestID;
			}
			string[] categories = new string[3] { "battle", "end_result", null };
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["game_id"] = num;
			if (GameDataScript.GetInstance() != null)
			{
				dictionary["num_rounds"] = (GameDataScript.GetInstance().Turn + 1) / 2;
			}
			LogEvent(categories, "Quest_Lost", 1, dataLevel, dictionary);
		}
	}

	public void LogEndResultsEvents()
	{
		if (!TurnOff)
		{
			LogBR_Hit();
			LogBR_Miss();
			LogBR_Crit();
			LogBR_Block();
			LogBR_Counter();
			BR_HitCount = 0;
			BR_MissCount = 0;
			BR_CritCount = 0;
			BR_BlockCount = 0;
			BR_CounterCount = 0;
		}
	}

	public void LogQuestQuit()
	{
		if (!TurnOff)
		{
			int num = 0;
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			if (instance != null)
			{
				num = instance.GetCurrentGameID();
			}
			int dataLevel = 0;
			GameState instance2 = GameState.Instance;
			if (instance2 != null && instance2.ActiveQuest != null)
			{
				dataLevel = instance2.ActiveQuest.iQuestID;
			}
			string[] categories = new string[3] { "battle", "end_result", null };
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["game_id"] = num;
			if (GameDataScript.GetInstance() != null)
			{
				dictionary["num_rounds"] = (GameDataScript.GetInstance().Turn + 1) / 2;
			}
			LogEvent(categories, "Quest_Quit", 1, dataLevel, dictionary);
		}
	}

	public void LogQuestStars(int starCount)
	{
		if (!TurnOff)
		{
			int dataLevel = 0;
			GameState instance = GameState.Instance;
			if (instance != null && instance.ActiveQuest != null)
			{
				dataLevel = instance.ActiveQuest.iQuestID;
			}
			string[] categories = new string[3] { "battle", "end_result", null };
			LogEvent(categories, "Quest_Stars", starCount, dataLevel);
		}
	}

	public void LogNormalChestPurchase()
	{
		LogPurchase("PAY_Chest_Norm", "NormalChestPurchase");
	}

	public void LogPremiumChestPurchase()
	{
		LogPurchase("PAY_Chest_Premium", "PremiumChestPurchase");
	}

	public void LogReshufflePurchase()
	{
		LogPurchase("PAY_Quest_Reshuffle", "ReshufflePurchase");
	}

	public void LogResurrectPurchase(List<CardItem> earnedCards, int earnedCoins)
	{
		int num = 0;
		int num2 = 0;
		float num3 = 0f;
		if (earnedCards.Count > 0)
		{
			int num4 = 0;
			num = int.MaxValue;
			foreach (CardItem earnedCard in earnedCards)
			{
				int rarity = earnedCard.Form.Rarity;
				num = Math.Min(num, rarity);
				num2 = Math.Max(num2, rarity);
				num4 += rarity;
			}
			num3 = (float)num4 / (float)earnedCards.Count;
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["coins"] = earnedCoins;
		dictionary["cards"] = earnedCards.Count;
		dictionary["card_rarity_min"] = num;
		dictionary["card_rarity_max"] = num2;
		dictionary["card_rarity_avg"] = num3;
		dictionary["card_rarity_avg"] = num3;
		LogPurchase("PAY_Quest_Resurrect", "ResurrectPurchase", dictionary);
	}

	public void LogRechargePurchase()
	{
		LogPurchase("PAY_Recharge", "RechargePurchase");
	}

	public void LogBoxSpacePurchase()
	{
		LogPurchase("PAY_Box_Space", "BoxSpacePurchase");
	}

	public void LogDeckFindWarOpponentPurchase(int gemCost, int coinCost)
	{
		LogPurchase("PAY_DW_Find_Opponent", 1, gemCost, coinCost);
	}

	public void LogExtraDailySpinPurchase(int gemCost)
	{
		LogPurchase("PAY_Daily_Spin", 1, gemCost);
	}

	public void LogCalendarPrizePurchase(int gemCost)
	{
		LogPurchase("PAY_Calendar_Prize", 1, gemCost);
	}

	private void LogPurchase(string eventName, string prefsPurchase = null, int? gemCost = null, int? coinCost = null)
	{
		if (!TurnOff)
		{
			int num = 1;
			if (!string.IsNullOrEmpty(prefsPurchase))
			{
				num = PlayerPrefs.GetInt(prefsPurchase, 0) + 1;
				PlayerPrefs.SetInt(prefsPurchase, num);
			}
			LogPurchase(eventName, num, gemCost, coinCost);
		}
	}

	private void LogPurchase(string eventName, string prefsPurchase, Dictionary<string, object> extras)
	{
		if (!TurnOff)
		{
			int num = 1;
			if (!string.IsNullOrEmpty(prefsPurchase))
			{
				num = PlayerPrefs.GetInt(prefsPurchase, 0) + 1;
				PlayerPrefs.SetInt(prefsPurchase, num);
			}
			LogPurchase(eventName, num, extras);
		}
	}

	private void LogPurchase(string eventName, int purchaseCount, int? gemCost = null, int? coinCost = null)
	{
		if (TurnOff)
		{
			return;
		}
		Dictionary<string, object> dictionary = null;
		if (gemCost.HasValue || coinCost.HasValue)
		{
			dictionary = new Dictionary<string, object>();
			if (gemCost.HasValue)
			{
				dictionary.Add("gems", gemCost);
			}
			if (coinCost.HasValue)
			{
				dictionary.Add("coins", coinCost);
			}
		}
		LogPurchase(eventName, purchaseCount, dictionary);
	}

	private void LogPurchase(string eventName, int purchaseCount, Dictionary<string, object> extras)
	{
		if (!TurnOff)
		{
			int dataLevel = 0;
			GameState instance = GameState.Instance;
			if (instance != null && instance.ActiveQuest != null)
			{
				dataLevel = instance.ActiveQuest.iQuestID;
			}
			string[] categories = new string[3] { "paywall", null, null };
			LogEvent(categories, eventName, purchaseCount, dataLevel, extras);
		}
	}

	public void LogEnterShop()
	{
		string text = AnalyticsShopContext.ContextLast;
		if (string.IsNullOrEmpty(text))
		{
			text = "unknown";
		}
		string[] categories = new string[3] { "IAP", "enter_shop", null };
		LogEvent(categories, "ENTER_" + text, 1, 0);
	}

	public void LogLevelUpLeader(string leaderId, int rank)
	{
		if (!TurnOff)
		{
			string[] categories = new string[3] { "leader", "leader_up", null };
			LogEvent(categories, leaderId, rank, 0);
		}
	}

	public void LogLeaderEquipped(string leaderId, int rank)
	{
		if (!TurnOff)
		{
			int dataLevel = 0;
			GameState instance = GameState.Instance;
			if (instance != null && instance.ActiveQuest != null)
			{
				dataLevel = instance.ActiveQuest.iQuestID;
			}
			string[] categories = new string[3] { "battle", "leader_equipped", null };
			LogEvent(categories, leaderId, rank, dataLevel);
		}
	}

	public void LogDeckEquipped(List<CardItem> deck)
	{
		if (TurnOff)
		{
			return;
		}
		int dataLevel = 0;
		GameState instance = GameState.Instance;
		if (instance != null && instance.ActiveQuest != null)
		{
			dataLevel = instance.ActiveQuest.iQuestID;
		}
		List<string> list = new List<string>();
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (CardItem item in deck)
		{
			list.Add(item.Form.ID);
			string key = item.Form.Faction.ToString();
			int value = 0;
			dictionary.TryGetValue(key, out value);
			dictionary[key] = value + 1;
		}
		string[] categories = new string[3] { "battle", null, null };
		Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
		dictionary2.Add("deck", list);
		LogEvent(categories, "deck_equipped", deck.Count, dataLevel, dictionary2);
		Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
		foreach (KeyValuePair<string, int> item2 in dictionary)
		{
			dictionary3.Add(item2.Key, item2.Value);
		}
		LogEvent(categories, "deck_factions_equipped", deck.Count, dataLevel, dictionary3);
	}

	public void LogCardCrafted(string cardID)
	{
		if (!TurnOff)
		{
			string[] categories = new string[3] { "card", "craft", cardID };
			LogEvent(categories, "Card_Num_Craft", 0, 0);
		}
	}

	public void LogCardSold(string cardID)
	{
		if (!TurnOff)
		{
			string[] categories = new string[3] { "card", "sell", cardID };
			LogEvent(categories, "Card_Num_Sold", 0, 0);
		}
	}

	public void LogCardUsed(string cardID)
	{
		if (!TurnOff)
		{
			int dataLevel = 0;
			GameState instance = GameState.Instance;
			if (instance != null && instance.ActiveQuest != null)
			{
				dataLevel = instance.ActiveQuest.iQuestID;
			}
			string[] categories = new string[3] { "card", "used", cardID };
			LogEvent(categories, "Card_Num_Used", 0, dataLevel);
		}
	}

	public void LogDungeonQuestWin(string dungeonID, int questIndex)
	{
		if (!TurnOff)
		{
			questIndex = Mathf.Clamp(questIndex, 0, 255);
			string[] categories = new string[3] { "dungeon", "end_result", dungeonID };
			LogEvent(categories, "Quest_Won", 1, questIndex);
		}
	}

	public void LogDungeonQuestLoss(string dungeonID, int questIndex)
	{
		if (!TurnOff)
		{
			questIndex = Mathf.Clamp(questIndex, 0, 255);
			string[] categories = new string[3] { "dungeon", "end_result", dungeonID };
			LogEvent(categories, "Quest_Lost", 1, questIndex);
		}
	}

	public void LogDungeonQuestQuit(string dungeonID, int questIndex)
	{
		if (!TurnOff)
		{
			questIndex = Mathf.Clamp(questIndex, 0, 255);
			string[] categories = new string[3] { "dungeon", "end_result", dungeonID };
			LogEvent(categories, "Quest_Quit", 1, questIndex);
		}
	}

	private string[] GetFCCategories()
	{
		return new string[3] { "fionnacake", "sell", null };
	}

	public void LogFCEvent(string eventName)
	{
		if (!TurnOff)
		{
			TFUtils.DebugLog("LogFCEvent: " + eventName);
			LogEvent(GetFCCategories(), eventName, 0, 0);
		}
	}

	public void LogFCDemoStart()
	{
		LogFCEvent("demo_start");
	}

	public void LogFCDemoComplete()
	{
		LogFCEvent("demo_complete");
	}

	public void LogFCDemoReplay()
	{
		LogFCEvent("demo_replay");
	}

	public void LogFCHeroesAwarded()
	{
		LogFCEvent("heroes_awarded");
	}

	public void LogFCSellOpen()
	{
		LogFCEvent("sell_open");
	}

	public void LogFCSellClick()
	{
		LogFCEvent("sell_click");
	}

	public void LogTreasureChestCatCardAwarded(int questID, string cardID)
	{
		if (!TurnOff)
		{
			string[] categories = new string[3] { "battle", "end_result", null };
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("card_id", cardID);
			LogEvent(categories, "TCat_Loot", questID, 0, dictionary);
		}
	}

	private void LogBR_Hit()
	{
		int dataLevel = 0;
		GameState instance = GameState.Instance;
		if (instance != null && instance.ActiveQuest != null)
		{
			dataLevel = instance.ActiveQuest.iQuestID;
		}
		string[] categories = new string[3] { "battle", "end_result", null };
		LogEvent(categories, "BR_Attack_Hit", BR_HitCount, dataLevel);
	}

	private void LogBR_Miss()
	{
		int dataLevel = 0;
		GameState instance = GameState.Instance;
		if (instance != null && instance.ActiveQuest != null)
		{
			dataLevel = instance.ActiveQuest.iQuestID;
		}
		string[] categories = new string[3] { "battle", "end_result", null };
		LogEvent(categories, "BR_Attack_Miss", BR_MissCount, dataLevel);
	}

	private void LogBR_Crit()
	{
		int dataLevel = 0;
		GameState instance = GameState.Instance;
		if (instance != null && instance.ActiveQuest != null)
		{
			dataLevel = instance.ActiveQuest.iQuestID;
		}
		string[] categories = new string[3] { "battle", "end_result", null };
		LogEvent(categories, "BR_Attack_Crit", BR_CritCount, dataLevel);
	}

	private void LogBR_Block()
	{
		int dataLevel = 0;
		GameState instance = GameState.Instance;
		if (instance != null && instance.ActiveQuest != null)
		{
			dataLevel = instance.ActiveQuest.iQuestID;
		}
		string[] categories = new string[3] { "battle", "end_result", null };
		LogEvent(categories, "BR_Attack_Block", BR_BlockCount, dataLevel);
	}

	private void LogBR_Counter()
	{
		int dataLevel = 0;
		GameState instance = GameState.Instance;
		if (instance != null && instance.ActiveQuest != null)
		{
			dataLevel = instance.ActiveQuest.iQuestID;
		}
		string[] categories = new string[3] { "battle", "end_result", null };
		LogEvent(categories, "BR_Defend_Counter", BR_CounterCount, dataLevel);
	}

	private void Awake()
	{
		deviceId = SystemInfo.deviceUniqueIdentifier;
		deviceInfo = string.Format("{0} {1} {2}", SystemInfo.deviceModel, SystemInfo.processorType, SystemInfo.operatingSystem);
		KontagentApiKey = "8e16a5d014e04bb1b77be06a22ac537c";
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
		}
		StartSession();
		SendDeviceInfo();
	}

	private void Start()
	{
		LogFirstTimePlay("00_KFFLogo");
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			StartSession();
		}
		else
		{
			StopSession();
		}
	}

	private int CompressData(int input, int bucketSize)
	{
		int num = bucketSize * 255;
		float num2 = bucketSize;
		float num3 = (float)input / num2;
		if (input >= num)
		{
			num3 = 255f;
		}
		return (int)num3;
	}

	private void AddCommon(Dictionary<string, string> KontagentEventData, string[] Categories, string eventName, int dataLevel)
	{
		KontagentEventData["DeviceId"] = deviceId;
		KontagentEventData["DeviceInfo"] = deviceInfo;
		KontagentEventData["n"] = eventName;
		KontagentEventData["l"] = dataLevel.ToString();
		if (Categories.Length > 0 && Categories[0] != null)
		{
			KontagentEventData["st1"] = Categories[0];
		}
		if (Categories.Length > 1 && Categories[1] != null)
		{
			KontagentEventData["st2"] = Categories[1];
		}
		if (Categories.Length > 2 && Categories[2] != null)
		{
			KontagentEventData["st3"] = Categories[2];
		}
	}

	public void SendDeviceInfo()
	{
		if (!TurnOff)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["v_maj"] = "1.0";
			dictionary["d"] = SystemInfo.deviceModel;
			//KontagentBinding.sendDeviceInformation(dictionary);
		}
	}

	public void StartSession()
	{
		if (!TurnOff)
		{
			//KontagentBinding.startSession(KontagentApiKey, KontagentTestMode);
		}
	}

	public void StopSession()
	{
		if (!TurnOff)
		{
			//KontagentBinding.stopSession();
		}
	}

	private string ValueToJson(object value)
	{
		if (value is string)
		{
			return "\"" + (string)value + "\"";
		}
		if (value is int)
		{
			return value.ToString();
		}
		if (value is float)
		{
			return value.ToString();
		}
		if (value is IEnumerable)
		{
			StringBuilder stringBuilder = new StringBuilder("[");
			bool flag = true;
			foreach (object item in (IEnumerable)value)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(ValueToJson(item));
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}
		return "\"<unsupported>\"";
	}

	private void LogEvent(string[] Categories, string eventName, int dataValue, int dataLevel, Dictionary<string, object> extras = null)
	{
		if (TurnOff)
		{
			return;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		AddCommon(dictionary, Categories, eventName, dataLevel);
		dictionary["v"] = dataValue.ToString();
		if (extras != null && extras.Count > 0)
		{
			StringBuilder stringBuilder = new StringBuilder("{");
			bool flag = true;
			foreach (KeyValuePair<string, object> extra in extras)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append("\"" + extra.Key + "\": " + ValueToJson(extra.Value) + string.Empty);
			}
			stringBuilder.Append("}");
			dictionary["data"] = stringBuilder.ToString();
		}
		//KontagentBinding.customEvent(eventName, dictionary);
	}

	private void LogIAP(string[] Categories, string iapName, int buyPrice, int dataLevel)
	{
		if (!TurnOff && !KFFUpsightManager.UpsightIAPLoggingEnabled)
		{
			TFUtils.DebugLog("H: LogIAP is called : " + iapName);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			AddCommon(dictionary, Categories, iapName, dataLevel);
			dictionary["v"] = buyPrice.ToString();
			//KontagentBinding.revenueTracking(buyPrice, dictionary);
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary2.Add("item", iapName);
			dictionary2.Add("price", (float)buyPrice / 100f);
			//KFFKochavaAnalytics.LogEvent("Purchase", dictionary2);
		}
	}
}
