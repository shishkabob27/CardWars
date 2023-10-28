using System;

public class DamageOpponentAndHero : CreatureScript
{
	public override bool CanFloop()
	{
		return true;
	}

	public override void PopulateLaneMods()
	{
		if (base.CurrentLane.OpponentLane.HasCreature())
		{
			CreatureScript creature = base.CurrentLane.OpponentLane.GetCreature();
			CardScript.LaneMods[(int)(!base.Owner), creature.CurrentLane.Index].DEF = -Math.Min(creature.Health, base.Data.Val1);
		}
		CardScript.HealthMods[(int)(!base.Owner)] = -Math.Min(base.GameInstance.GetHealth(!base.Owner), base.Data.Val1);
	}

	public override void Floop()
	{
		if (base.CurrentLane.OpponentLane.HasCreature())
		{
			CreatureScript creature = base.CurrentLane.OpponentLane.GetCreature();
			TargetList.Add(creature);
			DoEffect();
		}
		else
		{
			CWFloopActionManager.GetInstance().DoEffect(this, !base.Owner);
		}
	}

	public override bool DoResult(CardScript target)
	{
		if (target != null)
		{
			CreatureScript creatureScript = target as CreatureScript;
			creatureScript.TakeDamage(this, base.Data.Val1);
		}
		base.GameInstance.DealDamage(!base.Owner, base.Data.Val1);
		return true;
	}
}
