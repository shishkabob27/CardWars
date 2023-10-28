using System.Collections.Generic;

public class BlockCardType : SpellScript
{
	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		if (CardScript.CanPlay(player, lane, card) && GameState.Instance.IsCastingEnabled(!player, (CardType)card.BaseVal1))
		{
			return true;
		}
		return false;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		List<CardItem> hand = GameState.Instance.GetHand(!player);
		int num = 0;
		foreach (CardItem item2 in hand)
		{
			if (item2.Form.Type == (CardType)item.Form.BaseVal1)
			{
				num++;
			}
		}
		return GameState.Instance.ScoreBoard() + num * 3 - item.Form.DetermineCost(player) * 1;
	}

	public override void Cast()
	{
		CWFloopActionManager.GetInstance().DoEffect(this, !base.Owner);
	}

	public override bool DoResult(CardScript target)
	{
		base.GameInstance.EnableCasting(!base.Owner, (CardType)base.Data.Form.BaseVal1, false);
		return true;
	}
}
