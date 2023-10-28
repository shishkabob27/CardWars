using System;

public class ATKBonusDamageSelf : CreatureScript
{
	public override bool CanFloop()
	{
		if (base.Health > base.Data.Val1)
		{
			return true;
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].DEF = -Math.Min(base.Health, base.Data.Val1);
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].ATK = base.Data.Val2;
	}

	public override void Floop()
	{
		TargetList.Add(this);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		creatureScript.TakeDamage(this, base.Data.Val1);
		creatureScript.ATKMod += base.Data.Val2;
		return true;
	}
}
