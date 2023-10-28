using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWResultFillTable : MonoBehaviour
{
	public GameObject LeaderPrefab;

	public GameObject CardPrefab;

	public PlayerType type;

	public GameObject RevealTween;

	public CWRevealCard RevealScript;

	public GameObject RevealAnimation;

	public GameObject HideTween;

	public float Interval = 0.3f;

	private bool currRevealing;

	private bool currWaiting;

	private float counter;

	private int currIndex;

	private int limit;

	private CWShowCard[] dropCards;

	public CWResultBannerAnimation BannerAnim;

	public GameObject FlipVFX;

	public void OnEnable()
	{
		reset();
		UpdateQuestGoal();
		StartCoroutine(FillTable(type));
	}

	public void reset()
	{
		foreach (Transform item in base.gameObject.transform)
		{
			Object.Destroy(item.gameObject);
		}
	}

	private void UpdateQuestGoal()
	{
		bool flag = false;
		string text = string.Empty;
		BattleResolver battleResolver = GameState.Instance.BattleResolver;
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		QuestData currentQuest = instance.GetCurrentQuest();
		int num = ((battleResolver == null) ? instance.GetQuestProgress(currentQuest) : battleResolver.questStars);
		if (num >= 0 && num < 3)
		{
			QuestConditionManager instance2 = QuestConditionManager.Instance;
			string conditionID = currentQuest.Condition[num];
			if (instance2.StatsMeetCondition(conditionID))
			{
				num = instance.IncQuestProgress(currentQuest);
				if (num == 3)
				{
					instance.Gems++;
				}
				text = instance2.ConditionDescription(conditionID);
				flag = true;
			}
		}
		Transform parent = base.transform.parent.parent;
		UILabel[] componentsInChildren = parent.GetComponentsInChildren<UILabel>(true);
		UILabel[] array = componentsInChildren;
		foreach (UILabel uILabel in array)
		{
			if (uILabel.name == "QuestGoal_Label")
			{
				uILabel.text = text;
			}
		}
		UISprite[] componentsInChildren2 = parent.GetComponentsInChildren<UISprite>(true);
		UISprite[] array2 = componentsInChildren2;
		foreach (UISprite uISprite in array2)
		{
			if (uISprite.name == "QuestGoal_Star")
			{
				uISprite.spriteName = ((!flag) ? "UI_Star_Empty" : "UI_Star_Full");
			}
		}
	}

	private GameObject AddItem(GameObject parent, GameObject prefab, float scale, int curItemNum)
	{
		GameObject gameObject = NGUITools.AddChild(parent, prefab);
		SQUtils.SetLayer(gameObject, parent.layer);
		gameObject.name = string.Format("EarnedCard{0:D3}", curItemNum);
		gameObject.transform.localScale = new Vector3(scale, scale, 1f);
		gameObject.GetComponent<Collider>().enabled = true;
		UIPanel component = gameObject.GetComponent<UIPanel>();
		if (component != null)
		{
			Object.Destroy(component);
		}
		return gameObject;
	}

	private GameObject AddCard(CardItem card, GameObject parent, GameObject prefab, float scale, int curItemNum, bool selectShowsX, GameObject revealTween, CWRevealCard revealScript)
	{
		if (card != null)
		{
			GameObject gameObject = AddItem(parent, prefab, scale, curItemNum);
			CWDeckCard component = gameObject.GetComponent<CWDeckCard>();
			if ((bool)component)
			{
				component.card = new CardItem(card.Form, card.DropLevel);
				component.SelectShowsX = selectShowsX;
			}
			PanelManagerBattle.GetInstance().ShowBlankCard(gameObject, card);
			CWShowCard cWShowCard = gameObject.AddComponent<CWShowCard>();
			cWShowCard.card = card;
			cWShowCard.tweenToObj = revealTween;
			cWShowCard.revealScript = revealScript;
			cWShowCard.RevealAnimation = RevealAnimation;
			cWShowCard.tweenToHide = HideTween;
			cWShowCard.FlipVFX = FlipVFX;
			UIButtonPlayAnimation component2 = gameObject.GetComponent<UIButtonPlayAnimation>();
			if (component2 != null)
			{
			}
			return gameObject;
		}
		return null;
	}

	private GameObject AddLeader(LeaderItem leader, GameObject parent, GameObject prefab, float scale, int curItemNum)
	{
		GameObject gameObject = AddItem(parent, prefab, scale, curItemNum);
		CWDeckHeroSelection.FillCardInfo(gameObject, leader);
		return gameObject;
	}

	private IEnumerator FillTable(PlayerType type)
	{
		PlayerInfoScript pinfo = PlayerInfoScript.GetInstance();
		QuestData qd = pinfo.GetCurrentQuest();
		UIGrid grid = base.gameObject.GetComponent<UIGrid>();
		List<CardItem> newCards = QuestEarningManager.GetInstance().earnedCards;
		int curItemNum = 0;
		if (!string.IsNullOrEmpty(qd.LeaderAwarded))
		{
			LeaderItem leader = LeaderManager.Instance.AddNewLeaderIfUnique(qd.LeaderAwarded);
			if (leader != null)
			{
				AddLeader(leader, base.gameObject, LeaderPrefab, 0.7f, curItemNum);
				grid.Reposition();
				curItemNum++;
			}
		}
		yield return null;
		for (int i = 0; i < newCards.Count; i++)
		{
			AddCard(newCards[i], base.gameObject, CardPrefab, 0.7f, curItemNum, true, RevealTween, RevealScript);
			grid.Reposition();
			curItemNum++;
			yield return null;
		}
	}

	public bool HaveAllBeenRevealed()
	{
		bool flag = true;
		CWShowCard[] componentsInChildren = base.gameObject.GetComponentsInChildren<CWShowCard>();
		CWShowCard[] array = componentsInChildren;
		foreach (CWShowCard cWShowCard in array)
		{
			flag = flag && cWShowCard.HasBeenShown();
		}
		return flag;
	}

	public void ForceShowAllDrops()
	{
		dropCards = base.gameObject.GetComponentsInChildren<CWShowCard>();
		currIndex = 0;
		limit = dropCards.Length;
		currWaiting = false;
		if (limit > 0)
		{
			currRevealing = true;
		}
	}

	private void Update()
	{
		if (!currRevealing || currWaiting)
		{
			return;
		}
		counter += Time.deltaTime;
		if (counter > Interval)
		{
			BannerAnim.SetCardData(dropCards[currIndex].card.Form);
			currWaiting = dropCards[currIndex].CanForceShowCard();
			currIndex++;
			if (currIndex >= limit)
			{
				currRevealing = false;
			}
			counter = 0f;
		}
	}

	public void ResumeAutomation()
	{
		currWaiting = false;
	}
}
