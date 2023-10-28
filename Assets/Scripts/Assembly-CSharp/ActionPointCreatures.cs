public class ActionPointCreatures : SpellScript
{
	private static bool Locked;

	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		if (Locked)
		{
			return false;
		}
		bool flag = CardScript.CanPlay(player, lane, card);
		if (flag)
		{
			int num = GameState.Instance.CreatureCount(player);
			if (num <= 0)
			{
				flag = false;
			}
			if (player == PlayerType.Opponent && flag)
			{
				GameState.Instance.AddMagicPoints(player, num);
				Locked = true;
				if (!GameState.Instance.HasLegalMove(player))
				{
					flag = false;
				}
				Locked = false;
				GameState.Instance.AddMagicPoints(player, -num);
			}
		}
		return flag;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		int num = GameState.Instance.CreatureCount(player);
		return GameState.Instance.ScoreBoard() + num * 1 - item.Form.DetermineCost(player) * 1;
	}

	public override void Cast()
	{
		CWFloopActionManager.GetInstance().DoEffect(this, base.Owner);
	}

	public override bool DoResult(CardScript target)
	{
		int points = base.GameInstance.CreatureCount(base.Owner);
		base.GameInstance.AddMagicPoints(base.Owner, points);
		return true;
	}
}
