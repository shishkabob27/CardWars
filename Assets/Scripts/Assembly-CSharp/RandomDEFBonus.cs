using System.Collections.Generic;
using UnityEngine;

public class RandomDEFBonus : CreatureScript
{
	public override bool CanFloop()
	{
		if (base.GameInstance.HasCreaturesInPlay(base.Owner))
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
				CardScript.LaneMods[(int)base.Owner, item.Index].DEF = (base.Data.Val1 + base.Data.Val2) / 2;
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
		(target as CreatureScript).DEFMod += Random.Range(base.Data.Val1, base.Data.Val2 + 1);
		return true;
	}
}
