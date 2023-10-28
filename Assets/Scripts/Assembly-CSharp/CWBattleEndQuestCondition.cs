using System.Collections;
using UnityEngine;

public class CWBattleEndQuestCondition : MonoBehaviour
{
	public Transform[] conditionTr;

	private PlayerInfoScript pInfo;

	public GameObject flashBG;

	public GameObject vfxObj;

	private void OnEnable()
	{
		pInfo = PlayerInfoScript.GetInstance();
		QuestData currentQuest = pInfo.GetCurrentQuest();
		bool flag = false;
		int questProgress = pInfo.GetQuestProgress(currentQuest);
		QuestConditionManager instance = QuestConditionManager.Instance;
		string conditionID = ((questProgress >= currentQuest.Condition.Length) ? currentQuest.Condition[currentQuest.Condition.Length - 1] : currentQuest.Condition[questProgress]);
		if (instance != null)
		{
			flag = instance.StatsMeetCondition(conditionID);
		}
		flashBG.transform.localPosition = conditionTr[questProgress].transform.localPosition;
		int num = 0;
		for (int i = 0; i < currentQuest.Condition.Length; i++)
		{
			UILabel componentInChildren = conditionTr[i].GetComponentInChildren<UILabel>();
			componentInChildren.text = currentQuest.Condition[i];
			componentInChildren.color = ((i >= questProgress) ? Color.black : Color.grey);
			UISprite componentInChildren2 = conditionTr[i].GetComponentInChildren<UISprite>();
			componentInChildren2.spriteName = ((i >= questProgress) ? "UI_Star_Empty" : "UI_Star_Full");
			if (i == questProgress)
			{
				num = i;
			}
		}
		if (flag)
		{
			StartCoroutine(MeetCondition(num));
		}
	}

	private IEnumerator MeetCondition(int num)
	{
		iTween.StopByName("Spin");
		yield return new WaitForSeconds(1f);
		UILabel lbl = conditionTr[num].GetComponentInChildren<UILabel>();
		TweenColor tw = lbl.GetComponent<TweenColor>();
		tw.from = Color.black;
		tw.enabled = true;
		UISprite sp = conditionTr[num].GetComponentInChildren<UISprite>();
		sp.spriteName = "UI_Star_Full";
		iTweenEvent tweenEvent2 = iTweenEvent.GetEvent(sp.gameObject, "PunchScale");
		if (tweenEvent2 != null)
		{
			tweenEvent2.Play();
		}
		tweenEvent2 = iTweenEvent.GetEvent(sp.gameObject, "Spin");
		if (tweenEvent2 != null)
		{
			tweenEvent2.Play();
		}
		Transform targetTr = base.transform.Find("FX_Target");
		SpawnFX(targetTr);
	}

	private void SpawnFX(Transform targetTr)
	{
		if (vfxObj != null)
		{
			GameObject gameObject = SLOTGame.InstantiateFX(vfxObj, targetTr.position, vfxObj.transform.rotation) as GameObject;
			gameObject.transform.parent = targetTr.parent;
		}
	}

	private void Update()
	{
	}
}
