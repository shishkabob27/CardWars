using System.Collections.Generic;

public class HealTargetAll : CreatureScript
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
		CWList<Lane> candidates = AITargetSelection(base.Owner, SelectionType.Creature);
		CreatureScript creatureScript = CardScript.BestCandidateForHealing(candidates, -1, true);
		CardScript.LaneMods[(int)base.Owner, creatureScript.CurrentLane.Index].DEF = creatureScript.Damage;
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
		return false;
	}

	private void DoHealing(Lane target)
	{
		CreatureScript creature = target.GetCreature();
		TargetList.Add(creature);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		creatureScript.Heal(creatureScript.Damage);
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
		CWList<Lane> candidates = AITargetSelection(base.Owner, SelectionType.Creature);
		CreatureScript creatureScript = CardScript.BestCandidateForHealing(candidates, -1, true);
		Lane currentLane = creatureScript.CurrentLane;
		DoHealing(currentLane);
	}
}
