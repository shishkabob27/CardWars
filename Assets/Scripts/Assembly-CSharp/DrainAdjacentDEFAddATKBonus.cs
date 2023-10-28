using System;

public class DrainAdjacentDEFAddATKBonus : CreatureScript
{
	public override bool CanFloop()
	{
		Lane[] adjacentLanes = base.CurrentLane.AdjacentLanes;
		foreach (Lane lane in adjacentLanes)
		{
			if (lane != null && lane.HasCreature())
			{
				return true;
			}
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].ATK = base.Data.Val2;
		Lane[] adjacentLanes = base.CurrentLane.AdjacentLanes;
		foreach (Lane lane in adjacentLanes)
		{
			if (lane != null && lane.HasCreature())
			{
				CardScript.LaneMods[(int)base.Owner, lane.Index].DEF = -Math.Min(CardScript.LaneMods[(int)base.Owner, lane.Index].DEF, base.Data.Val1);
			}
		}
	}

	public override void Floop()
	{
		TargetList.Add(this);
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
		if (creatureScript == this)
		{
			creatureScript.ATKMod += base.Data.Val2;
		}
		else
		{
			creatureScript.DEFMod -= base.Data.Val1;
		}
		return true;
	}
}
