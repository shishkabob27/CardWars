using System.Collections.Generic;

public class ResetTarget : CreatureScript
{
	public override bool CanFloop()
	{
		List<Lane> lanes = GameState.Instance.GetLanes(base.Owner);
		foreach (Lane item in lanes)
		{
			if (item.HasCreature())
			{
				CreatureScript creature = item.GetCreature();
				if (creature.Damage > 0 || creature.ATKMod != 0 || creature.DEFMod != 0)
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
		CreatureScript creatureScript = BestCandidateForReset(candidates);
		CardScript.ResetMods();
		CardScript.LaneMods[(int)base.Owner, creatureScript.CurrentLane.Index].ATK = -creatureScript.ATKMod;
		CardScript.LaneMods[(int)base.Owner, creatureScript.CurrentLane.Index].DEF = creatureScript.Damage - creatureScript.DEFMod;
	}

	public override bool SelectionFilter(Lane candidate)
	{
		if (candidate.HasCreature())
		{
			CreatureScript creature = candidate.GetCreature();
			if (creature.Damage > 0 || creature.ATKMod != 0 || creature.DEFMod != 0)
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
		creatureScript.Heal(creatureScript.Damage);
		creatureScript.ATKMod = 0;
		creatureScript.DEFMod = 0;
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
			StartTargetSelection(base.Owner, SelectionType.Creature, KFFLocalization.Get("!!TAP_CREATURE_TO_BOOST_ATTACK"));
			return;
		}
		CWList<Lane> candidates = AITargetSelection(base.Owner, SelectionType.Creature);
		CreatureScript creatureScript = BestCandidateForReset(candidates);
		Lane currentLane = creatureScript.CurrentLane;
		DoDamage(currentLane);
	}
}
