using System;
using System.Collections.Generic;

public class DamageOneHealAll : CreatureScript
{
	public override bool CanFloop()
	{
		int num = base.GameInstance.CreatureCount(base.Owner);
		if (num > 0)
		{
			return true;
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		List<Lane> lanes = base.GameInstance.GetLanes(base.Owner);
		foreach (Lane item in lanes)
		{
			if (item.HasCreature())
			{
				CreatureScript creature = item.GetCreature();
				if (creature == this)
				{
					CardScript.LaneMods[(int)base.Owner, item.Index].DEF = -Math.Min(creature.Health, base.Data.Val2);
				}
				else
				{
					CardScript.LaneMods[(int)base.Owner, item.Index].DEF = Math.Min(creature.Health, base.Data.Val1);
				}
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
		if (creatureScript != this)
		{
			creatureScript.Heal(base.Data.Val1);
		}
		else
		{
			creatureScript.TakeDamage(this, base.Data.Val2);
		}
		return true;
	}
}
