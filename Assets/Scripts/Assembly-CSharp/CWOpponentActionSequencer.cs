using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWOpponentActionSequencer : MonoBehaviour
{
	public GameObject reshuffleTween;

	public UIButtonTween floopPanelTween;

	public UIButtonTween spellPanelTween;

	private GameState GameInstance;

	private AIManager aiMgr;

	private CWPlayerHandsController handCtrlr;

	private GameDataScript gameData;

	private CWFloopActionManager floopActionMgr;

	private CreatureManagerScript creatureMgr;

	private BattlePhaseManager phaseMgr;

	private static CWOpponentActionSequencer g_OpSequencer;

	public bool resumeFlag;

	private void Awake()
	{
		g_OpSequencer = this;
	}

	public static CWOpponentActionSequencer GetInstance()
	{
		return g_OpSequencer;
	}

	private void Start()
	{
		GameInstance = GameState.Instance;
		aiMgr = AIManager.Instance;
		handCtrlr = CWPlayerHandsController.GetInstance();
		gameData = GameDataScript.GetInstance();
		floopActionMgr = CWFloopActionManager.GetInstance();
		creatureMgr = CreatureManagerScript.GetInstance();
		phaseMgr = BattlePhaseManager.GetInstance();
	}

	public IEnumerator StartOpponentSequence()
	{
		Deck deck = GameInstance.GetDeck(PlayerType.Opponent);
		bool AIReshuffle = GameInstance.ActiveQuest.AIReshuffle;
		if (deck.CardCount() == 0 && AIReshuffle)
		{
			TFUtils.DebugLog("Opponent reshuffling", "ai");
			yield return StartCoroutine(Reshuffle());
		}
		TFUtils.DebugLog("Opponent decision begin", "ai");
		for (AIDecision Decision = aiMgr.MakeDecision(); Decision != null; Decision = aiMgr.MakeDecision())
		{
			TFUtils.DebugLog("Opponent decision execution", "ai");
			yield return StartCoroutine(ExecuteDecision(Decision));
		}
		TFUtils.DebugLog("Opponent decision end", "ai");
		NextPhase();
	}

	private IEnumerator ExecuteDecision(AIDecision Decision)
	{
		phaseMgr.Phase = BattlePhase.P2Setup;
		if (Decision.IsLeaderAbility)
		{
			yield return new WaitForSeconds(0.5f);
			yield return StartCoroutine(LeaderAction());
		}
		else if (Decision.IsFloop)
		{
			yield return new WaitForSeconds(0.5f);
			yield return StartCoroutine(OpponentFloop(Decision));
		}
		else
		{
			yield return new WaitForSeconds(0.5f);
			yield return StartCoroutine(PlayCardToLane(Decision.CardChoice, Decision.LaneChoice));
		}
	}

	private void NextPhase()
	{
		if (GameDataScript.GetInstance().Turn <= 1)
		{
			BattleManagerScript.GetInstance().P2BattleFinished();
		}
		else
		{
			BattlePhaseManager.GetInstance().Phase = BattlePhase.P2BattleBanner;
		}
	}

	private IEnumerator Reshuffle()
	{
		List<CardItem> currentHand = GameInstance.GetHand(PlayerType.Opponent);
		while (currentHand.Count > 0)
		{
			CardItem card = currentHand[0];
			GameInstance.RemoveCardFromHand(PlayerType.Opponent, card);
			GameInstance.DiscardCard(PlayerType.Opponent, card);
		}
		List<CardItem> Pile = GameInstance.GetDiscardPile(PlayerType.Opponent);
		Deck CurrentDeck = GameInstance.GetDeck(PlayerType.Opponent);
		while (Pile.Count > 0)
		{
			CardItem item = Pile[0];
			Pile.RemoveAt(0);
			CurrentDeck.AddCard(item);
		}
		GameState.Instance.Reshuffle(PlayerType.Opponent);
		for (int i = 0; i < 5; i++)
		{
			GameState.Instance.DrawCard(PlayerType.Opponent);
		}
		yield return null;
	}

	private IEnumerator PlayCardToLane(CardItem card, Lane lane)
	{
		floopActionMgr.card = card;
		phaseMgr.AddCardToHistory(card.Form.ID);
		UIButtonPlayAnimation uiButtonAnim = GetComponent<UIButtonPlayAnimation>();
		if (uiButtonAnim != null)
		{
			uiButtonAnim.target = gameData.characterObjects[1].GetComponent<Animation>();
			uiButtonAnim.clipName = GetAnimClipName();
		}
		resumeFlag = false;
		Spawn(card, lane);
		while (!resumeFlag)
		{
			yield return null;
		}
		yield return null;
	}

	public void Spawn(CardItem card, Lane lane)
	{
		switch (card.Form.Type)
		{
		case CardType.Creature:
		case CardType.Building:
			GameInstance.Summon(PlayerType.Opponent, lane.Index, card);
			break;
		case CardType.Spell:
			handCtrlr.TriggerSpell(PlayerType.Opponent, card);
			break;
		}
	}

	private IEnumerator OpponentFloop(AIDecision Decision)
	{
		if (floopPanelTween != null)
		{
			floopPanelTween.Play(true);
		}
		GameObject currentInstance = creatureMgr.Instances[(int)PlayerType.Opponent, Decision.LaneChoice.Index, (int)Decision.CardChoice.Form.Type];
		floopActionMgr.anim = currentInstance.GetComponent<Animation>();
		floopActionMgr.lane = Decision.LaneChoice.Index;
		floopActionMgr.card = GameState.Instance.GetCard(PlayerType.Opponent, Decision.LaneChoice.Index, Decision.CardChoice.Form.Type);
		floopActionMgr.player = PlayerType.Opponent;
		resumeFlag = false;
		yield return StartCoroutine(floopActionMgr.PlayFloopAction());
		while (!resumeFlag)
		{
			yield return null;
		}
		yield return null;
	}

	private IEnumerator LeaderAction()
	{
		resumeFlag = false;
		phaseMgr.Phase = BattlePhase.P2LeaderAbility;
		CWFloopActionManager.GetInstance().TriggerLeader(PlayerType.Opponent);
		while (!resumeFlag)
		{
			yield return null;
		}
		yield return null;
	}

	private string GetAnimClipName()
	{
		GetTempQuestData();
		CharacterData character = GameInstance.GetCharacter(PlayerType.Opponent);
		if (GameInstance.GetCardsInHand(PlayerType.Opponent) > 1)
		{
			if (gameData.characterObjects[1] != null && character.PlayCardAnim != null)
			{
				return character.PlayCardAnim;
			}
		}
		else if (gameData.characterObjects[1] != null && character.LastCardAnim != null)
		{
			return character.LastCardAnim;
		}
		return string.Empty;
	}

	private void GetTempQuestData()
	{
		CharacterData character = GameInstance.GetCharacter(PlayerType.Opponent);
		if (character == null)
		{
			CharacterData characterData = CharacterDataManager.Instance.GetCharacterData("Jake");
			CharacterData characterData2 = CharacterDataManager.Instance.GetCharacterData("Finn");
			GameState.Instance.SetCharacters(characterData, characterData2);
		}
	}

	private void Update()
	{
	}
}
