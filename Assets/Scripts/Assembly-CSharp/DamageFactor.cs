public class DamageFactor : BuildingScript
{
	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		int result = GameState.Instance.ScoreBoard() - item.Form.DetermineCost(player) * 1;
		if (lane.HasCreature())
		{
			CreatureScript creature = lane.GetCreature();
			if (creature.Enemy != null)
			{
				result = GameState.Instance.ScoreBoard();
				int num = creature.Health - creature.Enemy.ATK;
				int num2 = creature.Health - creature.Enemy.ATK / 2;
				result = result - num + num2;
				result -= item.Form.DetermineCost(player) * 1;
			}
		}
		return result;
	}

	public override void OnCardEnterPlay(CardScript script)
	{
		if (script == this)
		{
			if (base.CurrentLane.HasCreature())
			{
				base.CurrentLane.GetCreature().DamageFactor *= 0.5f;
				TriggerEffects();
			}
		}
		else if (script.Data.Form.Type == CardType.Creature && script.CurrentLane == base.CurrentLane)
		{
			(script as CreatureScript).DamageFactor *= 0.5f;
			TriggerEffects();
		}
	}

	public override void OnCardLeftPlay(CardScript script)
	{
		if (script == this && base.CurrentLane.HasCreature())
		{
			base.CurrentLane.GetCreature().DamageFactor /= 0.5f;
		}
	}
}
