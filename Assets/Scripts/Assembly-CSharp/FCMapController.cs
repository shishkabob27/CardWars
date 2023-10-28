#define ASSERTS_ON
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FCMapController : MapControllerBase
{
	private static FCMapController ms_instance;

	public UIButtonTween TreasureCatInstructionsShow;

	public bool didShowBonusQuestLocation;

	private void Awake()
	{
		base.MapQuestType = "fc";
		ms_instance = this;
		gflags = GlobalFlags.Instance;
		if (pInfo == null)
		{
			pInfo = PlayerInfoScript.GetInstance();
		}
		pInfo = PlayerInfoScript.GetInstance();
		qMgr = QuestManager.Instance;
		if (pInfo.CurrentQuestType == base.MapQuestType || pInfo.CurrentQuestType == "tcat" || pInfo.CurrentQuestType == "elfisto")
		{
			MapControllerBase.SetActiveInstance(this, false);
		}
		else
		{
			HideMap();
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
		return PlayerPrefs.HasKey("FcStoredCamPosX");
	}

	public static Vector3 GetCameraPosFromPlayerPref()
	{
		Vector3 zero = Vector3.zero;
		zero.x = PlayerPrefs.GetFloat("FcStoredCamPosX");
		zero.y = PlayerPrefs.GetFloat("FcStoredCamPosY");
		zero.z = PlayerPrefs.GetFloat("FcStoredCamPosZ");
		return zero;
	}

	public static void StoreCameraPosToPlayerPref(Vector3 camPos, float orthoSize)
	{
		PlayerPrefs.SetFloat("FcStoredCamPosX", camPos.x);
		PlayerPrefs.SetFloat("FcStoredCamPosY", camPos.y);
		PlayerPrefs.SetFloat("FcStoredCamPosZ", camPos.z);
		PlayerPrefs.SetFloat("FcStoredCamOrthoSize", orthoSize);
		PlayerPrefs.Save();
	}

	public static float GetCameraOrthoSizeFromPlayerPref()
	{
		return PlayerPrefs.GetFloat("FcStoredCamOrthoSize");
	}

	public static void Activate(bool showMap)
	{
		MapControllerBase.SetActiveInstance(ms_instance, showMap);
		GlobalFlags.Instance.InMPMode = false;
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (instance.CurrentQuestType != "tcat")
		{
			instance.SetCurrentQuest(instance.GetCurrentQuest(ms_instance.MapQuestType));
		}
		else
		{
			instance.SetCurrentQuest(instance.GetCurrentQuest("tcat"));
		}
	}

	protected override IEnumerator QuestUnlockSequence(QuestData qd)
	{
		bool returnToMainMenu = gflags.ReturnToMainMenu;
		bool cleared = gflags.Cleared;
		while (MenuController.GetInstance().hasAwardStuff)
		{
			yield return null;
		}
		yield return StartCoroutine(base.QuestUnlockSequence(qd));
		if (returnToMainMenu && cleared && TutorialMonitor.Instance != null)
		{
			TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.FC_HiddenPathTutorial);
		}
		yield return StartCoroutine(UpdateBonusQuests());
	}

	protected override IEnumerator UpdatePlayerPosition(QuestData qd)
	{
		yield return null;
	}

	public static void SetupFCDeck()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		int num = -1;
		int num2 = -1;
		for (int i = 0; i < 5; i++)
		{
			Deck deck = instance.DeckManager.GetDeck(i);
			if (deck == null || deck.IsEmpty())
			{
				if (num2 < 0)
				{
					num2 = i;
				}
			}
			else if (deck.Leader.Form.FCWorld)
			{
				num = i;
				break;
			}
		}
		if (num >= 0)
		{
			instance.mQuestMapDeckIdx["fc"] = num;
			instance.Save();
		}
		else if (num2 >= 0)
		{
			Deck selectedDeckCopy = instance.GetSelectedDeckCopy();
			selectedDeckCopy.Leader = LeaderManager.Instance.GetLeader("Leader_Fionna");
			instance.DeckManager.SetDeck(num2, selectedDeckCopy);
			instance.mQuestMapDeckIdx["fc"] = num2;
			instance.Save();
		}
	}

	private void ValidateFCLeaders()
	{
		LeaderItem leaderItem = LeaderManager.Instance.AddNewLeaderIfUnique("Leader_Fionna");
		LeaderItem leaderItem2 = LeaderManager.Instance.AddNewLeaderIfUnique("Leader_Cake");
		if (leaderItem != null || leaderItem2 != null)
		{
			SetupFCDeck();
		}
	}

	public override void ShowMap()
	{
		ValidateFCLeaders();
		base.ShowMap();
	}

	public override int GetSideQuestFirstQuestID()
	{
		return ParametersManager.Instance.SideQuest_firstquest_FC;
	}

	public override int GetSideQuestMatchlapse()
	{
		return ParametersManager.Instance.SideQuest_matchlapse_FC;
	}

	private QuestData GetNextBonusQuest()
	{
		return QuestManager.Instance.GetRandomQuestWithState("tcat", QuestData.QuestState.PLAYABLE);
	}

	private QuestData GetNextBonusQuestLocation()
	{
		int num = 0;
		List<MapRegionInfo> list = new List<MapRegionInfo>();
		for (int i = 0; i < QuestMapInfo.Regions.Count; i++)
		{
			if (QuestMapInfo.Regions[i].GetState() != 0)
			{
				list.Add(QuestMapInfo.Regions[i]);
				num += QuestMapInfo.Regions[i].NumQuests;
			}
		}
		TFUtils.Assert(num > 0, "Map must have at least one unlocked region");
		int num2 = UnityEngine.Random.Range(0, num);
		QuestData result = null;
		for (int j = 0; j < list.Count; j++)
		{
			MapRegionInfo mapRegionInfo = list[j];
			if (num2 < mapRegionInfo.NumQuests)
			{
				result = mapRegionInfo.Quests[num2];
				break;
			}
			num2 -= mapRegionInfo.NumQuests;
		}
		return result;
	}

	private bool ShouldShowBonusQuest()
	{
		int num = QuestManager.Instance.CountQuestsWithState("tcat", QuestData.QuestState.PLAYABLE);
		if (num > 0 && pInfo.GetBonusQuestTimeLapse(base.MapQuestType).TotalSeconds > (double)ParametersManager.Instance.BonusQuest_timelapse_FC)
		{
			return true;
		}
		return false;
	}

	private bool IsBonusQuestVisible()
	{
		try
		{
			if (pInfo.BonusQuests[base.MapQuestType].ReplacedQuestID != 0)
			{
				return true;
			}
		}
		catch
		{
		}
		return false;
	}

	private bool ShouldMoveBonusQuest()
	{
		if (pInfo.GetBonusQuestMatchLapse(base.MapQuestType) > ParametersManager.Instance.BonusQuest_matchlapse_FC)
		{
			return true;
		}
		return false;
	}

	private void ShowBonusQuest()
	{
		BonusQuestStats bonusQuestStats = pInfo.BonusQuests[base.MapQuestType];
		int replacedQuestID = bonusQuestStats.ReplacedQuestID;
		QuestData questData;
		QuestData questData2;
		if (IsBonusQuestVisible())
		{
			if (ShouldMoveBonusQuest())
			{
				bonusQuestStats.LastReplacedQuestID = bonusQuestStats.ReplacedQuestID;
				questData = GetNextBonusQuestLocation();
				didShowBonusQuestLocation = false;
				questData2 = GetNextBonusQuest();
				pInfo.UpdateBonusQuestMatchStats(base.MapQuestType);
			}
			else
			{
				questData2 = QuestManager.Instance.GetQuestByID("tcat", bonusQuestStats.ActiveQuestID);
				questData = QuestManager.Instance.GetQuestByID(base.MapQuestType, bonusQuestStats.ReplacedQuestID);
			}
		}
		else
		{
			questData2 = GetNextBonusQuest();
			if (bonusQuestStats.ActiveQuestID == 0)
			{
				bonusQuestStats.firstAppearance = true;
				questData = QuestManager.Instance.GetQuestByID(base.MapQuestType, ParametersManager.Instance.BonusQuest_firstquest_FC);
			}
			else
			{
				questData = GetNextBonusQuestLocation();
			}
		}
		bonusQuestStats.ActiveQuestID = questData2.iQuestID;
		bonusQuestStats.ActiveQuest = questData2;
		bonusQuestStats.ReplacedQuestID = questData.iQuestID;
		if (questData.iQuestID == replacedQuestID)
		{
			didShowBonusQuestLocation = true;
		}
		else
		{
			didShowBonusQuestLocation = false;
		}
		QuestData questByID = QuestManager.Instance.GetQuestByID(base.MapQuestType, replacedQuestID);
		if (questByID != null)
		{
			UpdateQuestNode(questByID, questByID.GetState(), QuestMapInfo.GetRegionByID(questByID.RegionID).GetState());
		}
		if (replacedQuestID != questData.iQuestID)
		{
			UpdateQuestNode(questData, questData.GetState(), QuestMapInfo.GetRegionByID(questData.RegionID).GetState());
		}
		pInfo.Save();
	}

	private IEnumerator HideBonusQuest()
	{
		BonusQuestStats bonusStats = pInfo.BonusQuests[base.MapQuestType];
		int oldReplacedQuestID = bonusStats.ReplacedQuestID;
		if (bonusStats.ReplacedQuestID != 0)
		{
			QuestData bonusQuest = QuestManager.Instance.GetQuestByID("tcat", bonusStats.ActiveQuestID);
			bonusStats.ActiveQuestID = bonusQuest.iQuestID;
			bonusStats.ActiveQuest = bonusQuest;
			QuestData replacedQuest2 = QuestManager.Instance.GetQuestByID(base.MapQuestType, bonusStats.ReplacedQuestID);
			UpdateQuestNode(replacedQuest2, replacedQuest2.GetState(), QuestMapInfo.GetRegionByID(replacedQuest2.RegionID).GetState());
			yield return StartCoroutine(ShowBonusQuestVanish());
		}
		bonusStats.ActiveQuest = null;
		bonusStats.ReplacedQuestID = 0;
		if (oldReplacedQuestID != 0)
		{
			QuestData replacedQuest = QuestManager.Instance.GetQuestByID(base.MapQuestType, oldReplacedQuestID);
			UpdateQuestNode(replacedQuest, replacedQuest.GetState(), QuestMapInfo.GetRegionByID(replacedQuest.RegionID).GetState());
		}
		pInfo.Save();
		yield return null;
	}

	private IEnumerator ShowTreasureCatInstructions()
	{
		yield return new WaitForSeconds(2f);
		if (null != TreasureCatInstructionsShow)
		{
			TreasureCatInstructionsShow.Play(true);
		}
	}

	private IEnumerator UpdateBonusQuests()
	{
		try
		{
			if (!pInfo.BonusQuests.ContainsKey(base.MapQuestType))
			{
				pInfo.BonusQuests[base.MapQuestType] = new BonusQuestStats();
			}
			if (ShouldShowBonusQuest())
			{
				ShowBonusQuest();
			}
			else
			{
				StartCoroutine(HideBonusQuest());
			}
		}
		catch (Exception)
		{
		}
		yield return StartCoroutine(TryShowBonusQuest());
	}

	private IEnumerator TryShowBonusQuest()
	{
		if (didShowBonusQuestLocation)
		{
			yield break;
		}
		didShowBonusQuestLocation = true;
		BonusQuestStats bonusStats = pInfo.BonusQuests[base.MapQuestType];
		if (bonusStats.ActiveQuest == null)
		{
			yield break;
		}
		Vector3 oldCameraPos = base.mainCamera.transform.position;
		QuestData qd = QuestManager.Instance.GetQuestByID(base.MapQuestType, bonusStats.ReplacedQuestID);
		GameObject qNode = QuestMapInfo.GetQuestMapNode(qd);
		if (qNode != null)
		{
			yield return null;
			float duration3 = 1f;
			duration3 = TweenCameraOnFocus(qNode.transform.position, "ToRegion", true, "TryShowBonusQuest");
			iTween.FadeFrom(qNode, iTween.Hash("alpha", 0f, "delay", duration3, "time", 2f));
			yield return new WaitForSeconds(duration3);
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(GetComponent<AudioSource>(), bonusQuestAlertSound, true, false, SLOTAudioManager.AudioType.SFX);
			CWiTweenTrigger tweenTrigger = qNode.transform.GetComponentInChildren<CWiTweenTrigger>();
			if (tweenTrigger != null)
			{
				tweenTrigger.TriggerTweens("Spin");
				yield return new WaitForSeconds(2f);
			}
			if (pInfo.BonusQuests[base.MapQuestType].firstAppearance)
			{
				pInfo.BonusQuests[base.MapQuestType].firstAppearance = false;
				yield return StartCoroutine(ShowTreasureCatInstructions());
			}
			else
			{
				duration3 = TweenCameraOnFocus(oldCameraPos, "ToRegion", true, "Bonus back to Original Pos");
				yield return new WaitForSeconds(duration3);
			}
		}
	}

	private IEnumerator ShowBonusQuestVanish()
	{
		if (pInfo.BonusQuests[base.MapQuestType].ActiveQuest == null)
		{
			yield break;
		}
		Vector3 camPosFromPlayerPref = GetSavedCameraPos();
		QuestData qd = QuestManager.Instance.GetQuestByID(base.MapQuestType, pInfo.BonusQuests[base.MapQuestType].ReplacedQuestID);
		GameObject qNode = QuestMapInfo.GetQuestMapNode(qd);
		if (!(qNode != null))
		{
			yield break;
		}
		yield return null;
		float duration = TweenCameraOnFocus(qNode.transform.position, "ToRegion", true, "ShowBonusQuestVanish");
		yield return new WaitForSeconds(duration);
		CWiTweenTrigger tweenTrigger = qNode.transform.GetComponentInChildren<CWiTweenTrigger>();
		if (tweenTrigger != null)
		{
			tweenTrigger.TriggerTweens("Spin");
			iTween.FadeTo(qNode, 0f, 1f);
			yield return new WaitForSeconds(1f);
		}
		iTweenEvent scaler = qNode.GetComponent<iTweenEvent>();
		if ((bool)scaler)
		{
			scaler.enabled = true;
			yield return null;
			scaler.Play();
			if (scaler.Values.ContainsKey("time"))
			{
				duration = Convert.ToSingle(scaler.Values["time"]);
			}
			yield return new WaitForSeconds(duration);
		}
	}
}
