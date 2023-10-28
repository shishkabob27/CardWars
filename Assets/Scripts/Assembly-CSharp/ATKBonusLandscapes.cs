public class ATKBonusLandscapes : BuildingScript
{
	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		int result = GameState.Instance.ScoreBoard() - item.Form.DetermineCost(player) * 1;
		if (lane.HasCreature())
		{
			int num = GameState.Instance.LandscapeTypeCount(player);
			CardScript.ResetMods();
			CardScript.LaneMods[(int)player, lane.Index].ATK = item.Val1 * num;
			CardScript.APMods[(int)player] = -item.Form.DetermineCost(player);
			result = CardScript.ScoreBoard(player);
		}
		return result;
	}

	public override void OnCardEnterPlay(CardScript script)
	{
		int num = GameState.Instance.LandscapeTypeCount(base.Owner);
		if (script == this)
		{
			if (base.CurrentLane.HasCreature())
			{
				CreatureScript creature = base.CurrentLane.GetCreature();
				ModifyATK(creature, base.Data.Val1 * num);
				TriggerEffects();
			}
		}
		else if (script.Data.Form.Type == CardType.Creature && script.CurrentLane == base.CurrentLane)
		{
			CreatureScript script2 = script as CreatureScript;
			ModifyATK(script2, base.Data.Val1 * num);
			TriggerEffects();
		}
	}

	public override void OnCardLeftPlay(CardScript script)
	{
		int num = GameState.Instance.LandscapeTypeCount(base.Owner);
		if (script == this && base.CurrentLane.HasCreature())
		{
			CreatureScript creature = base.CurrentLane.GetCreature();
			ModifyATK(creature, -base.Data.Val1 * num);
		}
	}
}
