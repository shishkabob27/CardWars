using System;
using System.Collections.Generic;

public class DEFPenaltyFaction : CreatureScript
{
	public override bool CanFloop()
	{
		int num = base.GameInstance.CreatureFactionCount(!base.Owner, (Faction)base.Data.Form.BaseVal1);
		if (num > 0)
		{
			return true;
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		List<Lane> lanes = base.GameInstance.GetLanes(!base.Owner);
		foreach (Lane item in lanes)
		{
			if (item.HasCreature())
			{
				CreatureScript creature = item.GetCreature();
				if (creature.Data.Form.Faction == (Faction)base.Data.Form.BaseVal1)
				{
					CardScript.LaneMods[(int)(!base.Owner), creature.CurrentLane.Index].DEF = -Math.Min(creature.Health, base.Data.Val2);
				}
			}
		}
	}

	public override void Floop()
	{
		List<Lane> lanes = base.GameInstance.GetLanes(!base.Owner);
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
		}
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		(target as CreatureScript).DEFMod -= base.Data.Val2;
		return true;
	}
}
