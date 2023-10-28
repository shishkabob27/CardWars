using System.Collections.Generic;

public class DEFBonusBuildings : CreatureScript
{
	public override bool CanFloop()
	{
		int num = base.GameInstance.CreatureCount(base.Owner);
		int num2 = base.GameInstance.BuildingCount(base.Owner);
		if (num > 0 && num2 > 0)
		{
			return true;
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		List<Lane> lanes = base.GameInstance.GetLanes(base.Owner);
		int num = base.GameInstance.BuildingCount(base.Owner);
		foreach (Lane item in lanes)
		{
			if (item.HasCreature())
			{
				CardScript.LaneMods[(int)base.Owner, item.Index].DEF = base.Data.Val1 * num;
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
		int num = base.GameInstance.BuildingCount(base.Owner);
		creatureScript.DEFMod += base.Data.Val1 * num;
		return true;
	}
}
