using System.Collections.Generic;

public class ATKDEFFaction : SpellScript
{
	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		bool flag = CardScript.CanPlay(player, lane, card);
		if (flag)
		{
			int num = GameState.Instance.CreatureFactionCount(player, (Faction)card.BaseVal1);
			int num2 = GameState.Instance.CreatureFactionCount(!player, (Faction)card.BaseVal1);
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
				if (creature.Data.Form.Faction == (Faction)item.Form.BaseVal1)
				{
					CardScript.LaneMods[(int)player, creature.CurrentLane.Index].ATK = creature.ATK / 2;
					CardScript.LaneMods[(int)player, creature.CurrentLane.Index].DEF = creature.DEF / 2;
				}
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
				if (creature.Data.Form.Faction == (Faction)base.Data.Form.BaseVal1)
				{
					TargetList.Add(creature);
				}
			}
			if (item.OpponentLane.HasCreature())
			{
				CreatureScript creature2 = item.OpponentLane.GetCreature();
				if (creature2.Data.Form.Faction == (Faction)base.Data.Form.BaseVal1)
				{
					TargetList.Add(creature2);
				}
			}
		}
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		creatureScript.ATKFactor *= 1.5f;
		creatureScript.DEFFactor *= 1.5f;
		return true;
	}
}
