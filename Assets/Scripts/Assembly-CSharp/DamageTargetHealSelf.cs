using System;

public class DamageTargetHealSelf : CreatureScript
{
	public override bool CanFloop()
	{
		if (base.GameInstance.HasCreaturesInPlay(!base.Owner) || base.Damage > 0)
		{
			return true;
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		CWList<Lane> candidates = AITargetSelection(!base.Owner, SelectionType.Creature);
		CreatureScript creatureScript = CardScript.BestCandidateForAttack(candidates, base.Data.Val1);
		CardScript.LaneMods[(int)(!base.Owner), creatureScript.CurrentLane.Index].DEF = -Math.Min(creatureScript.Health, base.Data.Val1);
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].DEF = Math.Min(base.Damage, base.Data.Val2);
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
		TargetList.Add(this);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		if (creatureScript == this)
		{
			creatureScript.Heal(base.Data.Val2);
		}
		else
		{
			creatureScript.TakeDamage(this, base.Data.Val1);
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
		if (base.GameInstance.HasCreaturesInPlay(!base.Owner))
		{
			if (base.Owner == PlayerType.User)
			{
				StartTargetSelection(!base.Owner, SelectionType.Creature, KFFLocalization.Get("!!TAP_CREATURE_TO_ATTACK"));
				return;
			}
			CWList<Lane> candidates = AITargetSelection(!base.Owner, SelectionType.Creature);
			CreatureScript creatureScript = CardScript.BestCandidateForAttack(candidates, base.Data.Val1);
			Lane currentLane = creatureScript.CurrentLane;
			DoDamage(currentLane);
		}
		else
		{
			TargetList.Add(this);
			DoEffect();
		}
	}
}
