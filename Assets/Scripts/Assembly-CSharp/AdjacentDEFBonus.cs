public class AdjacentDEFBonus : CreatureScript
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
		Lane[] adjacentLanes = base.CurrentLane.AdjacentLanes;
		foreach (Lane lane in adjacentLanes)
		{
			if (lane != null && lane.HasCreature())
			{
				CardScript.LaneMods[(int)base.Owner, lane.Index].DEF = base.Data.Val1;
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
		(target as CreatureScript).DEFMod += base.Data.Val1;
		return true;
	}
}
