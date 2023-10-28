using System.Collections.Generic;

public class ReturnHandsDrawCardsAddMP : CreatureScript
{
	public override void StartTurn()
	{
		if (GameState.Instance.ExtraMagicPoints > 0)
		{
			base.GameInstance.AddMagicPoints(base.Owner, GameState.Instance.ExtraMagicPoints);
			GameState.Instance.ResetExtraMagicPoints();
		}
	}

	public override bool CanFloop()
	{
		return GameState.Instance.ExtraMagicPoints == 0;
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
		GameState.Instance.ExtraMagicPoints = base.Data.Form.BaseVal2;
		TargetList.Add(this);
		DoEffect();
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
		for (int i = 0; i < base.Data.Form.BaseVal1; i++)
		{
			base.GameInstance.DrawCard(base.Owner);
		}
		return true;
	}
}
