public class DEFBonusCreaturesBuilding : BuildingScript
{
	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		int result = GameState.Instance.ScoreBoard() - item.Form.DetermineCost(player) * 1;
		if (lane.HasCreature())
		{
			int num = GameState.Instance.CreatureCount(player);
			CardScript.ResetMods();
			CardScript.LaneMods[(int)player, lane.Index].DEF = item.Val1 * num;
			CardScript.APMods[(int)player] = -item.Form.DetermineCost(player);
			result = CardScript.ScoreBoard(player);
		}
		return result;
	}

	public override void OnCardEnterPlay(CardScript script)
	{
		int num = base.GameInstance.CreatureCount(base.Owner);
		if (script == this)
		{
			if (base.CurrentLane.HasCreature())
			{
				CreatureScript creature = base.CurrentLane.GetCreature();
				ModifyDEF(creature, base.Data.Val1 * num);
				TriggerEffects();
			}
		}
		else if (script.Data.Form.Type == CardType.Creature)
		{
			if (script.CurrentLane == base.CurrentLane)
			{
				CreatureScript script2 = script as CreatureScript;
				ModifyDEF(script2, base.Data.Val1 * num);
				TriggerEffects();
			}
			else if (base.CurrentLane.HasCreature())
			{
				CreatureScript creature2 = base.CurrentLane.GetCreature();
				ModifyDEF(creature2, base.Data.Val1);
				TriggerEffects();
			}
		}
	}

	public override void OnCardLeftPlay(CardScript script)
	{
		int num = base.GameInstance.CreatureCount(base.Owner);
		if (script == this)
		{
			if (base.CurrentLane.HasCreature())
			{
				CreatureScript creature = base.CurrentLane.GetCreature();
				ModifyDEF(creature, -base.Data.Val1 * num);
			}
		}
		else if (script.Data.Form.Type == CardType.Creature && script.CurrentLane != base.CurrentLane && base.CurrentLane.HasCreature())
		{
			CreatureScript creature2 = base.CurrentLane.GetCreature();
			ModifyDEF(creature2, -base.Data.Val1);
			TriggerEffects();
		}
	}
}
