public class ATKBonusAdjacentEmpty : CreatureScript
{
	public override bool CanFloop()
	{
		Lane[] adjacentLanes = base.CurrentLane.AdjacentLanes;
		foreach (Lane lane in adjacentLanes)
		{
			if (lane != null && !lane.HasCreature())
			{
				return true;
			}
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		int num = 0;
		Lane[] adjacentLanes = base.CurrentLane.AdjacentLanes;
		foreach (Lane lane in adjacentLanes)
		{
			if (lane != null && !lane.HasCreature())
			{
				num++;
			}
		}
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].ATK = base.Data.Val1 * num;
	}

	public override void Floop()
	{
		TargetList.Add(this);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		int num = 0;
		Lane[] adjacentLanes = base.CurrentLane.AdjacentLanes;
		foreach (Lane lane in adjacentLanes)
		{
			if (lane != null && !lane.HasCreature())
			{
				num++;
			}
		}
		creatureScript.ATKMod += num * base.Data.Val1;
		return true;
	}
}
