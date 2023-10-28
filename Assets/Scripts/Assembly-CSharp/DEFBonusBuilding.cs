public class DEFBonusBuilding : BuildingScript
{
	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		int result = GameState.Instance.ScoreBoard() - item.Form.DetermineCost(player) * 1;
		if (lane.HasCreature())
		{
			CardScript.ResetMods();
			CardScript.LaneMods[(int)player, lane.Index].DEF = item.Val1;
			CardScript.APMods[(int)player] = -item.Form.DetermineCost(player);
			result = CardScript.ScoreBoard(player);
		}
		return result;
	}

	public override void OnCardEnterPlay(CardScript script)
	{
		if (script == this)
		{
			if (base.CurrentLane.HasCreature())
			{
				CreatureScript creature = base.CurrentLane.GetCreature();
				ModifyDEF(creature, base.Data.Val1);
				TriggerEffects();
			}
		}
		else if (script.Data.Form.Type == CardType.Creature && script.CurrentLane == base.CurrentLane)
		{
			CreatureScript script2 = script as CreatureScript;
			ModifyDEF(script2, base.Data.Val1);
			TriggerEffects();
		}
	}

	public override void OnCardLeftPlay(CardScript script)
	{
		if (script == this && base.CurrentLane.HasCreature())
		{
			CreatureScript creature = base.CurrentLane.GetCreature();
			ModifyDEF(creature, -base.Data.Val1);
		}
	}
}
