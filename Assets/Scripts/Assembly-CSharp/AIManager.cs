using System.Collections.Generic;

public class AIManager
{
	private static AIManager g_AIManager;

	public static int WEIGHT_SCALE = 100;

	public static int DEF_FACTOR = 2;

	private WeightedList<AIDecision> DecisionList = new WeightedList<AIDecision>();

	private List<Lane> Lanes;

	public AIDecision Decision;

	public static AIManager Instance
	{
		get
		{
			if (g_AIManager == null)
			{
				g_AIManager = new AIManager();
			}
			return g_AIManager;
		}
	}

	public AIManager()
	{
		Lanes = GameState.Instance.GetLanes(PlayerType.Opponent);
	}

	public void ConsiderCards()
	{
		List<CardItem> hand = GameState.Instance.GetHand(PlayerType.Opponent);
		foreach (CardItem item in hand)
		{
			if (item.Form.Type == CardType.Spell)
			{
				if (item.Form.CanPlay(PlayerType.Opponent, -1))
				{
					AIDecision aIDecision = new AIDecision();
					aIDecision.CardChoice = item;
					aIDecision.LaneChoice = null;
					int weight = item.EvaluateLanePlacement(PlayerType.Opponent, null);
					DecisionList.AddSorted(aIDecision, weight);
				}
				continue;
			}
			foreach (Lane lane in Lanes)
			{
				if (!lane.HasCard(item.Form.Type) && item.Form.CanPlay(PlayerType.Opponent, lane.Index))
				{
					AIDecision aIDecision2 = new AIDecision();
					aIDecision2.CardChoice = item;
					aIDecision2.LaneChoice = lane;
					int weight2 = item.EvaluateLanePlacement(PlayerType.Opponent, lane);
					DecisionList.AddSorted(aIDecision2, weight2);
				}
			}
		}
	}

	public void ConsiderAbilities()
	{
		foreach (Lane lane in Lanes)
		{
			if (lane.HasCreature())
			{
				CreatureScript creature = lane.GetCreature();
				bool flag = GameState.Instance.CanFloopCard(PlayerType.Opponent, creature);
				if (flag)
				{
					AIDecision aIDecision = new AIDecision();
					aIDecision.IsFloop = flag;
					aIDecision.CardChoice = creature.Data;
					aIDecision.LaneChoice = lane;
					int weight = creature.EvaluateAbility();
					DecisionList.AddSorted(aIDecision, weight);
				}
			}
		}
		if (GameState.Instance.IsLeaderAbilityReady(PlayerType.Opponent))
		{
			AIDecision aIDecision2 = new AIDecision();
			aIDecision2.IsLeaderAbility = true;
			int weight2 = GameState.Instance.EvaluateLeaderAbility(PlayerType.Opponent);
			DecisionList.AddSorted(aIDecision2, weight2);
		}
	}

	public AIDecision MakeDecision()
	{
		DecisionList.Clear();
		ConsiderCards();
		ConsiderAbilities();
		Decision = null;
		int num = GameState.Instance.ScoreBoard();
		if (DecisionList.TopWeight() >= num)
		{
			Decision = DecisionList.TopCandidate();
		}
		return Decision;
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
}
