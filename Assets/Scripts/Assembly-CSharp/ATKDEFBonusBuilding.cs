public class ATKDEFBonusBuilding : BuildingScript
{
	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		int result = GameState.Instance.ScoreBoard() - item.Form.DetermineCost(player) * 1;
		if (lane.HasCreature())
		{
			CardScript.ResetMods();
			CardScript.LaneMods[(int)player, lane.Index].ATK = item.Val1;
			CardScript.LaneMods[(int)player, lane.Index].DEF = item.Val2;
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
				ModifyATK(creature, base.Data.Val1);
				ModifyDEF(creature, base.Data.Val2);
				TriggerEffects();
			}
		}
		else if (script.Data.Form.Type == CardType.Creature && script.CurrentLane == base.CurrentLane)
		{
			CreatureScript script2 = script as CreatureScript;
			ModifyATK(script2, base.Data.Val1);
			ModifyDEF(script2, base.Data.Val2);
			TriggerEffects();
		}
	}

	public override void OnCardLeftPlay(CardScript script)
	{
		if (script == this && base.CurrentLane.HasCreature())
		{
			CreatureScript creature = base.CurrentLane.GetCreature();
			ModifyATK(creature, -base.Data.Val1);
			ModifyDEF(creature, -base.Data.Val2);
		}
	}
}
