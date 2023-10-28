using System;
using System.Collections.Generic;
using UnityEngine;

public class CardScript
{
	public Dictionary<string, GameObject> FXList = new Dictionary<string, GameObject>();

	public CardScript TargetScript;

	protected static StatMod[,] LaneMods = new StatMod[2, 4]
	{
		{
			default(StatMod),
			default(StatMod),
			default(StatMod),
			default(StatMod)
		},
		{
			default(StatMod),
			default(StatMod),
			default(StatMod),
			default(StatMod)
		}
	};

	protected static int[] HealthMods = new int[2];

	protected static int[] APMods = new int[2];

	protected List<CardScript> TargetList = new List<CardScript>();

	protected bool isProtected;

	protected bool cantAttack;

	protected bool isHelpless;

	protected bool isFloopBlocked;

	public bool FloopDisabled;

	public CardItem Data { get; set; }

	public Lane CurrentLane { get; set; }

	public PlayerType Owner { get; set; }

	public bool Flooped { get; set; }

	public bool Flooping { get; set; }

	public bool IsSummoning { get; set; }

	public bool Moved { get; set; }

	public bool Fresh { get; set; }

	public bool Used { get; set; }

	public bool CardSelected { get; set; }

	public bool Protected
	{
		get
		{
			return isProtected;
		}
		set
		{
			isProtected = value;
			if (!value && CWFloopActionManager.GetInstance() != null && CurrentLane.OpponentLane.HasCreature())
			{
				CreatureScript creature = CurrentLane.OpponentLane.GetCreature();
				creature.EndVFX("Protection");
			}
		}
	}

	public bool CantAttack
	{
		get
		{
			return cantAttack;
		}
		set
		{
			cantAttack = value;
			if (!value && CWFloopActionManager.GetInstance() != null)
			{
				EndVFX("BlockAttack");
			}
		}
	}

	public bool FloopBlocked
	{
		get
		{
			return isFloopBlocked;
		}
		set
		{
			if (value != isFloopBlocked)
			{
				CWCreatureStatsFloorDisplay cWCreatureStatsFloorDisplay = ((Owner != PlayerType.User) ? PanelManagerBattle.GetInstance().P2FloorDisplays[CurrentLane.Index] : PanelManagerBattle.GetInstance().P1FloorDisplays[CurrentLane.Index]);
				CWFloopButtonTextureController component = cWCreatureStatsFloorDisplay.floopButton.GetComponent<CWFloopButtonTextureController>();
				if (value)
				{
					component.SetButtonColor(FloopButtonColor.Red);
				}
				else
				{
					component.SetButtonColor(FloopButtonColor.Green);
				}
			}
			isFloopBlocked = value;
			if (!value && CWFloopActionManager.GetInstance() != null)
			{
				EndVFX("BlockFloop");
			}
		}
	}

	public bool Helpless
	{
		get
		{
			return isHelpless;
		}
		set
		{
			isHelpless = value;
			if (!value && CWFloopActionManager.GetInstance() != null)
			{
				EndVFX("Helpless");
			}
		}
	}

	public GameState GameInstance { get; set; }

	public void EndAllVFX()
	{
		if (isProtected && CurrentLane.OpponentLane.HasCreature())
		{
			CreatureScript creature = CurrentLane.OpponentLane.GetCreature();
			creature.EndVFX("Protection");
		}
		foreach (GameObject value in FXList.Values)
		{
			CWFloopActionManager.GetInstance().RemoveVFX(value);
		}
		if (FloopBlocked)
		{
			CWCreatureStatsFloorDisplay cWCreatureStatsFloorDisplay = ((Owner != PlayerType.User) ? PanelManagerBattle.GetInstance().P2FloorDisplays[CurrentLane.Index] : PanelManagerBattle.GetInstance().P1FloorDisplays[CurrentLane.Index]);
			CWFloopButtonTextureController component = cWCreatureStatsFloorDisplay.floopButton.GetComponent<CWFloopButtonTextureController>();
			component.SetButtonColor(FloopButtonColor.Green);
		}
		FXList.Clear();
	}

	private void OnEnable()
	{
		GameInstance = GameState.Instance;
	}

