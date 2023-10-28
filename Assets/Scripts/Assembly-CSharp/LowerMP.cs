public class LowerMP : CreatureScript
{
	private bool Locked;

	public override bool CanFloop()
	{
		return true;
	}

	public override int EvaluateAbility()
	{
		CardScript.ResetMods();
		CardScript.APMods[(int)base.Owner] = -DetermineFloopCost();
		CardScript.APMods[(int)(!base.Owner)] = -base.Data.Form.BaseVal1;
		return CardScript.ScoreBoard(base.Owner);
	}

	public override void Floop()
	{
		TargetList.Add(this);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		base.GameInstance.AddMagicPoints(!base.Owner, -base.Data.Form.BaseVal1, false);
		return true;
	}
}
