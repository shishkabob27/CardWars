using System;
using System.Collections.Generic;

public class HealAll : CreatureScript
{
	public override bool CanFloop()
	{
		List<Lane> lanes = GameState.Instance.GetLanes(base.Owner);
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

	public override void PopulateLaneMods()
	{
		List<Lane> lanes = GameState.Instance.GetLanes(base.Owner);
		foreach (Lane item in lanes)
		{
			if (item.HasCreature())
			{
				CreatureScript creature = item.GetCreature();
				CardScript.LaneMods[(int)base.Owner, item.Index].DEF = Math.Min(creature.Damage, base.Data.Val1);
			}
		}
	}

	public override void Floop()
	{
		List<Lane> lanes = base.GameInstance.GetLanes(base.Owner);
		foreach (Lane item in lanes)
		{
			if (item.HasCreature())
			{
				CreatureScript creature = item.GetCreature();
				TargetList.Add(creature);
			}
		}
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		creatureScript.Heal(base.Data.Val1);
		return true;
	}
}
