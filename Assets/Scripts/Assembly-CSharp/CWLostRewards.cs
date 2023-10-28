using UnityEngine;

public class CWLostRewards : MonoBehaviour
{
	public UILabel LostCards;

	public UILabel LostCoins;

	public UILabel LostXP;

	public UILabel gemYouHave;

	private QuestEarningManager earningMgr;

	private PlayerInfoScript pInfo;

	private void OnEnable()
	{
		earningMgr = QuestEarningManager.GetInstance();
		pInfo = PlayerInfoScript.GetInstance();
		RefreshUI();
	}

	private void RefreshUI()
	{
		if (earningMgr != null && pInfo != null)
		{
			int count = earningMgr.earnedCards.Count;
			QuestData activeQuest = GameState.Instance.ActiveQuest;
			int earnedCoin = earningMgr.earnedCoin;
			if (LostCoins != null)
			{
				LostCoins.text = earnedCoin.ToString();
			}
			if (LostCards != null)
			{
				LostCards.text = "x" + count;
			}
			if (LostXP != null && activeQuest != null)
			{
				LostXP.text = activeQuest.XPRewarded.ToString();
			}
			gemYouHave.text = string.Format(KFFLocalization.Get("!!BS_M_9_GEMS"), pInfo.Gems.ToString());
		}
	}

	private void Update()
	{
	}

	private void OnClick()
	{
	}
}
