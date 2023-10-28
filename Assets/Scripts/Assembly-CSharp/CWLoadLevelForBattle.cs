using System.Collections.Generic;
using UnityEngine;

public class CWLoadLevelForBattle : MonoBehaviour
{
	public Transform EnvironmentParentTr;

	public GameObject levelObj;

	public GameObject tableObj;

	private bool initialized;

	private static CWLoadLevelForBattle instance;

	private void Awake()
	{
		instance = this;
	}

	public static CWLoadLevelForBattle GetInstance()
	{
		return instance;
	}

	private void SpawnLevelObjects()
	{
		QuestData activeQuest = GameState.Instance.ActiveQuest;
		SpawnEnvironmentPrefab("battle_level", activeQuest.LevelPrefab);
		SpawnEnvironmentPrefab("battle_table", activeQuest.TablePrefab);
		GameDataScript.GetInstance().UpdateCharacters();
	}

	public void Update()
	{
		if (!initialized)
		{
			SessionManager sessionManager = SessionManager.GetInstance();
			if (sessionManager.IsReady() && CardManagerScript.GetInstance().IsInitialized)
			{
				initialized = true;
				SpawnLevelObjects();
			}
		}
	}

	private void SpawnEnvironmentPrefab(string scheduleCategory, string defaultPrefab)
	{
		if (GameState.Instance.ActiveQuest.IsQuestType("main"))
		{
			List<ScheduleData> itemsAvailableAndUnlocked = ScheduleDataManager.Instance.GetItemsAvailableAndUnlocked(scheduleCategory, TFUtils.ServerTime.Ticks);
			foreach (ScheduleData item in itemsAvailableAndUnlocked)
			{
				if (TryLoadEnvironmentPrefab(item.ID))
				{
					return;
				}
			}
		}
		if (TryLoadEnvironmentPrefab(defaultPrefab))
		{
		}
	}

	private bool TryLoadEnvironmentPrefab(string prefab)
	{
		string path = "Environment/" + prefab;
        //Object @object = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource(path);
        GameObject original = Resources.Load(path, typeof(GameObject)) as GameObject;

        if (original != null)
		{
			levelObj = Instantiate(original);
			levelObj.transform.parent = EnvironmentParentTr;
			return true;
		}
		return false;
	}
}
