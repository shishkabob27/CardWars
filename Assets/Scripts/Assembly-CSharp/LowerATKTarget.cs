using System;
using System.Collections.Generic;

public class LowerATKTarget : CreatureScript
{
	public override bool CanFloop()
	{
		List<Lane> lanes = GameState.Instance.GetLanes(!base.Owner);
		foreach (Lane item in lanes)
		{
			if (item.HasCreature())
			{
				return true;
			}
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		CWList<Lane> candidates = AITargetSelection(!base.Owner, SelectionType.Creature);
		CreatureScript creatureScript = BestCandidateForCrippling(candidates, base.Data.Val1);
		CardScript.LaneMods[(int)(!base.Owner), creatureScript.CurrentLane.Index].ATK = -Math.Min(creatureScript.ATK, base.Data.Val1);
	}

	public override bool SelectionFilter(Lane candidate)
	{
		if (candidate.HasCreature())
		{
			return true;
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
		(target as CreatureScript).ATKMod -= base.Data.Val1;
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
			StartTargetSelection(!base.Owner, SelectionType.Creature, KFFLocalization.Get("!!TAP_CREATURE_TO_ATTACK"));
			return;
		}
		CWList<Lane> candidates = AITargetSelection(!base.Owner, SelectionType.Creature);
		CreatureScript creatureScript = BestCandidateForCrippling(candidates, base.Data.Val1);
		Lane currentLane = creatureScript.CurrentLane;
		DoDamage(currentLane);
	}
}
