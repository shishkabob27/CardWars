using System;

public class DamageHeroOnDeath : BuildingScript
{
	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		int result = GameState.Instance.ScoreBoard() - item.Form.DetermineCost(player) * 1;
		if (lane.HasCreature())
		{
			CreatureScript creature = lane.GetCreature();
			if (creature.InDanger)
			{
				CardScript.ResetMods();
				int num = -Math.Min(GameState.Instance.GetHealth(!player), item.Val1);
				CardScript.HealthMods[(int)(!player)] = num;
				CardScript.APMods[(int)player] = -item.Form.DetermineCost(player);
				result = CardScript.ScoreBoard(player);
			}
		}
		return result;
	}

	public override void OnCreatureDied(CardItem creature)
	{
		base.GameInstance.DealDamage(!base.Owner, base.Data.Val1);
		TriggerEffects();
	}
}
