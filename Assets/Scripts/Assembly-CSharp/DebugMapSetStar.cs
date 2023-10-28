using UnityEngine;

public class DebugMapSetStar : MonoBehaviour
{
	private PlayerInfoScript pInfo;

	private DebugFlagsScript debugFlag;

	private bool setFlag0;

	private bool setFlag1;

	private bool setFlag2;

	private bool setFlag3;

	private void Start()
	{
		pInfo = PlayerInfoScript.GetInstance();
		debugFlag = DebugFlagsScript.GetInstance();
	}

	private void setStarToUnlockedQuests(int starCount)
	{
		MapControllerBase instance = MapControllerBase.GetInstance();
		if (pInfo == null)
		{
			PlayerInfoScript.GetInstance();
		}
		QuestData currentQuest = pInfo.GetCurrentQuest();
		pInfo.SetQuestProgress(currentQuest, starCount);
		GameObject questMapNode = instance.GetQuestMapNode(currentQuest);
		CWMapQuestInfoSet componentInChildren = questMapNode.GetComponentInChildren<CWMapQuestInfoSet>();
		instance.UpdateQuestNodeStars(componentInChildren, currentQuest.GetState());
	}

	private void Update()
	{
		if (debugFlag.mapDebug.set0Star && !setFlag0)
		{
			setStarToUnlockedQuests(0);
			setFlag0 = true;
			debugFlag.mapDebug.set1Star = (setFlag1 = false);
			debugFlag.mapDebug.set2Star = (setFlag2 = false);
			debugFlag.mapDebug.set3Star = (setFlag3 = false);
		}
		if (debugFlag.mapDebug.set1Star && !setFlag1)
		{
			setStarToUnlockedQuests(1);
			setFlag1 = true;
			debugFlag.mapDebug.set0Star = (setFlag0 = false);
			debugFlag.mapDebug.set2Star = (setFlag2 = false);
			debugFlag.mapDebug.set3Star = (setFlag3 = false);
		}
		if (debugFlag.mapDebug.set2Star && !setFlag2)
		{
			setStarToUnlockedQuests(2);
			setFlag2 = true;
			debugFlag.mapDebug.set0Star = (setFlag0 = false);
			debugFlag.mapDebug.set1Star = (setFlag1 = false);
			debugFlag.mapDebug.set3Star = (setFlag3 = false);
		}
		if (debugFlag.mapDebug.set3Star && !setFlag3)
		{
			setStarToUnlockedQuests(3);
			setFlag3 = true;
			debugFlag.mapDebug.set0Star = (setFlag0 = false);
			debugFlag.mapDebug.set1Star = (setFlag1 = false);
			debugFlag.mapDebug.set2Star = (setFlag2 = false);
		}
	}
}
