public class HealOnVictory : BuildingScript
{
	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		int result = GameState.Instance.ScoreBoard() - item.Form.DetermineCost(player) * 1;
		if (lane.HasCreature())
		{
			CreatureScript creature = lane.GetCreature();
			if (!creature.InDanger)
			{
				CardScript.ResetMods();
				CardScript.LaneMods[(int)player, lane.Index].DEF = item.Val1 * creature.ATK;
				CardScript.APMods[(int)player] = -item.Form.DetermineCost(player);
				result = CardScript.ScoreBoard(player);
			}
		}
		return result;
	}

	public override void OnCreatureWon()
	{
		if (base.CurrentLane.HasCreature())
		{
			CreatureScript creature = base.CurrentLane.GetCreature();
			creature.Heal(base.Data.Val1);
			TriggerEffects();
		}
	}
}
