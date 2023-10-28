using System.Collections.Generic;

public class ReduceATK : SpellScript
{
	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		bool flag = CardScript.CanPlay(player, lane, card);
		if (flag)
		{
			int num = GameState.Instance.CreatureCount(player);
			int num2 = GameState.Instance.CreatureCount(!player);
			if (num2 > 0 || num > 0)
			{
				return true;
			}
			return false;
		}
		return flag;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		List<Lane> lanes = GameState.Instance.GetLanes(player);
		CardScript.ResetMods();
		foreach (Lane item2 in lanes)
		{
			if (item2.HasCreature())
			{
				CreatureScript creature = item2.GetCreature();
				CardScript.LaneMods[(int)player, creature.CurrentLane.Index].ATK = creature.ATK / 2;
			}
			if (item2.OpponentLane.HasCreature())
			{
				CreatureScript creature2 = item2.OpponentLane.GetCreature();
				CardScript.LaneMods[(int)(!player), creature2.CurrentLane.Index].ATK = creature2.ATK / 2;
			}
		}
		CardScript.APMods[(int)player] = -item.Form.DetermineCost(player);
		return CardScript.ScoreBoard(player);
	}

	public override void Cast()
	{
		List<Lane> lanes = GameState.Instance.GetLanes(base.Owner);
		foreach (Lane item in lanes)
		{
			if (item.HasCreature())
			{
				CreatureScript creature = item.GetCreature();
				TargetList.Add(creature);
			}
			if (item.OpponentLane.HasCreature())
			{
				CreatureScript creature2 = item.OpponentLane.GetCreature();
				TargetList.Add(creature2);
			}
		}
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		(target as CreatureScript).ATKFactor *= 0.5f;
		return true;
	}
}
