using System.Collections;
using UnityEngine;

public class CWBattleEndWinnerStats : MonoBehaviour
{
	public UILabel questIDLabel;

	public UILabel cardCountLabel;

	public UILabel earnedCoinLabel;

	public UILabel statCoinLabel;

	public UILabel earnedXPLabel;

	public UILabel currentXPLabel;

	public UILabel xpToNextLabel;

	public UISprite xpBar;

	public UILabel statCurrentXPLabel;

	public UILabel statXPToNextLabel;

	public UISprite statXPBar;

	public UILabel statRankLabel;

	public float DelayInterval;

	public GameObject[] TweenControllers;

	public GameObject lvlUpTween;

	public GameObject lvlUpDismissTween;

	public GameObject questConditionTween;

	public UISprite[] stars;

	public Transform targetTr;

	public GameObject starFX;

	public UILabel descOnPanel;

	private QuestEarningManager earningMgr;

	private SLOTAudioManager audioMgr;

	private QuestData qd;

	private LeaderItem leaderCard;

	private CWBattleEndPlayerStats battleSts;

	private int prevXP;

	private int earnedCoinCount;

	private int currentCoin;

	private int coinToAdd;

	private int xpTotalToPrev;

	private int xpTotalToNext;

	private int earnedXP;

	private int currentXP;

	public int currentHP;

	private int xpToAdd;

	public int currentRank;

	private int alreadyAddedXP;

	private UISprite currentSp;

	private bool conditionComplete;

	public float XPBarInterval = 2f;

	public UILabel debugConterLabel;

	public UILabel debugXPAnimateFlag;

	private int rankUpCount;

	private bool _keyPressed;

	private void Start()
	{
		earningMgr = QuestEarningManager.GetInstance();
		audioMgr = SLOTGameSingleton<SLOTAudioManager>.GetInstance();
	}

	private void OnEnable()
	{
		earningMgr = QuestEarningManager.GetInstance();
		battleSts = CWBattleEndPlayerStats.GetInstance();
		qd = GameState.Instance.ActiveQuest;
		leaderCard = GameState.Instance.GetLeader(PlayerType.User);
		RefreshUI();
		StartCoroutine(StepForward());
		Singleton<AnalyticsManager>.Instance.LogEndResultsEvents();
	}

	private IEnumerator StepForward()
	{
		yield return new WaitForSeconds(DelayInterval);
		yield return StartCoroutine(TriggerTween(TweenControllers[0]));
		yield return StartCoroutine(AnimateCoinEarned());
		yield return StartCoroutine(TriggerTween(TweenControllers[1]));
		currentRank = battleSts.ssRank;
		currentHP = battleSts.ssHP;
		currentXP = battleSts.ssXP;
		prevXP = currentXP;
		while (currentXP < battleSts.ssXP + earnedXP)
		{
			prevXP += xpToAdd;
			alreadyAddedXP += xpToAdd;
			battleSts.rankLabel.text = currentRank.ToString();
			yield return StartCoroutine(AnimateXpEarned());
		}
		yield return StartCoroutine(TriggerTween(TweenControllers[2]));
		if (conditionComplete)
		{
			yield return StartCoroutine(MeetCondition());
		}
		else
		{
			yield return new WaitForSeconds(1f);
		}
		yield return StartCoroutine(TriggerTween(TweenControllers[3]));
	}

	private IEnumerator AnimateCoinEarned()
	{
		float timer = 0f;
		audioMgr.PlaySound(GetComponent<AudioSource>());
		while (timer != DelayInterval)
		{
			timer += Time.deltaTime;
			coinToAdd = Mathf.RoundToInt(timer / XPBarInterval * (float)earnedCoinCount);
			earnedCoinLabel.text = coinToAdd.ToString();
			statCoinLabel.text = (currentCoin + coinToAdd).ToString();
			if (coinToAdd >= earnedCoinCount)
			{
				coinToAdd = earnedCoinCount;
				earnedCoinLabel.text = coinToAdd.ToString();
				statCoinLabel.text = (currentCoin + coinToAdd).ToString();
				GetComponent<AudioSource>().Stop();
				timer = DelayInterval;
			}
			yield return new WaitForSeconds(Time.deltaTime);
		}
	}

