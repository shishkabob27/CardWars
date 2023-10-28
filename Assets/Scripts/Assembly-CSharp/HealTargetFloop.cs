using System;
using System.Collections.Generic;

public class HealTargetFloop : CreatureScript
{
	public override bool CanFloop()
	{
		List<Lane> lanes = GameState.Instance.GetLanes(base.Owner);
		int floopCountTurn = base.GameInstance.GetFloopCountTurn(base.Owner);
		if (floopCountTurn <= 0)
		{
			return false;
		}
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
		int floopCountTurn = base.GameInstance.GetFloopCountTurn(base.Owner);
		CreatureScript creatureScript = CardScript.BestCandidateForHealing(candidates, base.Data.Val1 * floopCountTurn, true);
		CardScript.LaneMods[(int)base.Owner, creatureScript.CurrentLane.Index].DEF = Math.Min(base.Data.Val1 * floopCountTurn, creatureScript.Damage);
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
		int num = base.GameInstance.GetFloopCountTurn(base.Owner) - 1;
		CreatureScript creatureScript = target as CreatureScript;
		creatureScript.Heal(base.Data.Val1 * num);
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
		int floopCountTurn = base.GameInstance.GetFloopCountTurn(base.Owner);
		CWList<Lane> candidates = AITargetSelection(base.Owner, SelectionType.Creature);
		CreatureScript creatureScript = CardScript.BestCandidateForHealing(candidates, base.Data.Val1 * floopCountTurn, true);
		Lane currentLane = creatureScript.CurrentLane;
		DoHealing(currentLane);
	}
}
