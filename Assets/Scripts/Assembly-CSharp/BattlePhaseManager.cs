using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePhaseManager : MonoBehaviour
{
	public BattlePhase Phase;

	public BattlePhase currentPhase;

	public BattlePhase prevPhase;

	public GameObject currentCreature;

	public string currentCardID;

	public bool newTurn;

	public GameObject tweenToP1Setup;

	public GameObject tweenToP1Setup_LabelOnly;

	public GameObject tweenToP1SetupAction;

	public GameObject tweenToP1SetupActionRareCard;

	public GameObject tweenToP1SetupActionSpell;

	public GameObject tweenToP1FloopAction;

	public GameObject tweenToP1FloopActionHero;

	public GameObject tweenToP1LeaderAbility;

	public GameObject tweenToP1LeaderAbilityAction;

	public GameObject tweenToP1LeaderAbilityActionHero;

	public GameObject tweenToP1SpellCreature;

	public GameObject tweenToP1SpellHero;

	public GameObject tweenToP2Setup;

	public GameObject tweenToP2Setup_LabelOnly;

	public GameObject tweenToP2SetupAction;

	public GameObject tweenToP2SetupActionRareCard;

	public GameObject tweenToP2SetupActionSpell;

	public GameObject tweenToP2FloopAction;

	public GameObject tweenToP2FloopActionHero;

	public GameObject tweenToP2LeaderAbility;

	public GameObject tweenToP2LeaderAbilityAction;

	public GameObject tweenToP2LeaderAbilityActionHero;

	public GameObject tweenToP2SpellCreature;

	public GameObject tweenToP2SpellHero;

	public GameObject tweenToP1SetupLanePlyr;

	public GameObject tweenToP1SetupLaneOpp;

	public GameObject tweenToP1Battle;

	public GameObject tweenToLootAfterP1Battle;

	public GameObject tweenToP2Battle;

	public GameObject tweenToLootAfterP2Battle;

	public GameObject tweenToResultP1Defeated;

	public GameObject tweenToResultP2Defeated;

	public GameObject tweenToResultP1Win;

	public GameObject tweenToResultP1WinPanel;

	public GameObject tweenToResultP1Lose;

	public GameObject tweenToResultP1LosePanel;

	public GameObject tweenToResultP1Dweeb;

	public GameObject tweenToResultP1MPWin;

	public GameObject tweenToResultP1MPWinPanel;

	public GameObject tweenToResultP1MPLose;

	public GameObject tweenToResultP1MPLosePanel;

	public GameObject tweenToOutOfCard;

	public GameObject tweenToReshuffle;

	public GameObject tweenToP2Win;

	public GameObject tweenToP2Dweeb;

	public GameObject tweenToGameOver;

	public List<string> alreadySummonedCard = new List<string>();

	public GameObject winnerResult;

	public GameObject loserResult;

	public GameObject tweenToPosgameWin;

	public GameObject tweenToPosgameLose;

	public GameObject tweenToBannerP1;

	public GameObject tweenToBannerP2;

	public GameObject tweenToBattleBannerP1;

	public GameObject tweenToBattleBannerP2;

	private GameDataScript gameData;

	private static BattlePhaseManager g_battlePhaseManager;

	private int prevGameDataTurn;

	private void Awake()
	{
		g_battlePhaseManager = this;
	}

	public static BattlePhaseManager GetInstance()
	{
		return g_battlePhaseManager;
	}

	private void Start()
	{
		gameData = GameDataScript.GetInstance();
	}

	private void Update()
	{
		if (!BattleManagerScript.GetInstance().BattlePaused && Phase != currentPhase && (GameDataScript.GetInstance().Turn != 1 || Phase != BattlePhase.P1Battle))
		{
			if (gameData.Turn != prevGameDataTurn)
			{
				newTurn = true;
				prevGameDataTurn = gameData.Turn;
			}
			else
			{
				newTurn = false;
			}
			prevPhase = currentPhase;
			currentPhase = Phase;
			GameObject tweenTarget = GetTweenTarget(currentPhase);
			bool flag = false;
			if (tweenTarget != null && !flag)
			{
				tweenTarget.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			}
			if (currentPhase == BattlePhase.P1Setup && newTurn)
			{
				tweenToP1Setup_LabelOnly.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			}
			else if (currentPhase == BattlePhase.P2Setup && newTurn)
			{
				tweenToP2Setup_LabelOnly.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private GameObject GetTweenTarget(BattlePhase currentPhase)
	{
		switch (currentPhase)
		{
		case BattlePhase.P1Setup:
			return tweenToP1Setup;
		case BattlePhase.P2Setup:
			return tweenToP2Setup;
		case BattlePhase.P1Battle:
			return tweenToP1Battle;
		case BattlePhase.P2Battle:
			return tweenToP2Battle;
		case BattlePhase.LootAfterP1Battle:
			return tweenToLootAfterP1Battle;
		case BattlePhase.LootAfterP2Battle:
			return tweenToLootAfterP2Battle;
		case BattlePhase.P1SetupAction:
			return tweenToP1SetupAction;
		case BattlePhase.P1SetupActionRareCard:
			return tweenToP1SetupActionRareCard;
		case BattlePhase.P2SetupActionRareCard:
			return tweenToP2SetupActionRareCard;
		case BattlePhase.P1SetupActionSpell:
			return tweenToP1SetupActionSpell;
		case BattlePhase.P2SetupActionSpell:
			return tweenToP2SetupActionSpell;
		case BattlePhase.P1FloopAction:
			return tweenToP1FloopAction;
		case BattlePhase.P1FloopActionHero:
			return tweenToP1FloopActionHero;
		case BattlePhase.P2SetupAction:
			return tweenToP2SetupAction;
		case BattlePhase.P2FloopAction:
			return tweenToP2FloopAction;
		case BattlePhase.P2FloopActionHero:
			return tweenToP2FloopActionHero;
		case BattlePhase.P1SetupLanePlyr:
			return tweenToP1SetupLanePlyr;
		case BattlePhase.P1SetupLaneOpp:
			return tweenToP1SetupLaneOpp;
		case BattlePhase.P1SpellCreature:
			return tweenToP1SpellCreature;
		case BattlePhase.P1SpellHero:
			return tweenToP1SpellHero;
		case BattlePhase.P2SpellCreature:
			return tweenToP2SpellCreature;
		case BattlePhase.P2SpellHero:
			return tweenToP2SpellHero;
		case BattlePhase.P1LeaderAbility:
			return tweenToP1LeaderAbility;
		case BattlePhase.P2LeaderAbility:
			return tweenToP2LeaderAbility;
		case BattlePhase.P1LeaderAbilityAction:
			return tweenToP1LeaderAbilityAction;
		case BattlePhase.P2LeaderAbilityAction:
			return tweenToP2LeaderAbilityAction;
		case BattlePhase.P1LeaderAbilityActionHero:
			return tweenToP1LeaderAbilityActionHero;
		case BattlePhase.P2LeaderAbilityActionHero:
			return tweenToP2LeaderAbilityActionHero;
		case BattlePhase.Result_P1Defeated:
			return tweenToResultP1Defeated;
		case BattlePhase.Result_P2Defeated:
			return tweenToResultP2Defeated;
		case BattlePhase.Result_P1Win:
			if (GlobalFlags.Instance.InMPMode)
			{
				return tweenToResultP1MPWin;
			}
			return tweenToResultP1Win;
		case BattlePhase.Result_P1WinPanel:
			return tweenToResultP1WinPanel;
		case BattlePhase.Result_P1MPWinPanel:
			return tweenToResultP1MPWinPanel;
		case BattlePhase.Result_P1Lose:
			if (GlobalFlags.Instance.InMPMode)
			{
				return tweenToResultP1MPLose;
			}
			return tweenToResultP1Lose;
		case BattlePhase.Result_P1LosePanel:
			return tweenToResultP1LosePanel;
		case BattlePhase.Result_P1MPLosePanel:
			return tweenToResultP1MPLosePanel;
		case BattlePhase.Result_P1Dweeb:
			return tweenToResultP1Dweeb;
		case BattlePhase.P1BattleBanner:
			return tweenToBattleBannerP1;
		case BattlePhase.P2BattleBanner:
			return tweenToBattleBannerP2;
		case BattlePhase.P1SetupBanner:
			return tweenToBannerP1;
		case BattlePhase.P2SetupBanner:
			return tweenToBannerP2;
		case BattlePhase.OutOfCards:
			return tweenToOutOfCard;
		case BattlePhase.Reshuffling:
			return tweenToReshuffle;
		case BattlePhase.P2Win:
			return tweenToP2Win;
		case BattlePhase.P2Dweeb:
			return tweenToP2Dweeb;
		case BattlePhase.GameOver:
			return tweenToGameOver;
		case BattlePhase.PostgameWin:
			return tweenToPosgameWin;
		case BattlePhase.PostgameLose:
			return tweenToPosgameLose;
		default:
			return null;
		}
	}

	public void ActivateWinnerTween()
	{
		StartCoroutine(SetResultTween(3.5f, winnerResult));
	}

	public void ActivateLoserTween()
	{
		StartCoroutine(SetResultTween(3.5f, loserResult));
	}

	private IEnumerator SetResultTween(float delay, GameObject resultScreen)
	{
		yield return new WaitForSeconds(delay);
		if (resultScreen != null)
		{
			resultScreen.SendMessage("OnClick");
		}
	}

	public void SetPhase(float delay, BattlePhase phase)
	{
		StartCoroutine(SetPhaseWithDelay(delay, phase));
	}

	private IEnumerator SetPhaseWithDelay(float delay, BattlePhase phase)
	{
		yield return new WaitForSeconds(delay);
		GetInstance().Phase = phase;
	}

	public void AddCardToHistory(string cardID)
	{
		currentCardID = cardID;
	}
}
