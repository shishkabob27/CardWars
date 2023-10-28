public class CardsDEFBonus : CreatureScript
{
	public override bool CanFloop()
	{
		if (base.GameInstance.GetCardsInHand(base.Owner) > 0)
		{
			return true;
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		int cardsInHand = base.GameInstance.GetCardsInHand(base.Owner);
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].DEF = base.Data.Val1 * cardsInHand;
	}

	public override void Floop()
	{
		TargetList.Add(this);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		int cardsInHand = base.GameInstance.GetCardsInHand(base.Owner);
		creatureScript.DEFMod += cardsInHand * base.Data.Val1;
		return true;
	}
}
