using System.Collections;
using UnityEngine;

public class QuestNodeController : MonoBehaviour
{
	public MonoBehaviour[] disableWhenNotSelected;

	public UIButtonTween iconWarpTween;

	private bool hasDisplayedPlayerIconTween;

	private bool isRunningQuestNodeOnClick;

	private void Start()
	{
	}

	private void OnEnable()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		CWMapQuestInfoSet component = GetComponent<CWMapQuestInfoSet>();
		if (component != null && component.questData != null && component.questData.iQuestID == instance.GetCurrentQuestID())
		{
			Enable(true);
			hasDisplayedPlayerIconTween = true;
		}
		else
		{
			Enable(false);
		}
	}

	private void OnClick()
	{
		if (MapControllerBase.GetInstance().MapQuestType == "fc" && TutorialMonitor.Instance != null && TutorialMonitor.Instance.ShouldTriggerTutorial(TutorialTrigger.FC_LeaderTutorial))
		{
			TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.FC_LeaderTutorial);
		}
		else if (base.enabled && !hasDisplayedPlayerIconTween && !isRunningQuestNodeOnClick)
		{
			isRunningQuestNodeOnClick = true;
			StartCoroutine(QuestNodeOnClick());
		}
	}

	private IEnumerator QuestNodeOnClick()
	{
		CWMapQuestInfoSet qInfo = GetComponent<CWMapQuestInfoSet>();
		if (qInfo != null)
		{
			PlayerInfoScript pInfo = PlayerInfoScript.GetInstance();
			string lastQType = pInfo.CurrentQuestType;
			string mapType = MapControllerBase.GetInstance().MapQuestType;
			pInfo.SetCurrentQuest(qInfo.questData);
			QuestData qdCurrent2 = null;
			qdCurrent2 = ((!(lastQType == "tcat") && !(lastQType == "elfisto")) ? PlayerInfoScript.GetInstance().GetLastClearedQuest(mapType) : PlayerInfoScript.GetInstance().GetLastClearedQuest(lastQType));
			if (qdCurrent2 != null && qInfo.questData != qdCurrent2)
			{
				GameObject qnode2 = null;
				if (lastQType == "tcat")
				{
					int qID = pInfo.BonusQuests[mapType].ReplacedQuestID;
					QuestData rQuest = QuestManager.Instance.GetQuestByID(mapType, qID);
					qnode2 = MapControllerBase.GetInstance().GetQuestMapNode(rQuest);
				}
				else
				{
					qnode2 = MapControllerBase.GetInstance().GetQuestMapNode(qdCurrent2);
				}
				if (qnode2 != null)
				{
					QuestNodeController qnControl = qnode2.GetComponentInChildren<QuestNodeController>();
					if ((bool)qnControl)
					{
						qnControl.hasDisplayedPlayerIconTween = false;
						qnControl.Enable(false);
					}
				}
				PlayerInfoScript.GetInstance().SetLastClearedQuest(qInfo.questData);
				yield return StartCoroutine(PlayerIconMoveSequence(qInfo));
			}
			Enable(true);
			PlayerInfoScript.GetInstance().Save();
			hasDisplayedPlayerIconTween = true;
			SendMessage("OnClick");
		}
		isRunningQuestNodeOnClick = false;
	}

	private IEnumerator PlayerIconMoveSequence(CWMapQuestInfoSet qInfo)
	{
		MapControllerBase mapController = MapControllerBase.GetInstance();
		TweenScale scaler = mapController.playerIcon.GetComponent<TweenScale>();
		if ((bool)scaler)
		{
			scaler.enabled = true;
			yield return null;
			scaler.Play(true);
			yield return new WaitForSeconds(scaler.duration);
		}
		Vector3 pos = qInfo.gameObject.transform.position;
		mapController.playerIcon.transform.position = pos;
		if (mapController.vfxPrefab == null)
		{
			mapController.vfxPrefab = SLOTGame.InstantiateFX(mapController.vfx) as GameObject;
			if (mapController.QuestMapRoot != null)
			{
				mapController.vfxPrefab.transform.parent = mapController.QuestMapRoot.transform;
			}
		}
		mapController.vfxPrefab.SetActive(true);
		mapController.vfxPrefab.transform.position = new Vector3(pos.x, pos.y + 3f, pos.z);
		if ((bool)scaler)
		{
			scaler.Play(false);
			yield return new WaitForSeconds(scaler.duration);
			scaler.enabled = false;
		}
	}

	private void Enable(bool enable)
	{
		MonoBehaviour[] array = disableWhenNotSelected;
		foreach (MonoBehaviour monoBehaviour in array)
		{
			monoBehaviour.enabled = enable;
		}
	}
}