	private IEnumerator AnimateXpEarned()
	{
		float timer = 0f;
		audioMgr.PlaySound(GetComponent<AudioSource>());
		while (timer != DelayInterval)
		{
			timer += Time.deltaTime;
			xpToAdd = Mathf.FloorToInt(timer / XPBarInterval * (float)earnedXP);
			currentXP = prevXP + xpToAdd;
			if (alreadyAddedXP + xpToAdd >= earnedXP)
			{
				timer = DelayInterval;
				GetComponent<AudioSource>().Stop();
			}
			else
			{
				earnedXPLabel.text = (alreadyAddedXP + xpToAdd).ToString();
				statCurrentXPLabel.text = Mathf.Min(xpTotalToNext, currentXP).ToString();
				statXPToNextLabel.text = xpTotalToNext.ToString();
				xpToNextLabel.text = Mathf.Max(xpTotalToNext - currentXP, 0).ToString();
				UISprite uISprite = xpBar;
				float fillAmount = (float)(currentXP - xpTotalToPrev) / (float)(xpTotalToNext - xpTotalToPrev);
				statXPBar.fillAmount = fillAmount;
				uISprite.fillAmount = fillAmount;
			}
			if (currentXP >= xpTotalToNext)
			{
				rankUpCount++;
				currentRank++;
				currentHP += 5;
				lvlUpTween.SendMessage("OnClick");
				GetComponent<AudioSource>().Stop();
				RefreshXPvariables();
				timer = DelayInterval;
				yield return StartCoroutine(WaitForKeyPress(lvlUpDismissTween));
			}
			yield return new WaitForSeconds(Time.deltaTime);
		}
	}

	private void RefreshUI()
	{
		earnedCoinCount = earningMgr.earnedCoin + qd.CoinsRewarded;
		currentCoin = battleSts.ssCoin;
		earnedCoinLabel.text = "0";
		int count = earningMgr.earnedCards.Count;
		cardCountLabel.text = count.ToString();
		if (qd.IsQuestType("elfisto"))
		{
			questIDLabel.text = KFFLocalization.Get("!!FC_EL_FISTO_ROUND", "<val>", qd.QuestLabel);
		}
		else
		{
			questIDLabel.text = string.Format(KFFLocalization.Get("!!FORMAT_QUEST"), qd.QuestLabel);
		}
		earnedXP = qd.XPRewarded;
		earnedXPLabel.text = "0";
		RefreshXPvariables();
		RefreshQuestCondition();
	}

	private void RefreshQuestCondition()
	{
		int ssNumStars = battleSts.ssNumStars;
		string text = string.Empty;
		if (ssNumStars >= 0)
		{
			Singleton<AnalyticsManager>.Instance.LogQuestStars(ssNumStars);
			QuestConditionManager instance = QuestConditionManager.Instance;
			string text2 = ((ssNumStars >= qd.Condition.Length) ? string.Empty : qd.Condition[ssNumStars]);
			if (instance != null && text2 != string.Empty)
			{
				conditionComplete = instance.StatsMeetCondition(text2);
				text = instance.ConditionDescription(text2);
			}
		}
		else
		{
			conditionComplete = false;
		}
		descOnPanel.text = text;
		for (int i = 0; i < qd.Condition.Length; i++)
		{
			stars[i].spriteName = ((i >= ssNumStars) ? "UI_Star_Empty" : "UI_Star_Full");
			if (i == ssNumStars)
			{
				currentSp = stars[i];
			}
		}
	}

	private IEnumerator MeetCondition()
	{
		yield return new WaitForSeconds(0.5f);
		currentSp.spriteName = "UI_Star_Full";
		iTweenEvent tweenEvent2 = iTweenEvent.GetEvent(currentSp.gameObject, "PunchScale");
		if (tweenEvent2 != null)
		{
			tweenEvent2.Play();
		}
		tweenEvent2 = iTweenEvent.GetEvent(currentSp.gameObject, "Spin");
		if (tweenEvent2 != null)
		{
			tweenEvent2.Play();
		}
		targetTr.localPosition = currentSp.transform.localPosition;
		SpawnFX(targetTr);
		yield return new WaitForSeconds(0.5f);
		questConditionTween.SendMessage("OnClick");
		yield return new WaitForSeconds(2f);
	}

	private void SpawnFX(Transform targetTr)
	{
		if (starFX != null)
		{
			GameObject gameObject = SLOTGame.InstantiateFX(starFX, targetTr.position, starFX.transform.rotation) as GameObject;
			gameObject.transform.parent = targetTr.parent;
		}
	}

	private void Update()
	{
	}

	private void RefreshXPvariables()
	{
		xpTotalToPrev = XPManager.Instance.FindRequiredXP(leaderCard.Form.LvUpSchemeID, battleSts.ssRank + rankUpCount);
		xpTotalToNext = XPManager.Instance.FindRequiredXP(leaderCard.Form.LvUpSchemeID, battleSts.ssRank + rankUpCount + 1);
	}

	private IEnumerator WaitForKeyPress(GameObject tween)
	{
		while (!_keyPressed)
		{
			if (Input.GetMouseButtonDown(0))
			{
				if (tween != null)
				{
					tween.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
				}
				yield return null;
				break;
			}
			yield return 0;
		}
	}

	private IEnumerator TriggerTween(GameObject obj)
	{
		yield return 0;
		obj.SendMessage("OnClick");
	}
}
