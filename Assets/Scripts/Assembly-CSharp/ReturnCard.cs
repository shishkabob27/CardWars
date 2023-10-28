using System.Collections.Generic;

public class ReturnCard : SpellScript
{
	private static int StoredScore;

	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		bool flag = CardScript.CanPlay(player, lane, card);
		if (flag)
		{
			List<CardItem> discardPile = GameState.Instance.GetDiscardPile(player);
			foreach (CardItem item in discardPile)
			{
				if (item.Form.Type == (CardType)card.BaseVal1)
				{
					return true;
				}
			}
			return false;
		}
		return flag;
	}

	private static CardItem SelectCard(PlayerType player, CardItem item)
	{
		List<CardItem> discardPile = GameState.Instance.GetDiscardPile(player);
		WeightedList<CardItem> weightedList = new WeightedList<CardItem>();
		List<Lane> lanes = GameState.Instance.GetLanes(player);
		foreach (CardItem item2 in discardPile)
		{
			if (item2.Form.Type != (CardType)item.Form.BaseVal1)
			{
				continue;
			}
			if (item2.Form.Type == CardType.Spell)
			{
				if (item2.Form.ScriptName != "ReturnCard")
				{
					if (item2.Form.CanPlay(player, -1))
					{
						int weight = item2.EvaluateLanePlacement(player, null);
						weightedList.AddSorted(item2, weight);
					}
					else
					{
						int weight2 = GameState.Instance.ScoreBoard() - item.Form.DetermineCost(player) * 1;
						weightedList.AddSorted(item2, weight2);
					}
				}
				continue;
			}
			foreach (Lane item3 in lanes)
			{
				int weight3 = item2.EvaluateLanePlacement(player, item3);
				weightedList.AddSorted(item2, weight3);
			}
		}
		CardItem result = weightedList.TopCandidate();
		StoredScore = weightedList.TopWeight();
		return result;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		SelectCard(player, item);
		return StoredScore;
	}

	private void TakeCard(CardItem item)
	{
		GameState.Instance.RemoveCardFromDiscardPile(base.Owner, item);
		GameState.Instance.PlaceCardInHand(base.Owner, item);
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

	public override void Cast()
	{
		CWFloopActionManager.GetInstance().DoEffect(this, base.Owner);
	}

	public override bool DoResult(CardScript target)
	{
		if (base.Owner == PlayerType.User)
		{
			OpenDiscardPile();
		}
		else
		{
			CardItem item = SelectCard(base.Owner, base.Data);
			TakeCard(item);
		}
		return true;
	}
}
