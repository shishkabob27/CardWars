using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JsonFx.Json;
using UnityEngine;

public class QuestManager : ILoadable
{
	private const string FILENAME_QUEST_MAIN = "db_Quest.json";

	private const string FILENAME_QUEST_EXTRA = "db_QuestExtra.json";

	private Dictionary<string, Dictionary<int, int>> _questIDToIndexMap;

	private Dictionary<string, List<QuestData>> _questMap;

	private static QuestManager instance;

	public List<QuestData> quests
	{
		get
		{
			return questMap["main"];
		}
	}

	public List<QuestData> fcQuests
	{
		get
		{
			return questMap["fc"];
		}
	}

	public List<QuestData> MPquests
	{
		get
		{
			return questMap["deck_wars"];
		}
	}

	public List<QuestData> dungeonQuests
	{
		get
		{
			return questMap["dungeon"];
		}
	}

	public Dictionary<string, Dictionary<int, int>> questIDToIndexMap
	{
		get
		{
			return _questIDToIndexMap;
		}
	}

	public Dictionary<string, List<QuestData>> questMap
	{
		get
		{
			return _questMap;
		}
	}

	public static QuestManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new QuestManager();
			}
			return instance;
		}
	}

	public List<QuestData> GetQuestsByType(string questType)
	{
		if (questMap.ContainsKey(questType))
		{
			return questMap[questType];
		}
		return new List<QuestData>();
	}

	public QuestData GetRandomQuestByType(string questType)
	{
		try
		{
			int index = UnityEngine.Random.Range(0, questMap[questType].Count);
			return questMap[questType][index];
		}
		catch (Exception)
		{
			return null;
		}
	}

	public QuestData GetRandomQuestWithState(string questType, QuestData.QuestState state)
	{
		try
		{
			List<QuestData> list = questMap[questType];
			List<QuestData> list2 = new List<QuestData>(list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].GetState() == state)
				{
					list2.Add(list[i]);
				}
			}
			if (list2.Count > 0)
			{
				int index = UnityEngine.Random.Range(0, list2.Count);
				return list2[index];
			}
		}
		catch
		{
		}
		return null;
	}

	public int CountQuestsWithState(string questType, QuestData.QuestState state)
	{
		try
		{
			int num = 0;
			List<QuestData> list = questMap[questType];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].GetState() == state)
				{
					num++;
				}
			}
			return num;
		}
		catch
		{
		}
		return 0;
	}

	public IEnumerator Load()
	{
		while (!AIDeckManager.Instance.Loaded)
		{
			yield return null;
		}
		while (!CharacterDataManager.Instance.Loaded)
		{
			yield return null;
		}
		string filePath = Path.Combine("Blueprints", "db_Quest.json");
		string json = TFUtils.GetJsonFileContent(filePath);
		Dictionary<string, object>[] data2 = JsonReader.Deserialize<Dictionary<string, object>[]>(json);
		if (LoadingManager.ShouldYield())
		{
			yield return null;
		}
		_questMap = new Dictionary<string, List<QuestData>>();
		_questIDToIndexMap = new Dictionary<string, Dictionary<int, int>>();
		Dictionary<string, object>[] array = data2;
		foreach (Dictionary<string, object> row in array)
		{
			QuestData quest = QuestData.CreateQuestData(row);
			if (quest != null)
			{
				List<QuestData> questList;
				try
				{
					questList = _questMap[quest.QuestType];
				}
				catch (KeyNotFoundException)
				{
					questList = new List<QuestData>();
					_questMap.Add(quest.QuestType, questList);
					_questIDToIndexMap.Add(quest.QuestType, new Dictionary<int, int>());
				}
				quest.Populate(row, questList.Count);
				questList.Add(quest);
				_questIDToIndexMap[quest.QuestType][quest.iQuestID] = quest.iQuestIndex;
			}
			if (LoadingManager.ShouldYield())
			{
				yield return null;
			}
		}
		string filePath2 = Path.Combine("Blueprints", "db_QuestExtra.json");
		string json2 = TFUtils.GetJsonFileContent(filePath2);
		Dictionary<string, object>[] data = JsonReader.Deserialize<Dictionary<string, object>[]>(json2);
		if (LoadingManager.ShouldYield())
		{
			yield return null;
		}
		Dictionary<string, object>[] array2 = data;
		foreach (Dictionary<string, object> row2 in array2)
		{
			try
			{
				string questId = TFUtils.LoadString(row2, "QuestID");
				QuestData qd = GetQuest(questId);
				if (qd == null)
				{
					TFUtils.WarnLog("Extra info found for non-existent quest " + questId);
				}
				else
				{
					qd.FillQuestDataExtra(row2);
				}
			}
			catch (Exception e)
			{
				TFUtils.ErrorLog("Extra quest data load exception:\n" + e.ToString() + "\nRow data:\n" + row2.ToString());
			}
		}
		yield return null;
	}

	public void Destroy()
	{
		instance = null;
	}

	public QuestData GetQuest(string ID)
	{
		try
		{
			int iD = int.Parse(ID);
			return GetQuest(iD);
		}
		catch
		{
		}
		return null;
	}

	public QuestData GetQuest(int ID)
	{
		if (_questMap != null)
		{
			foreach (string key in _questMap.Keys)
			{
				QuestData questByID = GetQuestByID(key, ID);
				if (questByID != null)
				{
					return questByID;
				}
			}
		}
		return null;
	}

	public QuestData GetQuestByID(string questType, int questID)
	{
		try
		{
			int index = _questIDToIndexMap[questType][questID];
			return _questMap[questType][index];
		}
		catch (Exception)
		{
		}
		return null;
	}

	public QuestData GetQuestByID(string questType, string ID)
	{
		try
		{
			int questID = int.Parse(ID);
			return GetQuestByID(questType, questID);
		}
		catch (Exception)
		{
		}
		return null;
	}

	public QuestData GetQuestByIndex(string questType, int questIndex)
	{
		try
		{
			return _questMap[questType][questIndex];
		}
		catch
		{
		}
		return null;
	}

	public QuestData GetMainQuestByID(string ID)
	{
		if (ID != null && quests != null)
		{
			return quests.Find((QuestData q) => q.QuestID.Equals(ID, StringComparison.InvariantCultureIgnoreCase));
		}
		return null;
	}

	public QuestData GetMainQuestByIndex(int idx)
	{
		if (idx < 0 || idx >= quests.Count)
		{
			return null;
		}
		return quests[idx];
	}

	public QuestData GetMainQuestByID(int questID)
	{
		return GetQuestByID("main", questID);
	}

	public QuestData GetMPQuest(int idx)
	{
		if (idx < 0 || idx >= MPquests.Count)
		{
			return null;
		}
		return MPquests[idx];
	}

	public QuestData GetDungeonQuest(string ID)
	{
		if (ID != null && dungeonQuests != null)
		{
			return dungeonQuests.Find((QuestData q) => q.QuestID.Equals(ID, StringComparison.InvariantCultureIgnoreCase));
		}
		return null;
	}

	public void InitializeQuestStates()
	{
		foreach (string key in questMap.Keys)
		{
			InitializeQuestStates(key);
		}
	}

	public void InitializeQuestStates(string questType)
	{
		if (questMap == null || questMap.Count < 1 || !QuestTypes.AutoUnlockQuests(questType))
		{
			return;
		}
		PlayerInfoScript playerInfoScript = PlayerInfoScript.GetInstance();
		if (!(playerInfoScript != null) || !questMap.ContainsKey(questType) || questMap[questType].Count <= 0)
		{
			return;
		}
		Dictionary<int, QuestData> dictionary = new Dictionary<int, QuestData>();
		List<QuestData> list = questMap[questType];
		for (int i = 0; i < list.Count; i++)
		{
			QuestData questData = list[i];
			questData.SetState(QuestData.QuestState.LOCKED);
		}
		dictionary[list[0].iQuestID] = list[0];
		while (dictionary.Count > 0)
		{
			Dictionary<int, QuestData> dictionary2 = new Dictionary<int, QuestData>();
			foreach (KeyValuePair<int, QuestData> item in dictionary)
			{
				QuestData value = item.Value;
				if (value.GetState() == QuestData.QuestState.PLAYABLE)
				{
					continue;
				}
				value.SetState(QuestData.QuestState.PLAYABLE);
				if (value.UnlockQuestIDs == null)
				{
					continue;
				}
				int questProgress = playerInfoScript.GetQuestProgress(value);
				for (int j = 0; j < questProgress && j < value.UnlockQuestIDs.Count; j++)
				{
					if (value.UnlockQuestIDs[j] == null || value.UnlockQuestIDs[j].Count <= 0)
					{
						continue;
					}
					foreach (int item2 in value.UnlockQuestIDs[j])
					{
						QuestData quest = GetQuest(item2);
						if (quest != null && quest.GetState() != QuestData.QuestState.PLAYABLE)
						{
							dictionary2[quest.iQuestID] = quest;
						}
					}
				}
			}
			dictionary = dictionary2;
		}
	}

	public int CompareQuestOrder(QuestData a, QuestData b)
	{
		if (a == null && b == null)
		{
			return 0;
		}
		if (a == null && b != null)
		{
			return -1;
		}
		if (a != null && b == null)
		{
			return 1;
		}
		if (a.QuestType == b.QuestType && a.QuestType != "fc")
		{
			if (a.iQuestIndex > b.iQuestIndex)
			{
				return 1;
			}
			if (a.iQuestIndex < b.iQuestIndex)
			{
				return -1;
			}
			return 0;
		}
		return 0;
	}

	public QuestData GetNextQuest(QuestData curQuest, int questStarsOverride = 0)
	{
		QuestData result = null;
		int num = 99;
		PlayerInfoScript playerInfoScript = PlayerInfoScript.GetInstance();
		if (curQuest != null && curQuest.UnlockQuestIDs != null && curQuest.UnlockQuestIDs.Count > 0)
		{
			int num2 = questStarsOverride;
			if (num2 <= 0)
			{
				num2 = playerInfoScript.GetQuestProgress(curQuest);
			}
			for (int num3 = num2 - 1; num3 >= 0; num3--)
			{
				if (curQuest.UnlockQuestIDs[num3] != null && curQuest.UnlockQuestIDs[num3].Count > 0)
				{
					for (int i = 0; i < curQuest.UnlockQuestIDs[num3].Count; i++)
					{
						int questID = curQuest.UnlockQuestIDs[num3][0];
						QuestData questByID = GetQuestByID(curQuest.QuestType, questID);
						if (questByID != null)
						{
							int questProgress = playerInfoScript.GetQuestProgress(questByID);
							if (questProgress < num)
							{
								result = questByID;
								num = questProgress;
							}
						}
					}
				}
			}
		}
		return result;
	}

	public QuestData GetFirstQuest(string questType)
	{
		return GetQuestByIndex(questType, 0);
	}

	public int GetFirstQuestID(string questType)
	{
		QuestData firstQuest = GetFirstQuest(questType);
		if (firstQuest != null)
		{
			return firstQuest.iQuestID;
		}
		return 0;
	}

	public List<string> GetQuestTypes()
	{
		List<string> list = new List<string>();
		foreach (string key in questMap.Keys)
		{
			list.Add(key);
		}
		return list;
	}
}
