using System;

public class HealAdjacent : CreatureScript
{
	public override bool CanFloop()
	{
		Lane[] adjacentLanes = base.CurrentLane.AdjacentLanes;
		foreach (Lane lane in adjacentLanes)
		{
			if (lane != null && lane.HasCreature())
			{
				CreatureScript creature = lane.GetCreature();
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
		Lane[] adjacentLanes = base.CurrentLane.AdjacentLanes;
		foreach (Lane lane in adjacentLanes)
		{
			if (lane != null && lane.HasCreature())
			{
				CreatureScript creature = lane.GetCreature();
				CardScript.LaneMods[(int)base.Owner, lane.Index].DEF = Math.Min(creature.Damage, base.Data.Val1);
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
		creatureScript.Heal(base.Data.Val1);
		return true;
	}
}
