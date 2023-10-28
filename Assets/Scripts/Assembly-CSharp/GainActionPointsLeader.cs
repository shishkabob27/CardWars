public class GainActionPointsLeader : LeaderScript
{
	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		bool flag = LeaderScript.CanPlay(player, lane, card);
		if (player == PlayerType.Opponent && flag)
		{
			LeaderForm form = GameState.Instance.GetLeader(player).Form;
			int baseVal = form.BaseVal1;
			GameState.Instance.AddMagicPoints(player, baseVal);
			if (!GameState.Instance.HasLegalMove(player))
			{
				flag = false;
			}
			GameState.Instance.AddMagicPoints(player, -baseVal);
		}
		return flag;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		LeaderForm form = GameState.Instance.GetLeader(player).Form;
		return GameState.Instance.ScoreBoard() + form.BaseVal1 * 1;
	}

	public override void Cast()
	{
		CWFloopActionManager.GetInstance().DoEffect(this, base.Owner);
	}

	public override bool DoResult(CardScript target)
	{
		base.GameInstance.AddMagicPoints(base.Owner, base.Leader.Form.BaseVal1);
		return true;
	}
}
