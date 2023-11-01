#define ASSERTS_ON
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using JsonFx.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInfoScript : MonoBehaviour
{
	public enum Tag
	{
		GotCreaturePig,
		GotCreatureSnuggleTree,
		GotCreatureEmbarassingBard,
		GotCreatureMusicMallard,
		GotSpellCerebral,
		GotSpellBriefPower,
		GotSpikedMaceStump,
		GotFreezyJ,
		GotAwesomatude,
		GotMeMow
	}

	public delegate void SaveCallback();

	public delegate void CloudLoadCallback(bool success);

	private const string UNCONSUMABLES_PREFIX = "ent_";

	public const string CLOUD_SAVE_FILENAME = "CloudSave.json";

	private const int PLAYERINFO_VERSION = 4;

	private const string LAST_USER_FILE = "lastUserName";

	private const string TIMESTAMP_FILE = "lastTimeStamp";

	private static readonly string[] mCryptKeys = new string[12]
	{
		"Lorem ipsum dolor sit amet", "consectetur adipisicing elit", "sed do eiusmod tempor incididunt ut labore", "et dolore magna aliqua", "Ut enim ad minim veniam", "quis nostrud exercitation ullamco laboris nisi", "ut aliquip ex ea commodo consequat", "Duis aute irure dolor in reprehenderit in voluptate ", "velit esse cillum dolore eu fugiat nulla pariatur", "Excepteur sint occaecat cupidatat non proident ",
		"sunt in culpa qui officia deserunt", "mollit anim id est laborum"
	};

	private XorCrypto mCrypto;

	private readonly TimeSpan SAVE_TIMEOUT_TIME = new TimeSpan(0, 1, 0);

	private readonly string[] INTRO_TUTORIALS = new string[20]
	{
		"FIRSTQUEST", "GATCHA", "GATCHA2", "DECKBUILDER", "SECONDQUEST", "SPELL", "BUILDING", "OOMP", "FLOOP", "LOOTCHEST",
		"CRAFTING", "LEADERABILITY", "A31", "A32", "A33", "A34", "B3", "B4", "B5", "B6"
	};

	private int version = 4;

	public string PlayerName;

	public string Avatar = string.Empty;

	public string LastSaveTimeStamp;

	private int CurrentGameID;

	public int CampaignProgress;

	public int MatchProgress;

	public int OpponentCostumeID;

	public int OpponentID;

	public int PlayerCostumeID;

	public int PlayerID;

	public int PlayerAge;

	public int DefaultAgeGate = 13;

	public int DefaultAgeGateUK = 16;

	public int TOSCurrentVersion;

	private int TOSVersionAgreedTo;

	public int PPCurrentVersion;

	private int PPVersionAgreedTo;

	public int MatchID;

	public bool UsePresetDeck;

	public bool Tutorial;

	public int DeckID;

	private string mCoins;

	private string mCoinsAccumulated;

	private string mGemStr;

	private string mGemsAccumulated;

	public int NumMPGamesPlayed;

	public int CurrentMPQuest;

	private string mCurrentQuestType = "main";

	private Dictionary<string, int> mCurrentQuestIDs = new Dictionary<string, int>();

	private Dictionary<string, int> mLastClearedQuestIDs = new Dictionary<string, int>();

	private Dictionary<string, HashSet<int>> UnlockedRegions = new Dictionary<string, HashSet<int>>();

	private QuestData LastPlayedQuest;

	public Dictionary<string, BonusQuestStats> BonusQuests = new Dictionary<string, BonusQuestStats>();

	public Dictionary<string, MatchStats> mQuestMatchStats = new Dictionary<string, MatchStats>();

	private Dictionary<string, SideQuestManagerInfo> mSideQuestManagerInfo = new Dictionary<string, SideQuestManagerInfo>();

	public int Stamina;

	public int Stamina_Max;

	public string LastTimestamp;

	public DateTime LastGiftTimestamp;

	public DateTime DailyGiftTimestamp;

	public int NumUsedFreeGifts;

	public DateTime FirstCalendarTimestamp;

	public DateTime LastCalendarTimestamp;

	private uint CalendarDaysClaimed;

	private string mMaxInventory;

	private int _SelectedDeck;

	public int SelectedMPDeck;

	public string MPDeckLeaderID;

	public PlayerDeckManager DeckManager = new PlayerDeckManager();

	public Dictionary<string, int> mQuestMapDeckIdx = new Dictionary<string, int>();

	private string CurrentMapType;

	public Dictionary<string, Dictionary<int, int>> QuestProgress = new Dictionary<string, Dictionary<int, int>>();

	public GachaKeyList GachaKeys = new GachaKeyList();

	public List<string> Tags = new List<string>();

	private Dictionary<string, int> DungeonProgress;

	private Dictionary<string, int> OccuranceCounter;

	private string mChecksum;

	public bool NotificationEnabled = true;

	public List<string> tutorialsCompleted = new List<string>();

	public bool AutoBattleSetting;

	public string Party;

	public int PartyExpiration;

	public bool LoginAttempted;

	public bool isInitialized;

	private int mNumberofTrophies;

	private string mMPPlayerName;

	private string mMPOpponetName;

	private int mMPWinTrophies;

	private int mMPLossTrophies;

	private int mWinStreak;

	private int mStreakBonus;

	private bool mCheater;

	private static PlayerInfoScript g_playerInfoScript = null;

	private bool RemoteSaveInProgress;

	private string RemoteJsonPending;

	private bool ReloadInProgress;

	private SaveCallback RemoteSaveCallback;

	private bool mGatchaKeyRewarded;

	private static readonly string[] TUTORIAL_FIXUP_POST_MAIN_QUEST_4 = new string[6] { "CRAFTING", "SECONDQUEST", "DECKBUILDER", "GATCHA2", "GATCHA", "FIRSTQUEST" };

	private static readonly string[] TUTORIAL_FIXUP_POST_MAIN_QUEST_2 = new string[5] { "SECONDQUEST", "DECKBUILDER", "GATCHA2", "GATCHA", "FIRSTQUEST" };

	public bool HasEnteredAge
	{
		get
		{
			return PlayerAge != 0;
		}
	}

	public bool IsUnderage
	{
		get
		{
			string deviceLocale = Language.getDeviceLocale();
			int @int = PlayerPrefs.GetInt("PlayerAge");
			if (deviceLocale == "gb")
			{
				return @int < DefaultAgeGateUK;
			}
			return @int < DefaultAgeGate;
		}
	}

	public bool HasAgreedToTOS
	{
		get
		{
			return TOSVersionAgreedTo == TOSCurrentVersion;
		}
	}

	public bool HasAgreedToPP
	{
		get
		{
			return PPVersionAgreedTo == PPCurrentVersion;
		}
	}

	public string CurrentQuestType
	{
		get
		{
			return mCurrentQuestType;
		}
	}

	public int SelectedDeck
	{
		get
		{
			return _SelectedDeck;
		}
		set
		{
			_SelectedDeck = value;
			if (CurrentMapType != null)
			{
				mQuestMapDeckIdx[CurrentMapType] = value;
			}
		}
	}

	public string Checksum
	{
		get
		{
			return mChecksum;
		}
		set
		{
			mChecksum = value;
		}
	}

	public int TotalTrophies
	{
		get
		{
			return mNumberofTrophies;
		}
		set
		{
			mNumberofTrophies = value;
		}
	}

	public string MPPlayerName
	{
		get
		{
			return mMPPlayerName;
		}
		set
		{
			mMPPlayerName = value;
		}
	}

	public string MPOpponentName
	{
		get
		{
			return mMPOpponetName;
		}
		set
		{
			mMPOpponetName = value;
		}
	}

	public int MPWinTrophies
	{
		get
		{
			return mMPWinTrophies;
		}
		set
		{
			mMPWinTrophies = value;
		}
	}

	public int MPLossTrophies
	{
		get
		{
			return mMPLossTrophies;
		}
		set
		{
			mMPLossTrophies = value;
		}
	}

	public int WinStreak
	{
		get
		{
			return mWinStreak;
		}
		set
		{
			mWinStreak = value;
		}
	}

	public int StreakBonus
	{
		get
		{
			return mStreakBonus;
		}
		set
		{
			mStreakBonus = value;
		}
	}

	public bool Cheater
	{
		get
		{
			return mCheater;
		}
		set
		{
			mCheater = value;
		}
	}

	public string MultiplayerRank { get; set; }

	public int MaxInventory
	{
		get
		{
			return decryptValue(mMaxInventory);
		}
		set
		{
			mMaxInventory = encryptValue(value);
		}
	}

	public int Coins
	{
		get
		{
			return decryptValue(mCoins);
		}
		set
		{
			value = Math.Max(0, value);
			int num = decryptValue(mCoins);
			int num2 = value - num;
			int num3 = decryptValue(mCoinsAccumulated);
			if (num2 > 0)
			{
				num3 += num2;
				mCoinsAccumulated = encryptValue(num3);
			}
			mCoins = encryptValue(value);
		}
	}

	public int Gems
	{
		get
		{
			return decryptValue(mGemStr);
		}
		set
		{
			int num = decryptValue(mGemStr);
			int num2 = value - num;
			int num3 = decryptValue(mGemsAccumulated);
			if (num2 > 0)
			{
				num3 += num2;
				mGemsAccumulated = encryptValue(num3);
			}
			mGemStr = encryptValue(value);
		}
	}

	public bool IsSaveInProgress
	{
		get
		{
			return RemoteSaveInProgress;
		}
	}

	public int CoinsAccumulated
	{
		get
		{
			return decryptValue(mCoinsAccumulated);
		}
	}

	public int GemsAccumulated
	{
		get
		{
			return decryptValue(mGemsAccumulated);
		}
	}

	public QuestData GetLastPlayedQuest()
	{
		return LastPlayedQuest;
	}

	public bool HasFirstTimeKeyAwarded()
	{
		return !mGatchaKeyRewarded;
	}

	public void GatchaKeyAwarded()
	{
		mGatchaKeyRewarded = true;
	}

	private void Awake()
	{
		if (g_playerInfoScript == null)
		{
			g_playerInfoScript = this;
		}
		UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
		Application.targetFrameRate = 300;
	}

	private void Start()
	{
		if (mCrypto == null)
		{
			int num = UnityEngine.Random.Range(0, mCryptKeys.Length);
			string asciiMask = mCryptKeys[num] + UnityEngine.Random.Range(0, int.MaxValue);
			mCrypto = new XorCrypto(asciiMask);
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("PlayerInfo");
		if (array.Length > 1)
		{
			GameObject[] array2 = array;
			foreach (GameObject gameObject in array2)
			{
				PlayerInfoScript component = gameObject.GetComponent<PlayerInfoScript>();
				if (!component || component != g_playerInfoScript)
				{
					UnityEngine.Object.Destroy(gameObject);
				}
			}
		}
		PlayerID = 1;
	}

	private void OnEnable()
	{
		Session.OnSessionUserLoginSucceed += OnLoginSucceed;
	}

	private void OnDisable()
	{
		Session.OnSessionUserLoginSucceed -= OnLoginSucceed;
	}

	public static PlayerInfoScript GetInstance()
	{
		return g_playerInfoScript;
	}

	public bool IsReady()
	{
		SessionManager instance = SessionManager.GetInstance();
		return instance.IsReady();
	}

	private void Initialize()
	{
		CrashAnalytics.LogBreadcrumb("playerreset " + PlayerName);
		DeckManager.InitializeNewPlayer();
		LeaderManager.Instance.FillLeadersWithDummyData();
		string text;
		try
		{
			text = SessionManager.GetInstance().GetGameStateJson();
			if (string.IsNullOrEmpty(text))
			{
				throw new Exception("empty GameStateJSON received");
			}
		}
		catch (FileNotFoundException)
		{
			text = GetDefaultGameStateJson();
		}
		catch (Exception ex2)
		{
			TFUtils.ErrorLog(ex2.ToString());
			CrashAnalytics.LogException(ex2);
			text = GetDefaultGameStateJson();
		}
		Deserialize(text);
		CrashAnalytics.LogBreadcrumb("playerloaded " + g_playerInfoScript.PlayerName);
	}

	public string GetValue(string fieldName)
	{
		if (string.IsNullOrEmpty(fieldName))
		{
			return null;
		}
		FieldInfo field = GetType().GetField(fieldName);
		if (field == null)
		{
			return null;
		}
		return field.GetValue(this).ToString();
	}

	public void SetQuestStars(int questIndex, int numStars, string questType = "main")
	{
		QuestData questByIndex = QuestManager.Instance.GetQuestByIndex(questType, questIndex);
		SetQuestProgress(questByIndex, numStars);
	}

	public int GetQuestStars(int questIdx, string questType = "main")
	{
		QuestData questByIndex = QuestManager.Instance.GetQuestByIndex(questType, questIdx);
		return GetQuestProgress(questByIndex);
	}

	public bool IsRegionLocked(string mapQuestType, int regionID)
	{
		try
		{
			return !UnlockedRegions[mapQuestType].Contains(regionID);
		}
		catch (KeyNotFoundException)
		{
		}
		return true;
	}

	public void LockRegion(string mapQuestType, int regionID)
	{
		if (UnlockedRegions.ContainsKey(mapQuestType))
		{
			UnlockedRegions[mapQuestType].Remove(regionID);
		}
	}

	public void UnlockRegion(string mapQuestType, int regionID)
	{
		if (!UnlockedRegions.ContainsKey(mapQuestType))
		{
			UnlockedRegions[mapQuestType] = new HashSet<int>();
		}
		UnlockedRegions[mapQuestType].Add(regionID);
	}

	public void AcceptTOS()
	{
		TOSVersionAgreedTo = TOSCurrentVersion;
		Singleton<AnalyticsManager>.Instance.LogDebug("Accept_ToS", TOSCurrentVersion);
		Save();
	}

	public void AcceptPP()
	{
		PPVersionAgreedTo = PPCurrentVersion;
		Singleton<AnalyticsManager>.Instance.LogDebug("Accept_PP", PPCurrentVersion);
		Save();
	}

	public bool HasPurchasedFC()
	{
		return true;
	}

	public bool SetHasPurchasedFC(string productId)
	{
		string item = "ent_" + productId;
		if (Tags.Contains(item))
		{
			return false;
		}
		Tags.Add(item);
		IncOccuranceCounter("HasPurchasedFC");
		return true;
	}

	public void UnsetHasPurchasedFC(string productId)
	{
		string item = "ent_" + productId;
		if (Tags.Contains(item))
		{
			Tags.Remove(item);
			DecOccuranceCounter("HasPurchasedFC");
		}
	}

	public bool HasCompletedFCDemo()
	{
		return GetOccuranceCounter("HasCompletedFCDemo") != 0;
	}

	public void SetHasCompletedFCDemo()
	{
		IncOccuranceCounter("HasCompletedFCDemo");
	}

	public bool HasStartedFCDemo()
	{
		return GetOccuranceCounter("HasStartedFCDemo") != 0;
	}

	public void SetHasStartedFCDemo()
	{
		IncOccuranceCounter("HasStartedFCDemo");
	}

	public bool HasSeenFCUpsellScreen()
	{
		return GetOccuranceCounter("HasSeenFCUpsellScreen") != 0;
	}

	public void SetHasSeenFCUpsellScreen()
	{
		IncOccuranceCounter("HasSeenFCUpsellScreen");
	}

	public void ResetFCDemo()
	{
		SetOccuranceCounter("HasSeenFCUpsellScreen", 0);
	}

	public bool HasReceivedFCCards()
	{
		return GetOccuranceCounter("HasReceivedFCCards") != 0;
	}

	public void SetHasReceivedFCCards()
	{
		IncOccuranceCounter("HasReceivedFCCards");
	}

	private DateTime GetNow()
	{
		DateTime result = TFUtils.ServerTime;
		if (null != DebugFlagsScript.GetInstance() && DebugFlagsScript.GetInstance().specifyCalendarDate != string.Empty)
		{
			try
			{
				result = Convert.ToDateTime(DebugFlagsScript.GetInstance().specifyCalendarDate);
			}
			catch (FormatException)
			{
			}
		}
		return result;
	}

	public void ClaimCalendarGiftDay(int index)
	{
		TFUtils.DebugLog("ClaimCalendarGiftDay: " + index, "calendar");
		TFUtils.Assert(index > -1 && index < 31, "ClaimCalendarGiftDay: Invalid index");
		DateTime now = GetNow();
		CalendarDaysClaimed |= (uint)(1 << index);
		LastCalendarTimestamp = now;
		if (FirstCalendarTimestamp == DateTime.MinValue)
		{
			FirstCalendarTimestamp = LastCalendarTimestamp;
		}
	}

	public bool NeedsCalendarReset()
	{
		DateTime now = GetNow();
		return FirstCalendarTimestamp != DateTime.MinValue && (FirstCalendarTimestamp.Month < now.Month || FirstCalendarTimestamp.Year < now.Year);
	}

	public void ResetCalendar()
	{
		FirstCalendarTimestamp = (LastCalendarTimestamp = DateTime.MinValue);
		CalendarDaysClaimed = 0u;
	}

	public bool IsKeyGatchaUnlocked()
	{
		string gatchaKey_Unlock_Tutorial = ParametersManager.Instance.GatchaKey_Unlock_Tutorial;
		if (!DebugFlagsScript.GetInstance().stopTutorial && !string.IsNullOrEmpty(gatchaKey_Unlock_Tutorial) && !TutorialManager.Instance.isTutorialCompleted(gatchaKey_Unlock_Tutorial))
		{
			return false;
		}
		return true;
	}

	public bool IsCalendarUnlocked()
	{
		string calendar_Unlock_Tutorial = ParametersManager.Instance.Calendar_Unlock_Tutorial;
		return DebugFlagsScript.GetInstance().stopTutorial || string.IsNullOrEmpty(calendar_Unlock_Tutorial) || TutorialManager.Instance.isTutorialCompleted(calendar_Unlock_Tutorial);
	}

	public bool HasUnclaimedCalendarGift()
	{
		if (!IsCalendarUnlocked())
		{
			return false;
		}
		DateTime now = GetNow();
		DateTime dateTime = new DateTime(LastCalendarTimestamp.Year, LastCalendarTimestamp.Month, LastCalendarTimestamp.Day);
		DateTime dateTime2 = new DateTime(now.Year, now.Month, now.Day);
		bool flag = dateTime < dateTime2;
		if (flag)
		{
			ScheduleDataManager instance = ScheduleDataManager.Instance;
			List<ScheduleData> itemsAvailable = instance.GetItemsAvailable("calendar_gift", now.Ticks);
			flag = itemsAvailable.Any();
			if (flag)
			{
				int numCalendarGiftsClaimed = GetNumCalendarGiftsClaimed();
				int numCalendarGifts = CalendarGiftDataManager.Instance.GetNumCalendarGifts();
				flag = numCalendarGiftsClaimed < numCalendarGifts || NeedsCalendarReset();
			}
		}
		TFUtils.DebugLog("HasUnclaimedCalendarGift: " + flag, "calendar");
		return flag;
	}

	public bool[] GetCalendarGiftClaimHistory(ScheduleData calendar = null)
	{
		List<bool> list = new List<bool>();
		int numCalendarGifts = CalendarGiftDataManager.Instance.GetNumCalendarGifts(calendar);
		for (int i = 0; i < numCalendarGifts; i++)
		{
			bool item = (CalendarDaysClaimed & (1 << i)) != 0;
			list.Add(item);
		}
		return list.ToArray();
	}

	public int GetNumCalendarGiftsClaimed(ScheduleData calendar = null)
	{
		bool[] calendarGiftClaimHistory = GetCalendarGiftClaimHistory(calendar);
		if (calendarGiftClaimHistory != null)
		{
			IEnumerable<bool> enumerable = calendarGiftClaimHistory.Where((bool a) => a);
			return (enumerable != null) ? enumerable.Count() : 0;
		}
		return 0;
	}

	public int GetCurrentCalendarGiftIndex()
	{
		int i;
		for (i = 1; (CalendarDaysClaimed & (1 << i)) != 0L; i++)
		{
		}
		if (HasUnclaimedCalendarGift())
		{
			if (FirstCalendarTimestamp == DateTime.MinValue)
			{
				return 0;
			}
			return i;
		}
		return i - 1;
	}

	private void UnlockDungeonQuests(IEnumerable<string> dungeonIDs)
	{
		DungeonDataManager instance = DungeonDataManager.Instance;
		if (instance == null)
		{
			return;
		}
		foreach (string dungeonID in dungeonIDs)
		{
			DungeonData dungeon = instance.GetDungeon(dungeonID);
			int num = DungeonProgress[dungeonID] + 1;
			for (int num2 = Math.Min(dungeon.Quests.Count - 1, num); num2 >= 0; num2--)
			{
				dungeon.UnlockQuest(num2, num2 < num);
			}
		}
	}

	public void SetDungeonProgress(DungeonBattleResult result)
	{
		int dungeonProgress = GetDungeonProgress(result.DungeonID);
		if (result.cleared && result.QuestIndex > dungeonProgress)
		{
			if (DungeonProgress == null)
			{
				DungeonProgress = new Dictionary<string, int>();
			}
			DungeonProgress[result.DungeonID] = result.QuestIndex;
			List<string> list = new List<string>();
			list.Add(result.DungeonID);
			UnlockDungeonQuests(list);
		}
	}

	public int GetDungeonProgress(string dungeonID)
	{
		int value;
		if (DungeonProgress != null && DungeonProgress.TryGetValue(dungeonID, out value))
		{
			return value;
		}
		return -1;
	}

	private string SerializeDungeonProgress()
	{
		if (DungeonProgress == null)
		{
			return "{}";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		bool flag = true;
		foreach (string key in DungeonProgress.Keys)
		{
			int num = DungeonProgress[key];
			if (!flag)
			{
				stringBuilder.Append(',');
			}
			stringBuilder.Append("\"" + key + "\":\"" + num + "\"");
			flag = false;
		}
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	public int GetOccuranceCounter(string key)
	{
		int value;
		if (OccuranceCounter != null && OccuranceCounter.TryGetValue(key, out value))
		{
			return value;
		}
		return 0;
	}

	public int IncOccuranceCounter(string key, int amount = 1)
	{
		if (amount > 0)
		{
			int occuranceCounter = GetOccuranceCounter(key);
			SetOccuranceCounter(key, occuranceCounter + amount);
		}
		return GetOccuranceCounter(key);
	}

	public int DecOccuranceCounter(string key, int amount = 1)
	{
		if (amount > 0)
		{
			int occuranceCounter = GetOccuranceCounter(key);
			SetOccuranceCounter(key, occuranceCounter - amount);
		}
		return GetOccuranceCounter(key);
	}

	public void SetOccuranceCounter(string key, int value)
	{
		if (OccuranceCounter == null)
		{
			OccuranceCounter = new Dictionary<string, int>();
		}
		OccuranceCounter[key] = value;
		Save();
	}

	private string SerializeOccuranceCounter()
	{
		if (OccuranceCounter == null)
		{
			return "{}";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		bool flag = true;
		foreach (string key in OccuranceCounter.Keys)
		{
			int num = OccuranceCounter[key];
			if (!flag)
			{
				stringBuilder.Append(',');
			}
			stringBuilder.Append("\"" + key + "\":\"" + num + "\"");
			flag = false;
		}
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	private string SerializeQuestStars(string questType, string field)
	{
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		if (QuestProgress.ContainsKey(questType))
		{
			foreach (KeyValuePair<int, int> item in QuestProgress[questType])
			{
				stringBuilder2.Append(',');
				if (field.Equals("key"))
				{
					stringBuilder2.Append(item.Key.ToString());
				}
				else if (field.Equals("value"))
				{
					stringBuilder2.Append(item.Value.ToString());
				}
			}
		}
		stringBuilder.Append('[');
		string text = stringBuilder2.ToString();
		if (!string.IsNullOrEmpty(text) && text.Length >= 1)
		{
			stringBuilder.Append(text.Substring(1));
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	private string SerializeTags()
	{
		if (Tags == null)
		{
			return "[]";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		bool flag = true;
		foreach (string tag in Tags)
		{
			if (!flag)
			{
				stringBuilder.Append(',');
			}
			flag = false;
			stringBuilder.Append("\"" + tag + "\"");
		}
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}

	private string SerializeTutorials()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		bool flag = true;
		foreach (string item in tutorialsCompleted)
		{
			if (!flag)
			{
				stringBuilder.Append(',');
			}
			flag = false;
			stringBuilder.Append("\"" + item + "\"");
		}
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}

	private string SerializeUnlockedRegions()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append('{');
		bool flag = true;
		foreach (KeyValuePair<string, HashSet<int>> unlockedRegion in UnlockedRegions)
		{
			if (!flag)
			{
				stringBuilder.Append(',');
			}
			flag = false;
			stringBuilder.Append("\"" + unlockedRegion.Key + "\":" + SerializeListOfInts(unlockedRegion.Value));
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	private string SerializeListOfInts(IEnumerable<int> list)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = true;
		stringBuilder.Append("[");
		foreach (int item in list)
		{
			if (!flag)
			{
				stringBuilder.Append(',');
			}
			flag = false;
			stringBuilder.Append(item);
		}
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}

	public Deck GetSelectedDeck()
	{
		return DeckManager.GetDeck(SelectedDeck);
	}

	public Deck GetSelectedDeckCopy()
	{
		return DeckManager.GetDeckCopy(SelectedDeck);
	}

	public bool HasTag(Tag tag)
	{
		return HasTag(tag.ToString());
	}

	public bool HasTag(string tag)
	{
		return Tags.Contains(tag);
	}

	public bool SetTag(Tag tag)
	{
		return SetTag(tag.ToString());
	}

	public bool SetTag(string tag)
	{
		if (HasTag(tag))
		{
			return false;
		}
		Tags.Add(tag);
		return true;
	}

	public static string ConvertPlayerIDToCode(string id)
	{
		string[] array = id.Split('_');
		if (array.Length < 2)
		{
			return null;
		}
		int num = Convert.ToInt32(array[0]);
		int num2 = Convert.ToInt32(array[1]);
		long num3 = 0L;
		if (((uint)num & (true ? 1u : 0u)) != 0)
		{
			num3 |= 0x4000000;
		}
		if (((uint)num & 2u) != 0)
		{
			num3 |= 0x20000000;
		}
		if (((uint)num & 4u) != 0)
		{
			num3 |= 1;
		}
		if (((uint)num & 8u) != 0)
		{
			num3 |= 0x10000000;
		}
		if (((uint)num & 0x10u) != 0)
		{
			num3 |= 8;
		}
		if (((uint)num & 0x20u) != 0)
		{
			num3 |= 0x400000;
		}
		if (((uint)num & 0x40u) != 0)
		{
			num3 |= 0x100;
		}
		if (((uint)num & 0x80u) != 0)
		{
			num3 |= 0x80;
		}
		if (((uint)num & 0x100u) != 0)
		{
			num3 |= 0x20000;
		}
		if (((uint)num & 0x200u) != 0)
		{
			num3 |= 0x20;
		}
		if (((uint)num2 & (true ? 1u : 0u)) != 0)
		{
			num3 |= 0x1000000;
		}
		if (((uint)num2 & 2u) != 0)
		{
			num3 |= 0x10;
		}
		if (((uint)num2 & 4u) != 0)
		{
			num3 |= 0x80000;
		}
		if (((uint)num2 & 8u) != 0)
		{
			num3 |= 0x200;
		}
		if (((uint)num2 & 0x10u) != 0)
		{
			num3 |= 0x10000;
		}
		if (((uint)num2 & 0x20u) != 0)
		{
			num3 |= 0x4000;
		}
		if (((uint)num2 & 0x40u) != 0)
		{
			num3 |= 0x2000;
		}
		if (((uint)num2 & 0x80u) != 0)
		{
			num3 |= 0x2000000;
		}
		if (((uint)num2 & 0x100u) != 0)
		{
			num3 |= 0x40000;
		}
		if (((uint)num2 & 0x200u) != 0)
		{
			num3 |= 0x800;
		}
		if (((uint)num2 & 0x400u) != 0)
		{
			num3 |= 0x200000;
		}
		if (((uint)num2 & 0x800u) != 0)
		{
			num3 |= 0x100000;
		}
		if (((uint)num2 & 0x1000u) != 0)
		{
			num3 |= 2;
		}
		if (((uint)num2 & 0x2000u) != 0)
		{
			num3 |= 0x1000;
		}
		if (((uint)num2 & 0x4000u) != 0)
		{
			num3 |= 0x800000;
		}
		if (((uint)num2 & 0x8000u) != 0)
		{
			num3 |= 0x8000;
		}
		if (((uint)num2 & 0x10000u) != 0)
		{
			num3 |= 4;
		}
		if (((uint)num2 & 0x20000u) != 0)
		{
			num3 |= 0x40;
		}
		if (((uint)num2 & 0x40000u) != 0)
		{
			num3 |= 0x400;
		}
		if (((uint)num2 & 0x80000u) != 0)
		{
			num3 |= 0x40000000;
		}
		if (((uint)num2 & 0x100000u) != 0)
		{
			num3 |= 0x200;
		}
		if (((uint)num2 & 0x200000u) != 0)
		{
			num3 |= 1;
		}
		if (((uint)num2 & 0x400000u) != 0)
		{
			num3 |= int.MinValue;
		}
		if (((uint)num2 & 0x800000u) != 0)
		{
			num3 |= 0x40;
		}
		if (((uint)num2 & 0x1000000u) != 0)
		{
			num3 |= 0x8000000;
		}
		if (((uint)num2 & 0x2000000u) != 0)
		{
			num3 |= 0x10;
		}
		if (((uint)num2 & 0x4000000u) != 0)
		{
			num3 |= 0x100;
		}
		if (((uint)num2 & 0x8000000u) != 0)
		{
			num3 |= 0x80;
		}
		if (((uint)num2 & 0x10000000u) != 0)
		{
			num3 |= 4;
		}
		if (((uint)num2 & 0x20000000u) != 0)
		{
			num3 |= 2;
		}
		if (((uint)num2 & 0x40000000u) != 0)
		{
			num3 |= 8;
		}
		if (((uint)num2 & 0x80000000u) != 0)
		{
			num3 |= 0x20;
		}
		return num3.ToString("000,000,000");
	}

	public static string ConvertPlayerCodeToID(string code)
	{
		int num = int.Parse(code, NumberStyles.AllowThousands);
		int num2 = 0;
		int num3 = 0;
		if (((uint)num & 0x4000000u) != 0)
		{
			num2 |= 1;
		}
		if (((uint)num & 0x20000000u) != 0)
		{
			num2 |= 2;
		}
		if (((uint)num & (true ? 1u : 0u)) != 0)
		{
			num2 |= 4;
		}
		if (((uint)num & 0x10000000u) != 0)
		{
			num2 |= 8;
		}
		if (((uint)num & 8u) != 0)
		{
			num2 |= 0x10;
		}
		if (((uint)num & 0x400000u) != 0)
		{
			num2 |= 0x20;
		}
		if (((uint)num & 0x100u) != 0)
		{
			num2 |= 0x40;
		}
		if (((uint)num & 0x80u) != 0)
		{
			num2 |= 0x80;
		}
		if (((uint)num & 0x20000u) != 0)
		{
			num2 |= 0x100;
		}
		if (((uint)num & 0x20u) != 0)
		{
			num2 |= 0x200;
		}
		if (((uint)num & 0x1000000u) != 0)
		{
			num3 |= 1;
		}
		if (((uint)num & 0x10u) != 0)
		{
			num3 |= 2;
		}
		if (((uint)num & 0x80000u) != 0)
		{
			num3 |= 4;
		}
		if (((uint)num & 0x200u) != 0)
		{
			num3 |= 8;
		}
		if (((uint)num & 0x10000u) != 0)
		{
			num3 |= 0x10;
		}
		if (((uint)num & 0x4000u) != 0)
		{
			num3 |= 0x20;
		}
		if (((uint)num & 0x2000u) != 0)
		{
			num3 |= 0x40;
		}
		if (((uint)num & 0x2000000u) != 0)
		{
			num3 |= 0x80;
		}
		if (((uint)num & 0x40000u) != 0)
		{
			num3 |= 0x100;
		}
		if (((uint)num & 0x800u) != 0)
		{
			num3 |= 0x200;
		}
		if (((uint)num & 0x200000u) != 0)
		{
			num3 |= 0x400;
		}
		if (((uint)num & 0x100000u) != 0)
		{
			num3 |= 0x800;
		}
		if (((uint)num & 2u) != 0)
		{
			num3 |= 0x1000;
		}
		if (((uint)num & 0x1000u) != 0)
		{
			num3 |= 0x2000;
		}
		if (((uint)num & 0x800000u) != 0)
		{
			num3 |= 0x4000;
		}
		if (((uint)num & 0x8000u) != 0)
		{
			num3 |= 0x8000;
		}
		if (((uint)num & 4u) != 0)
		{
			num3 |= 0x10000;
		}
		if (((uint)num & 0x40u) != 0)
		{
			num3 |= 0x20000;
		}
		if (((uint)num & 0x400u) != 0)
		{
			num3 |= 0x40000;
		}
		if (((uint)num & 0x40000000u) != 0)
		{
			num3 |= 0x80000;
		}
		if (((uint)num & 0x200u) != 0)
		{
			num3 |= 0x100000;
		}
		if (((uint)num & (true ? 1u : 0u)) != 0)
		{
			num3 |= 0x200000;
		}
		if (((uint)num & 0x80000000u) != 0)
		{
			num3 |= 0x400000;
		}
		if (((uint)num & 0x40u) != 0)
		{
			num3 |= 0x800000;
		}
		if (((uint)num & 0x8000000u) != 0)
		{
			num3 |= 0x1000000;
		}
		if (((uint)num & 0x10u) != 0)
		{
			num3 |= 0x2000000;
		}
		if (((uint)num & 0x100u) != 0)
		{
			num3 |= 0x4000000;
		}
		if (((uint)num & 0x80u) != 0)
		{
			num3 |= 0x8000000;
		}
		if (((uint)num & 4u) != 0)
		{
			num3 |= 0x10000000;
		}
		if (((uint)num & 2u) != 0)
		{
			num3 |= 0x20000000;
		}
		if (((uint)num & 8u) != 0)
		{
			num3 |= 0x40000000;
		}
		if (((uint)num & 0x20u) != 0)
		{
			num3 |= int.MinValue;
		}
		return string.Format("{0}_{1}", num2, num3);
	}

	public string GetPlayerCode()
	{
		SessionManager instance = SessionManager.GetInstance();
		return instance.PlayerID;
	}

	public static void SavePlayerName(string _playerName)
	{
		string path = Path.Combine(Application.persistentDataPath, "lastUserName");
		File.WriteAllText(path, _playerName);
	}

	public static void SaveTimeStamp(string timestamp)
	{
		string path = Path.Combine(Application.persistentDataPath, "lastTimeStamp");
		File.WriteAllText(path, timestamp);
	}

	public void LoadTimeStamp()
	{
		string path = Path.Combine(Application.persistentDataPath, "lastTimeStamp");
		if (File.Exists(path))
		{
			LastSaveTimeStamp = File.ReadAllText(path);
		}
	}

	public static string LoadPlayerName()
	{
		string result = null;
		string path = Path.Combine(Application.persistentDataPath, "lastUserName");
		if (File.Exists(path))
		{
			result = File.ReadAllText(path);
		}
		return result;
	}

	public static void ResetPlayerName()
	{
		string path = Path.Combine(Application.persistentDataPath, "lastUserName");
		if (File.Exists(path))
		{
			File.Delete(path);
		}
	}

	public void Login()
	{
		if (LoginAttempted)
		{
			return;
		}
		LoginAttempted = true;
		LoadTimeStamp();
		string text = null;
		PlayerName = LoadPlayerName();
		text = PlayerPrefs.GetString("SocialLogin", null);
		if (string.IsNullOrEmpty(PlayerName) && string.IsNullOrEmpty(text))
		{
			PlayerName = Guid.NewGuid().ToString();
			SavePlayerName(PlayerName);
		}
		else if (string.IsNullOrEmpty(PlayerName))
		{
			PlayerName = text;
			SavePlayerName(PlayerName);
		}
		SessionManager instance = SessionManager.GetInstance();
		instance.OnReadyCallback = null;
		TFUtils.WarnLog("Login: SocialID= " + text + ", PlayerName= " + PlayerName);
		instance.Login(false, text, PlayerName);
	}

	private void OnLoginSucceed()
	{
		string @string = PlayerPrefs.GetString("SocialLogin", null);
		if (!string.IsNullOrEmpty(@string))
		{
			PlayerName = @string;
			SavePlayerName(PlayerName);
		}
	}

	public bool Reauthenticate()
	{
		SessionManager instance = SessionManager.GetInstance();
		if (!instance.IsReady() || !instance.IsLoggedIn() || instance.IsAuthenticated())
		{
			return false;
		}
		instance.OnReadyCallback = null;
		instance.Login(false, null, PlayerName);
		return true;
	}

	public static string MakeJS(string key, object val)
	{
		string text = "\"" + key + "\":";
		if (val is string)
		{
			return string.Concat(text, "\"", val, "\"");
		}
		if (val is bool)
		{
			return text + ((!(bool)val) ? "0" : "1");
		}
		if (val is ICWSerializable)
		{
			ICWSerializable iCWSerializable = val as ICWSerializable;
			return text + iCWSerializable.Serialize();
		}
		if (val is IDictionary)
		{
			IDictionary dictionary = val as IDictionary;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{");
			bool flag = true;
			foreach (object key2 in dictionary.Keys)
			{
				if (!flag)
				{
					stringBuilder.Append(",");
				}
				flag = false;
				stringBuilder.Append(MakeJS(string.Format("{0}", key2), dictionary[key2]));
			}
			stringBuilder.Append("}");
			return text + stringBuilder.ToString();
		}
		if (val != null)
		{
			return text + val.ToString();
		}
		return text;
	}

	public string Serialize()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append('{');
		stringBuilder.Append(MakeJS("PlayerInfoVersion", version) + ",");
		stringBuilder.Append(MakeJS("PlayerName", PlayerName) + ",");
		stringBuilder.Append(MakeJS("PlayerAge", PlayerAge) + ",");
		stringBuilder.Append(MakeJS("CurrentGameID", CurrentGameID) + ",");
		stringBuilder.Append(MakeJS("CampaignProgress", CampaignProgress) + ",");
		stringBuilder.Append(MakeJS("Avatar", Avatar) + ",");
		stringBuilder.Append(MakeJS("UsePresetDeck", UsePresetDeck) + ",");
		stringBuilder.Append(MakeJS("Tutorial", Tutorial) + ",");
		stringBuilder.Append(MakeJS("DeckID", DeckID) + ",");
		stringBuilder.Append(MakeJS("Coins", decryptValue(mCoins)) + ",");
		stringBuilder.Append(MakeJS("Gems", decryptValue(mGemStr)) + ",");
		stringBuilder.Append(MakeJS("CoinsAccumulated", decryptValue(mCoinsAccumulated)) + ",");
		stringBuilder.Append(MakeJS("GemsAccumulated", decryptValue(mGemsAccumulated)) + ",");
		stringBuilder.Append(MakeJS("NumMPGamesPlayed", NumMPGamesPlayed) + ",");
		stringBuilder.Append(MakeJS("Stamina", Stamina) + ",");
		stringBuilder.Append(MakeJS("Stamina_Max", Stamina_Max) + ",");
		stringBuilder.Append(MakeJS("LastTimestamp", LastTimestamp) + ",");
		stringBuilder.Append(MakeJS("MaxInventory", decryptValue(mMaxInventory)) + ",");
		stringBuilder.Append(MakeJS("SelectedDeck", SelectedDeck) + ",");
		stringBuilder.Append(MakeJS("SelectedMPDeck", SelectedMPDeck) + ",");
		stringBuilder.Append(MakeJS("MPDeckLeaderID", MPDeckLeaderID) + ",");
		stringBuilder.Append(MakeJS("NotificationEnabled", NotificationEnabled) + ",");
		stringBuilder.Append(MakeJS("AutoBattleSetting", AutoBattleSetting) + ",");
		stringBuilder.Append(MakeJS("Magic", DetectCheater.CreateChecksum(DetectCheater.MD5Input())) + ",");
		stringBuilder.Append(MakeJS("KeyRewarded", mGatchaKeyRewarded) + ",");
		stringBuilder.Append(MakeJS("TOSVersionAgreedTo", TOSVersionAgreedTo) + ",");
		stringBuilder.Append(MakeJS("PPVersionAgreedTo", PPVersionAgreedTo) + ",");
		if (Party != null)
		{
			stringBuilder.Append(MakeJS("Party", Party) + ",");
		}
		if (PartyExpiration > 0)
		{
			stringBuilder.Append(MakeJS("PartyExpiration", PartyExpiration) + ",");
		}
		stringBuilder.Append(MakeJS("NumberofTrophies", mNumberofTrophies) + ",");
		if (mMPPlayerName != null)
		{
			stringBuilder.Append(MakeJS("MPPlayerName", mMPPlayerName) + ",");
		}
		if (MultiplayerRank != null)
		{
			stringBuilder.Append(MakeJS("MultiplayerRank", MultiplayerRank) + ",");
		}
		if (DailyGiftTimestamp != DateTime.MinValue)
		{
			stringBuilder.Append(MakeJS("DailyGiftTimestamp", DailyGiftTimestamp.ToString()) + ",");
			stringBuilder.Append(MakeJS("LastGiftTimestamp", LastGiftTimestamp.ToString()) + ",");
			stringBuilder.Append(MakeJS("NumUsedFreeGifts", NumUsedFreeGifts) + ",");
		}
		if (FirstCalendarTimestamp != DateTime.MinValue)
		{
			stringBuilder.Append(MakeJS("FirstCalendarTimestamp", FirstCalendarTimestamp.ToString()) + ",");
			stringBuilder.Append(MakeJS("LastCalendarTimestamp", LastCalendarTimestamp.ToString()) + ",");
			stringBuilder.Append(MakeJS("CalendarDaysClaimed", CalendarDaysClaimed) + ",");
		}
		stringBuilder.Append("\"Leaders\":" + LeaderManager.Instance.Serialize() + ",");
		stringBuilder.Append("\"GachaKeys\":" + GachaKeys.Serialize() + ",");
		stringBuilder.Append("\"Tags\":" + SerializeTags() + ",");
		stringBuilder.Append("\"DungeonProgress\":" + SerializeDungeonProgress() + ",");
		stringBuilder.Append("\"OccuranceCounter\":" + SerializeOccuranceCounter() + ",");
		stringBuilder.Append("\"Fusion\":" + FusionManager.Instance.Serialize() + ",");
		stringBuilder.Append("\"Inventory\":" + DeckManager.Serialize() + ",");
		stringBuilder.Append(MakeJS("CurrentQuest", GetCurrentQuestID("main")) + ",");
		stringBuilder.Append(MakeJS("LastClearedQuest", GetLastClearedQuestID("main")) + ",");
		stringBuilder.Append(MakeJS("CurrentFCQuestID", GetCurrentQuestID("fc")) + ",");
		stringBuilder.Append(MakeJS("LastClearedFCQuestID", GetLastClearedQuestID("fc")) + ",");
		stringBuilder.Append("\"QuestProgress_MainIDs\":" + SerializeQuestStars("main", "key") + ",");
		stringBuilder.Append("\"QuestProgress_MainStars\":" + SerializeQuestStars("main", "value") + ",");
		stringBuilder.Append("\"QuestProgress_FCIDs\":" + SerializeQuestStars("fc", "key") + ",");
		stringBuilder.Append("\"QuestProgress_FCStars\":" + SerializeQuestStars("fc", "value") + ",");
		stringBuilder.Append("\"QuestProgress_TCatIDs\":" + SerializeQuestStars("tcat", "key") + ",");
		stringBuilder.Append("\"QuestProgress_TCatStars\":" + SerializeQuestStars("tcat", "value") + ",");
		stringBuilder.Append("\"QuestProgress_ElFistoIDs\":" + SerializeQuestStars("elfisto", "key") + ",");
		stringBuilder.Append("\"QuestProgress_ElFistoStars\":" + SerializeQuestStars("elfisto", "value") + ",");
		stringBuilder.Append("\"UnlockedRegions\":" + SerializeUnlockedRegions() + ",");
		if (mQuestMapDeckIdx.ContainsKey("main"))
		{
			stringBuilder.Append(MakeJS("DeckMain", mQuestMapDeckIdx["main"]) + ",");
		}
		if (mQuestMapDeckIdx.ContainsKey("fc"))
		{
			stringBuilder.Append(MakeJS("DeckFC", mQuestMapDeckIdx["fc"]) + ",");
		}
		if (mQuestMatchStats.ContainsKey("main"))
		{
			stringBuilder.Append(MakeJS("CWAttempts", mQuestMatchStats["main"].Attempts) + ",");
			stringBuilder.Append(MakeJS("CWWins", mQuestMatchStats["main"].Wins) + ",");
			stringBuilder.Append(MakeJS("CWLosses", mQuestMatchStats["main"].Losses) + ",");
		}
		if (mQuestMatchStats.ContainsKey("fc"))
		{
			stringBuilder.Append(MakeJS("FCAttempts", mQuestMatchStats["fc"].Attempts) + ",");
			stringBuilder.Append(MakeJS("FCWins", mQuestMatchStats["fc"].Wins) + ",");
			stringBuilder.Append(MakeJS("FCLosses", mQuestMatchStats["fc"].Losses) + ",");
		}
		if (mQuestMatchStats.ContainsKey("tcat"))
		{
			stringBuilder.Append(MakeJS("TCATAttempts", mQuestMatchStats["tcat"].Attempts) + ",");
			stringBuilder.Append(MakeJS("TCATWins", mQuestMatchStats["tcat"].Wins) + ",");
			stringBuilder.Append(MakeJS("TCATLosses", mQuestMatchStats["tcat"].Losses) + ",");
		}
		if (mQuestMatchStats.ContainsKey("elfisto"))
		{
			stringBuilder.Append(MakeJS("EFAttempts", mQuestMatchStats["elfisto"].Attempts) + ",");
			stringBuilder.Append(MakeJS("EFWins", mQuestMatchStats["elfisto"].Wins) + ",");
			stringBuilder.Append(MakeJS("EFLosses", mQuestMatchStats["elfisto"].Losses) + ",");
		}
		StringBuilder stringBuilder2 = new StringBuilder();
		StringBuilder stringBuilder3 = new StringBuilder();
		StringBuilder stringBuilder4 = new StringBuilder();
		StringBuilder stringBuilder5 = new StringBuilder();
		StringBuilder stringBuilder6 = new StringBuilder();
		StringBuilder stringBuilder7 = new StringBuilder();
		StringBuilder stringBuilder8 = new StringBuilder();
		StringBuilder stringBuilder9 = new StringBuilder();
		foreach (KeyValuePair<string, BonusQuestStats> bonusQuest in BonusQuests)
		{
			stringBuilder2.Append(",");
			stringBuilder2.Append("\"" + bonusQuest.Key.ToString() + "\"");
			stringBuilder3.Append(",");
			stringBuilder3.Append(Convert.ToInt32(bonusQuest.Value.firstAppearance));
			stringBuilder4.Append(",");
			stringBuilder4.Append(bonusQuest.Value.ActiveQuestID);
			stringBuilder5.Append(",");
			stringBuilder5.Append(bonusQuest.Value.ReplacedQuestID);
			stringBuilder6.Append(",");
			stringBuilder6.Append(bonusQuest.Value.CachedMatchStats.Attempts);
			stringBuilder7.Append(",");
			stringBuilder7.Append(bonusQuest.Value.CachedMatchStats.Wins);
			stringBuilder8.Append(",");
			stringBuilder8.Append(bonusQuest.Value.CachedMatchStats.Losses);
			stringBuilder9.Append(",");
			stringBuilder9.Append("\"" + bonusQuest.Value.LastPlayedTime.ToString() + "\"");
		}
		stringBuilder.Append("\"BonusQuest_QuestTypes\":");
		stringBuilder.Append('[');
		string text = stringBuilder2.ToString();
		if (!string.IsNullOrEmpty(text) && text.Length >= 1)
		{
			stringBuilder.Append(text.Substring(1));
		}
		stringBuilder.Append(']' + ",");
		stringBuilder.Append("\"BonusQuest_FirstAppearance\":");
		stringBuilder.Append('[');
		text = stringBuilder3.ToString();
		if (!string.IsNullOrEmpty(text) && text.Length >= 1)
		{
			stringBuilder.Append(text.Substring(1));
		}
		stringBuilder.Append(']' + ",");
		stringBuilder.Append("\"BonusQuest_ActiveQuestIDs\":");
		stringBuilder.Append('[');
		text = stringBuilder4.ToString();
		if (!string.IsNullOrEmpty(text) && text.Length >= 1)
		{
			stringBuilder.Append(text.Substring(1));
		}
		stringBuilder.Append(']' + ",");
		stringBuilder.Append("\"BonusQuest_ReplacedQuestIDs\":");
		stringBuilder.Append('[');
		text = stringBuilder5.ToString();
		if (!string.IsNullOrEmpty(text) && text.Length >= 1)
		{
			stringBuilder.Append(text.Substring(1));
		}
		stringBuilder.Append(']' + ",");
		stringBuilder.Append("\"BonusQuest_StatsAttempts\":");
		stringBuilder.Append('[');
		text = stringBuilder6.ToString();
		if (!string.IsNullOrEmpty(text) && text.Length >= 1)
		{
			stringBuilder.Append(text.Substring(1));
		}
		stringBuilder.Append(']' + ",");
		stringBuilder.Append("\"BonusQuest_StatsWins\":");
		stringBuilder.Append('[');
		text = stringBuilder7.ToString();
		if (!string.IsNullOrEmpty(text) && text.Length >= 1)
		{
			stringBuilder.Append(text.Substring(1));
		}
		stringBuilder.Append(']' + ",");
		stringBuilder.Append("\"BonusQuest_StatsLosses\":");
		stringBuilder.Append('[');
		text = stringBuilder8.ToString();
		if (!string.IsNullOrEmpty(text) && text.Length >= 1)
		{
			stringBuilder.Append(text.Substring(1));
		}
		stringBuilder.Append(']' + ",");
		stringBuilder.Append("\"BonusQuest_LastPlayed\":");
		stringBuilder.Append('[');
		text = stringBuilder9.ToString();
		if (!string.IsNullOrEmpty(text) && text.Length >= 1)
		{
			stringBuilder.Append(text.Substring(1));
		}
		stringBuilder.Append(']' + ",");
		stringBuilder.Append(MakeJS("SideQuestManagerInfo", mSideQuestManagerInfo) + ",");
		stringBuilder.Append("\"Tutorials\":" + SerializeTutorials());
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	public void Deserialize(string json)
	{
		Dictionary<string, object> dictionary;
		try
		{
			dictionary = JsonReader.Deserialize<Dictionary<string, object>>(json);
		}
		catch (Exception e)
		{
			Singleton<AnalyticsManager>.Instance.LogDebug("exception_deserialize");
			CrashAnalytics.LogException(e);
			dictionary = GetDefaultGameStateDictionary();
		}
		version = TFUtils.LoadInt(dictionary, "PlayerInfoVersion", 4);
		if (version >= 1)
		{
			string text = TFUtils.LoadString(dictionary, "PlayerName", PlayerName);
			if (!string.IsNullOrEmpty(text))
			{
				PlayerName = text;
			}
			PlayerAge = TFUtils.LoadInt(dictionary, "PlayerAge", 0);
			CurrentGameID = TFUtils.LoadInt(dictionary, "CurrentGameID", 0);
			CampaignProgress = TFUtils.LoadInt(dictionary, "CampaignProgress", 0);
			Avatar = TFUtils.LoadString(dictionary, "Avatar", string.Empty);
			UsePresetDeck = TFUtils.LoadBoolAsInt(dictionary, "UsePresetDeck", false);
			Tutorial = TFUtils.LoadBoolAsInt(dictionary, "Tutorial", false);
			DeckID = TFUtils.LoadInt(dictionary, "DeckID", 0);
			mCoins = encryptValue(TFUtils.LoadInt(dictionary, "Coins", ParametersManager.Instance.New_Player_Coins));
			mGemStr = encryptValue(TFUtils.LoadInt(dictionary, "Gems", ParametersManager.Instance.New_Player_Gems));
			mCoinsAccumulated = encryptValue(TFUtils.LoadInt(dictionary, "CoinsAccumulated", 0));
			mGemsAccumulated = encryptValue(TFUtils.LoadInt(dictionary, "GemsAccumulated", 0));
			mCurrentQuestIDs["main"] = TFUtils.LoadInt(dictionary, "CurrentQuest", QuestManager.Instance.GetFirstQuestID("main"));
			mLastClearedQuestIDs["main"] = TFUtils.LoadInt(dictionary, "LastClearedQuest", 0);
			NumMPGamesPlayed = TFUtils.LoadInt(dictionary, "NumMPGamesPlayed", 0);
			Stamina = TFUtils.LoadInt(dictionary, "Stamina", ParametersManager.Instance.New_Player_Max_Stamina);
			Stamina_Max = TFUtils.LoadInt(dictionary, "Stamina_Max", ParametersManager.Instance.New_Player_Max_Stamina);
			mMaxInventory = encryptValue(TFUtils.LoadInt(dictionary, "MaxInventory", ParametersManager.Instance.New_Player_Max_Inventory));
			SelectedDeck = TFUtils.LoadInt(dictionary, "SelectedDeck", 0);
			SelectedMPDeck = TFUtils.LoadInt(dictionary, "SelectedMPDeck", 0);
			NotificationEnabled = TFUtils.LoadBoolAsInt(dictionary, "NotificationEnabled", false);
			Party = TFUtils.LoadString(dictionary, "Party", null);
			PartyExpiration = TFUtils.LoadInt(dictionary, "PartyExpiration", 0);
			mNumberofTrophies = TFUtils.LoadInt(dictionary, "NumberofTrophies", 0);
			mMPPlayerName = TFUtils.LoadString(dictionary, "MPPlayerName", string.Empty);
			mChecksum = TFUtils.LoadString(dictionary, "Magic", " ");
			mGatchaKeyRewarded = TFUtils.LoadBoolAsInt(dictionary, "KeyRewarded", false);
			TOSVersionAgreedTo = TFUtils.LoadInt(dictionary, "TOSVersionAgreedTo", 0);
			PPVersionAgreedTo = TFUtils.LoadInt(dictionary, "PPVersionAgreedTo", 0);
			try
			{
				MultiplayerRank = TFUtils.TryLoadString(dictionary, "MultiplayerRank");
			}
			catch (Exception e2)
			{
				Singleton<AnalyticsManager>.Instance.LogDebug("exception_deserialize", 10);
				CrashAnalytics.LogException(e2);
				MultiplayerRank = null;
			}
			try
			{
				string s = TFUtils.LoadString(dictionary, "DailyGiftTimestamp", DateTime.MinValue.ToString());
				DailyGiftTimestamp = DateTime.Parse(s);
			}
			catch (Exception e3)
			{
				Singleton<AnalyticsManager>.Instance.LogDebug("exception_deserialize", 20);
				CrashAnalytics.LogException(e3);
				TFUtils.ErrorLog("Bad/corrupted DailyGiftTimestamp in save game.");
				DailyGiftTimestamp = TFUtils.ServerTime;
			}
			try
			{
				string s2 = TFUtils.LoadString(dictionary, "LastGiftTimestamp", DateTime.MinValue.ToString());
				LastGiftTimestamp = DateTime.Parse(s2);
			}
			catch (Exception e4)
			{
				Singleton<AnalyticsManager>.Instance.LogDebug("exception_deserialize", 20);
				CrashAnalytics.LogException(e4);
				TFUtils.ErrorLog("Bad/corrupted LastGiftTimestamp in save game.");
				LastGiftTimestamp = GetNow();
			}
			NumUsedFreeGifts = TFUtils.LoadInt(dictionary, "NumUsedFreeGifts", 0);
			try
			{
				string s3 = TFUtils.LoadString(dictionary, "LastGiftTimestamp", DateTime.MinValue.ToString());
				LastGiftTimestamp = DateTime.Parse(s3);
			}
			catch (Exception e5)
			{
				Singleton<AnalyticsManager>.Instance.LogDebug("exception_deserialize", 20);
				CrashAnalytics.LogException(e5);
				TFUtils.ErrorLog("Bad/corrupted LastGiftTimestamp in save game.");
				LastGiftTimestamp = GetNow();
			}
			bool flag = false;
			try
			{
				string s4 = TFUtils.LoadString(dictionary, "FirstCalendarTimestamp", DateTime.MinValue.ToString());
				FirstCalendarTimestamp = DateTime.Parse(s4);
				if (FirstCalendarTimestamp != DateTime.MinValue)
				{
					flag = true;
				}
			}
			catch (Exception e6)
			{
				Singleton<AnalyticsManager>.Instance.LogDebug("exception_deserialize", 20);
				CrashAnalytics.LogException(e6);
				TFUtils.ErrorLog("Bad/corrupted FirstCalendarTimestamp in save game.");
				FirstCalendarTimestamp = GetNow();
			}
			try
			{
				string s5 = TFUtils.LoadString(dictionary, "LastCalendarTimestamp", DateTime.MinValue.ToString());
				LastCalendarTimestamp = DateTime.Parse(s5);
				if (LastCalendarTimestamp != DateTime.MinValue)
				{
					flag = true;
				}
			}
			catch (Exception e7)
			{
				Singleton<AnalyticsManager>.Instance.LogDebug("exception_deserialize", 20);
				CrashAnalytics.LogException(e7);
				TFUtils.ErrorLog("Bad/corrupted LastCalendarTimestamp in save game.");
				LastCalendarTimestamp = GetNow();
			}
			uint? num = TFUtils.TryLoadUint(dictionary, "CalendarDaysClaimed");
			CalendarDaysClaimed = (num.HasValue ? num.Value : 0u);
			if (null != DebugFlagsScript.GetInstance() && DebugFlagsScript.GetInstance().resetCalendar)
			{
				TFUtils.DebugLog("DebugFlags.resetCalendar is true, resetting gift calendar data...");
				FirstCalendarTimestamp = (LastCalendarTimestamp = DateTime.MinValue);
				CalendarDaysClaimed = 0u;
			}
			if (flag)
			{
				string[] iNTRO_TUTORIALS = INTRO_TUTORIALS;
				foreach (string item in iNTRO_TUTORIALS)
				{
					if (!tutorialsCompleted.Contains(item))
					{
						tutorialsCompleted.Add(item);
					}
				}
			}
			if (dictionary.ContainsKey("LastTimestamp"))
			{
				LastTimestamp = (string)dictionary["LastTimestamp"];
				StaminaTimerController.GetInstance().Initiate(LastTimestamp);
			}
			else
			{
				StaminaTimerController.GetInstance().Initiate();
			}
			if (dictionary.ContainsKey("QuestStars"))
			{
				try
				{
					int[] array = (int[])dictionary["QuestStars"];
					QuestProgress["main"] = new Dictionary<int, int>();
					for (int j = 0; j < array.Count(); j++)
					{
						QuestProgress["main"].Add(j + 1, array[j]);
					}
				}
				catch (InvalidCastException e8)
				{
					Singleton<AnalyticsManager>.Instance.LogDebug("exception_deserialize", 30);
					CrashAnalytics.LogException(e8);
				}
			}
			if (dictionary.ContainsKey("Tags"))
			{
				try
				{
					string[] collection = (string[])dictionary["Tags"];
					Tags = new List<string>(collection);
				}
				catch (InvalidCastException)
				{
					Tags = new List<string>();
				}
			}
			else
			{
				Tags = new List<string>();
			}
			if (dictionary.ContainsKey("GachaKeys"))
			{
				try
				{
					GachaKeys.Deserialize((object[])dictionary["GachaKeys"]);
				}
				catch (InvalidCastException)
				{
				}
			}
			if (dictionary.ContainsKey("DungeonProgress"))
			{
				try
				{
					Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["DungeonProgress"];
					if (dictionary2 != null && dictionary2.Count > 0)
					{
						DungeonProgress = new Dictionary<string, int>();
						foreach (string key in dictionary2.Keys)
						{
							DungeonProgress[key] = Convert.ToInt32(dictionary2[key]);
						}
						UnlockDungeonQuests(DungeonProgress.Keys);
					}
				}
				catch (Exception e9)
				{
					Singleton<AnalyticsManager>.Instance.LogDebug("exception_deserialize", 40);
					CrashAnalytics.LogException(e9);
				}
			}
			if (dictionary.ContainsKey("OccuranceCounter"))
			{
				try
				{
					Dictionary<string, object> dictionary3 = (Dictionary<string, object>)dictionary["OccuranceCounter"];
					if (dictionary3 != null && dictionary3.Count > 0)
					{
						OccuranceCounter = new Dictionary<string, int>();
						foreach (string key2 in dictionary3.Keys)
						{
							OccuranceCounter[key2] = Convert.ToInt32(dictionary3[key2]);
						}
					}
				}
				catch (Exception e10)
				{
					Singleton<AnalyticsManager>.Instance.LogDebug("exception_deserialize", 40);
					CrashAnalytics.LogException(e10);
				}
				DebugFlagsScript instance = DebugFlagsScript.GetInstance();
				if (null != instance)
				{
					instance.FCCompleteDemo = HasCompletedFCDemo();
				}
			}
			if (dictionary.ContainsKey("AutoBattleSetting"))
			{
				AutoBattleSetting = TFUtils.LoadBool(dictionary, "AutoBattleSetting", false);
			}
			if (dictionary.ContainsKey("Tutorials"))
			{
				try
				{
					if (dictionary["Tutorials"].GetType() == typeof(string[]))
					{
						string[] collection2 = (string[])dictionary["Tutorials"];
						tutorialsCompleted = new List<string>(collection2);
					}
				}
				catch (InvalidCastException e11)
				{
					Singleton<AnalyticsManager>.Instance.LogDebug("exception_deserialize", 50);
					CrashAnalytics.LogException(e11);
				}
			}
			if (dictionary.ContainsKey("Fusion"))
			{
				FusionManager.Instance.InventoryFromDict((Dictionary<string, object>)dictionary["Fusion"]);
			}
			if (dictionary.ContainsKey("Inventory"))
			{
				DeckManager.InventoryFromDict((Dictionary<string, object>)dictionary["Inventory"]);
			}
			if (dictionary.ContainsKey("Leaders"))
			{
				LeaderManager.Instance.InventoryFromDict((Dictionary<string, object>)dictionary["Leaders"]);
			}
			else
			{
				LeaderManager.Instance.FillLeadersWithDummyData();
			}
			if (TFUtils.LoadBoolAsInt(dictionary, "HasRedeemedPig", false))
			{
				SetTag(Tag.GotCreaturePig);
			}
			if (TFUtils.LoadBoolAsInt(dictionary, "HasRedeemedSpell", false))
			{
				SetTag(Tag.GotSpellCerebral);
			}
			if (TFUtils.LoadBoolAsInt(dictionary, "HasRedeemedSpell_BriefPower", false))
			{
				SetTag(Tag.GotSpellBriefPower);
			}
			if (TFUtils.LoadBoolAsInt(dictionary, "HasRedeemedSnuggleTree", false))
			{
				SetTag(Tag.GotCreatureSnuggleTree);
			}
		}
		if (version >= 2)
		{
			mCurrentQuestIDs["fc"] = TFUtils.LoadInt(dictionary, "CurrentFCQuestID", QuestManager.Instance.GetFirstQuestID("fc"));
			mLastClearedQuestIDs["fc"] = TFUtils.LoadInt(dictionary, "LastClearedFCQuestID", 0);
			DeserializeDictionary(dictionary, QuestProgress, "main", "QuestProgress_MainIDs", "QuestProgress_MainStars");
			DeserializeDictionary(dictionary, QuestProgress, "fc", "QuestProgress_FCIDs", "QuestProgress_FCStars");
			DeserializeDictionary(dictionary, QuestProgress, "tcat", "QuestProgress_TCatIDs", "QuestProgress_TCatStars");
			DeserializeDictionary(dictionary, QuestProgress, "elfisto", "QuestProgress_ElfistoIDs", "QuestProgress_ElfistoStars");
			mQuestMatchStats["main"] = new MatchStats();
			mQuestMatchStats["main"].Attempts = TFUtils.LoadInt(dictionary, "CWAttempts", 0);
			mQuestMatchStats["main"].Wins = TFUtils.LoadInt(dictionary, "CWWins", 0);
			mQuestMatchStats["main"].Losses = TFUtils.LoadInt(dictionary, "CWLosses", 0);
			mQuestMatchStats["fc"] = new MatchStats();
			mQuestMatchStats["fc"].Attempts = TFUtils.LoadInt(dictionary, "FCAttempts", 0);
			mQuestMatchStats["fc"].Wins = TFUtils.LoadInt(dictionary, "FCWins", 0);
			mQuestMatchStats["fc"].Losses = TFUtils.LoadInt(dictionary, "FCLosses", 0);
			if (dictionary.ContainsKey("TCATAttempts"))
			{
				mQuestMatchStats["tcat"] = new MatchStats();
				mQuestMatchStats["tcat"].Attempts = TFUtils.LoadInt(dictionary, "TCATAttempts", 0);
				mQuestMatchStats["tcat"].Wins = TFUtils.LoadInt(dictionary, "TCATWins", 0);
				mQuestMatchStats["tcat"].Losses = TFUtils.LoadInt(dictionary, "TCATLosses", 0);
			}
			if (dictionary.ContainsKey("EFAttempts"))
			{
				mQuestMatchStats["elfisto"] = new MatchStats();
				mQuestMatchStats["elfisto"].Attempts = TFUtils.LoadInt(dictionary, "EFAttempts", 0);
				mQuestMatchStats["elfisto"].Wins = TFUtils.LoadInt(dictionary, "EFWins", 0);
				mQuestMatchStats["elfisto"].Losses = TFUtils.LoadInt(dictionary, "EFLosses", 0);
			}
			mQuestMapDeckIdx["main"] = TFUtils.LoadInt(dictionary, "DeckMain", 0);
			mQuestMapDeckIdx["fc"] = TFUtils.LoadInt(dictionary, "DeckFC", 0);
			try
			{
				if (!dictionary.ContainsKey("BonusQuest_QuestTypes") || !dictionary.ContainsKey("BonusQuest_FirstAppearance") || !dictionary.ContainsKey("BonusQuest_ActiveQuestIDs") || !dictionary.ContainsKey("BonusQuest_ReplacedQuestIDs") || !dictionary.ContainsKey("BonusQuest_StatsAttempts") || !dictionary.ContainsKey("BonusQuest_StatsWins") || !dictionary.ContainsKey("BonusQuest_StatsLosses") || !dictionary.ContainsKey("BonusQuest_LastPlayed"))
				{
					throw new ApplicationException("missing bonusquest keys");
				}
				string[] array2 = (string[])dictionary["BonusQuest_QuestTypes"];
				int[] array3 = (int[])dictionary["BonusQuest_FirstAppearance"];
				int[] array4 = (int[])dictionary["BonusQuest_ActiveQuestIDs"];
				int[] array5 = (int[])dictionary["BonusQuest_ReplacedQuestIDs"];
				int[] array6 = (int[])dictionary["BonusQuest_StatsAttempts"];
				int[] array7 = (int[])dictionary["BonusQuest_StatsWins"];
				int[] array8 = (int[])dictionary["BonusQuest_StatsLosses"];
				string[] array9 = (string[])dictionary["BonusQuest_LastPlayed"];
				if (array2.Length != array4.Length || array2.Length != array5.Length)
				{
					UnityEngine.Debug.LogError(string.Format("Inconsistent array lengths between {0}({1}) and {2}({3})", "BonusQuest_QuestTypes", array2.Length, "BonusQuest_ActiveQuestIDs", array4.Length));
				}
				for (int k = 0; k < array2.Count(); k++)
				{
					BonusQuests[array2[k]] = new BonusQuestStats();
					int activeQuestID = array4[k];
					BonusQuests[array2[k]].firstAppearance = Convert.ToBoolean(array3[k]);
					BonusQuests[array2[k]].ActiveQuestID = activeQuestID;
					BonusQuests[array2[k]].ReplacedQuestID = array5[k];
					try
					{
						BonusQuests[array2[k]].LastPlayedTime = Convert.ToDateTime(array9[k]);
					}
					catch (FormatException)
					{
						BonusQuests[array2[k]].LastPlayedTime = DateTime.MinValue;
					}
					BonusQuests[array2[k]].CachedMatchStats.Attempts = array6[k];
					BonusQuests[array2[k]].CachedMatchStats.Wins = array7[k];
					BonusQuests[array2[k]].CachedMatchStats.Losses = array8[k];
				}
			}
			catch (InvalidCastException e12)
			{
				Singleton<AnalyticsManager>.Instance.LogDebug("exception_deserialize", 30);
				CrashAnalytics.LogException(e12);
				BonusQuests["fc"] = new BonusQuestStats();
			}
			catch (ApplicationException)
			{
				BonusQuests["fc"] = new BonusQuestStats();
			}
		}
		if (version >= 3)
		{
			mQuestMatchStats["elfisto"] = new MatchStats();
			mQuestMatchStats["elfisto"].Attempts = TFUtils.LoadInt(dictionary, "EFAttempts", 0);
			mQuestMatchStats["elfisto"].Wins = TFUtils.LoadInt(dictionary, "EFWins", 0);
			mQuestMatchStats["elfisto"].Losses = TFUtils.LoadInt(dictionary, "EFLosses", 0);
			try
			{
				if (dictionary.ContainsKey("SideQuestManagerInfo"))
				{
					Dictionary<string, object> dictionary4 = (Dictionary<string, object>)dictionary["SideQuestManagerInfo"];
					foreach (KeyValuePair<string, object> item2 in dictionary4)
					{
						SideQuestManagerInfo sideQuestManagerInfo = new SideQuestManagerInfo();
						sideQuestManagerInfo.Deserialize(item2.Value);
						mSideQuestManagerInfo.Add(item2.Key, sideQuestManagerInfo);
					}
				}
			}
			catch (Exception)
			{
			}
		}
		if (version >= 4)
		{
			try
			{
				UnlockedRegions = new Dictionary<string, HashSet<int>>();
				if (dictionary.ContainsKey("UnlockedRegions"))
				{
					Dictionary<string, object> dictionary5 = (Dictionary<string, object>)dictionary["UnlockedRegions"];
					foreach (KeyValuePair<string, object> item3 in dictionary5)
					{
						try
						{
							int[] collection3 = (int[])dictionary5[item3.Key];
							UnlockedRegions[item3.Key] = new HashSet<int>(collection3);
						}
						catch (Exception)
						{
						}
					}
				}
			}
			catch (Exception)
			{
			}
			MPDeckLeaderID = TFUtils.LoadString(dictionary, "MPDeckLeaderID", string.Empty);
			if (string.IsNullOrEmpty(MPDeckLeaderID))
			{
				MPDeckLeaderID = DeckManager.Decks[SelectedMPDeck].Leader.Form.ID;
			}
			if (LeaderManager.Instance.IsLeaderFromFC(MPDeckLeaderID))
			{
				MPDeckLeaderID = "Leader_Jake";
			}
		}
		version = 4;
		PostDeserializeFixUpQuestProgress();
		PostDeserializeFixUpTutorialFlow();
	}

	private void PostDeserializeFixUpQuestProgress()
	{
		QuestManager instance = QuestManager.Instance;
		foreach (string questType in instance.GetQuestTypes())
		{
			if (!mCurrentQuestIDs.ContainsKey(questType) || mCurrentQuestIDs[questType] == 0)
			{
				QuestData firstQuest = instance.GetFirstQuest(questType);
				if (firstQuest != null)
				{
					mCurrentQuestIDs[questType] = firstQuest.iQuestID;
				}
			}
		}
	}

	private void PostDeserializeFixUpTutorialFlow()
	{
		string[] array = null;
		if (GetQuestProgress("main", 4) > 0)
		{
			array = TUTORIAL_FIXUP_POST_MAIN_QUEST_4;
		}
		else if (GetQuestProgress("main", 2) > 0)
		{
			array = TUTORIAL_FIXUP_POST_MAIN_QUEST_2;
		}
		if (array != null)
		{
			string[] array2 = array;
			foreach (string qname in array2)
			{
				TutorialManager.Instance.markTutorialCompleted(qname);
			}
		}
		SLOTGame.GetInstance().FixupTutorialFlow();
	}

	private void DeserializeDictionary(Dictionary<string, object> newInfo, Dictionary<string, Dictionary<int, int>> dst, string dstKey, string keys, string vals)
	{
		if (!newInfo.ContainsKey(keys) || !newInfo.ContainsKey(vals))
		{
			return;
		}
		try
		{
			dst[dstKey] = new Dictionary<int, int>();
			if (newInfo[keys].GetType() == typeof(int[]))
			{
				int[] array = (int[])newInfo[keys];
				int[] array2 = (int[])newInfo[vals];
				if (array.Length != array2.Length)
				{
					UnityEngine.Debug.LogError(string.Format("Inconsistent array lengths between {0}({1}) and {2}({3})", keys, array.Length, vals, array2.Length));
				}
				for (int i = 0; i < array.Count(); i++)
				{
					dst[dstKey].Add(array[i], array2[i]);
				}
			}
		}
		catch (InvalidCastException e)
		{
			Singleton<AnalyticsManager>.Instance.LogDebug("exception_deserialize", 30);
			CrashAnalytics.LogException(e);
		}
	}

	private IEnumerator DoRemoteSave(string targetJson, SaveCallback saveCallback)
	{
		RemoteJsonPending = targetJson;
		PlayerInfoScript playerInfoScript = this;
		playerInfoScript.RemoteSaveCallback = (SaveCallback)Delegate.Combine(playerInfoScript.RemoteSaveCallback, saveCallback);
		if (RemoteSaveInProgress)
		{
			yield break;
		}
		TFUtils.DebugLog("DoRemoteSave", "saveload");
		RemoteSaveInProgress = true;
		yield return null;
		SessionManager smgr = SessionManager.GetInstance();
		while (RemoteJsonPending != null)
		{
			string json = RemoteJsonPending;
			SaveCallback callbacks = RemoteSaveCallback;
			RemoteJsonPending = null;
			RemoteSaveCallback = null;
			TFUtils.DebugLog("RemoteJsonPending", "saveload");
			try
			{
				LastSaveTimeStamp = TFUtils.ServerTime.ToString();
				SaveTimeStamp(LastSaveTimeStamp);
				SaveToCloud(json);
				smgr.SaveToServer(json);
			}
			catch (Exception e)
			{
				TFUtils.DebugLog("remote_save_exception", "saveload");
				Singleton<AnalyticsManager>.Instance.LogDebug("remote_save_exception", 0, 0, e);
				CrashAnalytics.LogException(e);
				break;
			}
			DateTime timeOutTime = TFUtils.ServerTime + SAVE_TIMEOUT_TIME;
			while (!smgr.theSession.TheGame.SaveDone)
			{
				if (TFUtils.ServerTime >= timeOutTime)
				{
					TFUtils.DebugLog("remote_save_timeout", "saveload");
					Singleton<AnalyticsManager>.Instance.LogDebug("remote_save_timeout");
					break;
				}
				yield return null;
			}
			if (callbacks != null)
			{
				callbacks();
			}
		}
		RemoteSaveInProgress = false;
	}

	public void Save(SaveCallback saveCallback = null)
	{
		try
		{
			SessionManager instance = SessionManager.GetInstance();
			string text = Serialize();
			instance.SetGameStateJson(text);
			if (!SessionManager.GetInstance().LocalRemoteSaveGameConflict)
			{
				StartCoroutine(DoRemoteSave(text, saveCallback));
			}
			else
			{
				TFUtils.DebugLog("Only saving local due to local/remote save game conflict", "saveload");
			}
		}
		catch (Exception ex)
		{
			TFUtils.ErrorLog(ex.ToString());
			Singleton<AnalyticsManager>.Instance.LogDebug("save_exception", 0, 0, ex);
			CrashAnalytics.LogException(ex);
		}
	}

	public static void ValidateAndFixLocalSave()
	{
		try
		{
			string gameStateJson = SessionManager.GetInstance().GetGameStateJson();
			JsonReader.Deserialize<Dictionary<string, object>>(gameStateJson);
		}
		catch (FileNotFoundException)
		{
		}
		catch (Exception ex2)
		{
			TFUtils.ErrorLog("Local game data validation failed - invalidating the bad data: " + ex2.ToString());
			try
			{
				string[] array = SessionManager.GetInstance().PlayerID.Split('_');
				Singleton<AnalyticsManager>.Instance.LogDebug("corrupted_save", Convert.ToInt32(array[0]) / 8, Convert.ToInt32(array[1]));
			}
			catch (Exception)
			{
				Singleton<AnalyticsManager>.Instance.LogDebug("corrupted_save", -1, -1);
			}
			SessionManager.GetInstance().ClearSaveStateLocal();
		}
	}

	public static void Load()
	{
		g_playerInfoScript.Initialize();
	}

	private string GetDefaultGameStateJson()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		stringBuilder.Append("\"PlayerInfoVersion\":" + 4);
		stringBuilder.Append(",");
		stringBuilder.Append("\"PlayerName\":\"" + PlayerName + "\"");
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	private Dictionary<string, object> GetDefaultGameStateDictionary()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["PlayerInfoVersion"] = 4;
		dictionary["PlayerName"] = PlayerName;
		return dictionary;
	}

	public bool SaveToCloud(string json = null)
	{
		return false;
	}

	private bool CheckTimeStamp()
	{
		return false;
	}

	private bool CheckPlayer()
	{
		return false;
	}

	public void ReloadPlayer()
	{
	}

	public void ReloadGame()
	{
		if (!ReloadInProgress)
		{
			StartCoroutine(CoroutineReloadGame());
		}
	}

	private IEnumerator CoroutineReloadGame()
	{
		ReloadInProgress = true;
		float timeScalePrev = Time.timeScale;
		Time.timeScale = 0f;
		while (RemoteSaveInProgress)
		{
			yield return null;
		}
		ReloadInProgress = false;
		Time.timeScale = timeScalePrev;
		Session session = SessionManager.GetInstance().theSession;
		if (session != null)
		{
			session.ClearNeedsReload();
		}
		SceneManager.LoadScene("AppReloadScene");
	}

	public void iCloudCheckUpdate()
	{
	}

	public bool CloudSaveExists()
	{
		return false;
	}

	public static bool LoadFromCloud(CloudLoadCallback callback = null)
	{
		return false;
	}

	public bool IsAnySideQuestStarted(string questType)
	{
		if (!mSideQuestManagerInfo.ContainsKey(questType))
		{
			return false;
		}
		foreach (KeyValuePair<int, SideQuestProgress> item in mSideQuestManagerInfo[questType].SideQuestsProgress)
		{
			SideQuestProgress value = item.Value;
			if (value.State != 0)
			{
				return true;
			}
		}
		return false;
	}

	public SideQuestProgress.SideQuestState GetSideQuestState(SideQuestData sqd)
	{
		SideQuestProgress sideQuestProgress = GetSideQuestProgress(sqd);
		if (sideQuestProgress != null)
		{
			return sideQuestProgress.State;
		}
		return SideQuestProgress.SideQuestState.Inactive;
	}

	public void SetSideQuestState(SideQuestData sqd, SideQuestProgress.SideQuestState state)
	{
		if (sqd != null)
		{
			if (!mSideQuestManagerInfo.ContainsKey(sqd.QuestType))
			{
				mSideQuestManagerInfo[sqd.QuestType] = new SideQuestManagerInfo();
			}
			if (!mSideQuestManagerInfo[sqd.QuestType].SideQuestsProgress.ContainsKey(sqd.iQuestID))
			{
				mSideQuestManagerInfo[sqd.QuestType].SideQuestsProgress[sqd.iQuestID] = new SideQuestProgress();
			}
			mSideQuestManagerInfo[sqd.QuestType].SideQuestsProgress[sqd.iQuestID].State = state;
		}
	}

	public SideQuestProgress GetSideQuestProgress(SideQuestData sqd, bool createNewIfNeeded = false)
	{
		SideQuestProgress result = null;
		try
		{
			if (sqd != null)
			{
				result = mSideQuestManagerInfo[sqd.QuestType].SideQuestsProgress[sqd.iQuestID];
			}
		}
		catch (KeyNotFoundException)
		{
			if (createNewIfNeeded)
			{
				if (!mSideQuestManagerInfo.ContainsKey(sqd.QuestType))
				{
					mSideQuestManagerInfo[sqd.QuestType] = new SideQuestManagerInfo();
				}
				if (!mSideQuestManagerInfo[sqd.QuestType].SideQuestsProgress.ContainsKey(sqd.iQuestID))
				{
					mSideQuestManagerInfo[sqd.QuestType].SideQuestsProgress[sqd.iQuestID] = new SideQuestProgress();
				}
				result = mSideQuestManagerInfo[sqd.QuestType].SideQuestsProgress[sqd.iQuestID];
			}
		}
		return result;
	}

	public int IncSideQuestProgress(SideQuestData sqd, int numCollectedItems, QuestData qd)
	{
		SideQuestProgress sideQuestProgress = GetSideQuestProgress(sqd, true);
		int collected = sideQuestProgress.Collected;
		collected += numCollectedItems;
		collected = Math.Min(Math.Max(0, collected), sqd.NumCollectibles);
		sideQuestProgress.Collected = collected;
		if (qd != null)
		{
			if (!sideQuestProgress.CollectedPerQuestnode.ContainsKey(qd.iQuestID))
			{
				sideQuestProgress.CollectedPerQuestnode[qd.iQuestID] = numCollectedItems;
			}
			else
			{
				Dictionary<int, int> collectedPerQuestnode;
				Dictionary<int, int> dictionary = (collectedPerQuestnode = sideQuestProgress.CollectedPerQuestnode);
				int iQuestID;
				int key = (iQuestID = qd.iQuestID);
				iQuestID = collectedPerQuestnode[iQuestID];
				dictionary[key] = iQuestID + numCollectedItems;
			}
		}
		return sideQuestProgress.Collected;
	}

	public void IncSideQuestLandScapeCountProgress(SideQuestData sqd, Dictionary<LandscapeType, int> landscapeCount, QuestData qd)
	{
		SideQuestProgress sideQuestProgress = GetSideQuestProgress(sqd, true);
		foreach (KeyValuePair<LandscapeType, int> item in landscapeCount)
		{
			if (!sideQuestProgress.DeployedLandscapeCount.ContainsKey(item.Key))
			{
				sideQuestProgress.DeployedLandscapeCount[item.Key] = item.Value;
				continue;
			}
			Dictionary<LandscapeType, int> deployedLandscapeCount;
			Dictionary<LandscapeType, int> dictionary = (deployedLandscapeCount = sideQuestProgress.DeployedLandscapeCount);
			LandscapeType key;
			LandscapeType key2 = (key = item.Key);
			int num = deployedLandscapeCount[key];
			dictionary[key2] = num + item.Value;
		}
	}

	public int GetQuestProgress(string questType, int questID)
	{
		return GetQuestProgress(QuestManager.Instance.GetQuestByID(questType, questID));
	}

	public int GetQuestProgress(QuestData qd)
	{
		if (qd == null)
		{
			return 0;
		}
		if (QuestProgress.ContainsKey(qd.QuestType) && QuestProgress[qd.QuestType].ContainsKey(qd.iQuestID))
		{
			return QuestProgress[qd.QuestType][qd.iQuestID];
		}
		return 0;
	}

	public int IncQuestProgress(QuestData qd, int maxProgress = 3)
	{
		if (qd == null)
		{
			return 0;
		}
		if (!QuestProgress.ContainsKey(qd.QuestType))
		{
			QuestProgress[qd.QuestType] = new Dictionary<int, int>();
		}
		int num = 0;
		if (QuestProgress[qd.QuestType].ContainsKey(qd.iQuestID))
		{
			num = QuestProgress[qd.QuestType][qd.iQuestID];
		}
		num = Math.Max(0, Math.Min(maxProgress, num + 1));
		QuestProgress[qd.QuestType][qd.iQuestID] = num;
		return num;
	}

	public void ResetQuestProgress(QuestData qd)
	{
		SetQuestProgress(qd, 0);
	}

	public int SetQuestProgress(QuestData qd, int numStars, int maxProgress = 3)
	{
		if (qd == null)
		{
			return 0;
		}
		if (!QuestProgress.ContainsKey(qd.QuestType))
		{
			QuestProgress.Add(qd.QuestType, new Dictionary<int, int>());
		}
		int num = Math.Max(0, Math.Min(maxProgress, numStars));
		QuestProgress[qd.QuestType][qd.iQuestID] = num;
		return num;
	}

	public void ResetQuestProgress(string questType)
	{
		QuestProgress[questType] = new Dictionary<int, int>();
	}

	public QuestData GetCurrentQuest()
	{
		return GetCurrentQuest(mCurrentQuestType);
	}

	public QuestData GetCurrentQuest(string questType)
	{
		QuestData questData = null;
		if (string.IsNullOrEmpty(questType))
		{
			questType = "main";
		}
		if (mCurrentQuestIDs.ContainsKey(questType))
		{
			int questID = mCurrentQuestIDs[questType];
			questData = QuestManager.Instance.GetQuestByID(questType, questID);
		}
		if (questData == null)
		{
			questData = QuestManager.Instance.GetFirstQuest(questType);
			if (questData != null)
			{
				UpdateCurrentQuestID(questType, questData.iQuestID);
			}
		}
		return questData;
	}

	public void SetCurrentQuest(QuestData quest)
	{
		if (quest != null)
		{
			mCurrentQuestType = quest.QuestType;
			mCurrentQuestIDs[quest.QuestType] = quest.iQuestID;
		}
	}

	public int GetCurrentQuestID()
	{
		return GetCurrentQuestID(mCurrentQuestType);
	}

	public int GetCurrentQuestID(string questType)
	{
		if (mCurrentQuestIDs.ContainsKey(questType))
		{
			return mCurrentQuestIDs[questType];
		}
		return 0;
	}

	public void UpdateCurrentQuestID(string questType, int questID)
	{
		if (!string.IsNullOrEmpty(questType))
		{
			mCurrentQuestIDs[questType] = questID;
		}
	}

	public void SetLastClearedQuest(QuestData quest)
	{
		if (quest != null)
		{
			SetLastClearedQuestID(quest.QuestType, quest.iQuestID);
		}
	}

	public void SetLastClearedQuestID(string questType, int questID)
	{
		mLastClearedQuestIDs[questType] = questID;
	}

	public void ResetLastClearedQuestID(string questType)
	{
		mLastClearedQuestIDs[questType] = -1;
	}

	public QuestData GetLastClearedQuest()
	{
		return GetLastClearedQuest(mCurrentQuestType);
	}

	public QuestData GetLastClearedQuest(string questType)
	{
		if (mLastClearedQuestIDs.ContainsKey(questType))
		{
			int questID = mLastClearedQuestIDs[questType];
			return QuestManager.Instance.GetQuestByID(questType, questID);
		}
		return null;
	}

	public int GetLastClearedQuestID()
	{
		return GetLastClearedQuestID(mCurrentQuestType);
	}

	public int GetLastClearedQuestID(string questType)
	{
		if (mLastClearedQuestIDs.ContainsKey(questType))
		{
			return mLastClearedQuestIDs[questType];
		}
		return 0;
	}

	public void StartMatch(QuestData qd, int staminaCost)
	{
		Stamina -= staminaCost;
		LastTimestamp = TFUtils.ServerTime.ToString();
		if (Stamina + staminaCost == Stamina_Max)
		{
			StaminaTimerController.GetInstance().Initiate(LastTimestamp);
		}
		GetQuestMatchStats(qd.QuestType).Attempts++;
		SetCurrentQuest(qd);
		LastPlayedQuest = qd;
		CurrentGameID++;
		try
		{
			if (qd.iQuestID == BonusQuests[MapControllerBase.GetInstance().MapQuestType].ActiveQuest.iQuestID)
			{
				BonusQuests[MapControllerBase.GetInstance().MapQuestType].LastPlayedTime = TFUtils.ServerTime;
			}
		}
		catch (Exception)
		{
		}
	}

	public int GetCurrentGameID()
	{
		return CurrentGameID;
	}

	public MatchStats GetQuestMatchStats(string questType)
	{
		if (!mQuestMatchStats.ContainsKey(questType))
		{
			mQuestMatchStats.Add(questType, new MatchStats());
		}
		return mQuestMatchStats[questType];
	}

	public MatchStats GetSideQuestMatchStats(string questType)
	{
		if (!mSideQuestManagerInfo.ContainsKey(questType))
		{
			mSideQuestManagerInfo.Add(questType, new SideQuestManagerInfo());
		}
		return mSideQuestManagerInfo[questType].matchlapse;
	}

	public int GetSideQuestRequiredMatchLapse(string questType)
	{
		if (!mSideQuestManagerInfo.ContainsKey(questType))
		{
			mSideQuestManagerInfo.Add(questType, new SideQuestManagerInfo());
		}
		return mSideQuestManagerInfo[questType].RequiredMatchLapse;
	}

	public void CacheSideQuestMatchStats(SideQuestData sqd)
	{
		if (!mSideQuestManagerInfo.ContainsKey(sqd.QuestType))
		{
			mSideQuestManagerInfo.Add(sqd.QuestType, new SideQuestManagerInfo());
		}
		MatchStats questMatchStats = GetQuestMatchStats(sqd.QuestType);
		mSideQuestManagerInfo[sqd.QuestType].matchlapse.Copy(questMatchStats);
		mSideQuestManagerInfo[sqd.QuestType].RequiredMatchLapse = sqd.MatchLapse;
	}

	public TimeSpan GetBonusQuestTimeLapse(string questType)
	{
		try
		{
			return TFUtils.ServerTime - BonusQuests[questType].LastPlayedTime;
		}
		catch (Exception)
		{
			return TimeSpan.Zero;
		}
	}

	public int GetBonusQuestMatchLapse(string questType)
	{
		try
		{
			return mQuestMatchStats[questType].Completed - BonusQuests[questType].CachedMatchStats.Completed;
		}
		catch (Exception)
		{
		}
		return 0;
	}

	public void UpdateBonusQuestMatchStats(string questType)
	{
		try
		{
			BonusQuests[questType].CachedMatchStats.Attempts = mQuestMatchStats[questType].Attempts;
			BonusQuests[questType].CachedMatchStats.Wins = mQuestMatchStats[questType].Wins;
			BonusQuests[questType].CachedMatchStats.Losses = mQuestMatchStats[questType].Losses;
		}
		catch (Exception)
		{
		}
	}

	public void SetCurrentMap(string mapQuestType)
	{
		CurrentMapType = mapQuestType;
	}

	public void SelectDeckForMap()
	{
		try
		{
			SelectedDeck = mQuestMapDeckIdx[CurrentMapType];
		}
		catch (KeyNotFoundException)
		{
			mQuestMapDeckIdx[CurrentMapType] = SelectedDeck;
		}
	}

	private int Shuffle(int x)
	{
		int num = (x ^ (x >> 8)) & 0xFF00;
		x = x ^ num ^ (num << 8);
		num = (x ^ (x >> 4)) & 0xF000F0;
		x = x ^ num ^ (num << 4);
		num = (x ^ (x >> 2)) & 0xC0C0C0C;
		x = x ^ num ^ (num << 2);
		num = (x ^ (x >> 1)) & 0x22222222;
		x = x ^ num ^ (num << 1);
		x ^= 0x55555555;
		return x;
	}

	private int Unshuffle(int x)
	{
		x ^= 0x55555555;
		int num = (x ^ (x >> 1)) & 0x22222222;
		x = x ^ num ^ (num << 1);
		num = (x ^ (x >> 2)) & 0xC0C0C0C;
		x = x ^ num ^ (num << 2);
		num = (x ^ (x >> 4)) & 0xF000F0;
		x = x ^ num ^ (num << 4);
		num = (x ^ (x >> 8)) & 0xFF00;
		x = x ^ num ^ (num << 8);
		return x;
	}

	private string encryptValue(int value)
	{
		try
		{
			value = Shuffle(value);
			return mCrypto.Encrypt(value.ToString());
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}

	private int decryptValue(string str)
	{
		try
		{
			string s = mCrypto.Decrypt(str);
			int result;
			if (!int.TryParse(s, out result))
			{
				return 0;
			}
			return Unshuffle(result);
		}
		catch (Exception)
		{
			return 0;
		}
	}
}
