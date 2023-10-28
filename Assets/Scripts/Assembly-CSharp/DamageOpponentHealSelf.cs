using System;

public class DamageOpponentHealSelf : CreatureScript
{
	public override bool CanFloop()
	{
		if (base.CurrentLane.OpponentLane.HasCreature() || base.Damage > 0)
		{
			return true;
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		if (base.Enemy != null)
		{
			CardScript.LaneMods[(int)(!base.Owner), base.Enemy.CurrentLane.Index].DEF = -Math.Min(base.Enemy.Health, base.Data.Val1);
		}
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].DEF = Math.Min(base.Damage, base.Data.Val2);
	}

	public override void Floop()
	{
		if (base.CurrentLane.OpponentLane.HasCreature())
		{
			CreatureScript creature = base.CurrentLane.OpponentLane.GetCreature();
			TargetList.Add(creature);
		}
		if (base.Damage > 0)
		{
			TargetList.Add(this);
		}
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
}
