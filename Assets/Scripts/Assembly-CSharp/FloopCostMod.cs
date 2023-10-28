public class FloopCostMod : BuildingScript
{
	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		int result = GameState.Instance.ScoreBoard() - item.Form.DetermineCost(player) * 1;
		if (lane.HasCreature())
		{
			CreatureScript creature = lane.GetCreature();
			if (creature.CanFloop())
			{
				result = creature.EvaluateAbility();
			}
		}
		return result;
	}

	public override void OnCardEnterPlay(CardScript script)
	{
		if (script == this)
		{
			base.CurrentLane.FloopMod -= base.Data.Form.BaseVal1;
		}
	}

	public override void OnCreatureFlooped()
	{
		TriggerEffects();
	}

	public override void OnCardLeftPlay(CardScript script)
	{
		if (script == this)
		{
			base.CurrentLane.FloopMod += base.Data.Form.BaseVal1;
		}
	}
}
