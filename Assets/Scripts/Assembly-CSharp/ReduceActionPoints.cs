public class ReduceActionPoints : SpellScript
{
	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		CardScript.APMods[(int)player] = -item.Form.DetermineCost(player);
		CardScript.APMods[(int)(!player)] = -2;
		return CardScript.ScoreBoard(player);
	}

	public override void Cast()
	{
		CWFloopActionManager.GetInstance().DoEffect(this, !base.Owner);
	}

	public override bool DoResult(CardScript target)
	{
		base.GameInstance.AddMagicPoints(!base.Owner, -2, false);
		return true;
	}
}
