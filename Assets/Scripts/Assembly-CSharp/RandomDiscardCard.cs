using System.Collections.Generic;
using UnityEngine;

public class RandomDiscardCard : CreatureScript
{
	public override bool CanFloop()
	{
		if (base.GameInstance.GetCardsInHand(base.Owner) >= 7)
		{
			return false;
		}
		List<CardItem> discardPile = GameState.Instance.GetDiscardPile(base.Owner);
		if (discardPile.Count > 0)
		{
			return true;
		}
		return false;
	}

	private int ScoreCards()
	{
		List<CardItem> discardPile = GameState.Instance.GetDiscardPile(base.Owner);
		List<Lane> lanes = base.GameInstance.GetLanes(base.Owner);
		int num = 0;
		int num2 = 0;
		foreach (CardItem item in discardPile)
		{
			if (item.Form.Type == CardType.Spell)
			{
				if (item.Form.CanPlay(base.Owner, -1))
				{
					num += item.EvaluateLanePlacement(base.Owner, null);
					num2++;
				}
				else
				{
					num += base.GameInstance.ScoreBoard() - item.Form.DetermineCost(base.Owner) * 1;
					num2++;
				}
				continue;
			}
			foreach (Lane item2 in lanes)
			{
				num += item.EvaluateLanePlacement(base.Owner, item2);
				num2++;
			}
		}
		return num / num2;
	}

	public override int EvaluateAbility()
	{
		return ScoreCards();
	}

	private void TakeCard(CardItem item)
	{
		GameState.Instance.RemoveCardFromDiscardPile(base.Owner, item);
		GameState.Instance.PlaceCardInHand(base.Owner, item);
		DoEffect();
	}

	public override void Floop()
	{
		List<CardItem> discardPile = GameState.Instance.GetDiscardPile(base.Owner);
		int index = Random.Range(0, discardPile.Count);
		CardItem item = discardPile[index];
		TakeCard(item);
	}
}
