public class DEFBonus : CreatureScript
{
	public override bool CanFloop()
	{
		return true;
	}

	public override void PopulateLaneMods()
	{
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].DEF = base.Data.Val1;
	}

	public override void Floop()
	{
		TargetList.Add(this);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		(target as CreatureScript).DEFMod += base.Data.Val1;
		return true;
	}
}
