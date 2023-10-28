public class EmptyLaneCards : SpellScript
{
	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		bool flag = CardScript.CanPlay(player, lane, card);
		if (flag)
		{
			if (GameState.Instance.GetCardsInHand(player) < 7 && GameState.Instance.GetDeck(player).CardCount() > 0 && GameState.Instance.EmptyLaneCount(player) > 0)
			{
				return true;
			}
			return false;
		}
		return flag;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		int num = GameState.Instance.EmptyLaneCount(player);
		return GameState.Instance.ScoreBoard() + num * 3 - item.Form.DetermineCost(player) * 1;
	}

	public override void Cast()
	{
		CWFloopActionManager.GetInstance().DoEffect(this, base.Owner);
	}

	public override bool DoResult(CardScript target)
	{
		int num = base.GameInstance.EmptyLaneCount(base.Owner);
		for (int i = 0; i < num; i++)
		{
			base.GameInstance.DrawCard(base.Owner);
		}
		return true;
	}
}
