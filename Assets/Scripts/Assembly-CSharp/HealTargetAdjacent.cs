using System;
using System.Collections.Generic;

public class HealTargetAdjacent : CreatureScript
{
	public override bool CanFloop()
	{
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
		CWList<Lane> cWList = AITargetSelection(base.Owner, SelectionType.Creature);
		CreatureScript creatureScript = CardScript.BestCandidateForHealing(cWList, base.Data.Val1, true);
		cWList.Remove(creatureScript.CurrentLane);
		CreatureScript secondTarget = CardScript.BestCandidateForHealing(cWList, base.Data.Val1, true);
		creatureScript = MaximizeAdjacentTargets(creatureScript, secondTarget);
		CardScript.LaneMods[(int)base.Owner, creatureScript.CurrentLane.Index].DEF = Math.Min(creatureScript.Damage, base.Data.Val1);
		Lane[] adjacentLanes = creatureScript.CurrentLane.AdjacentLanes;
		foreach (Lane lane in adjacentLanes)
		{
			if (lane != null && lane.HasCreature())
			{
				CreatureScript creature = lane.GetCreature();
				CardScript.LaneMods[(int)base.Owner, lane.Index].DEF = Math.Min(creature.Damage, base.Data.Val1);
			}
		}
	}

	public override bool SelectionFilter(Lane candidate)
	{
		if (candidate.HasCreature())
		{
			CreatureScript creature = candidate.GetCreature();
			if (creature.Damage > 0)
			{
				return true;
			}
		}
		Lane[] adjacentLanes = candidate.AdjacentLanes;
		foreach (Lane lane in adjacentLanes)
		{
			if (lane != null && lane.HasCreature())
			{
				CreatureScript creature2 = lane.GetCreature();
				if (creature2.Damage > 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void DoHealing(Lane target)
	{
		CreatureScript creature = target.GetCreature();
		TargetList.Add(creature);
		Lane[] adjacentLanes = target.AdjacentLanes;
		foreach (Lane lane in adjacentLanes)
		{
			if (lane != null && lane.HasCreature())
			{
				creature = lane.GetCreature();
				if (creature.Damage > 0)
				{
					TargetList.Add(creature);
				}
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

	public override void OnTargetSelected(Lane target)
	{
		DoHealing(target);
		EndTargetSelection();
	}

	public override void Floop()
	{
		if (base.Owner == PlayerType.User)
		{
			StartTargetSelection(base.Owner, SelectionType.Creature, KFFLocalization.Get("!!TAP_CREATURE_TO_HEAL"));
			return;
		}
		CWList<Lane> cWList = AITargetSelection(base.Owner, SelectionType.Creature);
		CreatureScript creatureScript = CardScript.BestCandidateForHealing(cWList, base.Data.Val1, true);
		cWList.Remove(creatureScript.CurrentLane);
		CreatureScript secondTarget = CardScript.BestCandidateForHealing(cWList, base.Data.Val1, true);
		creatureScript = MaximizeAdjacentTargets(creatureScript, secondTarget);
		Lane currentLane = creatureScript.CurrentLane;
		DoHealing(currentLane);
	}
}
