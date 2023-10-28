using System.Collections.Generic;

public class ReturnCardLeader : LeaderScript
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
			if (discardPile.Count > 0)
			{
				return true;
			}
			return false;
		}
		return flag;
	}

	private static CardItem SelectCard(PlayerType player)
	{
		List<CardItem> discardPile = GameState.Instance.GetDiscardPile(player);
		WeightedList<CardItem> weightedList = new WeightedList<CardItem>();
		List<Lane> lanes = GameState.Instance.GetLanes(player);
		foreach (CardItem item in discardPile)
		{
			if (item.Form.Type == CardType.Spell)
			{
				if (item.Form.CanPlay(player, -1))
				{
					int weight = item.EvaluateLanePlacement(player, null);
					weightedList.AddSorted(item, weight);
				}
				else
				{
					int weight2 = GameState.Instance.ScoreBoard();
					weightedList.AddSorted(item, weight2);
				}
				continue;
			}
			foreach (Lane item2 in lanes)
			{
				int weight3 = item.EvaluateLanePlacement(player, item2);
				weightedList.AddSorted(item, weight3);
			}
		}
		CardItem result = weightedList.TopCandidate();
		StoredScore = weightedList.TopWeight();
		return result;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		SelectCard(player);
		return StoredScore;
	}

	private void TakeCard(CardItem item)
	{
		GameState.Instance.RemoveCardFromDiscardPile(base.Owner, item);
		GameState.Instance.PlaceCardInHand(base.Owner, item);
	}

	public override bool CardFilter(CardItem item)
	{
		return true;
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
			CardItem item = SelectCard(base.Owner);
			TakeCard(item);
		}
		return true;
	}
}
