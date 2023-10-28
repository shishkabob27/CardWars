using System.Collections.Generic;

public class ATKBonusCreatureFaction : CreatureScript
{
	public override bool CanFloop()
	{
		int num = base.GameInstance.CreatureFactionCount(base.Owner, (Faction)base.Data.Form.BaseVal2);
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
				if (creature.Data.Form.Faction == (Faction)base.Data.Form.BaseVal2)
				{
					CardScript.LaneMods[(int)base.Owner, item.Index].ATK = base.Data.Val1;
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
				if (creature.Data.Form.Faction == (Faction)base.Data.Form.BaseVal2)
				{
					TargetList.Add(creature);
				}
			}
		}
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		(target as CreatureScript).ATKMod += base.Data.Val1;
		return true;
	}
}
