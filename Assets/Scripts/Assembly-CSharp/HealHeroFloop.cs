using System;

public class HealHeroFloop : CreatureScript
{
	public override bool CanFloop()
	{
		int floopCountTurn = base.GameInstance.GetFloopCountTurn(base.Owner);
		if (base.GameInstance.GetHealth(base.Owner) < base.GameInstance.GetMaxHealth(base.Owner) && floopCountTurn > 0)
		{
			return true;
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		int floopCountTurn = base.GameInstance.GetFloopCountTurn(base.Owner);
		CardScript.HealthMods[(int)base.Owner] = Math.Min(base.GameInstance.GetMaxHealth(base.Owner) - base.GameInstance.GetHealth(base.Owner), base.Data.Val1 * floopCountTurn);
	}

	public override void Floop()
	{
		CWFloopActionManager.GetInstance().DoEffect(this, base.Owner);
	}

	public override bool DoResult(CardScript target)
	{
		int num = base.GameInstance.GetFloopCountTurn(base.Owner) - 1;
		base.GameInstance.DealDamage(base.Owner, -base.Data.Val1 * num);
		return true;
	}
}
