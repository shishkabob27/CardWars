public class CreatureDamageOnEntry : BuildingScript
{
	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		int result = GameState.Instance.ScoreBoard() - item.Form.DetermineCost(player) * 1;
		if (lane.OpponentLane.HasCreature())
		{
			if (lane.HasCreature())
			{
				CreatureScript creature = lane.GetCreature();
				if (creature.InDanger && !creature.Enemy.InDanger)
				{
					CardScript.ResetMods();
					CardScript.LaneMods[(int)player, lane.Index].ATK = item.Val1;
					CardScript.APMods[(int)player] = -item.Form.DetermineCost(player);
					result = CardScript.ScoreBoard(player);
				}
			}
			else
			{
				CardScript.ResetMods();
				CardScript.LaneMods[(int)player, lane.Index].EmptyLaneScore = item.Val1;
				CardScript.APMods[(int)player] = -item.Form.DetermineCost(player);
				result = CardScript.ScoreBoard(player);
			}
		}
		return result;
	}

	public override void OnCardEnterPlay(CardScript script)
	{
		if (script.Data.Form.Type == CardType.Creature && script.CurrentLane == base.CurrentLane && base.CurrentLane.OpponentLane.HasCreature())
		{
			CreatureScript creature = base.CurrentLane.OpponentLane.GetCreature();
			creature.TakeDamage(this, base.Data.Val1);
			TriggerEffects();
		}
	}
}
