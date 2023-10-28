public class ReduceCostLeader : LeaderScript
{
	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		bool flag = LeaderScript.CanPlay(player, lane, card);
		if (flag)
		{
			if (GameState.Instance.GetCardsInHand(player) > 1)
			{
				return true;
			}
			return false;
		}
		return flag;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		int cardsInHand = GameState.Instance.GetCardsInHand(player);
		return GameState.Instance.ScoreBoard() + cardsInHand * 1;
	}

	public override void Cast()
	{
		CWFloopActionManager.GetInstance().DoEffect(this, base.Owner);
	}

	public override bool DoResult(CardScript target)
	{
		base.GameInstance.SetDiscount(base.Owner, CardType.None, 1);
		return true;
	}
}
