public class RarityGate : BuildingScript
{
	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		int num = GameState.Instance.ScoreBoard() - item.Form.DetermineCost(player) * 1;
		Lane lane2 = GameState.Instance.GetLane(player, lane.Index);
		int num2 = GameState.Instance.ScoreLane(lane2) - GameState.Instance.ScoreLane(lane2.OpponentLane);
		return num + num2;
	}

	public override void OnCardEnterPlay(CardScript script)
	{
		if (script == this)
		{
			base.CurrentLane.OpponentLane.RarityGate = base.Data.Form.BaseVal1;
			TriggerEffects();
		}
	}

	public override void OnCardLeftPlay(CardScript script)
	{
		if (script == this)
		{
			base.CurrentLane.OpponentLane.RarityGate = int.MaxValue;
		}
	}
}
