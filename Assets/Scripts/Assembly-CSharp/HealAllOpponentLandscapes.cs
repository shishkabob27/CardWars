using System;
using System.Collections.Generic;

public class HealAllOpponentLandscapes : CreatureScript
{
	public override bool CanFloop()
	{
		int num = base.GameInstance.LandscapeCount(!base.Owner, (LandscapeType)base.Data.Form.BaseVal2);
		if (num <= 0)
		{
			return false;
		}
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
		int num = base.GameInstance.LandscapeCount(!base.Owner, (LandscapeType)base.Data.Form.BaseVal2);
		List<Lane> lanes = GameState.Instance.GetLanes(base.Owner);
		foreach (Lane item in lanes)
		{
			if (item.HasCreature())
			{
				CreatureScript creature = item.GetCreature();
				CardScript.LaneMods[(int)base.Owner, creature.CurrentLane.Index].DEF = Math.Min(creature.Damage, base.Data.Val1 * num);
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
		int num = base.GameInstance.LandscapeCount(!base.Owner, (LandscapeType)base.Data.Form.BaseVal2);
		CreatureScript creatureScript = target as CreatureScript;
		creatureScript.Heal(base.Data.Val1 * num);
		return true;
	}
}
