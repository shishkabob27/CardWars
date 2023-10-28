using System;

public class DamageHero : CreatureScript
{
	public override bool CanFloop()
	{
		return true;
	}

	public override void PopulateLaneMods()
	{
		CardScript.HealthMods[(int)(!base.Owner)] = -Math.Min(base.GameInstance.GetHealth(!base.Owner), base.Data.Val1);
	}

	public override void Floop()
	{
		CWFloopActionManager.GetInstance().DoEffect(this, !base.Owner);
	}

	public override bool DoResult(CardScript target)
	{
		base.GameInstance.DealDamage(!base.Owner, base.Data.Val1);
		return true;
	}
}
