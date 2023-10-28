using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JsonFx.Json;

public class DungeonDataManager : ILoadable
{
	private const string BASE_FILEPATH = "Blueprints";

	private const string DATA_FILEPATH = "db_Dungeon.json";

	private const string QUESTDATA_FILEPATH = "db_DungeonQuestList.json";

	private const string GROUPSCHEDULEDATA_FILEPATH = "db_DungeonGroupSchedule.json";

	private static DungeonDataManager sInstance;

	private Dictionary<string, DungeonData> Dungeons;

	public static DungeonDataManager Instance
	{
		get
		{
			if (sInstance == null)
			{
				sInstance = new DungeonDataManager();
			}
			return sInstance;
		}
	}

	public bool Loaded { get; private set; }

	private DungeonDataManager()
	{
		Dungeons = new Dictionary<string, DungeonData>();
	}

	public List<DungeonData> GetAvailableDungeons(long timeCurr)
	{
		List<DungeonData> list = new List<DungeonData>();
		foreach (DungeonData value in Dungeons.Values)
		{
			if (value.IsUnlocked(timeCurr))
			{
				list.Add(value);
			}
		}
		return list;
	}

	public List<DungeonData> GetAllDungeons()
	{
		List<DungeonData> list = new List<DungeonData>();
		foreach (DungeonData value in Dungeons.Values)
		{
			list.Add(value);
		}
		return list;
	}

	public DungeonData GetDungeon(string dungeonID)
	{
		DungeonData value;
		Dungeons.TryGetValue(dungeonID, out value);
		return value;
	}

	public string GetDungeonQuestID(string dungeonID, int questIndex)
	{
		try
		{
			return GetDungeon(dungeonID).Quests[questIndex].ID;
		}
		catch (Exception)
		{
			return null;
		}
	}

	public IEnumerator Load()
	{
		Dictionary<string, DungeonData.DungeonGroup> groups = new Dictionary<string, DungeonData.DungeonGroup>();
		Dictionary<string, object>[] array = LoadJsonData("db_Dungeon.json");
		foreach (Dictionary<string, object> dungeonDef in array)
		{
			string dungeonID = TFUtils.LoadString(dungeonDef, "dungeon");
			string groupID = TFUtils.LoadString(dungeonDef, "group");
			if (Dungeons.ContainsKey(dungeonID))
			{
				TFUtils.WarnLog("Duplicate dungeon entry. Ignoring newer entry: " + dungeonID);
				continue;
			}
			TFUtils.DebugLog("Loading dungeon '" + dungeonID + "' (group '" + groupID + "')");
			DungeonData.DungeonGroup group;
			if (!groups.TryGetValue(groupID, out group))
			{
				group = new DungeonData.DungeonGroup(groupID);
				groups.Add(groupID, group);
			}
			DungeonData dungeonData = new DungeonData(dungeonID, TFUtils.LoadString(dungeonDef, "name"), group, TFUtils.LoadString(dungeonDef, "icon"), TFUtils.LoadString(dungeonDef, "info"));
			Dungeons.Add(dungeonID, dungeonData);
			if (LoadingManager.ShouldYield())
			{
				yield return null;
			}
		}
		Dictionary<string, object>[] array2 = LoadJsonData("db_DungeonQuestList.json");
		foreach (Dictionary<string, object> questDef in array2)
		{
			string dungeonID2 = TFUtils.LoadString(questDef, "dungeon");
			string questID = TFUtils.LoadString(questDef, "questID");
			DungeonData dungeonData2;
			if (!Dungeons.TryGetValue(dungeonID2, out dungeonData2))
			{
				TFUtils.WarnLog("Missing dungeon for quest data. Ignoring: " + dungeonID2 + " - quest " + questID);
				continue;
			}
			bool locked = dungeonData2.Quests.Count != 0;
			dungeonData2.AppendQuest(questID, TFUtils.LoadInt(questDef, "heartCost"), locked);
			if (LoadingManager.ShouldYield())
			{
				yield return null;
			}
		}
		Dictionary<string, object>[] array3 = LoadJsonData("db_DungeonGroupSchedule.json");
		foreach (Dictionary<string, object> groupScheduleDef in array3)
		{
			string groupID2 = TFUtils.LoadString(groupScheduleDef, "group");
			string startDate = TFUtils.LoadString(groupScheduleDef, "startDate");
			string endDate = TFUtils.LoadString(groupScheduleDef, "endDate");
			DungeonData.DungeonGroup group2;
			if (!groups.TryGetValue(groupID2, out group2))
			{
				TFUtils.WarnLog("Missing dungeon group reference in schedule data. Ignoring: " + groupID2 + " - " + startDate + " -> " + endDate);
				continue;
			}
			if (!group2.AddSchedule(startDate, endDate))
			{
				TFUtils.WarnLog("Invalid schedule data. Ignoring: " + groupID2 + " - " + startDate + " -> " + endDate);
			}
			if (LoadingManager.ShouldYield())
			{
				yield return null;
			}
		}
		Loaded = true;
	}

	public void Destroy()
	{
		if (sInstance == this)
		{
			sInstance = null;
		}
	}

	private Dictionary<string, object>[] LoadJsonData(string fileName)
	{
		string text = Path.Combine("Blueprints", fileName);
		TFUtils.DebugLog("Loading dungeon data from path: " + text);
		string jsonFileContent = TFUtils.GetJsonFileContent(text);
		return JsonReader.Deserialize<Dictionary<string, object>[]>(jsonFileContent);
	}
}
