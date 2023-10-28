using System;

public class LowerDEFFloop : CreatureScript
{
	public override bool CanFloop()
	{
		int floopCountTurn = base.GameInstance.GetFloopCountTurn(base.Owner);
		if (base.GameInstance.HasCreaturesInPlay(!base.Owner) && floopCountTurn > 0)
		{
			return true;
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		CWList<Lane> candidates = AITargetSelection(!base.Owner, SelectionType.Creature);
		int floopCountTurn = base.GameInstance.GetFloopCountTurn(base.Owner);
		CreatureScript creatureScript = CardScript.BestCandidateForAttack(candidates, base.Data.Val1 * floopCountTurn);
		CardScript.LaneMods[(int)(!base.Owner), creatureScript.CurrentLane.Index].DEF = -Math.Min(creatureScript.Health, base.Data.Val1 * floopCountTurn);
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
		int num = base.GameInstance.GetFloopCountTurn(base.Owner) - 1;
		(target as CreatureScript).DEFMod -= base.Data.Val1 * num;
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
		int floopCountTurn = base.GameInstance.GetFloopCountTurn(base.Owner);
		CWList<Lane> candidates = AITargetSelection(!base.Owner, SelectionType.Creature);
		CreatureScript creatureScript = CardScript.BestCandidateForAttack(candidates, base.Data.Val1 * floopCountTurn);
		Lane currentLane = creatureScript.CurrentLane;
		DoDamage(currentLane);
	}
}
