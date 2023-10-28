using System.Collections;
using UnityEngine;

public class CWMapController : MapControllerBase
{
	private static CWMapController ms_instance;

	private void Awake()
	{
		base.MapQuestType = "main";
		ms_instance = this;
		gflags = GlobalFlags.Instance;
		if (pInfo == null)
		{
			pInfo = PlayerInfoScript.GetInstance();
		}
		pInfo = PlayerInfoScript.GetInstance();
		qMgr = QuestManager.Instance;
		dbMapJsonFileName = "db_Map.json";
		MapControllerBase instance = MapControllerBase.GetInstance();
		if (instance == null || pInfo.CurrentQuestType == base.MapQuestType || pInfo.CurrentQuestType == "deck_wars")
		{
			MapControllerBase.SetActiveInstance(this, false);
		}
		else
		{
			HideMap();
		}
	}

	private void Start()
	{
		if (MenuController.GetInstance() == null)
		{
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			if (!instance.isInitialized)
			{
				instance.Login();
			}
		}
	}

	public override bool IsCameraPosSaved()
	{
		return IsCameraPosSavedToPlayerPref();
	}

	public override Vector3 GetSavedCameraPos()
	{
		return GetCameraPosFromPlayerPref();
	}

	public override void SaveCameraPos(Vector3 camPos, float orthoSize)
	{
		StoreCameraPosToPlayerPref(camPos, orthoSize);
	}

	public override float GetSavedCameraOrthoSize()
	{
		return GetCameraOrthoSizeFromPlayerPref();
	}

	public static bool IsCameraPosSavedToPlayerPref()
	{
		return PlayerPrefs.HasKey("storedCamPosX");
	}

	public static Vector3 GetCameraPosFromPlayerPref()
	{
		Vector3 zero = Vector3.zero;
		zero.x = PlayerPrefs.GetFloat("storedCamPosX");
		zero.y = PlayerPrefs.GetFloat("storedCamPosY");
		zero.z = PlayerPrefs.GetFloat("storedCamPosZ");
		return zero;
	}

	public static void StoreCameraPosToPlayerPref(Vector3 camPos, float orthoSize)
	{
		PlayerPrefs.SetFloat("storedCamPosX", camPos.x);
		PlayerPrefs.SetFloat("storedCamPosY", camPos.y);
		PlayerPrefs.SetFloat("storedCamPosZ", camPos.z);
		PlayerPrefs.SetFloat("storedCamOrthoSize", orthoSize);
		PlayerPrefs.Save();
	}

	public static float GetCameraOrthoSizeFromPlayerPref()
	{
		return PlayerPrefs.GetFloat("storedCamOrthoSize");
	}

	public override void ShowMap()
	{
		base.ShowMap();
		if (!GlobalFlags.Instance.InMPMode)
		{
			TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.EnterQuestMap);
		}
	}

	public static void Activate(bool showMap)
	{
		MapControllerBase.SetActiveInstance(ms_instance, showMap);
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		instance.SetCurrentQuest(instance.GetCurrentQuest(ms_instance.MapQuestType));
	}

	protected override IEnumerator QuestUnlockFlow()
	{
		QuestData qd = pInfo.GetCurrentQuest();
		int questId = qd.iQuestID;
		if (questId >= 2)
		{
			SocialManager.Instance.ReportAchievement(SocialManager.AchievementIDs.AT_QUEST_3);
		}
		if (questId >= 7)
		{
			SocialManager.Instance.ReportAchievement(SocialManager.AchievementIDs.AT_QUEST_8);
		}
		if (questId >= 14)
		{
			SocialManager.Instance.ReportAchievement(SocialManager.AchievementIDs.AT_QUEST_15);
		}
		if (questId >= 29)
		{
			SocialManager.Instance.ReportAchievement(SocialManager.AchievementIDs.AT_QUEST_30);
		}
		if (questId >= 49)
		{
			SocialManager.Instance.ReportAchievement(SocialManager.AchievementIDs.AT_QUEST_50);
		}
		if (questId >= 98)
		{
			SocialManager.Instance.ReportAchievement(SocialManager.AchievementIDs.AT_QUEST_99);
		}
		yield return StartCoroutine(base.QuestUnlockFlow());
	}

	protected override IEnumerator QuestUnlockSequence(QuestData qd)
	{
		bool returnToMainMenu = gflags.ReturnToMainMenu;
		bool cleared = gflags.Cleared;
		yield return StartCoroutine(base.QuestUnlockSequence(qd));
		if (returnToMainMenu && cleared)
		{
			if (TutorialMonitor.Instance != null)
			{
				TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.StartGatchaTutorial);
				TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.StartCraftingTutorial);
			}
		}
		else
		{
			TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.RestartFirstQuest);
		}
	}

	protected override IEnumerator UpdatePlayerPosition(QuestData qd)
	{
		if (!gflags.NewlyCleared)
		{
			yield break;
		}
		QuestData nextQuest = QuestManager.Instance.GetNextQuest(qd);
		if (nextQuest != null)
		{
			if (pInfo.GetQuestProgress(nextQuest) == 0)
			{
				yield return StartCoroutine(MovePlayerIcon(qd, nextQuest));
			}
		}
		else
		{
			yield return StartCoroutine(ReachedEndOfTheRoad());
		}
	}

	protected override float FocusCameraOnPlayer()
	{
		bool flag = false;
		float result = 0f;
		if (!TutorialManager.Instance.isTutorialCompleted(TutorialTrigger.PressQuestStartButton))
		{
			QuestData currentQuest = pInfo.GetCurrentQuest();
			GameObject questMapNode = QuestMapInfo.GetQuestMapNode(currentQuest);
			if (questMapNode != null)
			{
				Transform transform = questMapNode.transform;
				result = TweenCameraOnFocus(transform.position, "ToRegion", false, "CurrentNode - FocusCameraOnPlayer");
				base.mainCamera.GetComponent<Camera>().orthographicSize = 22f;
				flag = true;
			}
		}
		if (!flag)
		{
			result = base.FocusCameraOnPlayer();
		}
		return result;
	}

	public override int GetSideQuestFirstQuestID()
	{
		return ParametersManager.Instance.SideQuest_firstquest_Main;
	}

	public override int GetSideQuestMatchlapse()
	{
		return ParametersManager.Instance.SideQuest_matchlapse_Main;
	}
}
