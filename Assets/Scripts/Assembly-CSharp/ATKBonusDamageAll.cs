using System;
using System.Collections.Generic;

public class ATKBonusDamageAll : CreatureScript
{
	public override bool CanFloop()
	{
		if (base.Health > base.Data.Val1)
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
				CardScript.LaneMods[(int)base.Owner, creature.CurrentLane.Index].DEF = -Math.Min(creature.Health, base.Data.Val1);
			}
		}
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].ATK = base.Data.Val2;
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
		creatureScript.TakeDamage(this, base.Data.Val1);
		if (creatureScript == this)
		{
			creatureScript.ATKMod += base.Data.Val2;
		}
		return true;
	}
}
