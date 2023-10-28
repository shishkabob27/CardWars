using System;

public class DEFPenaltyATKBonusAdjacent : CreatureScript
{
	public override bool CanFloop()
	{
		Lane[] adjacentLanes = base.CurrentLane.AdjacentLanes;
		foreach (Lane lane in adjacentLanes)
		{
			if (lane != null && lane.HasCreature())
			{
				CreatureScript creature = lane.GetCreature();
				if (creature.Health > base.Data.Val1)
				{
					return true;
				}
			}
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		Lane[] adjacentLanes = base.CurrentLane.AdjacentLanes;
		foreach (Lane lane in adjacentLanes)
		{
			if (lane != null && lane.HasCreature())
			{
				CreatureScript creature = lane.GetCreature();
				CardScript.LaneMods[(int)base.Owner, lane.Index].DEF = -Math.Min(creature.Health, base.Data.Val1);
				CardScript.LaneMods[(int)base.Owner, lane.Index].ATK = base.Data.Val2;
			}
		}
	}

	public override void Floop()
	{
		Lane[] adjacentLanes = base.CurrentLane.AdjacentLanes;
		foreach (Lane lane in adjacentLanes)
		{
			if (lane != null && lane.HasCreature())
			{
				CreatureScript creature = lane.GetCreature();
				TargetList.Add(creature);
			}
		}
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		creatureScript.DEFMod -= base.Data.Val1;
		creatureScript.ATKMod += base.Data.Val2;
		return true;
	}
}
