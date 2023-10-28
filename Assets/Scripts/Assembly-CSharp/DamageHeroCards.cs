using System;

public class DamageHeroCards : CreatureScript
{
	public override bool CanFloop()
	{
		return true;
	}

	public override void PopulateLaneMods()
	{
		int cardsInHand = base.GameInstance.GetCardsInHand(base.Owner);
		CardScript.HealthMods[(int)(!base.Owner)] = -Math.Min(base.GameInstance.GetHealth(!base.Owner), base.Data.Val1 * cardsInHand);
	}

	public override void Floop()
	{
		CWFloopActionManager.GetInstance().DoEffect(this, !base.Owner);
	}

	public override bool DoResult(CardScript target)
	{
		int cardsInHand = base.GameInstance.GetCardsInHand(base.Owner);
		base.GameInstance.DealDamage(!base.Owner, base.Data.Val1 * cardsInHand);
		return true;
	}
}
