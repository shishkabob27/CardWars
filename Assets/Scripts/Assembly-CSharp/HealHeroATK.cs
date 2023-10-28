using System;

public class HealHeroATK : CreatureScript
{
	public override bool CanFloop()
	{
		if (base.GameInstance.GetHealth(base.Owner) < base.GameInstance.GetMaxHealth(base.Owner))
		{
			return true;
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		CardScript.HealthMods[(int)base.Owner] = Math.Min(base.GameInstance.GetMaxHealth(base.Owner) - base.GameInstance.GetHealth(base.Owner), base.ATK);
	}

	public override void Floop()
	{
		CWFloopActionManager.GetInstance().DoEffect(this, base.Owner);
	}

	public override bool DoResult(CardScript target)
	{
		base.GameInstance.DealDamage(base.Owner, -base.ATK);
		return true;
	}
}
