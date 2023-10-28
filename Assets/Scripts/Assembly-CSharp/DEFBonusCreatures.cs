using System.Collections.Generic;

public class DEFBonusCreatures : CreatureScript
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
				CardScript.LaneMods[(int)base.Owner, item.Index].DEF = base.Data.Val1;
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
		(target as CreatureScript).DEFMod += base.Data.Val1;
		return true;
	}
}
