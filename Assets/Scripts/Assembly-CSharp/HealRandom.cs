using System;
using System.Collections.Generic;

public class HealRandom : CreatureScript
{
	public override bool CanFloop()
	{
		List<Lane> lanes = base.GameInstance.GetLanes(base.Owner);
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

	public override int EvaluateAbility()
	{
		List<Lane> lanes = base.GameInstance.GetLanes(base.Owner);
		int num = 0;
		int num2 = 0;
		foreach (Lane item in lanes)
		{
			if (item.HasCreature())
			{
				CreatureScript creature = item.GetCreature();
				CardScript.ResetMods();
				CardScript.LaneMods[(int)base.Owner, creature.CurrentLane.Index].DEF = Math.Min(creature.Damage, base.Data.Val1);
				CardScript.APMods[(int)base.Owner] = -DetermineFloopCost();
				num += CardScript.ScoreBoard(base.Owner);
				num2++;
			}
		}
		return num / num2;
	}

	public override void Floop()
	{
		List<Lane> lanes = base.GameInstance.GetLanes(base.Owner);
		CWList<CreatureScript> cWList = new CWList<CreatureScript>();
		foreach (Lane item in lanes)
		{
			if (item.HasCreature())
			{
				CreatureScript creature = item.GetCreature();
				if (creature.Damage > 0)
				{
					cWList.Add(creature);
				}
			}
		}
		TargetList.Add(cWList.RandomItem());
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		creatureScript.Heal(base.Data.Val1);
		return true;
	}
}
