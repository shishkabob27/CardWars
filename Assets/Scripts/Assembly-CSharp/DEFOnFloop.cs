using System;

public class DEFOnFloop : BuildingScript
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
				if (creature.Enemy != null)
				{
					int num = Math.Max(0, creature.Health - creature.Enemy.ATK);
					int num2 = Math.Max(0, creature.Health + item.Val1 - creature.Enemy.ATK);
					result = result - num + num2;
				}
				else
				{
					int health = creature.Health;
					int num3 = creature.Health + item.Val1;
					result = result - health + num3;
				}
				result -= item.Form.DetermineCost(player) * 1;
			}
		}
		return result;
	}

	public override void OnCreatureFlooped()
	{
		CreatureScript creature = base.CurrentLane.GetCreature();
		ModifyDEF(creature, base.Data.Val1);
		TriggerEffects();
	}
}
