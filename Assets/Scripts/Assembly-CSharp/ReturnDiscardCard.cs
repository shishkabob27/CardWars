using System.Collections.Generic;

public class ReturnDiscardCard : CreatureScript
{
	private int StoredScore;

	public override bool CanFloop()
	{
		if (base.GameInstance.GetCardsInHand(base.Owner) >= 7)
		{
			return false;
		}
		List<CardItem> discardPile = GameState.Instance.GetDiscardPile(base.Owner);
		foreach (CardItem item in discardPile)
		{
			if (item.Form.Type == (CardType)base.Data.Form.BaseVal1)
			{
				return true;
			}
		}
		return false;
	}

	private CardItem SelectCard()
	{
		List<CardItem> discardPile = GameState.Instance.GetDiscardPile(base.Owner);
		WeightedList<CardItem> weightedList = new WeightedList<CardItem>();
		List<Lane> lanes = base.GameInstance.GetLanes(base.Owner);
		foreach (CardItem item in discardPile)
		{
			if (item.Form.Type != (CardType)base.Data.Form.BaseVal1)
			{
				continue;
			}
			if (item.Form.Type == CardType.Spell)
			{
				if (item.Form.CanPlay(base.Owner, -1))
				{
					int weight = item.EvaluateLanePlacement(base.Owner, null);
					weightedList.AddSorted(item, weight);
				}
				else
				{
					int weight2 = base.GameInstance.ScoreBoard() - item.Form.DetermineCost(base.Owner) * 1;
					weightedList.AddSorted(item, weight2);
				}
				continue;
			}
			foreach (Lane item2 in lanes)
			{
				int weight3 = item.EvaluateLanePlacement(base.Owner, item2);
				weightedList.AddSorted(item, weight3);
			}
		}
		CardItem result = weightedList.TopCandidate();
		StoredScore = weightedList.TopWeight();
		return result;
	}

	public override int EvaluateAbility()
	{
		SelectCard();
		return StoredScore;
	}

	private void TakeCard(CardItem item)
	{
		GameState.Instance.RemoveCardFromDiscardPile(base.Owner, item);
		GameState.Instance.PlaceCardInHand(base.Owner, item);
		DoEffect();
	}

	public override bool CardFilter(CardItem item)
	{
		if (item.Form.Type == (CardType)base.Data.Form.BaseVal1)
		{
			return true;
		}
		return false;
	}

	public override void CardSelection(CardItem item)
	{
		TakeCard(item);
		CloseDiscardPile();
	}

	public override void Floop()
	{
		if (base.Owner == PlayerType.User)
		{
			OpenDiscardPile();
			return;
		}
		CardItem item = SelectCard();
		TakeCard(item);
	}
}
