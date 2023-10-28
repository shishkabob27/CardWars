using System;

public class HealHeroCreatures : CreatureScript
{
	public override bool CanFloop()
	{
		if (base.GameInstance.GetHealth(base.Owner) < base.GameInstance.GetMaxHealth(base.Owner) && base.GameInstance.HasCreaturesInPlay(base.Owner))
		{
			return true;
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		int num = base.GameInstance.CreatureCount(base.Owner);
		CardScript.HealthMods[(int)base.Owner] = Math.Min(base.GameInstance.GetMaxHealth(base.Owner) - base.GameInstance.GetHealth(base.Owner), base.Data.Val1 * num);
	}

	public override void Floop()
	{
		CWFloopActionManager.GetInstance().DoEffect(this, base.Owner);
	}

	public override bool DoResult(CardScript target)
	{
		int num = base.GameInstance.CreatureCount(base.Owner);
		base.GameInstance.DealDamage(base.Owner, -(base.Data.Val1 * num));
		return true;
	}
}
