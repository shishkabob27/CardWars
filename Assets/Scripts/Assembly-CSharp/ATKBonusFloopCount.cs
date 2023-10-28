public class ATKBonusFloopCount : CreatureScript
{
	private int floopCount = 1;

	public override bool CanFloop()
	{
		return true;
	}

	public override void PopulateLaneMods()
	{
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].ATK = floopCount;
	}

	public override void Floop()
	{
		TargetList.Add(this);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		base.ATKMod += floopCount;
		floopCount++;
		return true;
	}
}
