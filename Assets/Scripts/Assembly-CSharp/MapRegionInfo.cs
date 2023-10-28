using System.Collections.Generic;

public class MapRegionInfo
{
	public enum RegionState
	{
		HIDDEN,
		PLAYABLE
	}

	public List<QuestData> Quests = new List<QuestData>();

	public string MapQuestType { get; private set; }

	public int RegionID { get; private set; }

	public int RegionIndex { get; private set; }

	public string RoadMeshName { get; private set; }

	public string RootNodeName { get; private set; }

	public string RoadTextureName { get; private set; }

	public string RegionName { get; private set; }

	public string RegionSpriteName { get; private set; }

	public int NumQuests { get; private set; }

	public int FirstQuestIndex { get; private set; }

	public bool NodesAlwaysVisible { get; private set; }

	public MapRegionInfo(string mapQuestType, Dictionary<string, object> dict, int regionIndex, int firstQuestIndex)
	{
		MapQuestType = mapQuestType;
		RegionID = TFUtils.LoadInt(dict, "MapID", 0);
		RegionIndex = regionIndex;
		RoadMeshName = TFUtils.LoadString(dict, "RoadMesh");
		RootNodeName = TFUtils.LoadString(dict, "Nodes");
		RoadTextureName = TFUtils.LoadString(dict, "RoadTexture");
		RegionName = TFUtils.LoadString(dict, "RegionName");
		RegionSpriteName = TFUtils.LoadString(dict, "RegionSpriteName");
		NumQuests = TFUtils.LoadInt(dict, "NumQuests", 0);
		FirstQuestIndex = firstQuestIndex;
		NodesAlwaysVisible = TFUtils.LoadBool(dict, "NodesAlwaysVisible", false);
	}

	public bool IsLocked()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		return instance.IsRegionLocked(MapQuestType, RegionID);
	}

	public void Unlock()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		instance.UnlockRegion(MapQuestType, RegionID);
	}

	public bool IsPlayable()
	{
		return GetState() == RegionState.PLAYABLE;
	}

	public RegionState GetState()
	{
		for (int i = 0; i < Quests.Count; i++)
		{
			if (Quests[i].GetState() == QuestData.QuestState.PLAYABLE)
			{
				return RegionState.PLAYABLE;
			}
		}
		return RegionState.HIDDEN;
	}

	public void ClearQuests()
	{
		Quests.Clear();
	}

	public void AddQuest(QuestData quest)
	{
		Quests.Add(quest);
	}
}
