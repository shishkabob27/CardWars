using System.Collections.Generic;

public class ReturnCardTypeLeader : LeaderScript
{
	private static int StoredScore;

	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		bool flag = LeaderScript.CanPlay(player, lane, card);
		if (flag)
		{
			if (GameState.Instance.GetCardsInHand(player) >= 7)
			{
				return false;
			}
			List<CardItem> discardPile = GameState.Instance.GetDiscardPile(player);
			LeaderForm form = GameState.Instance.GetLeader(player).Form;
			foreach (CardItem item in discardPile)
			{
				if (item.Form.Type == form.forCardType)
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
		LeaderForm form = GameState.Instance.GetLeader(player).Form;
		foreach (CardItem item2 in discardPile)
		{
			CardType type = item2.Form.Type;
			CardType? forCardType = form.forCardType;
			if (type != forCardType.Value)
			{
				continue;
			}
			if (item2.Form.Type == CardType.Spell)
			{
				if (item2.Form.CanPlay(player, -1))
				{
					int weight = item2.EvaluateLanePlacement(player, null);
					weightedList.AddSorted(item2, weight);
				}
				else
				{
					int weight2 = GameState.Instance.ScoreBoard();
					weightedList.AddSorted(item2, weight2);
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
		CardType type = item.Form.Type;
		CardType? forCardType = base.Leader.Form.forCardType;
		if (type == forCardType.Value)
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
