using System;
using System.Collections.Generic;

public class DamageAllHealAll : CreatureScript
{
	public override bool CanFloop()
	{
		int num = base.GameInstance.CreatureCount(base.Owner);
		int num2 = base.GameInstance.CreatureCount(!base.Owner);
		if (num > 0 || num2 > 0)
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
				CardScript.LaneMods[(int)base.Owner, item.Index].DEF = Math.Min(creature.Health, base.Data.Val2);
			}
		}
		lanes = base.GameInstance.GetLanes(!base.Owner);
		foreach (Lane item2 in lanes)
		{
			if (item2.HasCreature())
			{
				CreatureScript creature2 = item2.GetCreature();
				CardScript.LaneMods[(int)(!base.Owner), item2.Index].DEF = -Math.Min(creature2.Health, base.Data.Val1);
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
		lanes = base.GameInstance.GetLanes(!base.Owner);
		foreach (Lane item2 in lanes)
		{
			if (item2.HasCreature())
			{
				CreatureScript creature2 = item2.GetCreature();
				TargetList.Add(creature2);
			}
		}
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		if (creatureScript.Owner == base.Owner)
		{
			creatureScript.Heal(base.Data.Val2);
		}
		else
		{
			creatureScript.TakeDamage(this, base.Data.Val1);
		}
		return true;
	}
}
