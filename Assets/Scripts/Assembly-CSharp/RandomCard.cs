using System.Collections.Generic;

public class RandomCard : SpellScript
{
	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		bool flag = CardScript.CanPlay(player, lane, card);
		if (flag)
		{
			Deck deck = GameState.Instance.GetDeck(player);
			List<CardItem> cards = deck.GetCards();
			foreach (CardItem item in cards)
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

	private static int ScoreCards(PlayerType player, CardItem item)
	{
		List<CardItem> cards = GameState.Instance.GetDeck(player).GetCards();
		List<Lane> lanes = GameState.Instance.GetLanes(player);
		int num = 0;
		int num2 = 0;
		foreach (CardItem item2 in cards)
		{
			if (item2.Form.Type != (CardType)item.Form.BaseVal1)
			{
				continue;
			}
			if (item2.Form.Type == CardType.Spell)
			{
				if (item2.Form.ScriptName != "RandomCard")
				{
					if (item2.Form.CanPlay(player, -1))
					{
						num += item2.EvaluateLanePlacement(player, null);
						num2++;
					}
					else
					{
						num += GameState.Instance.ScoreBoard() - item.Form.DetermineCost(player) * 1;
						num2++;
					}
				}
				continue;
			}
			foreach (Lane item3 in lanes)
			{
				num += item2.EvaluateLanePlacement(player, item3);
				num2++;
			}
		}
		return num / num2;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		return ScoreCards(player, item);
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

	public override void Cast()
	{
		CWFloopActionManager.GetInstance().DoEffect(this, base.Owner);
	}

	public override bool DoResult(CardScript target)
	{
		CWList<CardItem> cWList = new CWList<CardItem>();
		List<CardItem> cards = base.GameInstance.GetDeck(base.Owner).GetCards();
		foreach (CardItem item2 in cards)
		{
			if (CardFilter(item2))
			{
				cWList.Add(item2);
			}
		}
		CardItem item = cWList.RandomItem();
		TakeCard(item);
		return true;
	}
}
