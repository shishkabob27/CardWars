using System.Collections.Generic;

public class ShuffleAndDraw : SpellScript
{
	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		int num = 5 - GameState.Instance.GetCardsInHand(player);
		return GameState.Instance.ScoreBoard() - item.Form.DetermineCost(player) * 1 + num * 3;
	}

	public override void Cast()
	{
		CWFloopActionManager.GetInstance().DoEffect(this, base.Owner);
	}

	public override bool DoResult(CardScript target)
	{
		List<CardItem> hand = base.GameInstance.GetHand(base.Owner);
		Deck deck = base.GameInstance.GetDeck(base.Owner);
		while (hand.Count > 0)
		{
			CardItem newCard = hand[0];
			deck.AddCard(newCard);
			hand.RemoveAt(0);
		}
		deck.Shuffle();
		for (int i = 0; i < 5; i++)
		{
			base.GameInstance.DrawCard(base.Owner);
		}
		return true;
	}
}
