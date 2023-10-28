using System;
using System.Collections.Generic;

public class LowerATKAll : CreatureScript
{
	public override bool CanFloop()
	{
		if (base.GameInstance.HasCreaturesInPlay(!base.Owner))
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
				CardScript.LaneMods[(int)(!base.Owner), item.Index].ATK = -Math.Min(item.GetCreature().ATK, base.Data.Val1);
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
				TargetList.Add(creature);
			}
		}
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		(target as CreatureScript).ATKMod -= base.Data.Val1;
		return true;
	}
}
