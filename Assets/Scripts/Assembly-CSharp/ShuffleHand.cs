using System.Collections.Generic;

public class ShuffleHand : CreatureScript
{
	public override bool CanFloop()
	{
		return true;
	}

	public override int EvaluateAbility()
	{
		int num = base.GameInstance.ScoreBoard();
		int num2 = base.GameInstance.ScoreCardsInHand(base.Owner);
		int num3 = 15 - num2;
		return num + num3;
	}

	public override void Floop()
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
