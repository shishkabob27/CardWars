using System.Collections.Generic;
using UnityEngine;

public class MapInfo : MonoBehaviour
{
	public string dbMapJsonFileName;

	public GameObject MainCameraObj;

	public BoxCollider MainCameraCollider;

	public TBPan MainCameraPanScript;

	public GameObject MPCameraObj;

	public GameObject[] RegionCameraPos;

	public GameObject[] HiddenPaths;

	public AudioSource MapMusic;

	public Texture LockedRoadTexture;

	public Texture[] RegionRoadTextures;

	public GameObject DefaultNodePrefab;

	public GameObject[] RegionNodePrefabs;

	protected Dictionary<int, GameObject> QuestMapNodesDict = new Dictionary<int, GameObject>();

	public Dictionary<string, GameObject> HiddenPathsDict = new Dictionary<string, GameObject>();

	private Dictionary<int, int> RegionIDToIndexDict;

	private Dictionary<string, int> RegionNameToIndexDict;

	public string MapQuestType { get; private set; }

	public List<MapRegionInfo> Regions { get; private set; }

	public Camera MainCamera { get; private set; }

	public Camera MPCamera { get; private set; }

	public GameObject FindDescendantGameObject(string goName)
	{
		if (base.gameObject != null && base.gameObject.transform != null)
		{
			Transform transform = base.gameObject.transform.FindDescendant(goName);
			if (transform != null)
			{
				return transform.gameObject;
			}
		}
		return null;
	}

	private void Awake()
	{
		if (MainCameraObj != null)
		{
			MainCamera = MainCameraObj.GetComponent<Camera>();
		}
		if (MPCameraObj != null)
		{
			MPCamera = MPCameraObj.GetComponent<Camera>();
		}
	}

	public void InitializeMapData(string questType)
	{
		MapQuestType = questType;
		LoadMapData();
		CacheQuestMapNodes();
		CacheHiddenPaths();
	}

	private void LoadMapData()
	{
		Dictionary<string, object>[] array = SQUtils.ReadJSONData(dbMapJsonFileName, false);
		int num = 0;
		Dictionary<string, object>[] array2 = array;
		foreach (Dictionary<string, object> dict in array2)
		{
			if (Regions == null)
			{
				Regions = new List<MapRegionInfo>();
				RegionIDToIndexDict = new Dictionary<int, int>();
				RegionNameToIndexDict = new Dictionary<string, int>();
			}
			int count = Regions.Count;
			MapRegionInfo mapRegionInfo = new MapRegionInfo(MapQuestType, dict, count, num);
			if (RegionIDToIndexDict.ContainsKey(mapRegionInfo.RegionID))
			{
				UnityEngine.Debug.LogError(string.Format("Two regions can't have the same IDs '{0}'", mapRegionInfo.RegionID));
			}
			if (!string.IsNullOrEmpty(mapRegionInfo.RegionName) && RegionNameToIndexDict.ContainsKey(mapRegionInfo.RegionName))
			{
				UnityEngine.Debug.LogError(string.Format("Two regions can't have the same NAME '{0}'", mapRegionInfo.RegionName));
			}
			RegionIDToIndexDict[mapRegionInfo.RegionID] = count;
			RegionNameToIndexDict[mapRegionInfo.RegionName] = count;
			Regions.Add(mapRegionInfo);
			num += mapRegionInfo.NumQuests;
		}
	}

	public void CacheQuestMapNodes()
	{
		QuestMapNodesDict = new Dictionary<int, GameObject>();
		QuestManager instance = QuestManager.Instance;
		for (int i = 0; i < Regions.Count; i++)
		{
			MapRegionInfo mapRegionInfo = Regions[i];
			GameObject gameObject = FindDescendantGameObject(mapRegionInfo.RootNodeName);
			if (gameObject != null)
			{
				for (int j = mapRegionInfo.FirstQuestIndex; j < mapRegionInfo.FirstQuestIndex + mapRegionInfo.NumQuests; j++)
				{
					QuestData questByIndex = instance.GetQuestByIndex(MapQuestType, j);
					string goName = string.Format("Q{0:D4}", questByIndex.iQuestID);
					GameObject value = FindDescendantGameObject(goName);
					QuestMapNodesDict[questByIndex.iQuestID] = value;
				}
			}
		}
	}

	public GameObject GetQuestMapNode(QuestData quest)
	{
		if (quest != null && QuestMapNodesDict.ContainsKey(quest.iQuestID))
		{
			return QuestMapNodesDict[quest.iQuestID];
		}
		return null;
	}

	private void CacheHiddenPaths()
	{
		if (HiddenPathsDict.Count != 0 || HiddenPaths == null || HiddenPaths.Length <= 0)
		{
			return;
		}
		GameObject[] hiddenPaths = HiddenPaths;
		foreach (GameObject gameObject in hiddenPaths)
		{
			if (gameObject != null)
			{
				HiddenPathsDict[gameObject.name] = gameObject;
				gameObject.SetActive(false);
			}
		}
	}

	public void HideAllPaths()
	{
		GameObject[] hiddenPaths = HiddenPaths;
		foreach (GameObject gameObject in hiddenPaths)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
		}
	}

	public void ShowAllPaths()
	{
		GameObject[] hiddenPaths = HiddenPaths;
		foreach (GameObject gameObject in hiddenPaths)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
		}
	}

	public GameObject GetHiddenPath(string pathName)
	{
		if (HiddenPathsDict.ContainsKey(pathName))
		{
			return HiddenPathsDict[pathName];
		}
		return null;
	}

	public MapRegionInfo GetRegionByName(string regionName)
	{
		if (RegionNameToIndexDict.ContainsKey(regionName))
		{
			int index = RegionNameToIndexDict[regionName];
			return Regions[index];
		}
		return null;
	}

	public MapRegionInfo GetRegionByID(int regionID)
	{
		if (RegionIDToIndexDict.ContainsKey(regionID))
		{
			int index = RegionIDToIndexDict[regionID];
			return Regions[index];
		}
		return null;
	}

	public GameObject GetQuestNodePrefab(QuestData quest)
	{
		if (!string.IsNullOrEmpty(quest.NodePrefabPath))
		{
			GameObject gameObject = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource(quest.NodePrefabPath) as GameObject;
			if (gameObject != null)
			{
				return gameObject;
			}
		}
		MapRegionInfo regionByID = GetRegionByID(quest.RegionID);
		if (regionByID != null && RegionNodePrefabs.Length > regionByID.RegionIndex && RegionNodePrefabs[regionByID.RegionIndex] != null)
		{
			return RegionNodePrefabs[regionByID.RegionIndex];
		}
		return DefaultNodePrefab;
	}

	public Vector3 GetRegionCameraPos(MapRegionInfo region)
	{
		if (RegionCameraPos.Length > region.RegionIndex && RegionCameraPos[region.RegionIndex].transform != null)
		{
			return RegionCameraPos[region.RegionIndex].transform.position;
		}
		return Vector3.zero;
	}
}
