using System.Collections.Generic;
using UnityEngine;

public class DebugUnlockQuests : MonoBehaviour
{
	private DebugFlagsScript debugFlag;

	private QuestManager qMgr;

	public int setLastClearedQuest;

	public GameObject debugPrefab;

	public GameObject debugPrefabObj;

	private PlayerInfoScript pInfo;

	private bool unlockedFlag;

	private bool displayFlag;

	private bool isCompQuest89;

	private bool isCompQuest104;

	private bool isCompQuest119;

	private void Start()
	{
		debugFlag = DebugFlagsScript.GetInstance();
		qMgr = QuestManager.Instance;
		pInfo = PlayerInfoScript.GetInstance();
	}

	private void SetButtonActive(GameObject obj, bool enable)
	{
		BoxCollider component = obj.GetComponent<BoxCollider>();
		component.enabled = enable;
		UISprite componentInChildren = obj.GetComponentInChildren<UISprite>();
		componentInChildren.enabled = enable;
		displayFlag = enable;
	}

	private void CompSpecificQuest(int questNo)
	{
		MapControllerBase instance = MapControllerBase.GetInstance();
		QuestData currentQuest = pInfo.GetCurrentQuest();
		string questType = currentQuest.QuestType;
		QuestData questByID = qMgr.GetQuestByID(questType, questNo);
		if (questByID != null)
		{
			pInfo.SetLastClearedQuest(questByID);
			List<QuestData> questsByType = qMgr.GetQuestsByType(questType);
			for (int i = questByID.iQuestIndex; i < questsByType.Count; i++)
			{
				pInfo.ResetQuestProgress(questsByType[i]);
			}
			instance.QuestMapRefresh();
			pInfo.Save();
		}
	}

	private void ResetQuestCompStat()
	{
		MapControllerBase instance = MapControllerBase.GetInstance();
		QuestData currentQuest = pInfo.GetCurrentQuest();
		QuestData firstQuest = QuestManager.Instance.GetFirstQuest(currentQuest.QuestType);
		pInfo.SetCurrentQuest(firstQuest);
		pInfo.ResetLastClearedQuestID(currentQuest.QuestType);
		pInfo.ResetQuestProgress(currentQuest.QuestType);
		pInfo.Save();
		instance.QuestMapRefresh();
	}

	private void Update()
	{
		if (debugFlag.mapDebug.compQuest89 && !isCompQuest89)
		{
			CompSpecificQuest(89);
			isCompQuest89 = true;
		}
		if (!debugFlag.mapDebug.compQuest89 && isCompQuest89)
		{
			ResetQuestCompStat();
			isCompQuest89 = false;
		}
		if (debugFlag.mapDebug.compQuest104 && !isCompQuest104)
		{
			CompSpecificQuest(104);
			isCompQuest104 = true;
		}
		if (!debugFlag.mapDebug.compQuest104 && isCompQuest104)
		{
			ResetQuestCompStat();
			isCompQuest104 = false;
		}
		if (debugFlag.mapDebug.compQuest119 && !isCompQuest119)
		{
			CompSpecificQuest(119);
			isCompQuest119 = true;
		}
		if (!debugFlag.mapDebug.compQuest119 && isCompQuest119)
		{
			ResetQuestCompStat();
			isCompQuest119 = false;
		}
		if (debugFlag.mapDebug.unlockQuest && !unlockedFlag)
		{
			MapControllerBase instance = MapControllerBase.GetInstance();
			List<QuestData> questsByType = qMgr.GetQuestsByType(pInfo.CurrentQuestType);
			QuestData lastClearedQuest = pInfo.GetLastClearedQuest();
			int num = setLastClearedQuest;
			if (num <= 0 || num > questsByType.Count)
			{
				num = questsByType.Count - 1;
			}
			pInfo.SetLastClearedQuest(questsByType[num]);
			for (int i = lastClearedQuest.iQuestIndex; i < questsByType.Count; i++)
			{
				pInfo.ResetQuestProgress(questsByType[i]);
			}
			instance.QuestMapRefresh();
			pInfo.Save();
			unlockedFlag = true;
		}
		if (!debugFlag.mapDebug.unlockQuest && unlockedFlag)
		{
			ResetQuestCompStat();
			unlockedFlag = false;
		}
		if (debugFlag.mapDebug.finishMapQuests && !displayFlag)
		{
			if (debugPrefabObj == null)
			{
				GameObject gameObject = GameObject.Find("_debugButtonParent");
				if (gameObject == null)
				{
					gameObject = base.gameObject;
				}
				debugPrefabObj = Object.Instantiate(debugPrefab, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
				if (gameObject != null)
				{
					debugPrefabObj.transform.parent = gameObject.transform;
				}
			}
			SetButtonActive(debugPrefabObj, true);
		}
		if (!debugFlag.mapDebug.finishMapQuests && displayFlag)
		{
			SetButtonActive(debugPrefabObj, false);
		}
	}
}
