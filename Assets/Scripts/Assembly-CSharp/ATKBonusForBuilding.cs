public class ATKBonusForBuilding : CreatureScript
{
	public override bool CanFloop()
	{
		if (base.GameInstance.BuildingCount(base.Owner) > 0)
		{
			return true;
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		CWList<Lane> candidates = AITargetSelection(base.Owner, SelectionType.Creature);
		int num = base.GameInstance.BuildingCount(base.Owner);
		CreatureScript creatureScript = BestCandidateForATKBonus(candidates, base.Data.Val1 * num);
		CardScript.LaneMods[(int)base.Owner, creatureScript.CurrentLane.Index].ATK = base.Data.Val1 * num;
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
		int num = base.GameInstance.BuildingCount(base.Owner);
		(target as CreatureScript).ATKMod += base.Data.Val1 * num;
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
		CreatureScript creatureScript = BestCandidateForATKBonus(candidates, base.Data.Val1);
		Lane currentLane = creatureScript.CurrentLane;
		DoDamage(currentLane);
	}
}
