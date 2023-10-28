using System;
using UnityEngine;

public class SuccessStatsScript : MonoBehaviour
{
	public UILabel LblQuestID;

	public UILabel LblCardsVal;

	public UILabel LblCoinsVal;

	public UILabel LblXPVal;

	public UILabel LblXPBar;

	public UILabel LblConditionTxt;

	public UILabel LblConditionStatus;

	public UISprite SprConditionStatus;

	public UIFilledSprite SprXPBar;

	private QuestEarningManager earningMgr;

	private PlayerInfoScript pInfo;

	public float DelayInterval;

	public GameObject[] TweenControllers;

	private float counter;

	private int tweenIndex;

	private bool skipped;

	private bool complete = true;

	private bool paused;

	private bool fillXPBar;

	private float xpCounter;

	private float rolloverXP;

	private bool rollover;

	public float XPBarInterval = 1f;

	public GameObject LvlUpTween;

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
			BattleResolver battleResolver = GameState.Instance.BattleResolver;
			LeaderItem leader = GameState.Instance.GetLeader(PlayerType.User);
			int num = ((battleResolver == null) ? pInfo.GetQuestProgress(pInfo.GetCurrentQuest()) : battleResolver.questStars);
			QuestData activeQuest = GameState.Instance.ActiveQuest;
			int count = earningMgr.earnedCards.Count;
			int num2 = earningMgr.earnedCoin + activeQuest.CoinsRewarded;
			bool flag = false;
			string text = null;
			text = ((GameState.Instance.BattleResolver != null) ? battleResolver.questConditionId : ((num >= activeQuest.Condition.Length) ? activeQuest.Condition[activeQuest.Condition.Length - 1] : activeQuest.Condition[num]));
			QuestConditionManager instance = QuestConditionManager.Instance;
			if (instance != null)
			{
				flag = text != null && instance.StatsMeetCondition(text);
			}
			if (LblQuestID != null && activeQuest != null)
			{
				LblQuestID.text = string.Format(KFFLocalization.Get("!!FORMAT_QUEST"), activeQuest.QuestLabel);
			}
			if (LblCardsVal != null)
			{
				LblCardsVal.text = "x" + count;
			}
			if (LblXPVal != null && activeQuest != null)
			{
				LblXPVal.text = activeQuest.XPRewarded.ToString();
			}
			if (LblCoinsVal != null)
			{
				LblCoinsVal.text = num2.ToString();
			}
			if (LblConditionTxt != null && activeQuest != null && num >= 0)
			{
				if (num >= 3)
				{
					LblConditionTxt.text = KFFLocalization.Get("!!GOT_ALL_THREE_STARS");
				}
				else
				{
					string conditionDescription = activeQuest.GetConditionDescription(num);
					if (string.IsNullOrEmpty(conditionDescription))
					{
						LblConditionTxt.text = KFFLocalization.Get("!!MISSING_QUEST");
					}
					else
					{
						LblConditionTxt.text = conditionDescription;
					}
				}
			}
			if (LblConditionStatus != null && activeQuest != null)
			{
				LblConditionStatus.text = ((!flag) ? KFFLocalization.Get("!!FAILED") : KFFLocalization.Get("!!SUCCESS_EXCL"));
			}
			if (LblXPBar != null && leader != null)
			{
				LblXPBar.text = leader.XP + "/" + (leader.XP + leader.ToNextRank);
			}
			if (SprConditionStatus != null && activeQuest != null)
			{
				SprConditionStatus.spriteName = ((!flag) ? "UI_Star_Empty" : "UI_Star_Full");
			}
			if (SprXPBar != null)
			{
				SprXPBar.fillAmount = (float)leader.XP / (float)leader.ToNextRank;
			}
		}
		counter = 0f;
		tweenIndex = 0;
		skipped = false;
		complete = false;
		xpCounter = 0f;
		fillXPBar = false;
		rollover = false;
		rolloverXP = 0f;
	}

	private void Update()
	{
		if (!skipped && !complete && !fillXPBar && !paused)
		{
			counter += Time.deltaTime;
			if (counter >= DelayInterval)
			{
				StepForward();
			}
		}
		if (!fillXPBar || !(pInfo != null))
		{
			return;
		}
		QuestData activeQuest = GameState.Instance.ActiveQuest;
		LeaderItem leader = GameState.Instance.GetLeader(PlayerType.User);
		if (xpCounter < XPBarInterval)
		{
			xpCounter += Time.deltaTime;
			float num = (rollover ? (xpCounter / XPBarInterval * rolloverXP + (float)leader.XP) : (xpCounter / XPBarInterval * (float)activeQuest.XPRewarded + (float)leader.XP));
			float num2 = num / (float)(leader.XP + leader.ToNextRank);
			if (num2 >= 1f)
			{
				if (SprXPBar != null)
				{
					SprXPBar.fillAmount = 1f;
				}
				if (LblXPBar != null && leader != null)
				{
					LblXPBar.text = leader.XP + leader.ToNextRank + "/" + (leader.XP + leader.ToNextRank);
				}
				fillXPBar = false;
				paused = true;
				if (LvlUpTween != null)
				{
					LvlUpTween.SendMessage("OnClick");
				}
				rollover = true;
				rolloverXP = activeQuest.XPRewarded - leader.ToNextRank;
				leader.XP += leader.ToNextRank;
			}
			else
			{
				if (SprXPBar != null)
				{
					SprXPBar.fillAmount = num2;
				}
				if (LblXPBar != null && leader != null)
				{
					LblXPBar.text = (int)Math.Round(num) + "/" + (leader.XP + leader.ToNextRank);
				}
			}
		}
		else
		{
			fillXPBar = false;
			xpCounter = 0f;
			if (rollover)
			{
				leader.XP += leader.ToNextRank;
			}
			else
			{
				leader.XP += activeQuest.XPRewarded;
			}
		}
	}

	private void OnClick()
	{
		skipped = true;
		if (!complete)
		{
			StepForward();
		}
		skipped = false;
	}

	private void StepForward()
	{
		if (TweenControllers != null && TweenControllers.Length > 0)
		{
			GameObject gameObject = TweenControllers[tweenIndex];
			if (gameObject != null)
			{
				gameObject.SendMessage("OnClick");
			}
		}
		tweenIndex++;
		if (tweenIndex >= TweenControllers.Length)
		{
			complete = true;
			GetComponent<Collider>().enabled = false;
		}
		else
		{
			counter = 0f;
		}
	}

	public void XPBarFill()
	{
		xpCounter = 0f;
		fillXPBar = true;
		paused = false;
		skipped = false;
	}
}
