using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SideQuestManager : ILoadable
{
	private static SideQuestManager ms_instance;

	public string dbSideQuestsJsonFileName = "db_SideQuests.json";

	public Dictionary<string, List<SideQuestData>> sideQuests = new Dictionary<string, List<SideQuestData>>();

	private Dictionary<string, Dictionary<int, int>> sideQuestIDToIndexMap;

	private Dictionary<int, int> PrerequisiteQuestID = new Dictionary<int, int>();

	private Dictionary<string, SideQuestData> CurrentQuest = new Dictionary<string, SideQuestData>();

	private Dictionary<string, List<SideQuestData>> ActiveSideQuests = new Dictionary<string, List<SideQuestData>>();

	private Dictionary<string, UnityEngine.Object> mCollectiblesPrefabDict = new Dictionary<string, UnityEngine.Object>();

	public static SideQuestManager Instance
	{
		get
		{
			if (ms_instance == null)
			{
				ms_instance = new SideQuestManager();
			}
			return ms_instance;
		}
	}

	public IEnumerator Load()
	{
		Dictionary<string, object>[] data = SQUtils.ReadJSONData(dbSideQuestsJsonFileName, false);
		if (LoadingManager.ShouldYield())
		{
			yield return null;
		}
		sideQuests = new Dictionary<string, List<SideQuestData>>();
		sideQuestIDToIndexMap = new Dictionary<string, Dictionary<int, int>>();
		Dictionary<string, object>[] array = data;
		foreach (Dictionary<string, object> row in array)
		{
			SideQuestData sq = SideQuestData.Create(row);
			if (sq != null)
			{
				if (sq.UnlockQuestIDs != null)
				{
					for (int i = 0; i < sq.UnlockQuestIDs.Count; i++)
					{
						int id = sq.UnlockQuestIDs[i];
						PrerequisiteQuestID[id] = sq.iQuestID;
					}
				}
				if (!sideQuests.ContainsKey(sq.QuestType))
				{
					sideQuests[sq.QuestType] = new List<SideQuestData>();
					sideQuestIDToIndexMap[sq.QuestType] = new Dictionary<int, int>();
				}
				sideQuests[sq.QuestType].Add(sq);
				sideQuestIDToIndexMap[sq.QuestType][sq.iQuestID] = sideQuests[sq.QuestType].Count - 1;
			}
			if (LoadingManager.ShouldYield())
			{
				yield return null;
			}
		}
	}

	public void Destroy()
	{
		ms_instance = null;
	}

	public List<SideQuestData> GetSideQuests(string questType)
	{
		try
		{
			return sideQuests[questType];
		}
		catch (KeyNotFoundException)
		{
		}
		return new List<SideQuestData>();
	}

	public List<SideQuestData> GetSideQuests(string questType, string groupName)
	{
		List<SideQuestData> list = GetSideQuests(questType);
		return list.FindAll((SideQuestData sqd) => sqd.Group == groupName);
	}

	public List<string> GetSideQuestGroups(string questType)
	{
		HashSet<string> hashSet = new HashSet<string>();
		List<SideQuestData> list = GetSideQuests(questType);
		for (int i = 0; i < list.Count; i++)
		{
			hashSet.Add(list[i].Group);
		}
		return new List<string>(hashSet);
	}

	public List<SideQuestData> GetActiveSideQuests(string questType)
	{
		try
		{
			return ActiveSideQuests[questType];
		}
		catch (KeyNotFoundException)
		{
		}
		return new List<SideQuestData>();
	}

	public SideQuestData GetActiveSideQuest(string questType, string sqGroup)
	{
		try
		{
			List<SideQuestData> list = ActiveSideQuests[questType];
			return list.Find((SideQuestData sqd) => sqd.Group == sqGroup);
		}
		catch (KeyNotFoundException)
		{
		}
		catch (NullReferenceException)
		{
		}
		return null;
	}

	public void ActivateSideQuest(SideQuestData sqd)
	{
		if (sqd != null)
		{
			if (!ActiveSideQuests.ContainsKey(sqd.QuestType))
			{
				ActiveSideQuests[sqd.QuestType] = new List<SideQuestData>();
			}
			ActiveSideQuests[sqd.QuestType].Add(sqd);
		}
	}

	public void DeactivateSideQuest(SideQuestData sqd)
	{
		if (sqd != null && ActiveSideQuests.ContainsKey(sqd.QuestType))
		{
			ActiveSideQuests[sqd.QuestType].Remove(sqd);
		}
	}

	public SideQuestData GetSideQuestById(string questType, int id)
	{
		try
		{
			int index = sideQuestIDToIndexMap[questType][id];
			return sideQuests[questType][index];
		}
		catch (KeyNotFoundException)
		{
		}
		catch (IndexOutOfRangeException)
		{
		}
		return null;
	}

	public SideQuestProgress.SideQuestState SetSideQuestState(SideQuestData sqd, SideQuestProgress.SideQuestState state)
	{
		if (sqd == null)
		{
			return SideQuestProgress.SideQuestState.Inactive;
		}
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		SideQuestProgress sideQuestProgress = instance.GetSideQuestProgress(sqd, true);
		sideQuestProgress.State = state;
		switch (state)
		{
		case SideQuestProgress.SideQuestState.Complete:
		case SideQuestProgress.SideQuestState.Failed:
			sideQuestProgress.CollectedPerQuestnode.Clear();
			instance.CacheSideQuestMatchStats(sqd);
			ClearSideQuestDropItem(sqd);
			instance.Save();
			UpdatePlayableSideQuests(sqd);
			break;
		default:
			instance.Save();
			break;
		case SideQuestProgress.SideQuestState.Pending:
			break;
		}
		return state;
	}

	public SideQuestProgress.SideQuestState IntroduceSideQuest(SideQuestData sqd)
	{
		return SetSideQuestState(sqd, SideQuestProgress.SideQuestState.Pending);
	}

	public SideQuestProgress.SideQuestState StartSideQuest(SideQuestData sqd)
	{
		return SetSideQuestState(sqd, SideQuestProgress.SideQuestState.InProgress);
	}

	public SideQuestProgress.SideQuestState CompleteSideQuest(SideQuestData sqd)
	{
		DeactivateSideQuest(sqd);
		return SetSideQuestState(sqd, SideQuestProgress.SideQuestState.Complete);
	}

	public SideQuestProgress.SideQuestState FailSideQuest(SideQuestData sqd)
	{
		DeactivateSideQuest(sqd);
		return SetSideQuestState(sqd, SideQuestProgress.SideQuestState.Failed);
	}

	public SideQuestProgress.SideQuestState AccomplishSideQuest(SideQuestData sqd)
	{
		return SetSideQuestState(sqd, SideQuestProgress.SideQuestState.Accomplished);
	}

	public SideQuestProgress.SideQuestState ExpireSideQuest(SideQuestData sqd)
	{
		return SetSideQuestState(sqd, SideQuestProgress.SideQuestState.Expired);
	}

	public void ResetSideQuest(SideQuestData sqd)
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		SideQuestProgress sideQuestProgress = instance.GetSideQuestProgress(sqd, true);
		sideQuestProgress.Reset();
		ClearSideQuestDropItem(sqd);
	}

	public List<SideQuestData> GetInitialSideQuests(string questType)
	{
		List<SideQuestData> list = new List<SideQuestData>();
		if (sideQuests.ContainsKey(questType))
		{
			List<SideQuestData> list2 = sideQuests[questType];
			for (int i = 0; i < list2.Count; i++)
			{
				int iQuestID = list2[i].iQuestID;
				if (!PrerequisiteQuestID.ContainsKey(iQuestID))
				{
					list.Add(list2[i]);
				}
			}
		}
		return list;
	}

	public void UpdatePlayableSideQuests(QuestData qd, string sideQuestGroup)
	{
		List<SideQuestData> list = new List<SideQuestData>();
		foreach (string key in sideQuests.Keys)
		{
			List<SideQuestData> list2 = sideQuests[key];
			for (int i = 0; i < list2.Count; i++)
			{
				SideQuestData sideQuestData = list2[i];
				if (!sideQuestData.IsPlayable && sideQuestData.Group == sideQuestGroup && sideQuestData.PrerequisiteMapQuestID.HasValue && sideQuestData.PrerequisiteMapQuestID.Value == qd.iQuestID)
				{
					list.Add(sideQuestData);
				}
			}
		}
		if (list.Count > 0)
		{
			UpdatePlayableSideQuests(list);
		}
	}

	private void UpdatePlayableSideQuests(SideQuestData sqd)
	{
		List<SideQuestData> list = new List<SideQuestData>(1);
		list.Add(sqd);
		List<SideQuestData> initialQuestList = list;
		UpdatePlayableSideQuests(initialQuestList);
	}

	private void UpdatePlayableSideQuests(List<SideQuestData> initialQuestList)
	{
		List<SideQuestData> list = initialQuestList;
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		HashSet<int> hashSet = new HashSet<int>();
		while (list.Count > 0)
		{
			List<SideQuestData> list2 = new List<SideQuestData>();
			for (int i = 0; i < list.Count; i++)
			{
				SideQuestData sideQuestData = list[i];
				if (hashSet.Contains(sideQuestData.iQuestID))
				{
					continue;
				}
				hashSet.Add(sideQuestData.iQuestID);
				bool flag = true;
				bool flag2 = false;
				if (sideQuestData.PrerequisiteMapQuestID.HasValue)
				{
					QuestData quest = QuestManager.Instance.GetQuest(sideQuestData.PrerequisiteMapQuestID.Value);
					if (quest != null)
					{
						int questProgress = instance.GetQuestProgress(quest);
						if (questProgress < 1 || quest.GetState() == QuestData.QuestState.LOCKED)
						{
							flag = false;
							flag2 = false;
						}
					}
				}
				if (flag)
				{
					switch (instance.GetSideQuestState(sideQuestData))
					{
					case SideQuestProgress.SideQuestState.Inactive:
					case SideQuestProgress.SideQuestState.Pending:
						flag2 = false;
						if (sideQuestData.IsEventEnded)
						{
							flag = false;
							FailSideQuest(sideQuestData);
						}
						else
						{
							flag = (sideQuestData.IsEventStarted ? true : false);
						}
						break;
					case SideQuestProgress.SideQuestState.InProgress:
						flag = true;
						flag2 = false;
						if (sideQuestData.IsEventEnded)
						{
							SetSideQuestState(sideQuestData, SideQuestProgress.SideQuestState.Expired);
						}
						break;
					case SideQuestProgress.SideQuestState.Expired:
					case SideQuestProgress.SideQuestState.Accomplished:
						flag = true;
						flag2 = false;
						break;
					case SideQuestProgress.SideQuestState.Complete:
						flag = false;
						flag2 = true;
						break;
					case SideQuestProgress.SideQuestState.Failed:
						flag = false;
						flag2 = false;
						break;
					}
				}
				sideQuestData.IsPlayable = flag;
				if (!flag2 || sideQuestData.UnlockQuestIDs == null)
				{
					continue;
				}
				for (int j = 0; j < sideQuestData.UnlockQuestIDs.Count; j++)
				{
					int id = sideQuestData.UnlockQuestIDs[j];
					SideQuestData sideQuestById = GetSideQuestById(sideQuestData.QuestType, id);
					if (sideQuestById != null)
					{
						list2.Add(sideQuestById);
					}
				}
			}
			list = list2;
		}
	}

	public void InitializeSideQuests(string questType)
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		try
		{
			ActiveSideQuests[questType] = new List<SideQuestData>();
			List<SideQuestData> list = sideQuests[questType];
			for (int i = 0; i < list.Count; i++)
			{
				SideQuestData sideQuestData = list[i];
				sideQuestData.IsPlayable = false;
			}
			List<SideQuestData> initialSideQuests = GetInitialSideQuests(questType);
			UpdatePlayableSideQuests(initialSideQuests);
		}
		catch (KeyNotFoundException)
		{
		}
	}

	public void InitializeQuestStates()
	{
		foreach (string key in sideQuests.Keys)
		{
			InitializeSideQuests(key);
		}
	}

	private void ClearSideQuestDropItems()
	{
		foreach (KeyValuePair<string, UnityEngine.Object> item in mCollectiblesPrefabDict)
		{
			Resources.UnloadAsset(item.Value);
		}
		mCollectiblesPrefabDict.Clear();
	}

	private void ClearSideQuestDropItem(SideQuestData sqd)
	{
		if (mCollectiblesPrefabDict.ContainsKey(sqd.CollectiblePrefab))
		{
			Resources.UnloadAsset(mCollectiblesPrefabDict[sqd.CollectiblePrefab]);
			mCollectiblesPrefabDict.Remove(sqd.CollectiblePrefab);
		}
	}

	public GameObject GetSideQuestDropObject(SideQuestData sqd)
	{
		if (sqd != null)
		{
			UnityEngine.Object @object = null;
			@object = ((!mCollectiblesPrefabDict.ContainsKey(sqd.CollectiblePrefab)) ? SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource(sqd.CollectiblePrefab) : mCollectiblesPrefabDict[sqd.CollectiblePrefab]);
			if (@object != null)
			{
				mCollectiblesPrefabDict[sqd.CollectiblePrefab] = @object;
				return @object as GameObject;
			}
		}
		return null;
	}

	private void PostBattle_CountSummonedCards(SideQuestData sqd, QuestData battleQuest)
	{
		if (!string.IsNullOrEmpty(sqd.InitialCardID))
		{
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			GameState instance2 = GameState.Instance;
			int summonedCardCount = instance2.GetSummonedCardCount(PlayerType.User, sqd.InitialCardID);
			instance.IncSideQuestProgress(sqd, summonedCardCount, battleQuest);
		}
	}

	private void PostBattle_CountCardsInDeck(SideQuestData sqd, QuestData battleQuest)
	{
		if (!string.IsNullOrEmpty(sqd.InitialCardID))
		{
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			if (instance.DeckManager.HasCard(sqd.InitialCardID))
			{
				instance.IncSideQuestProgress(sqd, 1, battleQuest);
			}
		}
	}

	private void PostBattle_CountCollectibles(SideQuestData sqd, QuestData battleQuest)
	{
		if (!string.IsNullOrEmpty(sqd.CollectiblePrefab))
		{
			QuestEarningManager instance = QuestEarningManager.GetInstance();
			if (instance.sideQuestEarnedItem.ContainsKey(sqd.iQuestID))
			{
				PlayerInfoScript instance2 = PlayerInfoScript.GetInstance();
				int numCollectedItems = instance.sideQuestEarnedItem[sqd.iQuestID];
				instance2.IncSideQuestProgress(sqd, numCollectedItems, battleQuest);
			}
		}
	}

	private void PostBattle_CountSummonedCardOnLandscape(SideQuestData sqd, QuestData battleQuest)
	{
		if (!string.IsNullOrEmpty(sqd.InitialCardID))
		{
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			GameState instance2 = GameState.Instance;
			int summonedCardCount = instance2.GetSummonedCardCount(PlayerType.User, sqd.InitialCardID);
			instance.IncSideQuestProgress(sqd, summonedCardCount, battleQuest);
			Dictionary<LandscapeType, int> cardLandscapedCount = instance2.GetCardLandscapedCount(PlayerType.User, sqd.InitialCardID);
			instance.IncSideQuestLandScapeCountProgress(sqd, cardLandscapedCount, battleQuest);
		}
	}

	public void OnBattleEndLoser(QuestData battleQuest)
	{
		ClearSideQuestDropItems();
	}

	public void OnBattleEndWinner(QuestData battleQuest)
	{
		ClearSideQuestDropItems();
		List<SideQuestData> activeSideQuests = GetActiveSideQuests(battleQuest.QuestType);
		for (int i = 0; i < activeSideQuests.Count; i++)
		{
			SideQuestData sideQuestData = activeSideQuests[i];
			MethodInfo method = GetType().GetMethod(sideQuestData.PostBattleMethodName, BindingFlags.Instance | BindingFlags.NonPublic);
			if (method != null)
			{
				method.Invoke(this, new object[2] { sideQuestData, battleQuest });
			}
		}
	}

	public bool IsSideQuestAccomplished(SideQuestData sqd)
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		SideQuestProgress sideQuestProgress = instance.GetSideQuestProgress(sqd, true);
		if (sideQuestProgress.Collected >= sqd.NumCollectibles)
		{
			return true;
		}
		return false;
	}

	public string GetRewardCardID(SideQuestData sqd)
	{
		if (string.IsNullOrEmpty(sqd.RewardCardMethodName))
		{
			return sqd.RewardID;
		}
		MethodInfo method = GetType().GetMethod(sqd.RewardCardMethodName, BindingFlags.Instance | BindingFlags.NonPublic);
		string result = string.Empty;
		if (method != null)
		{
			result = (string)method.Invoke(this, new object[1] { sqd });
		}
		return result;
	}

	private string RewardCard_MostDeployedLandscape(SideQuestData sqd)
	{
		string empty = string.Empty;
		int num = 0;
		LandscapeType land = LandscapeType.None;
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		SideQuestProgress sideQuestProgress = instance.GetSideQuestProgress(sqd, true);
		foreach (KeyValuePair<LandscapeType, int> item in sideQuestProgress.DeployedLandscapeCount)
		{
			if (num == 0)
			{
				land = item.Key;
				num = item.Value;
			}
			if (item.Value >= num)
			{
				land = item.Key;
				num = item.Value;
			}
		}
		empty = sqd.GetRewardCardByLandScape(land);
		if (string.IsNullOrEmpty(empty))
		{
			empty = sqd.RewardID;
		}
		return empty;
	}
}
