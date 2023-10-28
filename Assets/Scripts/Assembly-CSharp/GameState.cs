#define ASSERTS_ON
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
	public const int MAX_PLAYERS = 2;

	public const int CARDS_TO_DEAL = 5;

	public const int MAX_HAND = 7;

	public const int LANE_COUNT = 4;

	public const int LANDSCAPE_TYPES = 5;

	public const int ACTION_POINT_VALUE = 1;

	public const int CARD_POINT_VALUE = 3;

	private bool autoBattle;

	private BattlePhaseManager phaseMgr;

	private static GameState instance;

	private static int refCount;

	private CharacterData[] Characters = new CharacterData[2];

	private Deck[] Decks = new Deck[2];

	private List<CardItem>[] Hands = new List<CardItem>[2]
	{
		new List<CardItem>(),
		new List<CardItem>()
	};

	private List<CardItem>[] DiscardPiles = new List<CardItem>[2]
	{
		new List<CardItem>(),
		new List<CardItem>()
	};

	private Dictionary<string, List<LandscapeType>>[] SummonedCards = new Dictionary<string, List<LandscapeType>>[2]
	{
		new Dictionary<string, List<LandscapeType>>(),
		new Dictionary<string, List<LandscapeType>>()
	};

	private List<CardItem> DweebCup = new List<CardItem>();

	private Lane[,] Lanes = new Lane[2, 4];

	private int[] MagicPoints = new int[2];

	private int[] BonusPoints = new int[2];

	private int[] SpellPoints = new int[2];

	private int[] FloopCountTurn = new int[2];

	private int[] FloopCountGame = new int[2];

	public int[] SpellsCast = new int[2];

	public int[] CreaturesSummoned = new int[2];

	public int[] CreaturesRemoved = new int[2];

	private bool[,] CanCast = new bool[3, 2]
	{
		{ true, true },
		{ true, true },
		{ true, true }
	};

	private bool[] CanFloop = new bool[2] { true, true };

	private bool[] CanPlay = new bool[2] { true, true };

	private int[] Health = new int[2] { 25, 25 };

	private int[] MaxHealth = new int[2] { 25, 25 };

	private int[] MinHealth = new int[2];

	private int[] LeaderCooldown = new int[2];

	private bool[] Attacked = new bool[2];

	private LandscapeType[] OverrideLandscapes = new LandscapeType[2]
	{
		LandscapeType.None,
		LandscapeType.None
	};

	private List<LandscapeType>[] FakeLandscapes = new List<LandscapeType>[2]
	{
		new List<LandscapeType>(),
		new List<LandscapeType>()
	};

	private bool[] RandomizeSummon = new bool[2];

	private List<CardScript>[] SpellsInEffect = new List<CardScript>[2]
	{
		new List<CardScript>(),
		new List<CardScript>()
	};

	private List<CardScript>[] PersistentSpellsInEffect = new List<CardScript>[2]
	{
		new List<CardScript>(),
		new List<CardScript>()
	};

	private int CoinsRewarded;

	private int XPRewarded;

	private CardScript TargetingListener;

	private PlayerType SelectionSide;

	private List<CardDiscount>[] Discounts = new List<CardDiscount>[2]
	{
		new List<CardDiscount>(),
		new List<CardDiscount>()
	};

	private int[] FloopCostMods = new int[2];

	private int[] FlatFloopCost = new int[2] { -1, -1 };

	private int[] ATKPenalty = new int[2];

	private CardScript ReplacedScript;

	private List<CardItem> StaticLootList = new List<CardItem>();

	public Dictionary<string, GameObject>[] CharacterFXList = new Dictionary<string, GameObject>[2]
	{
		new Dictionary<string, GameObject>(),
		new Dictionary<string, GameObject>()
	};

	public QuestData ActiveQuest;

	public CardManagerScript CardManager { get; set; }

	public CreatureManagerScript CreatureManager { get; set; }

	public LandscapeManagerScript LandscapeManager { get; set; }

	public GameDataScript GameData { get; set; }

	public bool Stealing { get; set; }

	public CWPlayerHandsController handsController { get; set; }

	public BattleResolver BattleResolver { get; set; }

	public bool IsSetUp { get; set; }

	public float HitAreaModifier { get; set; }

	public float DefenseAreaModifier { get; set; }

	public float DefenseAreaCritModifier { get; set; }

	public float CritAreaModifier { get; set; }

	public int CurrentMagicPoints { get; set; }

	public int ExtraMagicPoints { get; set; }

	public static GameState Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new GameState();
			}
			return instance;
		}
	}

	public GameState()
	{
		refCount++;
		IsSetUp = false;
		for (int i = 0; i < 2; i++)
		{
			Lane lane = null;
			for (int j = 0; j < 4; j++)
			{
				Lane lane2 = new Lane();
				Lanes[i, j] = lane2;
				lane2.Index = j;
				lane2.AdjacentLanes[0] = lane;
				if (lane != null)
				{
					lane.AdjacentLanes[1] = lane2;
				}
				PlayerType playerType = i;
				Lane lane4 = (lane2.OpponentLane = Lanes[(int)(!playerType), 3 - j]);
				if (lane4 != null)
				{
					lane4.OpponentLane = lane2;
				}
				lane = lane2;
			}
		}
	}

	public void ResetAreaModifiers()
	{
		float num2 = (CritAreaModifier = 0f);
		num2 = (DefenseAreaCritModifier = num2);
		num2 = (DefenseAreaModifier = num2);
		HitAreaModifier = num2;
	}

	public void ResetExtraMagicPoints()
	{
		ExtraMagicPoints = 0;
	}

	public static void FullReset()
	{
		instance = null;
	}

	public void ResetFromQuestData(QuestData qd, Deck playerDeckCopy, Deck opponentDeckCopy = null)
	{
		Reset();
		LeaderItem leaderItem = playerDeckCopy.Leader;
		if (opponentDeckCopy == null)
		{
			Deck deck = null;
			switch (Deck.DeckTypeFromID(qd.OpponentDeckID))
			{
			case DeckType.NORMAL:
				deck = AIDeckManager.Instance.GetDeck(qd.OpponentDeckID);
				break;
			case DeckType.PLAYER_CLONE_DECK:
				deck = playerDeckCopy;
				break;
			default:
				TFUtils.WarnLog("Quest " + qd.QuestID + " is using an unknown special deck: '" + qd.OpponentDeckID + "'");
				break;
			}
			if (deck == null)
			{
				TFUtils.ErrorLog("Quest " + qd.QuestID + " deck '" + qd.OpponentDeckID + "' cannot be resolved. Resort to using the player deck to prevent a crash!");
				deck = playerDeckCopy;
			}
			opponentDeckCopy = deck.Clone();
			opponentDeckCopy.Leader = LeaderManager.Instance.CreateLeader(qd.LeaderID, qd.LeaderLevel);
		}
		if (qd.OverridePlayerDeckId != null)
		{
			playerDeckCopy = AIDeckManager.Instance.GetDeckCopy(qd.OverridePlayerDeckId);
			playerDeckCopy.Leader = leaderItem;
		}
		if (qd.OverridePlayerLeaderId != null || qd.OverridePlayerLeaderLevel > 0)
		{
			leaderItem = (playerDeckCopy.Leader = LeaderManager.Instance.CreateLeader((qd.OverridePlayerLeaderId == null) ? leaderItem.Form.ID : qd.OverridePlayerLeaderId, (qd.OverridePlayerLeaderLevel <= 0) ? playerDeckCopy.Leader.Rank : qd.OverridePlayerLeaderLevel));
		}
		SetCharacters(CharacterDataManager.Instance.GetCharacterData(leaderItem.Form.CharacterID), CharacterDataManager.Instance.GetCharacterData(opponentDeckCopy.Leader.Form.CharacterID));
		SetDecks(playerDeckCopy, opponentDeckCopy);
		if (DebugFlagsScript.GetInstance().QuickWin)
		{
			SetHealth(1000, 1);
		}
		else
		{
			SetHealth(playerDeckCopy.Leader.HP, opponentDeckCopy.Leader.HP);
		}
		SetMinHealth(0, 0);
		qd.XPRewarded = UnityEngine.Random.Range(qd.MinXP, qd.MaxXP);
		qd.CoinsRewarded = UnityEngine.Random.Range(qd.MinCoins, qd.MaxCoins);
		ActiveQuest = qd;
	}

	public void SetCharacters(CharacterData user, CharacterData opponent)
	{
		Characters[(int)PlayerType.User] = user;
		Characters[(int)PlayerType.Opponent] = opponent;
	}

	public CharacterData GetCharacter(PlayerType player)
	{
		return Characters[(int)player];
	}

	public void SetDecks(Deck user, Deck opponent)
	{
		Decks[(int)PlayerType.User] = user;
		Decks[(int)PlayerType.Opponent] = opponent;
		QuestStats currentStats = QuestConditionManager.Instance.currentStats;
		currentStats.NumLandscapesUsed = user.GetUniqueLandscapeCount();
		currentStats.DeckCost = user.SumOfAllLevels();
	}

	public void SetHealth(int user, int opponent)
	{
		Health[(int)PlayerType.User] = (MaxHealth[(int)PlayerType.User] = user);
		Health[(int)PlayerType.Opponent] = (MaxHealth[(int)PlayerType.Opponent] = opponent);
	}

	public int GetMaxHealth(PlayerType player)
	{
		return MaxHealth[(int)player];
	}

	public void SetMinHealth(int user, int opponent)
	{
		MinHealth[(int)PlayerType.User] = user;
		MinHealth[(int)PlayerType.Opponent] = opponent;
	}

	public int GetMinHealth(PlayerType player)
	{
		return MinHealth[(int)player];
	}

	public LeaderItem GetLeader(PlayerType player)
	{
		Deck deck = Decks[(int)player];
		if (deck == null)
		{
			return null;
		}
		return deck.Leader;
	}

	public int GetLeaderCooldown(PlayerType player)
	{
		return LeaderCooldown[(int)player];
	}

	public bool IsLeaderAbilityReady(PlayerType player)
	{
		LeaderItem leader = GetLeader(player);
		return leader.CanPlay(player);
	}

	public int EvaluateLeaderAbility(PlayerType player)
	{
		LeaderItem leader = GetLeader(player);
		return leader.EvaluateLeaderAbility(player);
	}

	public void UseLeaderAbility(PlayerType player)
	{
		LeaderItem leader = GetLeader(player);
		LeaderScript leaderScript = leader.InstanceScript() as LeaderScript;
		leaderScript.Owner = player;
		leaderScript.Leader = leader;
		leaderScript.Data.Form.Name = leader.Form.Name;
		leaderScript.Data.Form.ScriptName = leader.Form.ScriptName;
		leaderScript.Data.Form.ScriptVizName = leader.Form.ScriptName;
		leaderScript.Data.Form.RawDescription = leader.Description;
		leaderScript.Cast();
		LeaderCooldown[(int)player] = leader.Form.Cooldown;
		VOManager.Instance.PlayEvent(player, VOEvent.LeaderAbility);
	}

	public void Setup()
	{
		PlayerInfoScript playerInfoScript = PlayerInfoScript.GetInstance();
		QuestData questData = ActiveQuest;
		if (Decks[0] == null || Decks[1] == null)
		{
			questData = playerInfoScript.GetCurrentQuest();
			Reset();
			ActiveQuest = questData;
			Deck deck = AIDeckManager.Instance.GetDeckCopy("Quest71_Deck");
			deck.Leader = LeaderManager.Instance.CreateLeader("Leader_Jake", 1);
			if (DebugFlagsScript.GetInstance().battleDisplay.usePinfoDeck)
			{
				deck = playerInfoScript.GetSelectedDeck();
			}
			Deck deckCopy = AIDeckManager.Instance.GetDeckCopy("Quest71_Deck");
			deckCopy.Leader = LeaderManager.Instance.CreateLeader("Leader_Finn", 1);
			SetDecks(deck, deckCopy);
			SetHealth(10000, 10000);
			questData.XPRewarded = UnityEngine.Random.Range(questData.MinXP, questData.MaxXP);
			questData.CoinsRewarded = UnityEngine.Random.Range(questData.MinCoins, questData.MaxCoins);
		}
		TFUtils.Assert(questData != null, "GameState's ActiveQuest is not set up properly!");
		CreatureManagerScript.GetInstance().SetupUniqueListForPool();
		LandscapeManagerScript.GetInstance().PoolLandscape();
		Deck deck2 = Decks[(int)PlayerType.User];
		deck2.Shuffle();
		Singleton<AnalyticsManager>.Instance.LogQuestStart();
		Singleton<AnalyticsManager>.Instance.LogLeaderEquipped(deck2.Leader.Form.ID, deck2.Leader.Rank);
		Singleton<AnalyticsManager>.Instance.LogDeckEquipped(deck2.GetCards());
		LeaderCooldown[(int)PlayerType.User] = deck2.Leader.Form.Cooldown;
		Hands[(int)PlayerType.User].Clear();
		for (int i = 0; i < 5; i++)
		{
			CardItem item = deck2.DealCard();
			Hands[(int)PlayerType.User].Add(item);
		}
		SummonedCards[(int)PlayerType.User].Clear();
		deck2 = Decks[(int)PlayerType.Opponent];
		deck2.Shuffle();
		deck2.ShuffleLandscapes();
		LeaderCooldown[(int)PlayerType.Opponent] = deck2.Leader.Form.Cooldown;
		StaticLootList.Clear();
		Hands[(int)PlayerType.Opponent].Clear();
		for (int j = 0; j < 5; j++)
		{
			CardItem item2 = deck2.DealCard();
			Hands[(int)PlayerType.Opponent].Add(item2);
		}
		SummonedCards[(int)PlayerType.Opponent].Clear();
		PanelManagerBattle.GetInstance().QuestStatusInit();
		IsSetUp = true;
	}

	public void Reset()
	{
		TargetingListener = null;
		BattleResolver = null;
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				for (int k = 0; k < 2; k++)
				{
					Lanes[i, j].Scripts[k] = null;
					Lanes[i, j].Type = LandscapeType.None;
					Lanes[i, j].AbilitiesBlocked = false;
					Lanes[i, j].FloopMod = 0;
					Lanes[i, j].RarityGate = int.MaxValue;
				}
			}
			SpellsInEffect[i].Clear();
			PersistentSpellsInEffect[i].Clear();
			Health[i] = 25;
			CanCast[0, i] = true;
			CanCast[1, i] = true;
			CanCast[2, i] = true;
			CanPlay[i] = true;
			CanFloop[i] = true;
			Attacked[i] = false;
			DiscardPiles[i].Clear();
			OverrideLandscapes[i] = LandscapeType.None;
			Discounts[i].Clear();
			FloopCostMods[i] = 0;
			FlatFloopCost[i] = -1;
			ATKPenalty[i] = 0;
			RandomizeSummon[i] = false;
			FakeLandscapes[i].Clear();
			SpellPoints[i] = 0;
			BonusPoints[i] = 0;
			FloopCountTurn[i] = 0;
			FloopCountGame[i] = 0;
			MagicPoints[i] = 0;
			CharacterFXList[i].Clear();
		}
		ResetAreaModifiers();
		ResetExtraMagicPoints();
		CurrentMagicPoints = ParametersManager.Instance.Starting_Magic_Points;
		QuestConditionManager.Instance.ResetStats();
	}

	public void AwardStaticLoot()
	{
		for (int i = 0; i < 2; i++)
		{
			Discounts[i].Clear();
		}
		foreach (CardItem staticLoot in StaticLootList)
		{
			QuestEarningManager.GetInstance().earnedCards.Add(staticLoot);
			QuestEarningManager.GetInstance().earnedCardsName.Add(staticLoot.Form.ID);
			QuestEarningManager.GetInstance().hasCardFlag.Add(PlayerInfoScript.GetInstance().DeckManager.HasCard(staticLoot.Form.ID));
		}
	}

	public bool WasAttacked(PlayerType player)
	{
		return Attacked[(int)player];
	}

	public Lane GetLane(PlayerType player, int lane)
	{
		return Lanes[(int)player, lane];
	}

	public void DealDamage(PlayerType player, int damage, int lane)
	{
		DealDamage(player, damage);
		if (damage > 0)
		{
			LandscapeManagerScript.GetInstance().SpawnLandDamageFX(player, lane);
		}
	}

	public void DealDamage(PlayerType player, int damage)
	{
		CWBattleSequenceController cWBattleSequenceController = CWBattleSequenceController.GetInstance();
		if (cWBattleSequenceController.result == "Crit")
		{
			damage = (int)((float)damage * cWBattleSequenceController.damageModifierCrit);
		}
		Health[(int)player] -= damage;
		if (player == PlayerType.User && damage > 0)
		{
			QuestConditionManager.Instance.currentStats.HPLost += damage;
			PanelManagerBattle.GetInstance().QuestStatusUpdate();
			TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.HeroDamaged);
		}
		if (Health[(int)player] < MinHealth[(int)player])
		{
			Health[(int)player] = MinHealth[(int)player];
		}
		if (Health[(int)player] > MaxHealth[(int)player])
		{
			Health[(int)player] = MaxHealth[(int)player];
		}
		GameData.UpdateText();
		if (damage > 0)
		{
			GameObject gameObject = ((player != PlayerType.User) ? PanelManagerBattle.GetInstance().tweenP2Damage : PanelManagerBattle.GetInstance().tweenP1Damage);
			gameObject.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			VOManager.Instance.PlayEvent(player, VOEvent.HeroDamaged);
		}
	}

	public int GetHealth(PlayerType player)
	{
		return Health[(int)player];
	}

	public void SetHealth(PlayerType player, int health)
	{
		Health[(int)player] = health;
	}

	public void SetMaxHealth(PlayerType player)
	{
		SetHealth(player, GetMaxHealth(player));
	}

	public void SetMagicPoints(PlayerType player, int points)
	{
		MagicPoints[(int)player] = points;
	}

	public int GetMagicPoints(PlayerType player)
	{
		return MagicPoints[(int)player];
	}

	public void AddMagicPoints(PlayerType player, int points, bool checkForOutOfActions = true)
	{
		if (DebugFlagsScript.GetInstance().InfiniteMagic && player == PlayerType.User && points < 0)
		{
			return;
		}
		MagicPoints[(int)player] += points;
		if (checkForOutOfActions && player == PlayerType.User && MagicPoints[(int)player] == 0)
		{
			if (TutorialMonitor.Instance.PopupActive)
			{
				TutorialMonitor.Instance.QueueTutorial(TutorialTrigger.OutOfActions);
			}
			else
			{
				TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.OutOfActions);
			}
		}
	}

	public int GetFloopCountTurn(PlayerType player)
	{
		return FloopCountTurn[(int)player];
	}

	public int GetFloopCountGame(PlayerType player)
	{
		return FloopCountGame[(int)player];
	}

	public void AddBonusPoints(PlayerType player, int points)
	{
		BonusPoints[(int)player] += points;
	}

	public int GetSpellPoints(PlayerType player)
	{
		return SpellPoints[(int)player];
	}

	public void SetSpellPoints(PlayerType player, int points)
	{
		SpellPoints[(int)player] = points;
	}

	public void AddSpellPoints(PlayerType player, int points)
	{
		SpellPoints[(int)player] += points;
	}

	public bool CanPlayCard(PlayerType player)
	{
		List<CardItem> hand = GetHand(player);
		for (int i = 0; i < hand.Count; i++)
		{
			CardForm form = hand[i].Form;
			for (int j = 0; j < 4; j++)
			{
				Lane lane = GetLane(player, j);
				if ((form.Type != 0 || !lane.HasCreature()) && (form.Type != CardType.Building || !lane.HasBuilding()) && form.CanPlay(player, j))
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool CanFloopCreature(PlayerType player)
	{
		for (int i = 0; i < 4; i++)
		{
			Lane lane = GetLane(player, i);
			if (lane.HasCreature())
			{
				CreatureScript creature = lane.GetCreature();
				if (CanFloopCard(player, creature))
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool HasLegalMove(PlayerType player)
	{
		if (CanPlayCard(player) || CanFloopCreature(player))
		{
			return true;
		}
		return false;
	}

	public List<CardItem> GetHand(PlayerType player)
	{
		return Hands[(int)player];
	}

	public CardItem GetCardInHand(PlayerType player, int idx)
	{
		List<CardItem> list = Hands[(int)player];
		if (idx < list.Count)
		{
			return list[idx];
		}
		return null;
	}

	public int GetCardsInHand(PlayerType player)
	{
		List<CardItem> list = Hands[(int)player];
		return list.Count;
	}

	public void DrawCard(PlayerType player)
	{
		Deck deck = Decks[(int)player];
		List<CardItem> list = Hands[(int)player];
		if (list.Count >= 7 || deck.CardCount() <= 0)
		{
			return;
		}
		CardItem item = deck.DealCard();
		list.Add(item);
		if (player == PlayerType.User)
		{
			CWPlayerHandsController cWPlayerHandsController = CWPlayerHandsController.GetInstance();
			if (cWPlayerHandsController != null)
			{
				cWPlayerHandsController.UpdateCards(list);
			}
			AudioClip cardDrawSound = CWPlayerHandsController.GetInstance().cardDrawSound;
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayOneShot(cardDrawSound);
		}
	}

	public CardItem PopCard(PlayerType player)
	{
		Deck deck = Decks[(int)player];
		return deck.DealCard();
	}

	public void PushCard(PlayerType player, CardItem item)
	{
		Deck deck = Decks[(int)player];
		deck.PlaceCard(item);
	}

	public void Reshuffle(PlayerType player)
	{
		Deck deck = Decks[(int)player];
		deck.Shuffle();
	}

	public Deck GetDeck(PlayerType player)
	{
		return Decks[(int)player];
	}

	public void PlaceCardInHand(PlayerType player, CardItem card)
	{
		List<CardItem> list = Hands[(int)player];
		int count = list.Count;
		list.Add(card);
		if (player == PlayerType.User)
		{
			CardManager.NewCard(count);
			CWPlayerHandsController cWPlayerHandsController = CWPlayerHandsController.GetInstance();
			if (cWPlayerHandsController != null)
			{
				cWPlayerHandsController.UpdateCards(list);
			}
		}
	}

	public void RemoveCardFromHand(PlayerType player, CardItem card)
	{
		List<CardItem> list = Hands[(int)player];
		list.Remove(card);
		if (player == PlayerType.User)
		{
			handsController = CWPlayerHandsController.GetInstance();
			if (handsController != null)
			{
				handsController.UpdateCards(list);
			}
		}
	}

	public void RemoveCardFromPlay(PlayerType player, int lane, CardType type)
	{
		CardScript cardScript = Lanes[(int)player, lane].Scripts[(int)type];
		cardScript.Flooped = false;
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				if (LaneHasCard(player, i, (CardType)j))
				{
					CardScript script = GetScript(player, i, (CardType)j);
					script.OnCardLeftPlay(cardScript);
					script.EndAllVFX();
				}
			}
		}
		Lanes[(int)player, lane].Scripts[(int)type] = null;
		if (type == CardType.Creature)
		{
			CreaturesRemoved[(int)player]++;
		}
		CreatureManager.RemoveInstance(player, lane, type);
	}

	public List<CardItem> GetCardsInDeck(PlayerType player)
	{
		return Decks[(int)player].GetCards();
	}

	public void DiscardCard(PlayerType player, CardItem card)
	{
		DiscardPiles[(int)player].Insert(0, card);
	}

	public List<CardItem> GetDiscardPile(PlayerType player)
	{
		return DiscardPiles[(int)player];
	}

	public void RemoveCardFromDiscardPile(PlayerType player, CardItem card)
	{
		DiscardPiles[(int)player].Remove(card);
	}

	public void ReturnCardToHand(PlayerType player, CardType type)
	{
		CardItem cardItem = null;
		List<CardItem> list = DiscardPiles[(int)player];
		List<CardItem> list2 = new List<CardItem>();
		int num = 0;
		foreach (CardItem item in list)
		{
			if (item.Form.Type == type)
			{
				list2.Add(item);
			}
		}
		if (list2.Count > 0)
		{
			num = UnityEngine.Random.Range(0, list2.Count);
			cardItem = list2[num];
		}
		if (cardItem != null)
		{
			PlaceCardInHand(player, cardItem);
		}
	}

	public bool DiscardPileContains(PlayerType player, CardType type)
	{
		List<CardItem> list = DiscardPiles[(int)player];
		foreach (CardItem item in list)
		{
			if (item.Form.Type == type)
			{
				return true;
			}
		}
		return false;
	}

	public CardItem GetCard(PlayerType player, int lane, CardType type)
	{
		return Lanes[(int)player, lane].Scripts[(int)type].Data;
	}

	public CardForm GetCardForm(PlayerType player, int lane, CardType type)
	{
		return GetCard(player, lane, type).Form;
	}

	public bool HasCreaturesInPlay(PlayerType player)
	{
		for (int i = 0; i < 4; i++)
		{
			if (LaneHasCreature(player, i))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasCardTypeInPlay(PlayerType player, CardType type)
	{
		for (int i = 0; i < 4; i++)
		{
			if (LaneHasCard(player, i, type))
			{
				return true;
			}
		}
		return false;
	}

	public bool LaneHasCreature(PlayerType player, int lane)
	{
		return Lanes[(int)player, lane].HasCreature();
	}

	public bool LaneHasBuilding(PlayerType player, int lane)
	{
		return Lanes[(int)player, lane].HasBuilding();
	}

	public bool LaneHasCard(PlayerType player, int lane, CardType type)
	{
		return Lanes[(int)player, lane].HasCard(type);
	}

	public List<Lane> GetLanes(PlayerType player)
	{
		List<Lane> list = new List<Lane>();
		for (int i = 0; i < 4; i++)
		{
			list.Add(Lanes[(int)player, i]);
		}
		return list;
	}

	public void CheckForDeaths()
	{
		bool flag = false;
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				if (!LaneHasCreature(i, j))
				{
					continue;
				}
				CreatureScript creature = GetCreature(i, j);
				if (creature.Health <= 0 && !creature.MarkedForDeath)
				{
					DiscardCard(i, creature.Data);
					phaseMgr = BattlePhaseManager.GetInstance();
					if (phaseMgr.Phase != BattlePhase.P1Battle && phaseMgr.Phase != BattlePhase.P2Battle)
					{
						CreatureBattleScript component = CreatureManager.Instances[i, j, 0].GetComponent<CreatureBattleScript>();
						creature.MarkedForDeath = true;
						component.CheckForDefeat();
					}
					else
					{
						creature.MarkedForDeath = true;
					}
					if (i == (int)PlayerType.User)
					{
						QuestConditionManager.Instance.currentStats.NumCreaturesLost++;
					}
					else
					{
						QuestConditionManager.Instance.currentStats.NumCreaturesDefeated++;
					}
					PanelManagerBattle.GetInstance().QuestStatusUpdate();
					if (i == (int)PlayerType.Opponent)
					{
						CreatureManager.SpawnCoins(j);
					}
					flag = true;
				}
			}
		}
		if (flag)
		{
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(CreatureManager.GetComponent<AudioSource>(), CreatureManager.CreatureDeath);
		}
	}

	public bool IsMarkedForDeath(PlayerType player, int lane)
	{
		if (LaneHasCreature(player, lane))
		{
			CreatureScript creature = GetCreature(player, lane);
			return creature.MarkedForDeath;
		}
		return false;
	}

	public void MoveCard(PlayerType player, int source, int dest, CardType type)
	{
		Lane lane = Lanes[(int)player, source];
		Lane lane2 = Lanes[(int)player, dest];
		CardScript cardScript = lane.Scripts[(int)type];
		CardScript cardScript2 = lane2.Scripts[(int)type];
		lane2.Scripts[(int)type] = cardScript;
		lane.Scripts[(int)type] = cardScript2;
		if (cardScript != null)
		{
			cardScript.OnCardLeftPlay(cardScript);
			cardScript.CurrentLane = lane2;
			cardScript.OnCardEnterPlay(cardScript);
		}
		if (cardScript2 != null)
		{
			cardScript2.OnCardLeftPlay(cardScript2);
			cardScript2.CurrentLane = lane;
			cardScript2.OnCardEnterPlay(cardScript2);
		}
		CreatureManager.MoveInstance(player, source, dest, type);
	}

	public void MoveCard(PlayerType victim, int source, PlayerType thief, int dest, CardType type)
	{
		Lane lane = Lanes[(int)victim, source];
		Lane lane2 = Lanes[(int)thief, dest];
		CardScript cardScript = lane.Scripts[(int)type];
		CardScript cardScript2 = lane2.Scripts[(int)type];
		lane2.Scripts[(int)type] = cardScript;
		lane.Scripts[(int)type] = cardScript2;
		if (cardScript != null)
		{
			cardScript.Owner = thief;
			cardScript.CurrentLane = lane2;
		}
		if (cardScript2 != null)
		{
			cardScript2.Owner = victim;
			cardScript2.CurrentLane = lane;
		}
		CreatureManager.MoveInstance(victim, source, thief, dest, type);
	}

	public void CastSpell(PlayerType player, CardItem card)
	{
		if (!Stealing)
		{
			int num = card.Form.DetermineCost(player);
			int num2 = SpellPoints[(int)player] - num;
			if (num2 >= 0)
			{
				AddSpellPoints(player, -num);
			}
			else
			{
				SetSpellPoints(player, 0);
				AddMagicPoints(player, num2);
			}
			RemoveCardFromHand(player, card);
		}
		else
		{
			RemoveCardFromHand(!player, card);
			Stealing = false;
		}
		DiscardCard(player, card);
		Type scriptClass = card.Form.GetScriptClass();
		SpellScript spellScript = Activator.CreateInstance(scriptClass) as SpellScript;
		spellScript.Owner = player;
		spellScript.Data = card;
		spellScript.GameInstance = this;
		SpellsCast[(int)player]++;
		if (player == PlayerType.User)
		{
			TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.PlayedSpell);
			QuestStats currentStats = QuestConditionManager.Instance.currentStats;
			currentStats.IncrementTypeUsed(card.Form.Type);
			currentStats.IncrementFactionUsed(card.Form.Faction);
			currentStats.AddActionPoints(card.Form.Cost);
			PanelManagerBattle.GetInstance().QuestStatusUpdate();
		}
		VOManager.Instance.PlayEvent(player, VOEvent.PlaySpell);
		spellScript.Cast();
	}

	public void AddSpellEffect(PlayerType player, CardScript script)
	{
		SpellsInEffect[(int)player].Add(script);
	}

	public void EndSpellEffect(PlayerType player, CardScript script)
	{
		SpellsInEffect[(int)player].Remove(script);
	}

	public void AddPersistentSpellEffect(PlayerType player, CardScript script)
	{
		PersistentSpellsInEffect[(int)player].Add(script);
	}

	public void EndPersistentSpellEffect(PlayerType player, CardScript script)
	{
		PersistentSpellsInEffect[(int)player].Remove(script);
	}

	public void StartPreview(int lane, CardItem card)
	{
		Lane lane2 = Lanes[(int)PlayerType.User, lane];
		if (lane2.HasCard(card.Form.Type))
		{
			ReplacedScript = lane2.GetScript(card.Form.Type);
		}
		CardScript cardScript = card.Form.InstanceScript();
		lane2.Scripts[(int)card.Form.Type] = cardScript;
		cardScript.Data = card;
		cardScript.CurrentLane = lane2;
		cardScript.Owner = PlayerType.User;
		cardScript.GameInstance = this;
	}

	public void EndPreview(int lane, CardItem card)
	{
		Lane lane2 = Lanes[(int)PlayerType.User, lane];
		lane2.Scripts[(int)card.Form.Type] = ReplacedScript;
		ReplacedScript = null;
	}

	public void Summon(PlayerType player, int lane, CardItem card)
	{
		LandscapeType type = Lanes[(int)player, lane].Type;
		if (!SummonedCards[(int)player].ContainsKey(card.Form.ID))
		{
			SummonedCards[(int)player][card.Form.ID] = new List<LandscapeType>();
		}
		SummonedCards[(int)player][card.Form.ID].Add(type);
		phaseMgr = BattlePhaseManager.GetInstance();
		if (card.Form.Rarity >= 5)
		{
			phaseMgr.Phase = ((player != PlayerType.User) ? BattlePhase.P2SetupActionRareCard : BattlePhase.P1SetupActionRareCard);
			CWCharacterAnimController.GetInstance().DoEffectPlayCard(player, lane, card);
		}
		else
		{
			DoResultSummon(player, lane, card);
		}
	}

	public void DoResultSummon(PlayerType player, int lane, CardItem card)
	{
		AddMagicPoints(player, -card.Form.DetermineCost(player), false);
		RemoveCardFromHand(player, card);
		Lane lane2 = Lanes[(int)player, lane];
		if (lane2.HasCard(card.Form.Type))
		{
			CardScript script = lane2.GetScript(card.Form.Type);
			DiscardCard(player, script.Data);
			RemoveCardFromPlay(player, lane2.Index, card.Form.Type);
		}
		Type scriptClass = card.Form.GetScriptClass();
		CardScript cardScript = Activator.CreateInstance(scriptClass) as CardScript;
		lane2.Scripts[(int)card.Form.Type] = cardScript;
		cardScript.Data = card;
		cardScript.CurrentLane = lane2;
		cardScript.Owner = player;
		cardScript.GameInstance = this;
		cardScript.Fresh = true;
		cardScript.OnSummon();
		if (card.Form.Type == CardType.Creature)
		{
			CreaturesSummoned[(int)player]++;
			VOManager.Instance.PlayEvent(player, VOEvent.PlayCreature);
		}
		else
		{
			VOManager.Instance.PlayEvent(player, VOEvent.PlayBuilding);
		}
		if (player == PlayerType.User)
		{
			QuestStats currentStats = QuestConditionManager.Instance.currentStats;
			currentStats.IncrementTypeUsed(card.Form.Type);
			currentStats.IncrementFactionUsed(card.Form.Faction);
			currentStats.AddActionPoints(card.Form.Cost);
			PanelManagerBattle.GetInstance().QuestStatusUpdate();
		}
		CreatureManager.Summon(player, lane, card.Form, cardScript);
		phaseMgr.Phase = ((player != PlayerType.User) ? BattlePhase.P2SetupAction : BattlePhase.P1SetupAction);
	}

	public bool IsSummoning(PlayerType player, int lane, CardType type)
	{
		return Lanes[(int)player, lane].Scripts[(int)type].IsSummoning;
	}

	public CardScript GetSummoning(PlayerType player, int lane)
	{
		CardScript result = null;
		for (int i = 0; i < 2; i++)
		{
			CardScript cardScript = Lanes[(int)player, lane].Scripts[i];
			if (cardScript != null && cardScript.IsSummoning)
			{
				result = cardScript;
			}
		}
		return result;
	}

	public CardScript GetSummoning()
	{
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				for (int k = 0; k < 2; k++)
				{
					CardScript cardScript = Lanes[i, j].Scripts[k];
					if (cardScript != null && cardScript.IsSummoning)
					{
						return cardScript;
					}
				}
			}
		}
		return null;
	}

	public void FinishSummoning(CardScript script)
	{
		CreatureManager.FinishSummoning(script.Owner);
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				if (LaneHasCard(script.Owner, i, (CardType)j))
				{
					CardScript script2 = GetScript(script.Owner, i, (CardType)j);
					if (!script2.IsSummoning)
					{
						script2.OnCardEnterPlay(script);
					}
				}
			}
		}
		if (script.Owner == PlayerType.User)
		{
			if (script.Data.Form.Type == CardType.Creature)
			{
				TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.PlayedCreature);
			}
			if (script.Data.Form.Type == CardType.Building)
			{
				TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.PlayedBuilding);
			}
			CheckForOutOfActions();
		}
	}

	public void CheckForOutOfActions()
	{
		if (GetMagicPoints(PlayerType.User) == 0 || !HasLegalMove(PlayerType.User))
		{
			if (TutorialMonitor.Instance.PopupActive)
			{
				TutorialMonitor.Instance.QueueTutorial(TutorialTrigger.OutOfActions);
			}
			else
			{
				TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.OutOfActions);
			}
		}
	}

	public bool CanFloopCard(PlayerType player, CardScript Script)
	{
		if (!Script.Flooped && !Script.FloopDisabled && !Script.FloopBlocked && GetMagicPoints(player) >= Script.DetermineFloopCost() && Script.CanFloop())
		{
			return true;
		}
		return false;
	}

	public void FloopCard(PlayerType player, int lane, CardType type)
	{
		CardScript cardScript = Lanes[(int)player, lane].Scripts[(int)type];
		AddMagicPoints(player, -cardScript.DetermineFloopCost());
		FloopCountTurn[(int)player]++;
		FloopCountGame[(int)player]++;
		cardScript.Floop();
		cardScript.Flooped = true;
		if (cardScript.CurrentLane.HasBuilding())
		{
			BuildingScript building = cardScript.CurrentLane.GetBuilding();
			building.OnCreatureFlooped();
		}
		if (player == PlayerType.User)
		{
			QuestConditionManager.Instance.currentStats.NumFloopsUsed++;
			TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.FloopedCreature);
		}
		VOManager.Instance.PlayEvent(player, VOEvent.Floop);
	}

	public void UnfloopCard(PlayerType player, int lane, CardType type)
	{
		CardScript cardScript = Lanes[(int)player, lane].Scripts[(int)type];
		cardScript.Flooped = false;
	}

	public bool IsFlooping(PlayerType player)
	{
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				if (LaneHasCard(player, i, (CardType)j))
				{
					CardScript script = GetScript(player, i, (CardType)j);
					if (script.Flooping)
					{
						return true;
					}
				}
			}
		}
		for (int k = 0; k < SpellsInEffect[(int)player].Count; k++)
		{
			if (SpellsInEffect[(int)player][k].Flooping)
			{
				return true;
			}
		}
		for (int l = 0; l < PersistentSpellsInEffect[(int)player].Count; l++)
		{
			if (PersistentSpellsInEffect[(int)player][l].Flooping)
			{
				return true;
			}
		}
		return false;
	}

	public void CancelFloop(PlayerType player)
	{
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				if (LaneHasCard(player, i, (CardType)j))
				{
					CardScript script = GetScript(player, i, (CardType)j);
					if (script.Flooping)
					{
						script.CancelFloop();
					}
				}
			}
		}
	}

	public bool IsSummoning(PlayerType player)
	{
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				if (LaneHasCard(player, i, (CardType)j))
				{
					CardScript script = GetScript(player, i, (CardType)j);
					if (script.IsSummoning)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public void Update()
	{
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				for (int k = 0; k < 2; k++)
				{
					CardScript cardScript = Lanes[i, j].Scripts[k];
					if (cardScript != null)
					{
						cardScript.Update();
					}
				}
			}
			List<CardScript> list = SpellsInEffect[i];
			for (int l = 0; l < list.Count; l++)
			{
				list[l].Update();
			}
			list = PersistentSpellsInEffect[i];
			for (int m = 0; m < list.Count; m++)
			{
				list[m].Update();
			}
		}
		if (BattlePhaseManager.GetInstance().Phase == BattlePhase.P1Setup && !CWPlayerCardManager.inFirstTurnTutorial)
		{
			TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.FirstHand);
		}
		CheckForDeaths();
	}

	public int FloopCount(PlayerType player, CardType type)
	{
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			if (LaneHasCard(player, i, type) && Lanes[(int)player, i].Scripts[(int)type].Flooped)
			{
				num++;
			}
		}
		return num;
	}

	public void SetLandscape(PlayerType player, int lane, LandscapeType type)
	{
		Lanes[(int)player, lane].Type = type;
	}

	public LandscapeType GetLandscapeType(PlayerType player, int lane)
	{
		return Lanes[(int)player, lane].Type;
	}

	public void SetOverrideLandscape(PlayerType player, LandscapeType type)
	{
		OverrideLandscapes[(int)player] = type;
	}

	public LandscapeType GetOverrideLandscape(PlayerType player, int lane)
	{
		return OverrideLandscapes[(int)player];
	}

	public void AddFakeLandscape(PlayerType player, LandscapeType type)
	{
		FakeLandscapes[(int)player].Add(type);
	}

	public void RemoveFakeLandscape(PlayerType player, LandscapeType type)
	{
		FakeLandscapes[(int)player].Remove(type);
	}

	public bool IsLaneOfLandscapeType(PlayerType player, int lane, LandscapeType type)
	{
		if (Lanes[(int)player, lane].Type == type || OverrideLandscapes[(int)player] == type)
		{
			return true;
		}
		return false;
	}

	public int LandscapeCount(PlayerType player, LandscapeType type)
	{
		int num = 0;
		for (int i = 0; i < FakeLandscapes[(int)player].Count; i++)
		{
			if (FakeLandscapes[(int)player][i] == type)
			{
				num++;
			}
		}
		for (int j = 0; j < 4; j++)
		{
			Lane lane = Lanes[(int)player, j];
			if ((lane.Type == type || OverrideLandscapes[(int)player] == type) && !lane.Flipped)
			{
				num++;
			}
		}
		return num;
	}

	public int LandscapeCount(LandscapeType type)
	{
		int num = 0;
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < FakeLandscapes[i].Count; j++)
			{
				if (FakeLandscapes[i][j] == type)
				{
					num++;
				}
			}
			for (int k = 0; k < 4; k++)
			{
				Lane lane = Lanes[i, k];
				if ((lane.Type == type || OverrideLandscapes[i] == type) && !lane.Flipped)
				{
					num++;
				}
			}
		}
		return num;
	}

	public int LandscapeTypeCount(PlayerType player)
	{
		bool[] array = new bool[Enum.GetNames(typeof(LandscapeType)).Length];
		int num = 0;
		if (OverrideLandscapes[(int)player] != LandscapeType.None)
		{
			array[(int)OverrideLandscapes[(int)player]] = true;
			num++;
		}
		for (int i = 0; i < FakeLandscapes[(int)player].Count; i++)
		{
			if (!array[(int)FakeLandscapes[(int)player][i]])
			{
				array[(int)FakeLandscapes[(int)player][i]] = true;
				num++;
			}
		}
		for (int j = 0; j < 4; j++)
		{
			Lane lane = Lanes[(int)player, j];
			if (!array[(int)lane.Type])
			{
				array[(int)lane.Type] = true;
				num++;
			}
		}
		return num;
	}

	public int LandscapeTypeCount()
	{
		bool[] array = new bool[Enum.GetNames(typeof(LandscapeType)).Length];
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			Lane lane = Lanes[(int)PlayerType.User, i];
			if (!array[(int)lane.Type])
			{
				array[(int)lane.Type] = true;
				num++;
			}
			if (!array[(int)lane.OpponentLane.Type])
			{
				array[(int)lane.OpponentLane.Type] = true;
				num++;
			}
		}
		return num;
	}

	public int FlippedLandscapeCount(PlayerType player)
	{
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			Lane lane = Lanes[(int)player, i];
			if (lane.Flipped)
			{
				num++;
			}
		}
		return num;
	}

	public int CreatureFactionCount(PlayerType player)
	{
		bool[] array = new bool[Enum.GetNames(typeof(Faction)).Length];
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			Lane lane = Lanes[(int)player, i];
			if (lane.HasCreature())
			{
				CreatureScript creature = lane.GetCreature();
				Faction faction = creature.Data.Form.Faction;
				if (!array[(int)faction])
				{
					array[(int)faction] = true;
					num++;
				}
			}
		}
		return num;
	}

	public int CreatureFactionCount(PlayerType player, Faction faction)
	{
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			Lane lane = Lanes[(int)player, i];
			if (lane.HasCreature())
			{
				CreatureScript creature = lane.GetCreature();
				if (creature.Data.Form.Faction == faction)
				{
					num++;
				}
			}
		}
		return num;
	}

	public int CreatureCount(PlayerType player)
	{
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			Lane lane = Lanes[(int)player, i];
			if (lane.HasCreature())
			{
				num++;
			}
		}
		return num;
	}

	public int BuildingCount(PlayerType player)
	{
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			Lane lane = Lanes[(int)player, i];
			if (lane.HasBuilding())
			{
				num++;
			}
		}
		return num;
	}

	public int CardCount(PlayerType player, CardType type)
	{
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			Lane lane = Lanes[(int)player, i];
			if (lane.HasCard(type))
			{
				num++;
			}
		}
		return num;
	}

	public int EmptyLaneCount(PlayerType player)
	{
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			Lane lane = Lanes[(int)player, i];
			if (lane.IsEmpty())
			{
				num++;
			}
		}
		return num;
	}

	public int FloopedCardCount(PlayerType player)
	{
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			Lane lane = Lanes[(int)player, i];
			if (lane.HasCreature() && lane.GetCreature().Flooped)
			{
				num++;
			}
		}
		return num;
	}

	public LandscapeType GetLandscapeInDeck(PlayerType player, int lane)
	{
		return Decks[(int)player].GetLandscape(lane);
	}

	public CreatureScript GetCreature(PlayerType player, int lane)
	{
		return Lanes[(int)player, lane].GetCreature();
	}

	public BuildingScript GetBuilding(PlayerType player, int lane)
	{
		return Lanes[(int)player, lane].GetBuilding();
	}

	public CardScript GetScript(PlayerType player, int lane, CardType type)
	{
		return Lanes[(int)player, lane].Scripts[(int)type];
	}

	public int DiscardCount(PlayerType player)
	{
		return DiscardPiles[(int)player].Count;
	}

	public int DiscardCount(PlayerType player, CardType type)
	{
		int num = 0;
		foreach (CardItem item in DiscardPiles[(int)player])
		{
			if (item.Form.Type == type)
			{
				num++;
			}
		}
		return num;
	}

	public int GetDiscount(PlayerType player, CardForm form)
	{
		int num = 0;
		foreach (CardDiscount item in Discounts[(int)player])
		{
			if (item.FactionRequired)
			{
				if ((item.forType == CardType.None || form.Type == item.forType) && form.Faction == item.forFaction)
				{
					num += item.Discount;
				}
			}
			else if (item.forType == CardType.None || form.Type == item.forType)
			{
				num += item.Discount;
			}
		}
		return num;
	}

	public void SetDiscount(PlayerType player, CardType type, int discount, bool factionReq = false, Faction factionType = Faction.Universal)
	{
		CardDiscount item = default(CardDiscount);
		item.forType = type;
		item.FactionRequired = factionReq;
		item.forFaction = factionType;
		item.Discount = discount;
		Discounts[(int)player].Add(item);
	}

	public int GetFloopCostMod(PlayerType player)
	{
		return FloopCostMods[(int)player];
	}

	public void AddFloopCostMod(PlayerType player, int amount)
	{
		FloopCostMods[(int)player] += amount;
	}

	public int GetFlatFloopCost(PlayerType player)
	{
		return FlatFloopCost[(int)player];
	}

	public void SetFlatFloopCost(PlayerType player, int cost)
	{
		FlatFloopCost[(int)player] = cost;
		FloopCostMods[(int)player] = 0;
	}

	public int GetATKPenalty(PlayerType player)
	{
		return ATKPenalty[(int)player];
	}

	public void SetATKPenalty(PlayerType player, int penalty)
	{
		ATKPenalty[(int)player] = penalty;
	}

	public int GetSpellsCast(PlayerType player)
	{
		return SpellsCast[(int)player];
	}

	public void ClearSummonedCards()
	{
		for (int i = 0; i < SummonedCards.Length; i++)
		{
			SummonedCards[i].Clear();
		}
	}

	public Dictionary<string, List<LandscapeType>> GetSummonedCards(PlayerType player)
	{
		return SummonedCards[(int)player];
	}

	public Dictionary<LandscapeType, int> GetCardLandscapedCount(PlayerType player, string cardID)
	{
		Dictionary<LandscapeType, int> dictionary = new Dictionary<LandscapeType, int>();
		if (SummonedCards[(int)player].ContainsKey(cardID))
		{
			for (int i = 0; i < SummonedCards[(int)player][cardID].Count; i++)
			{
				LandscapeType landscapeType = SummonedCards[(int)player][cardID][i];
				if (!dictionary.ContainsKey(landscapeType))
				{
					dictionary[landscapeType] = 1;
					continue;
				}
				Dictionary<LandscapeType, int> dictionary2;
				Dictionary<LandscapeType, int> dictionary3 = (dictionary2 = dictionary);
				LandscapeType key;
				LandscapeType key2 = (key = landscapeType);
				int num = dictionary2[key];
				dictionary3[key2] = num + 1;
			}
		}
		return dictionary;
	}

	public int GetSummonedCardCount(PlayerType player, string cardID)
	{
		if (SummonedCards[(int)player].ContainsKey(cardID))
		{
			return SummonedCards[(int)player][cardID].Count;
		}
		return 0;
	}

	public int GetCreaturesSummoned(PlayerType player)
	{
		return CreaturesSummoned[(int)player];
	}

	public int GetCreaturesRemoved(PlayerType player)
	{
		return CreaturesRemoved[(int)player];
	}

	public void EnableCasting(PlayerType player, CardType type, bool flag)
	{
		CanCast[(int)type, (int)player] = flag;
	}

	public bool IsCastingEnabled(PlayerType player, CardType type)
	{
		return CanCast[(int)type, (int)player];
	}

	public void EnableFlooping(PlayerType player, bool flag)
	{
		CanFloop[(int)player] = false;
	}

	public void RandomizeSummoning(PlayerType player, bool flag)
	{
		RandomizeSummon[(int)player] = flag;
	}

	public bool CanPlayCards(PlayerType player)
	{
		return CanPlay[(int)player];
	}

	public bool IsFloopingEnabled(PlayerType player)
	{
		return CanFloop[(int)player];
	}

	public List<CardItem> GetDweebCup()
	{
		return DweebCup;
	}

	public void FlipLandscape(PlayerType player, int lane, bool flip)
	{
		Lanes[(int)player, lane].Flipped = flip;
		LandscapeManager.FlipLandscape(player, lane, flip);
	}

	public bool IsLandscapeFlipped(PlayerType player, int lane)
	{
		return Lanes[(int)player, lane].Flipped;
	}

	public void HighlightLandscape(PlayerType player, int lane)
	{
		LandscapeManager.HighlightLandscape(player, lane);
	}

	public void UnhighlightLandscape(PlayerType player, int lane)
	{
		LandscapeManager.UnhighlightLandscape(player, lane);
	}

	public void SetTargetingListener(PlayerType side, CardScript script)
	{
		SelectionSide = side;
		TargetingListener = script;
	}

	public void SelectTarget(int idx)
	{
		Lane lane = GetLane(SelectionSide, idx);
		TargetingListener.OnTargetSelected(lane);
	}

	public void ClearHighlights()
	{
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				UnhighlightLandscape(i, j);
			}
		}
	}

	public void StealFrom(PlayerType player)
	{
		if (player == PlayerType.Opponent)
		{
			CWPlayerHandsController cWPlayerHandsController = CWPlayerHandsController.GetInstance();
			if (cWPlayerHandsController != null)
			{
				cWPlayerHandsController.UpdateCards(Hands[(int)player]);
			}
			Stealing = true;
		}
	}

	public void StartTurn(PlayerType player)
	{
		SpellsCast[(int)player] = 0;
		CreaturesSummoned[(int)player] = 0;
		CreaturesRemoved[(int)player] = 0;
		OverrideLandscapes[(int)player] = LandscapeType.None;
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				if (LaneHasCard(player, i, (CardType)j))
				{
					CardScript script = GetScript(player, i, (CardType)j);
					script.Moved = false;
					script.Flooped = false;
					script.Helpless = false;
					script.Protected = false;
					script.Fresh = false;
					script.Used = false;
					script.StartTurn();
				}
			}
		}
		while (PersistentSpellsInEffect[(int)(!player)].Count > 0)
		{
			CardScript cardScript = PersistentSpellsInEffect[(int)(!player)][0];
			cardScript.OnOpponentStartTurn();
		}
		if (Decks[(int)player].CardCount() != 0)
		{
			DrawCard(player);
			if (player == PlayerType.Opponent)
			{
				TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.OpponentTurn);
			}
		}
		else
		{
			phaseMgr = BattlePhaseManager.GetInstance();
			if (player == PlayerType.User)
			{
				phaseMgr.Phase = BattlePhase.OutOfCards;
			}
		}
		QuestEarningManager.GetInstance().dropedThisBattle = false;
	}

	public void DoResultBleed(PlayerType player)
	{
		if (player == PlayerType.User)
		{
			LeaderItem leader = GetLeader(player);
			DealDamage(player, leader.Rank);
		}
	}

	public void ReshuffleOpponentCards(PlayerType player)
	{
		List<CardItem> hand = GetHand(player);
		while (hand.Count > 0)
		{
			CardItem card = hand[0];
			hand.RemoveAt(0);
			DiscardCard(player, card);
		}
		List<CardItem> discardPile = Instance.GetDiscardPile(player);
		Deck deck = Instance.GetDeck(player);
		while (discardPile.Count > 0)
		{
			CardItem newCard = discardPile[0];
			discardPile.RemoveAt(0);
			deck.AddCard(newCard);
		}
		Reshuffle(player);
		for (int i = 0; i < 5; i++)
		{
			DrawCard(player);
		}
	}

	public void EndVFX(PlayerType player, string context)
	{
		GameObject vfx = null;
		if (CharacterFXList[(int)player].ContainsKey(context))
		{
			vfx = CharacterFXList[(int)player][context];
			CharacterFXList[(int)player].Remove(context);
		}
		CWFloopActionManager.GetInstance().RemoveVFX(vfx);
	}

	public void ResetCasting(PlayerType player)
	{
		CanCast[0, (int)player] = true;
		EndVFX(player, "BlockCreature");
		CanCast[1, (int)player] = true;
		EndVFX(player, "BlockBuilding");
		CanCast[2, (int)player] = true;
		EndVFX(player, "BlockSpell");
	}

	public void EndTurn(PlayerType player)
	{
		if (GameData.FirstPlayer - 1 == (int)player)
		{
			CurrentMagicPoints++;
			if (CurrentMagicPoints > ParametersManager.Instance.Max_Magic_Points)
			{
				CurrentMagicPoints = ParametersManager.Instance.Max_Magic_Points;
			}
		}
		MagicPoints[(int)player] = CurrentMagicPoints + BonusPoints[(int)player];
		BonusPoints[(int)player] = 0;
		FloopCountTurn[(int)player] = 0;
		Discounts[(int)player].Clear();
		FloopCostMods[(int)player] = 0;
		FlatFloopCost[(int)player] = -1;
		ATKPenalty[(int)player] = 0;
		ResetCasting(player);
		CanPlay[(int)player] = true;
		CanFloop[(int)player] = true;
		RandomizeSummon[(int)player] = false;
		SpellsInEffect[(int)player].Clear();
		CWFloopActionManager.GetInstance().RemovePersistantVFX("BlockSpell");
		CWFloopActionManager.GetInstance().RemovePersistantVFX("BlockBuilding");
		CWFloopActionManager.GetInstance().RemovePersistantVFX("BlockCreature");
		if (LeaderCooldown[(int)player] > 0)
		{
			LeaderCooldown[(int)player]--;
		}
		for (int i = 0; i < 4; i++)
		{
			Lane lane = GetLane(player, i);
			if (lane.Disabled)
			{
				CWFloopActionManager.GetInstance().RemovePersistantVFX(player, i);
			}
			lane.Disabled = false;
			for (int j = 0; j < 2; j++)
			{
				if (LaneHasCard(player, i, (CardType)j))
				{
					CardScript script = GetScript(player, i, (CardType)j);
					script.FloopBlocked = false;
					script.CantAttack = false;
				}
			}
		}
		if (player == PlayerType.User)
		{
			QuestConditionManager.Instance.currentStats.NumTurns++;
			PanelManagerBattle.GetInstance().QuestStatusUpdate();
			CWPlayerHandsController.GetInstance().FlushDynamicTextures();
		}
	}

	public List<CreatureScript> GetCreaturesInDanger(PlayerType player)
	{
		List<CreatureScript> list = new List<CreatureScript>();
		List<Lane> lanes = GetLanes(player);
		foreach (Lane item in lanes)
		{
			if (item.HasCreature())
			{
				CreatureScript creature = item.GetCreature();
				if (creature.InDanger)
				{
					list.Add(creature);
				}
			}
		}
		return list;
	}

	public List<CreatureScript> GetCreaturesWithinHealthThreshold(PlayerType player, int threshold)
	{
		List<CreatureScript> list = new List<CreatureScript>();
		List<Lane> lanes = GetLanes(player);
		foreach (Lane item in lanes)
		{
			if (item.HasCreature())
			{
				CreatureScript creature = item.GetCreature();
				if (creature.Health <= threshold)
				{
					list.Add(creature);
				}
			}
		}
		return list;
	}

	public List<CreatureScript> GetCreaturesWithinHealthThresholdOfEnemy(PlayerType player, int threshold)
	{
		List<CreatureScript> list = new List<CreatureScript>();
		List<Lane> lanes = GetLanes(player);
		foreach (Lane item in lanes)
		{
			if (item.HasCreature())
			{
				CreatureScript creature = item.GetCreature();
				int num = 0;
				if (creature.Enemy != null)
				{
					num = creature.Enemy.ATK;
				}
				if (creature.Health <= threshold + num)
				{
					list.Add(creature);
				}
			}
		}
		return list;
	}

	public int ScoreHealth(PlayerType player)
	{
		List<Lane> lanes = GetLanes(!player);
		int num = 0;
		foreach (Lane item in lanes)
		{
			if (item.HasCreature())
			{
				CreatureScript creature = item.GetCreature();
				if (creature.Enemy == null)
				{
					num += creature.ATK;
				}
			}
		}
		int health = GetHealth(player);
		if (num <= 0)
		{
			return health;
		}
		return Math.Max(0, health - num);
	}

	public int ScoreHealth(PlayerType player, StatMod[,] LaneMods, int mod)
	{
		List<Lane> lanes = GetLanes(!player);
		int num = 0;
		foreach (Lane item in lanes)
		{
			if (item.HasCreature())
			{
				CreatureScript creature = item.GetCreature();
				if (creature.Enemy == null)
				{
					num += Math.Max(0, creature.ATK + LaneMods[(int)(!player), item.Index].ATK);
				}
			}
		}
		int num2 = GetHealth(player) + mod;
		if (num <= 0)
		{
			return num2;
		}
		return Math.Max(0, num2 - num);
	}

	public int ScoreHealth(PlayerType player, CardItem item, Lane TargetLane)
	{
		List<Lane> lanes = GetLanes(!player);
		int num = 0;
		foreach (Lane item2 in lanes)
		{
			if (item2 == TargetLane && !TargetLane.OpponentLane.HasCreature())
			{
				num += item.ATK;
			}
			else if (item2.HasCreature())
			{
				CreatureScript creature = item2.GetCreature();
				if (creature.Enemy == null && item2.OpponentLane != TargetLane)
				{
					num += creature.ATK;
				}
			}
		}
		int health = GetHealth(player);
		if (num <= 0)
		{
			return health;
		}
		return Math.Max(0, health - num);
	}

	public int ScoreActionPoints(PlayerType player)
	{
		return GetMagicPoints(player) * 1;
	}

	public int ScoreCardsInHand(PlayerType player)
	{
		return Hands[(int)player].Count * 3;
	}

	public int ScoreLane(Lane CurrentLane)
	{
		int result = 0;
		if (CurrentLane.HasCreature())
		{
			CreatureScript creature = CurrentLane.GetCreature();
			result = ((creature.Enemy != null) ? (creature.ATK + Math.Max(0, creature.Health - creature.Enemy.ATK)) : (creature.ATK + creature.Health));
		}
		else if (CurrentLane.OpponentLane.HasCreature())
		{
			CreatureScript creature2 = CurrentLane.OpponentLane.GetCreature();
			result = -creature2.ATK;
		}
		return result;
	}

	public int ScoreLane(PlayerType player, CardItem Item, Lane CurrentLane)
	{
		int num = 0;
		if (CurrentLane.OpponentLane.HasCreature())
		{
			CreatureScript creature = CurrentLane.OpponentLane.GetCreature();
			return Item.ATK + Math.Max(0, Item.DEF - creature.ATK);
		}
		return Item.ATK + Item.DEF;
	}

	public int ScoreLaneDefender(CardItem Item, Lane CurrentLane)
	{
		int num = 0;
		if (CurrentLane.HasCreature())
		{
			CreatureScript creature = CurrentLane.GetCreature();
			return creature.ATK + Math.Max(0, creature.Health - Item.ATK);
		}
		return -Item.ATK;
	}

	public int ScoreLane(StatMod[] Mods, int EnemyHPMod, Lane CurrentLane)
	{
		int result = 0;
		if (CurrentLane.HasCreature())
		{
			CreatureScript creature = CurrentLane.GetCreature();
			if (creature.Enemy != null)
			{
				int num = Math.Max(0, creature.ATK + Mods[0].ATK);
				int num2 = Math.Max(0, creature.Health + Mods[0].DEF);
				int num3 = Math.Max(0, creature.Enemy.ATK + Mods[1].ATK);
				result = ((num2 <= 0) ? (-num3) : (num + Math.Max(0, num2 - num3)));
			}
			else
			{
				int num4 = Math.Max(0, creature.Health + Mods[0].DEF);
				int num5 = Math.Max(0, creature.ATK + Mods[0].ATK);
				if (num4 > 0)
				{
					result = num5 + num4;
				}
			}
		}
		else if (CurrentLane.OpponentLane.HasCreature())
		{
			CreatureScript creature2 = CurrentLane.OpponentLane.GetCreature();
			int num6 = Math.Max(0, creature2.ATK + Mods[1].ATK);
			result = -num6 + Mods[0].EmptyLaneScore;
		}
		else
		{
			result = Mods[0].EmptyLaneScore;
		}
		return result;
	}

	public int ScoreBoard(PlayerType player)
	{
		List<Lane> lanes = GetLanes(player);
		int num = 0;
		foreach (Lane item in lanes)
		{
			num += ScoreLane(item);
		}
		num += ScoreHealth(player);
		num += ScoreActionPoints(player);
		return num + ScoreCardsInHand(player);
	}

	public int ScoreBoard()
	{
		int num = ScoreBoard(PlayerType.User);
		int num2 = ScoreBoard(PlayerType.Opponent);
		return num2 - num;
	}

	public int ScoreBoard(PlayerType player, CardItem Item, Lane CurrentLane)
	{
		int num = ScoreBoard(player);
		int num2 = ScoreBoard(!player);
		int num3 = ScoreHealth(player);
		int num4 = ScoreHealth(!player);
		int num5 = ScoreHealth(player, Item, CurrentLane);
		int num6 = ScoreHealth(!player, Item, CurrentLane);
		int num7 = ScoreLane(CurrentLane);
		int num8 = ScoreLane(CurrentLane.OpponentLane);
		int num9 = ScoreLane(player, Item, CurrentLane);
		int num10 = ScoreLaneDefender(Item, CurrentLane.OpponentLane);
		int num11 = ScoreActionPoints(player);
		int num12 = (GetMagicPoints(player) - Item.Form.DetermineCost(player)) * 1;
		num = num - num7 - num11 - num3 + (num9 + num12 + num5);
		num2 = num2 - num8 - num4 + (num10 + num6);
		return num - num2;
	}

	public int ScoreBoard(PlayerType player, StatMod[,] LaneMods, int[] HealthMod, int[] APMod)
	{
		int num = ScoreBoard(player);
		int num2 = ScoreBoard(!player);
		int num3 = ScoreHealth(player);
		int num4 = ScoreHealth(!player);
		int num5 = ScoreHealth(player, LaneMods, HealthMod[(int)player]);
		int num6 = ScoreHealth(!player, LaneMods, HealthMod[(int)(!player)]);
		int num7 = ScoreActionPoints(player);
		int num8 = ScoreActionPoints(!player);
		int num9 = (GetMagicPoints(player) + APMod[(int)player]) * 1;
		int num10 = (GetMagicPoints(!player) + APMod[(int)(!player)]) * 1;
		num = num - num3 - num7 + (num5 + num9);
		num2 = num2 - num4 - num8 + (num6 + num10);
		StatMod[] array = new StatMod[2];
		for (int i = 0; i < 4; i++)
		{
			Lane lane = GetLane(player, i);
			int num11 = ScoreLane(lane);
			int num12 = ScoreLane(lane.OpponentLane);
			array[0] = LaneMods[(int)player, i];
			array[1] = LaneMods[(int)(!player), lane.OpponentLane.Index];
			int num13 = ScoreLane(array, HealthMod[(int)(!player)], lane);
			array[0] = LaneMods[(int)(!player), lane.OpponentLane.Index];
			array[1] = LaneMods[(int)player, i];
			int num14 = ScoreLane(array, HealthMod[(int)player], lane.OpponentLane);
			num = num - num11 + num13;
			num2 = num2 - num12 + num14;
		}
		return num - num2;
	}
}
