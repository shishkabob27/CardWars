using System;

public class DamageReduction : BuildingScript
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
				int num = Math.Max(0, creature.Health - creature.Enemy.ATK);
				int num2 = Math.Max(0, creature.Health - (creature.Enemy.ATK - item.Val1));
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
				base.CurrentLane.GetCreature().DamageReduction += base.Data.Val1;
				TriggerEffects();
			}
		}
		else if (script.Data.Form.Type == CardType.Creature && script.CurrentLane == base.CurrentLane)
		{
			(script as CreatureScript).DamageReduction += base.Data.Val1;
			TriggerEffects();
		}
	}

	public override void OnCardLeftPlay(CardScript script)
	{
		if (script == this && base.CurrentLane.HasCreature())
		{
			base.CurrentLane.GetCreature().DamageReduction -= base.Data.Val1;
		}
	}
}
