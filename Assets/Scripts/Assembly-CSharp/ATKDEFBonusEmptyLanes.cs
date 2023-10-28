public class ATKDEFBonusEmptyLanes : BuildingScript
{
	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		int result = GameState.Instance.ScoreBoard() - item.Form.DetermineCost(player) * 1;
		if (lane.HasCreature())
		{
			int num = GameState.Instance.EmptyLaneCount(player);
			CardScript.ResetMods();
			CardScript.LaneMods[(int)player, lane.Index].ATK = item.Val1 * num;
			CardScript.LaneMods[(int)player, lane.Index].DEF = item.Val2 * num;
			CardScript.APMods[(int)player] = -item.Form.DetermineCost(player);
			result = CardScript.ScoreBoard(player);
		}
		return result;
	}

	public override void OnCardEnterPlay(CardScript script)
	{
		int num = base.GameInstance.EmptyLaneCount(base.Owner);
		if (script == this)
		{
			if (base.CurrentLane.HasCreature())
			{
				CreatureScript creature = base.CurrentLane.GetCreature();
				ModifyATK(creature, base.Data.Val1 * num);
				ModifyDEF(creature, base.Data.Val2 * num);
				TriggerEffects();
			}
		}
		else if (script.Data.Form.Type == CardType.Creature)
		{
			if (script.CurrentLane == base.CurrentLane)
			{
				CreatureScript creature2 = base.CurrentLane.GetCreature();
				ModifyATK(creature2, base.Data.Val1 * num);
				ModifyDEF(creature2, base.Data.Val2 * num);
				TriggerEffects();
			}
			else if (base.CurrentLane.HasCreature())
			{
				CreatureScript creature3 = base.CurrentLane.GetCreature();
				ModifyATK(creature3, -base.Data.Val1);
				ModifyDEF(creature3, -base.Data.Val2);
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
				ModifyATK(creature, -base.Data.Val1 * num);
				ModifyDEF(creature, -base.Data.Val2 * num);
			}
		}
		else if (script.Data.Form.Type == CardType.Creature && script.CurrentLane != base.CurrentLane && base.CurrentLane.HasCreature())
		{
			CreatureScript creature2 = base.CurrentLane.GetCreature();
			ModifyATK(creature2, base.Data.Val1);
			ModifyDEF(creature2, base.Data.Val2);
			TriggerEffects();
		}
	}
}
