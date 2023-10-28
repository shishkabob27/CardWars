using System;

public class HeroDamageOnEntry : BuildingScript
{
	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		int result = GameState.Instance.ScoreBoard() - item.Form.DetermineCost(player) * 1;
		if (!lane.HasCreature() || lane.GetCreature().InDanger)
		{
			CardScript.ResetMods();
			CardScript.HealthMods[(int)(!player)] = -Math.Min(GameState.Instance.GetHealth(!player), item.Val1);
			CardScript.APMods[(int)player] = -item.Form.DetermineCost(player);
			result = CardScript.ScoreBoard(player);
		}
		return result;
	}

	public override void OnCardEnterPlay(CardScript script)
	{
		if (script.Data.Form.Type == CardType.Creature && script.CurrentLane == base.CurrentLane)
		{
			base.GameInstance.DealDamage(!base.Owner, base.Data.Val1);
			TriggerEffects();
		}
	}
}