	public static int DetermineCost(PlayerType player, CardForm card)
	{
		int num = card.Cost - GameState.Instance.GetDiscount(player, card);
		if (num < 0)
		{
			num = 0;
		}
		return num;
	}

	public int DetermineFloopCost()
	{
		int num = GameInstance.GetFloopCostMod(Owner) + CurrentLane.FloopMod;
		int num2 = Data.Form.FloopCost + num;
		int flatFloopCost = GameInstance.GetFlatFloopCost(Owner);
		if (flatFloopCost >= 0)
		{
			num2 = flatFloopCost + num;
		}
		if (num2 < 0)
		{
			num2 = 0;
		}
		return num2;
	}

	public static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		GameState instance = GameState.Instance;
		if (!instance.CanPlayCards(player))
		{
			return false;
		}
		if (!instance.IsCastingEnabled(player, card.Type))
		{
			return false;
		}
		if (lane >= 0)
		{
			Lane lane2 = instance.GetLane(player, lane);
			if ((card.Type == CardType.Creature || card.Type == CardType.Building) && (lane2.Disabled || card.Rarity > lane2.RarityGate))
			{
				return false;
			}
		}
		if (card.Faction == Faction.Universal || instance.IsLaneOfLandscapeType(player, lane, (LandscapeType)card.Faction))
		{
			int num = card.DetermineCost(player);
			int magicPoints = instance.GetMagicPoints(player);
			if (magicPoints >= num)
			{
				return true;
			}
		}
		return false;
	}

	public static int EvaluateLanePlacement(PlayerType player, Lane Candidate, CardItem item)
	{
		return GameState.Instance.ScoreBoard() - item.Form.DetermineCost(player) * 1;
	}

	public virtual void PopulateLaneMods()
	{
	}

	public virtual int EvaluateAbility()
	{
		ResetMods();
		PopulateLaneMods();
		APMods[(int)Owner] = -DetermineFloopCost();
		return ScoreBoard(Owner);
	}

	public static int ScoreBoard(PlayerType player)
	{
		return GameState.Instance.ScoreBoard(player, LaneMods, HealthMods, APMods);
	}

	protected void StartTargetSelection(PlayerType player, SelectionType targetType, string instruction)
	{
		UICamera.useInputEnabler = false;
		Flooping = true;
		GameState instance = GameState.Instance;
		if (player == PlayerType.Opponent)
		{
			BattlePhaseManager.GetInstance().Phase = BattlePhase.P1SetupLaneOpp;
		}
		else
		{
			BattlePhaseManager.GetInstance().Phase = BattlePhase.P1SetupLanePlyr;
		}
		bool flag = false;
		for (int i = 0; i < 4; i++)
		{
			Lane lane = instance.GetLane(player, i);
			if (SelectionFilter(lane))
			{
				flag = true;
				instance.HighlightLandscape(player, i);
			}
		}
		instance.SetTargetingListener(player, this);
		if (!flag)
		{
			EndTargetSelection();
		}
	}

	protected CWList<Lane> AITargetSelection(PlayerType player, SelectionType targetType)
	{
		CWList<Lane> cWList = new CWList<Lane>();
		GameState instance = GameState.Instance;
		for (int i = 0; i < 4; i++)
		{
			Lane lane = instance.GetLane(player, i);
			if (SelectionFilter(lane))
			{
				cWList.Add(lane);
			}
		}
		return cWList;
	}

	public virtual bool SelectionFilter(Lane candidate)
	{
		return false;
	}

	public virtual void OnTargetSelected(Lane target)
	{
	}

	protected void EndTargetSelection()
	{
		Flooping = false;
		GameState instance = GameState.Instance;
		instance.ClearHighlights();
	}

	public void OpenDiscardPile()
	{
		UICamera.useInputEnabler = false;
		CWDiscardPileSet instance = CWDiscardPileSet.GetInstance(Owner);
		UIButtonTween component = instance.gameObject.GetComponent<UIButtonTween>();
		UIButtonTween[] components = instance.gameObject.GetComponents<UIButtonTween>();
		instance.SetFilterScript(this);
		UIButtonTween[] array = components;
		foreach (UIButtonTween uIButtonTween in array)
		{
			uIButtonTween.Play(true);
		}
		BoxCollider[] componentsInChildren = component.tweenTarget.GetComponentsInChildren<BoxCollider>();
		BoxCollider[] array2 = componentsInChildren;
		foreach (BoxCollider boxCollider in array2)
		{
			if (boxCollider.name == "DiscardCloseButton")
			{
				boxCollider.enabled = false;
			}
		}
		CardSelected = false;
	}

	public void CloseDiscardPile()
	{
		CWDiscardPileSet instance = CWDiscardPileSet.GetInstance(Owner);
		UIButtonTween component = instance.gameObject.GetComponent<UIButtonTween>();
		GameObject tweenTarget = component.tweenTarget;
		GameObject gameObject = tweenTarget.transform.Find("DiscardCloseButton").gameObject;
		instance.SetFilterScript(null);
		gameObject.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		BoxCollider component2 = gameObject.GetComponent<BoxCollider>();
		if (component2 != null)
		{
			component2.enabled = true;
		}
		CardSelected = true;
	}

	public virtual bool CardFilter(CardItem item)
	{
		return false;
	}

	public virtual void CardSelection(CardItem item)
	{
	}

	public virtual bool CanFloop()
	{
		return false;
	}

	public virtual void CancelFloop()
	{
		EndTargetSelection();
	}

	public virtual void OnSummon()
	{
		IsSummoning = true;
	}

	public virtual void Update()
	{
	}

	public void FinishSummoning()
	{
		IsSummoning = false;
		GameState instance = GameState.Instance;
		instance.FinishSummoning(this);
	}

	public virtual void OnCardLeftPlay(CardScript script)
	{
	}

	public virtual void OnCardEnterPlay(CardScript script)
	{
	}

	public virtual void Floop()
	{
	}

	public virtual void StartTurn()
	{
	}

	public virtual void OnOpponentStartTurn()
	{
	}

	public virtual void OnCreatureDied(CardItem deadCard)
	{
	}

	protected void DoEffect()
	{
		CWFloopActionManager.GetInstance().DoEffect(this, TargetList.ToArray());
		TargetList.Clear();
	}

	public virtual bool DoResult(CardScript target)
	{
		return true;
	}

	public void EndVFX(string context)
	{
		GameObject vfx = null;
		if (FXList.ContainsKey(context))
		{
			vfx = FXList[context];
			FXList.Remove(context);
		}
		CWFloopActionManager.GetInstance().RemoveVFX(vfx);
	}

	public static void ResetMods()
	{
		for (int i = 0; i < 2; i++)
		{
			HealthMods[i] = 0;
			APMods[i] = 0;
			for (int j = 0; j < 4; j++)
			{
				LaneMods[i, j].Reset();
			}
		}
	}

	public static List<CreatureScript> LanesToCreatures(CWList<Lane> Lanes)
	{
		List<CreatureScript> list = new List<CreatureScript>();
		foreach (Lane Lane in Lanes)
		{
			if (Lane.HasCreature())
			{
				CreatureScript creature = Lane.GetCreature();
				list.Add(creature);
			}
		}
		return list;
	}

	public static List<BuildingScript> LanesToBuildings(CWList<Lane> Lanes)
	{
		List<BuildingScript> list = new List<BuildingScript>();
		foreach (Lane Lane in Lanes)
		{
			if (Lane.HasBuilding())
			{
				BuildingScript building = Lane.GetBuilding();
				list.Add(building);
			}
		}
		return list;
	}

	public static List<CreatureScript> GetCreaturesWithinHealthThreshold(List<CreatureScript> Candidates, int threshold)
	{
		List<CreatureScript> list = new List<CreatureScript>();
		foreach (CreatureScript Candidate in Candidates)
		{
			switch (threshold)
			{
			case -1:
				list.Add(Candidate);
				continue;
			case -2:
				if (Candidate.Health <= Candidate.ATK)
				{
					list.Add(Candidate);
					continue;
				}
				break;
			}
			if (threshold == -3 && Candidate.Health <= Candidate.Damage)
			{
				list.Add(Candidate);
			}
			else if (Candidate.Health <= threshold)
			{
				list.Add(Candidate);
			}
		}
		return list;
	}

	public static List<CreatureScript> GetCreaturesWithinHealthThresholdOfEnemy(List<CreatureScript> Candidates, int threshold)
	{
		List<CreatureScript> list = new List<CreatureScript>();
		foreach (CreatureScript Candidate in Candidates)
		{
			if (threshold == -1)
			{
				list.Add(Candidate);
				continue;
			}
			int num = 0;
			if (Candidate.Enemy != null)
			{
				num = Candidate.Enemy.ATK;
			}
			if (threshold == -2 && Candidate.Health <= Candidate.ATK + num)
			{
				list.Add(Candidate);
			}
			else if (threshold == -3 && Candidate.Health <= Candidate.Damage + num)
			{
				list.Add(Candidate);
			}
			else if (Candidate.Health <= threshold + num)
			{
				list.Add(Candidate);
			}
		}
		return list;
	}

	public static CreatureScript BestCandidateForAttack(CWList<Lane> Candidates, int amount)
	{
		List<CreatureScript> list = LanesToCreatures(Candidates);
		List<CreatureScript> creaturesWithinHealthThreshold = GetCreaturesWithinHealthThreshold(list, amount);
		List<CreatureScript> creaturesWithinHealthThresholdOfEnemy = GetCreaturesWithinHealthThresholdOfEnemy(list, amount);
		List<CreatureScript> list2 = new List<CreatureScript>();
		List<CreatureScript> list3 = null;
		CreatureScript creatureScript = null;
		if (creaturesWithinHealthThreshold.Count > 0)
		{
			foreach (CreatureScript item in creaturesWithinHealthThreshold)
			{
				if (item.CanWin)
				{
					list2.Add(item);
				}
			}
		}
		list3 = ((list2.Count > 0) ? list2 : ((creaturesWithinHealthThreshold.Count > 0) ? creaturesWithinHealthThreshold : ((creaturesWithinHealthThresholdOfEnemy.Count <= 0) ? list : creaturesWithinHealthThresholdOfEnemy)));
		foreach (CreatureScript item2 in list3)
		{
			if (creatureScript == null || item2.Health > creatureScript.Health)
			{
				creatureScript = item2;
			}
		}
		return creatureScript;
	}

	public List<CreatureScript> GetCreaturesInDanger(List<CreatureScript> Candidates)
	{
		List<CreatureScript> list = new List<CreatureScript>();
		foreach (CreatureScript Candidate in Candidates)
		{
			if (Candidate.InDanger)
			{
				list.Add(Candidate);
			}
		}
		return list;
	}

	public static CreatureScript BestCandidateForHealing(CWList<Lane> Candidates, int amount, bool capDEF)
	{
		List<CreatureScript> list = LanesToCreatures(Candidates);
		List<CreatureScript> list2 = new List<CreatureScript>();
		List<CreatureScript> list3 = new List<CreatureScript>();
		List<CreatureScript> list4 = null;
		CreatureScript creatureScript = null;
		CreatureScript creatureScript2 = null;
		foreach (CreatureScript item in list)
		{
			int num = amount;
			switch (amount)
			{
			case -1:
				num = item.Damage;
				break;
			case -2:
				num = item.ATK;
				break;
			}
			int num2 = item.Health + num;
			if (capDEF)
			{
				num2 = Math.Min(num2, item.DEF);
			}
			if (item.Enemy != null && item.InDanger && !item.Enemy.InDanger)
			{
				if (num2 > item.Enemy.ATK)
				{
					list2.Add(item);
				}
				else
				{
					list3.Add(item);
				}
			}
		}
		list4 = ((list2.Count <= 0) ? list : list2);
		foreach (CreatureScript item2 in list4)
		{
			if (!list3.Contains(item2) && (creatureScript == null || item2.ATK > creatureScript.ATK))
			{
				creatureScript = item2;
			}
			if (creatureScript2 == null || item2.ATK > creatureScript2.ATK)
			{
				creatureScript2 = item2;
			}
		}
		if (creatureScript == null)
		{
			creatureScript = creatureScript2;
		}
		return creatureScript;
	}

	public CreatureScript MaximizeAdjacentTargets(CreatureScript BestTarget, CreatureScript SecondTarget)
	{
		Lane innerLane = BestTarget.CurrentLane.GetInnerLane();
		if (innerLane.HasCreature())
		{
			BestTarget = innerLane.GetCreature();
		}
		if (SecondTarget != null && !SecondTarget.CurrentLane.IsAdjacentTo(BestTarget.CurrentLane))
		{
			CreatureScript creatureScript = null;
			Lane[] adjacentLanes = SecondTarget.CurrentLane.AdjacentLanes;
			foreach (Lane lane in adjacentLanes)
			{
				Lane[] adjacentLanes2 = BestTarget.CurrentLane.AdjacentLanes;
				foreach (Lane lane2 in adjacentLanes2)
				{
					if (lane != null && lane == lane2 && lane.HasCreature())
					{
						creatureScript = lane.GetCreature();
					}
				}
			}
			if (creatureScript != null)
			{
				BestTarget = creatureScript;
			}
		}
		return BestTarget;
	}

	public CreatureScript BestCandidateForCrippling(CWList<Lane> Candidates, int amount)
	{
		List<CreatureScript> list = LanesToCreatures(Candidates);
		List<CreatureScript> list2 = new List<CreatureScript>();
		List<CreatureScript> list3 = new List<CreatureScript>();
		List<CreatureScript> list4 = new List<CreatureScript>();
		CreatureScript creatureScript = null;
		CreatureScript creatureScript2 = null;
		foreach (CreatureScript item in list)
		{
			if (item.CanWin && item.ATK - amount < GameInstance.GetHealth(Owner))
			{
				list2.Add(item);
			}
			if (item.Enemy != null && item.Enemy.InDanger && !item.InDanger)
			{
				if (item.ATK - amount < item.Enemy.Health)
				{
					list3.Add(item);
				}
				else
				{
					list4.Add(item);
				}
			}
		}
		List<CreatureScript> list5 = null;
		list5 = ((list2.Count > 0) ? list2 : ((list3.Count <= 0) ? list : list3));
		foreach (CreatureScript item2 in list5)
		{
			if (!list4.Contains(item2) && (creatureScript == null || creatureScript.ATK < item2.ATK))
			{
				creatureScript = item2;
			}
			if (creatureScript2 == null || creatureScript2.ATK < item2.ATK)
			{
				creatureScript2 = item2;
			}
		}
		if (creatureScript == null)
		{
			creatureScript = creatureScript2;
		}
		return creatureScript;
	}

	public CreatureScript BestCandidateForATKBonus(CWList<Lane> Candidates, int amount)
	{
		List<CreatureScript> list = LanesToCreatures(Candidates);
		List<CreatureScript> list2 = new List<CreatureScript>();
		List<CreatureScript> list3 = new List<CreatureScript>();
		List<CreatureScript> list4 = new List<CreatureScript>();
		List<CreatureScript> list5 = null;
		CreatureScript creatureScript = null;
		foreach (CreatureScript item in list)
		{
			if (item.Enemy == null)
			{
				list4.Add(item);
			}
			else if (!item.Enemy.InDanger && item.InDanger && item.ATK + amount >= item.Enemy.Health)
			{
				list3.Add(item);
			}
			else if (!item.Enemy.InDanger && item.ATK + amount >= item.Enemy.Health)
			{
				list2.Add(item);
			}
		}
		list5 = ((list3.Count > 0) ? list3 : ((list2.Count > 0) ? list2 : ((list4.Count <= 0) ? list : list4)));
		foreach (CreatureScript item2 in list5)
		{
			if (creatureScript == null || item2.ATK > creatureScript.ATK)
			{
				creatureScript = item2;
			}
		}
		return creatureScript;
	}

	public CreatureScript BestCandidateForReset(CWList<Lane> Candidates)
	{
		List<CreatureScript> list = LanesToCreatures(Candidates);
		CreatureScript creatureScript = null;
		int num = 0;
		foreach (CreatureScript item in list)
		{
			ResetMods();
			LaneMods[(int)Owner, item.CurrentLane.Index].ATK = -item.ATKMod;
			LaneMods[(int)Owner, item.CurrentLane.Index].DEF = item.Damage - item.DEFMod;
			int num2 = ScoreBoard(Owner);
			if (creatureScript == null || num2 > num)
			{
				creatureScript = item;
				num = num2;
			}
		}
		return creatureScript;
	}

	public static CreatureScript BestSacrifice(CWList<Lane> Candidates)
	{
		List<CreatureScript> list = LanesToCreatures(Candidates);
		List<CreatureScript> list2 = new List<CreatureScript>();
		List<CreatureScript> list3 = new List<CreatureScript>();
		List<CreatureScript> list4 = new List<CreatureScript>();
		List<CreatureScript> list5 = null;
		foreach (CreatureScript item in list)
		{
			if (item.Enemy != null)
			{
				if (item.InDanger && !item.Enemy.InDanger)
				{
					list2.Add(item);
				}
				else
				{
					list4.Add(item);
				}
			}
			else
			{
				list3.Add(item);
			}
		}
		CreatureScript creatureScript = null;
		list5 = ((list3.Count > 0) ? list3 : ((list2.Count <= 0) ? list4 : list2));
		foreach (CreatureScript item2 in list5)
		{
			if (creatureScript == null)
			{
				creatureScript = item2;
			}
			else if (item2.Enemy != null)
			{
				if (item2.Enemy.ATK < creatureScript.Enemy.ATK)
				{
					creatureScript = item2;
				}
			}
			else if (item2.ATK < creatureScript.ATK)
			{
				creatureScript = item2;
			}
		}
		return creatureScript;
	}

	public static BuildingScript BestBuildingSacrifice(CWList<Lane> Candidates)
	{
		List<BuildingScript> list = LanesToBuildings(Candidates);
		int num = 0;
		BuildingScript buildingScript = null;
		foreach (BuildingScript item in list)
		{
			item.StopTriggerEffects();
			item.OnCardLeftPlay(item);
			item.ResumeTriggerEffects();
			int num2 = GameState.Instance.ScoreBoard();
			item.StopTriggerEffects();
			item.OnCardEnterPlay(item);
			item.ResumeTriggerEffects();
			if (buildingScript == null)
			{
				buildingScript = item;
				num = num2;
			}
			else if (num2 > num)
			{
				buildingScript = item;
				num = num2;
			}
		}
		return buildingScript;
	}

	public static Lane BestDoubleBuildingSacrifice(PlayerType player, CWList<Lane> Candidates)
	{
		int num = 0;
		Lane lane = null;
		foreach (Lane Candidate in Candidates)
		{
			ResetMods();
			if (Candidate.HasBuilding())
			{
				BuildingScript building = Candidate.GetBuilding();
				building.StopTriggerEffects();
				building.OnCardLeftPlay(building);
				building.ResumeTriggerEffects();
			}
			if (Candidate.OpponentLane.HasBuilding())
			{
				BuildingScript building2 = Candidate.OpponentLane.GetBuilding();
				building2.StopTriggerEffects();
				building2.OnCardLeftPlay(building2);
				building2.ResumeTriggerEffects();
			}
			if (Candidate.HasCreature())
			{
				CreatureScript creature = Candidate.GetCreature();
				LaneMods[(int)creature.Owner, Candidate.Index].DEF = -creature.Health;
			}
			if (Candidate.OpponentLane.HasCreature())
			{
				CreatureScript creature2 = Candidate.OpponentLane.GetCreature();
				LaneMods[(int)creature2.Owner, Candidate.OpponentLane.Index].DEF = -creature2.Health;
			}
			int num2 = ScoreBoard(player);
			if (Candidate.HasBuilding())
			{
				BuildingScript building3 = Candidate.GetBuilding();
				building3.StopTriggerEffects();
				building3.OnCardEnterPlay(building3);
				building3.ResumeTriggerEffects();
			}
			if (Candidate.OpponentLane.HasBuilding())
			{
				BuildingScript building4 = Candidate.OpponentLane.GetBuilding();
				building4.StopTriggerEffects();
				building4.OnCardEnterPlay(building4);
				building4.ResumeTriggerEffects();
			}
			if (lane == null)
			{
				lane = Candidate;
				num = num2;
			}
			else if (num2 > num)
			{
				lane = Candidate;
				num = num2;
			}
		}
		return lane;
	}
}
