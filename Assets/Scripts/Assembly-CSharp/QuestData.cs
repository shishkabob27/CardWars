#define ASSERTS_ON
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class QuestData
{
	public enum QuestState
	{
		LOCKED,
		PLAYABLE
	}

	public enum LeaderRestrictionRules
	{
		ALLOW,
		NOTALLOW,
		NONE
	}

	private QuestState State;

	public string OpponentDeckID;

	public string[] Condition;

	public List<List<string>> UnlockPaths = new List<List<string>>();

	public List<List<int>> UnlockQuestIDs = new List<List<int>>();

	public List<string> CardDrops = new List<string>();

	public string LevelPrefab;

	public string LevelName;

	public string TablePrefab;

	public string ChairPrefab;

	public string UnlockRegion;

	public string ResultLootChestPrefab;

	public string NodePrefabPath;

	public string LoadingScreenTextureName;

	public string QuestID { get; private set; }

	public int iQuestID { get; private set; }

	public int iQuestIndex { get; private set; }

	public string QuestLabel { get; private set; }

	public int RegionID { get; set; }

	public string QuestType { get; private set; }

	public CharacterData Opponent { get; set; }

	public string LeaderID { get; set; }

	public int LeaderLevel { get; set; }

	public int MinXP { get; set; }

	public int MaxXP { get; set; }

	public int XPRewarded { get; set; }

	public int StaminaAwarded { get; set; }

	public string LeaderAwarded { get; set; }

	public int StaminaCost { get; set; }

	public int MinCoins { get; set; }

	public int MaxCoins { get; set; }

	public int CoinsRewarded { get; set; }

	public string[] RestrictedLeaders { get; set; }

	public LeaderRestrictionRules LeaderRestriction { get; set; }

	public TutorialTrigger LeaderErrTrigger { get; set; }

	public string DropProfileID { get; set; }

	public string NisPreLaunch { get; private set; }

	public string NisPreBattle { get; private set; }

	public string NisWinPostBattle { get; private set; }

	public string NisWinPostReturn { get; private set; }

	public bool NisPlayAlways { get; private set; }

	public string OverridePlayerLeaderId { get; private set; }

	public int OverridePlayerLeaderLevel { get; private set; }

	public string OverridePlayerDeckId { get; private set; }

	public string CustomPrefab { get; private set; }

	public int MaxLootDrops { get; set; }

	public int StaticDrops { get; set; }

	public bool AIReshuffle { get; set; }

	public float SpinFactor { get; set; }

	public float P1_HitAreaMod { get; set; }

	public float P1_CritAreaMod { get; set; }

	public float P2_HitAreaMod { get; set; }

	public float P2_CritAreaMod { get; set; }

	public bool UseNewCardDropSystem
	{
		get
		{
			return null != DropProfileDataManager.Instance.GetDropProfile(DropProfileID);
		}
	}

	public bool IsQuestType(string qt)
	{
		return QuestType.Equals(qt, StringComparison.InvariantCultureIgnoreCase);
	}

	public static QuestData CreateQuestData(Dictionary<string, object> dict)
	{
		string text = TFUtils.LoadString(dict, "QuestID");
		int result = -1;
		if (int.TryParse(text, out result))
		{
			QuestData questData = new QuestData();
			questData.QuestID = text;
			questData.iQuestID = result;
			questData.QuestType = TFUtils.LoadString(dict, "QuestType", "<undefined>");
			questData.RegionID = -1;
			return questData;
		}
		return null;
	}

	public void Populate(Dictionary<string, object> dict, int questIndex)
	{
		iQuestIndex = questIndex;
		QuestLabel = TFUtils.LoadString(dict, "QuestLabel", string.Empty);
		if (string.IsNullOrEmpty(QuestLabel))
		{
			QuestLabel = QuestID;
		}
		else
		{
			QuestLabel = KFFLocalization.Get(QuestLabel);
		}
		Opponent = CharacterDataManager.Instance.GetCharacterData(TFUtils.LoadString(dict, "Opponent"));
		OpponentDeckID = TFUtils.LoadString(dict, "Deck");
		LeaderID = TFUtils.LoadString(dict, "LeaderID");
		LeaderLevel = TFUtils.LoadInt(dict, "LeaderLevel", 1);
		MinXP = TFUtils.LoadInt(dict, "MinXP", 0);
		MaxXP = TFUtils.LoadInt(dict, "MaxXP", 0);
		StaminaAwarded = TFUtils.LoadInt(dict, "StaminaAwarded", 0);
		LeaderAwarded = TFUtils.LoadString(dict, "LeaderAwarded");
		StaminaCost = TFUtils.LoadInt(dict, "Stamina", 0);
		MinCoins = TFUtils.LoadInt(dict, "MinCoins", 0);
		MaxCoins = TFUtils.LoadInt(dict, "MaxCoins", 0);
		LevelPrefab = TFUtils.LoadString(dict, "LevelPrefab");
		LevelName = TFUtils.LoadLocalizedString(dict, "LevelName");
		TablePrefab = TFUtils.LoadString(dict, "TablePrefab");
		ChairPrefab = TFUtils.LoadString(dict, "ChairPrefab");
		NodePrefabPath = TFUtils.LoadString(dict, "NodePrefabPath", string.Empty);
		Condition = new string[3];
		Condition[0] = TFUtils.LoadString(dict, "Condition1", string.Empty);
		Condition[1] = TFUtils.LoadString(dict, "Condition2", string.Empty);
		Condition[2] = TFUtils.LoadString(dict, "Condition3", string.Empty);
		MaxLootDrops = TFUtils.LoadInt(dict, "MaxLootDrops", 0);
		StaticDrops = TFUtils.LoadInt(dict, "StaticDrops", 0);
		AIReshuffle = TFUtils.LoadBool(dict, "AIReshuffle", true);
		SpinFactor = TFUtils.LoadFloat(dict, "SpinFactor", 1f);
		P1_HitAreaMod = TFUtils.LoadFloat(dict, "P1_HitAreaMod", 0f);
		P1_CritAreaMod = TFUtils.LoadFloat(dict, "P1_CritAreaMod", 0f);
		P2_HitAreaMod = TFUtils.LoadFloat(dict, "P2_HitAreaMod", 0f);
		P2_CritAreaMod = TFUtils.LoadFloat(dict, "P2_CritAreaMod", 0f);
		NisPreLaunch = null;
		NisPreBattle = null;
		NisWinPostBattle = null;
		NisWinPostReturn = null;
		OverridePlayerLeaderId = null;
		OverridePlayerLeaderLevel = -1;
		OverridePlayerDeckId = null;
		CustomPrefab = null;
		for (int i = 0; i < 3; i++)
		{
			List<string> list = new List<string>();
			string text = TFUtils.LoadString(dict, string.Format("UnlockPaths{0}", i + 1), string.Empty);
			string[] array = text.Split(',');
			for (int j = 0; j < array.Length; j++)
			{
				if (!string.IsNullOrEmpty(array[j]))
				{
					list.Add(array[j]);
				}
			}
			UnlockPaths.Add(list);
			List<int> list2 = new List<int>();
			text = TFUtils.LoadString(dict, string.Format("UnlockQuestIDs{0}", i + 1), "0");
			array = text.Split(',');
			if (array.Length > 0)
			{
				for (int k = 0; k < array.Length; k++)
				{
					int result = 0;
					if (int.TryParse(array[k], out result))
					{
						list2.Add(result);
					}
				}
			}
			UnlockQuestIDs.Add(list2);
		}
		DropProfileID = TFUtils.TryLoadString(dict, "DropProfileID");
		if (!string.IsNullOrEmpty(DropProfileID))
		{
			int num = 1;
			string text2 = null;
			while (!string.IsNullOrEmpty(text2 = TFUtils.TryLoadString(dict, "CardDrop" + num)))
			{
				CardDrops.Add(text2);
				num++;
			}
		}
		string value = TFUtils.LoadString(dict, "LeaderRules", "NONE").Trim().ToUpper();
		if (Enum.IsDefined(typeof(LeaderRestrictionRules), value))
		{
			LeaderRestriction = (LeaderRestrictionRules)(int)Enum.Parse(typeof(LeaderRestrictionRules), value);
		}
		else
		{
			LeaderRestriction = LeaderRestrictionRules.NONE;
		}
		string text3 = TFUtils.LoadString(dict, "RestrictedLeaders", string.Empty);
		string[] array2 = text3.Trim().Split(',');
		if (array2.Length > 0)
		{
			RestrictedLeaders = new string[array2.Length];
			for (int l = 0; l < array2.Length; l++)
			{
				if (!string.IsNullOrEmpty(array2[l]))
				{
					RestrictedLeaders[l] = array2[l].Trim();
				}
			}
		}
		string value2 = TFUtils.LoadString(dict, "LeaderErrTrigger", "None").Trim();
		if (Enum.IsDefined(typeof(TutorialTrigger), value2))
		{
			LeaderErrTrigger = (TutorialTrigger)(int)Enum.Parse(typeof(TutorialTrigger), value2);
		}
		else
		{
			LeaderErrTrigger = TutorialTrigger.None;
		}
	}

	public void FillQuestDataExtra(Dictionary<string, object> dict)
	{
		NisPreLaunch = TFUtils.LoadString(dict, "StartNisPreLaunch", string.Empty).Trim();
		if (NisPreLaunch.Length <= 0)
		{
			NisPreLaunch = null;
		}
		NisPreBattle = TFUtils.LoadString(dict, "StartNisPreBattle", string.Empty).Trim();
		if (NisPreBattle.Length <= 0)
		{
			NisPreBattle = null;
		}
		NisWinPostBattle = TFUtils.LoadString(dict, "EndNisPreReturn", string.Empty).Trim();
		if (NisWinPostBattle.Length <= 0)
		{
			NisWinPostBattle = null;
		}
		NisWinPostReturn = TFUtils.LoadString(dict, "EndNisPostReturn", string.Empty).Trim();
		if (NisWinPostReturn.Length <= 0)
		{
			NisWinPostReturn = null;
		}
		NisPlayAlways = TFUtils.LoadBool(dict, "NisPlayAlways", false);
		OverridePlayerLeaderId = TFUtils.LoadString(dict, "PlayerLeader", string.Empty).Trim();
		if (OverridePlayerLeaderId.Length <= 0)
		{
			OverridePlayerLeaderId = null;
		}
		OverridePlayerLeaderLevel = TFUtils.LoadInt(dict, "PlayerLeaderLevel", -1);
		OverridePlayerDeckId = TFUtils.LoadString(dict, "PlayerDeck", string.Empty).Trim();
		if (OverridePlayerDeckId.Length <= 0)
		{
			OverridePlayerDeckId = null;
		}
		string text2 = (CustomPrefab = TFUtils.LoadString(dict, "CustomPrefab", null));
		if (text2 != null)
		{
			CustomPrefab = CustomPrefab.Trim();
		}
		ResultLootChestPrefab = TFUtils.LoadString(dict, "ResultLootChestPrefab", null);
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

	public string GetConditionDescription(int num)
	{
		if (num < 0 || num >= Condition.Length)
		{
			return null;
		}
		return QuestConditionManager.Instance.ConditionDescription(Condition[num]);
	}

	public GameObject LoadCustomPrefab()
	{
		if (!string.IsNullOrEmpty(CustomPrefab))
		{
			GameObject gameObject = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource(CustomPrefab) as GameObject;
			TFUtils.Assert(null != gameObject, "Can't LoadResource: " + CustomPrefab);
			return gameObject;
		}
		return null;
	}

	public QuestState GetState()
	{
		return State;
	}

	public void SetState(QuestState newState)
	{
		State = newState;
	}

	public int GetProgress()
	{
		return PlayerInfoScript.GetInstance().GetQuestProgress(this);
	}

	public CardItem GetWeightedCard()
	{
		if (!UseNewCardDropSystem)
		{
			return null;
		}
		DropProfile dropProfile = DropProfileDataManager.Instance.GetDropProfile(DropProfileID);
		WeightedList<CardItem> weightedList = new WeightedList<CardItem>();
		int num = 0;
		foreach (string cardDrop in CardDrops)
		{
			CardForm card = CardDataManager.Instance.GetCard(cardDrop);
			if (card != null)
			{
				CardItem item = new CardItem(card);
				int cardWeightAtIndex = dropProfile.GetCardWeightAtIndex(num);
				weightedList.Add(item, cardWeightAtIndex);
			}
			num++;
		}
		return (!weightedList.IsEmpty()) ? weightedList.RandomItem() : null;
	}

	public int GetWeightedCoins()
	{
		if (!UseNewCardDropSystem)
		{
			return 0;
		}
		DropProfile dropProfile = DropProfileDataManager.Instance.GetDropProfile(DropProfileID);
		WeightedList<int> weightedList = new WeightedList<int>();
		for (int i = 0; i < dropProfile.CoinAmounts.Count && i < dropProfile.CoinWeights.Count; i++)
		{
			int item = dropProfile.CoinAmounts[i];
			int weight = dropProfile.CoinWeights[i];
			weightedList.Add(item, weight);
		}
		return (!weightedList.IsEmpty()) ? weightedList.RandomItem() : 0;
	}

	public QuestData ShallowClone()
	{
		QuestData questData = new QuestData();
		questData.State = State;
		questData.QuestID = QuestID;
		questData.iQuestID = iQuestID;
		questData.iQuestIndex = iQuestIndex;
		questData.QuestLabel = QuestLabel;
		questData.RegionID = RegionID;
		questData.QuestType = QuestType;
		questData.Opponent = Opponent;
		questData.OpponentDeckID = OpponentDeckID;
		questData.LeaderID = LeaderID;
		questData.LeaderLevel = LeaderLevel;
		questData.MinXP = MinXP;
		questData.MaxXP = MaxXP;
		questData.XPRewarded = XPRewarded;
		questData.StaminaAwarded = StaminaAwarded;
		questData.LeaderAwarded = LeaderAwarded;
		questData.StaminaCost = StaminaCost;
		questData.MinCoins = MinCoins;
		questData.MaxCoins = MaxCoins;
		questData.CoinsRewarded = CoinsRewarded;
		questData.Condition = Condition;
		questData.UnlockPaths = UnlockPaths;
		questData.UnlockQuestIDs = UnlockQuestIDs;
		questData.DropProfileID = DropProfileID;
		questData.CardDrops = CardDrops;
		questData.LevelPrefab = LevelPrefab;
		questData.LevelName = LevelName;
		questData.TablePrefab = TablePrefab;
		questData.ChairPrefab = ChairPrefab;
		questData.ResultLootChestPrefab = ResultLootChestPrefab;
		questData.NodePrefabPath = NodePrefabPath;
		questData.NisPreLaunch = NisPreLaunch;
		questData.NisPreBattle = NisPreBattle;
		questData.NisWinPostBattle = NisWinPostBattle;
		questData.NisWinPostReturn = NisWinPostReturn;
		questData.NisPlayAlways = NisPlayAlways;
		questData.OverridePlayerLeaderId = OverridePlayerLeaderId;
		questData.OverridePlayerLeaderLevel = OverridePlayerLeaderLevel;
		questData.OverridePlayerDeckId = OverridePlayerDeckId;
		questData.CustomPrefab = CustomPrefab;
		questData.MaxLootDrops = MaxLootDrops;
		questData.StaticDrops = StaticDrops;
		questData.AIReshuffle = AIReshuffle;
		questData.SpinFactor = SpinFactor;
		questData.P1_HitAreaMod = P1_HitAreaMod;
		questData.P1_CritAreaMod = P1_CritAreaMod;
		questData.P2_HitAreaMod = P2_HitAreaMod;
		questData.P2_CritAreaMod = P2_CritAreaMod;
		questData.LeaderRestriction = LeaderRestriction;
		questData.RestrictedLeaders = RestrictedLeaders;
		questData.LeaderErrTrigger = LeaderErrTrigger;
		return questData;
	}
}
