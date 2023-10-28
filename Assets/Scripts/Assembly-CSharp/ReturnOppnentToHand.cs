public class ReturnOppnentToHand : CreatureScript
{
	public override bool CanFloop()
	{
		if (base.CurrentLane.OpponentLane.HasCard((CardType)base.Data.Form.BaseVal1))
		{
			return true;
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		CardScript.LaneMods[(int)(!base.Owner), base.Enemy.CurrentLane.Index].DEF = -base.Enemy.Health;
	}

	public override void Floop()
	{
		CardScript script = base.CurrentLane.OpponentLane.GetScript((CardType)base.Data.Form.BaseVal1);
		TargetList.Add(script);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		base.GameInstance.RemoveCardFromPlay(target.Owner, target.CurrentLane.Index, target.Data.Form.Type);
		if (base.GameInstance.GetCardsInHand(target.Owner) < 7)
		{
			base.GameInstance.PlaceCardInHand(target.Owner, target.Data);
		}
		else
		{
			base.GameInstance.DiscardCard(target.Owner, target.Data);
		}
		return true;
	}
}
