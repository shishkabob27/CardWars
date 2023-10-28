public class DrawCardsSpell : SpellScript
{
	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		bool flag = CardScript.CanPlay(player, lane, card);
		if (flag)
		{
			if (GameState.Instance.GetDeck(player).CardCount() > 0 && GameState.Instance.GetCardsInHand(player) < 7)
			{
				return true;
			}
			return false;
		}
		return flag;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		return GameState.Instance.ScoreBoard() - item.Form.DetermineCost(player) * 1 + item.Form.BaseVal1 * 3;
	}

	public override void Cast()
	{
		CWFloopActionManager.GetInstance().DoEffect(this, base.Owner);
	}

	public override bool DoResult(CardScript target)
	{
		for (int i = 0; i < base.Data.Form.BaseVal1; i++)
		{
			base.GameInstance.DrawCard(base.Owner);
		}
		return true;
	}
}
