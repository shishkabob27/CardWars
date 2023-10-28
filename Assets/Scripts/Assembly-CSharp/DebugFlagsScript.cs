using UnityEngine;

public class DebugFlagsScript : MonoBehaviour
{
	public enum ForceStartingPlayer
	{
		None,
		Me,
		Them
	}

	public bool AddDiamonds;

	public bool InfiniteMagic;

	public bool AttackBonus;

	public bool QuickWin;

	public bool quickLoose;

	public bool DebugInBuild;

	public bool UseLocalJsonFiles;

	public bool unlockLeaders;

	public bool autoWin;

	public bool autoLose;

	public bool recoverPlayerHP;

	public bool recoverOpponentHP;

	public bool cardSelection;

	public bool fingerGesture;

	public bool tutorialDebug;

	public bool stopTutorial;

	public bool failIAP;

	public bool disableDungeonTimeLock;

	public bool FCCompleteDemo;

	public ForceStartingPlayer forceStartingPlayer;

	public bool resetAchievements;

	public bool resetCalendar;

	public string specifyCalendarDate = string.Empty;

	public bool autoIncrementCalendar;

	public ElFistoController.ElFistoModes elFistoMode = ElFistoController.ElFistoModes.On;

	public bool plainTextGameJson;

	public BattleDebug battleDisplay;

	public MapDebug mapDebug;

	private static DebugFlagsScript g_debugFlagsScript;

	private bool setFlag;

	private GameObject levelObj;

	private GameObject tableObj;

	private void Awake()
	{
		if (g_debugFlagsScript == null)
		{
			g_debugFlagsScript = this;
		}
	}

	private void Start()
	{
		DebugInBuild = false;
		GameObject[] array = GameObject.FindGameObjectsWithTag("DebugFlags");
		if (array.Length > 1)
		{
			GameObject[] array2 = array;
			foreach (GameObject gameObject in array2)
			{
				DebugFlagsScript component = gameObject.GetComponent<DebugFlagsScript>();
				if (!component || component != g_debugFlagsScript)
				{
					Object.Destroy(gameObject);
				}
			}
		}
		if (GlobalFlags.Instance.stopTutorial)
		{
			stopTutorial = true;
		}
		if (GlobalFlags.Instance.disableDungeonTimeLock)
		{
			disableDungeonTimeLock = true;
		}
	}

	public static DebugFlagsScript GetInstance()
	{
		return g_debugFlagsScript;
	}

	public GameObject SpawnFPSObject(GameObject prefab, GameObject parent)
	{
		GameObject gameObject = null;
		if (prefab != null && parent != null)
		{
			gameObject = Object.Instantiate(prefab);
			if (parent != null)
			{
				gameObject.transform.parent = parent.transform;
			}
			parent.gameObject.SetActive(true);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
		}
		return gameObject;
	}

	public GameObject GetParentObj()
	{
		GameObject gameObject = null;
		GameObject gameObject2 = GameObject.Find("Battle UI Panel");
		if (gameObject2 == null)
		{
			gameObject2 = GameObject.Find("Menu UI Panel");
		}
		if (gameObject2 == null)
		{
			gameObject2 = GameObject.Find("QuestMap_UI_Panel");
		}
		if (gameObject2 != null)
		{
			gameObject = GameObject.Find("debugParentObj");
			if (gameObject == null)
			{
				gameObject = new GameObject("debugParentObj");
			}
			gameObject.transform.parent = gameObject2.transform;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = new Vector3(600f, 0f, 0f);
			gameObject.layer = gameObject2.layer;
		}
		return gameObject;
	}

	private void Update()
	{
		GlobalFlags instance = GlobalFlags.Instance;
		instance.disableDungeonTimeLock = disableDungeonTimeLock;
		if (battleDisplay.hideLevel && !setFlag)
		{
			CWLoadLevelForBattle instance2 = CWLoadLevelForBattle.GetInstance();
			if ((bool)instance2)
			{
				levelObj = instance2.levelObj;
				levelObj.SetActive(false);
				tableObj = instance2.tableObj;
				tableObj.SetActive(false);
				setFlag = true;
			}
		}
		if (!battleDisplay.hideLevel && setFlag)
		{
			if (levelObj != null)
			{
				levelObj.SetActive(true);
			}
			if (tableObj != null)
			{
				tableObj.SetActive(true);
			}
			setFlag = false;
		}
	}
}
