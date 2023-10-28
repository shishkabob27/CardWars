using System.Collections.Generic;

public class HealCreatures : SpellScript
{
	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		bool flag = CardScript.CanPlay(player, lane, card);
		if (flag)
		{
			List<Lane> lanes = GameState.Instance.GetLanes(player);
			foreach (Lane item in lanes)
			{
				if (item.HasCreature())
				{
					CreatureScript creature = item.GetCreature();
					if (creature.Damage > 0)
					{
						return true;
					}
				}
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
				CardScript.LaneMods[(int)player, item2.Index].DEF = creature.Damage;
			}
		}
		lanes = GameState.Instance.GetLanes(!player);
		foreach (Lane item3 in lanes)
		{
			if (item3.HasCreature())
			{
				CreatureScript creature2 = item3.GetCreature();
				CardScript.LaneMods[(int)(!player), item3.Index].DEF = creature2.Damage;
			}
		}
		CardScript.APMods[(int)player] = -item.Form.DetermineCost(player);
		return CardScript.ScoreBoard(player);
	}

	public override void Cast()
	{
		List<Lane> lanes = base.GameInstance.GetLanes(base.Owner);
		foreach (Lane item in lanes)
		{
			if (item.HasCreature())
			{
				CreatureScript creature = item.GetCreature();
				if (creature.Damage > 0)
				{
					TargetList.Add(creature);
				}
			}
		}
		lanes = base.GameInstance.GetLanes(!base.Owner);
		foreach (Lane item2 in lanes)
		{
			if (item2.HasCreature())
			{
				CreatureScript creature2 = item2.GetCreature();
				if (creature2.Damage > 0)
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
		creatureScript.Heal(creatureScript.Damage);
		return true;
	}
}
