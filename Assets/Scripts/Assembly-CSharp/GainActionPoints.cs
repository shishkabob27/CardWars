public class GainActionPoints : CreatureScript
{
	public override bool CanFloop()
	{
		return true;
	}

	public override int EvaluateAbility()
	{
		int num = base.GameInstance.ScoreBoard();
		int num2 = base.GameInstance.ScoreActionPoints(base.Owner);
		int num3 = (GameState.Instance.CurrentMagicPoints + base.Data.Form.BaseVal1) * 1;
		return num - num2 + num3;
	}

	public override void Floop()
	{
		TargetList.Add(this);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		base.GameInstance.AddBonusPoints(base.Owner, base.Data.Form.BaseVal1);
		return true;
	}
}
