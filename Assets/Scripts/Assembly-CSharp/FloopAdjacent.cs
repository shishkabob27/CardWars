public class FloopAdjacent : CreatureScript
{
	public override bool CanFloop()
	{
		Lane[] adjacentLanes = base.CurrentLane.AdjacentLanes;
		foreach (Lane lane in adjacentLanes)
		{
			if (lane != null && lane.HasCreature())
			{
				CreatureScript creature = lane.GetCreature();
				if (!(creature is FloopAdjacent) && creature.CanFloop())
				{
					return true;
				}
			}
		}
		return false;
	}

	public override int EvaluateAbility()
	{
		int num = 0;
		Lane[] adjacentLanes = base.CurrentLane.AdjacentLanes;
		foreach (Lane lane in adjacentLanes)
		{
			if (lane == null || !lane.HasCreature())
			{
				continue;
			}
			CreatureScript creature = lane.GetCreature();
			if (!(creature is FloopAdjacent) && creature.CanFloop())
			{
				int num2 = creature.EvaluateAbility();
				if (num2 > num)
				{
					num = num2;
				}
			}
		}
		return num;
	}

	public override bool SelectionFilter(Lane candidate)
	{
		if ((base.CurrentLane.AdjacentLanes[0] == candidate || base.CurrentLane.AdjacentLanes[1] == candidate) && candidate.HasCreature())
		{
			CreatureScript creature = candidate.GetCreature();
			if (!(creature is FloopAdjacent) && creature.CanFloop())
			{
				return true;
			}
		}
		return false;
	}

	private void DoDamage(Lane target)
	{
		CreatureScript creature = target.GetCreature();
		TargetList.Add(creature);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		if (creatureScript.CanFloop())
		{
			creatureScript.Floop();
			return false;
		}
		return true;
	}

	public override void OnTargetSelected(Lane target)
	{
		DoDamage(target);
		EndTargetSelection();
	}

	public override void Floop()
	{
		if (base.Owner == PlayerType.User)
		{
			StartTargetSelection(base.Owner, SelectionType.Creature, KFFLocalization.Get("!!TAP_CREATURE_TO_FLOOP"));
			return;
		}
		CreatureScript creatureScript = null;
		int num = 0;
		Lane[] adjacentLanes = base.CurrentLane.AdjacentLanes;
		foreach (Lane lane in adjacentLanes)
		{
			if (lane == null || !lane.HasCreature())
			{
				continue;
			}
			CreatureScript creature = lane.GetCreature();
			if (!(creature is FloopAdjacent) && creature.CanFloop())
			{
				int num2 = creature.EvaluateAbility();
				if (creatureScript == null || num2 > num)
				{
					creatureScript = creature;
					num = num2;
				}
			}
		}
		DoDamage(creatureScript.CurrentLane);
	}
}
