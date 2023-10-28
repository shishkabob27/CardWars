using System.Collections.Generic;

public class DiscardForPoints : SpellScript
{
	private static bool Locked;

	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		if (Locked)
		{
			return false;
		}
		bool flag = CardScript.CanPlay(player, lane, card);
		if (flag && player == PlayerType.Opponent && flag)
		{
			GameState.Instance.AddMagicPoints(player, card.BaseVal1);
			Locked = true;
			if (!GameState.Instance.HasLegalMove(player))
			{
				flag = false;
			}
			Locked = false;
			GameState.Instance.AddMagicPoints(player, -card.BaseVal1);
		}
		return flag;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		int num = item.Form.BaseVal1 - item.Form.DetermineCost(player);
		int cardsInHand = GameState.Instance.GetCardsInHand(player);
		return GameState.Instance.ScoreBoard() - cardsInHand * 3 + num * 1;
	}

	public override void Cast()
	{
		CWFloopActionManager.GetInstance().DoEffect(this, base.Owner);
	}

	public override bool DoResult(CardScript target)
	{
		List<CardItem> hand = base.GameInstance.GetHand(base.Owner);
		while (hand.Count > 0)
		{
			CardItem card = hand[0];
			base.GameInstance.DiscardCard(base.Owner, card);
			hand.RemoveAt(0);
		}
		base.GameInstance.AddMagicPoints(base.Owner, base.Data.Form.BaseVal1);
		return true;
	}
}
