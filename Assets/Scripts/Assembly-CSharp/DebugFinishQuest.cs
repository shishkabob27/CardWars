using System;
using UnityEngine;

public class DebugFinishQuest : MonoBehaviour
{
	public int starCount;

	private PlayerInfoScript pInfo;

	private void Start()
	{
		pInfo = PlayerInfoScript.GetInstance();
	}

	private void OnClick()
	{
		MapControllerBase instance = MapControllerBase.GetInstance();
		QuestData lastClearedQuest = pInfo.GetLastClearedQuest();
		QuestData currentQuest = pInfo.GetCurrentQuest();
		pInfo.IncQuestProgress(currentQuest);
		QuestData nextQuest = QuestManager.Instance.GetNextQuest(currentQuest);
		pInfo.SetLastClearedQuest(currentQuest);
		pInfo.SetCurrentQuest(nextQuest);
		pInfo.Save();
		GlobalFlags instance2 = GlobalFlags.Instance;
		instance2.ReturnToMainMenu = true;
		instance2.NewlyCleared = true;
		instance2.lastQuestConditionStatus = Math.Max(starCount - 1, 0);
		instance2.Cleared = true;
		instance.QuestMapRefresh();
	}

	private void Update()
	{
	}
}
