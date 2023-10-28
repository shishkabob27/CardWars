public class DEFBonusOpponentLandscapes : CreatureScript
{
	public override bool CanFloop()
	{
		int num = base.GameInstance.LandscapeCount(!base.Owner, (LandscapeType)base.Data.Form.BaseVal2);
		if (num > 0)
		{
			return true;
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		int num = base.GameInstance.LandscapeCount(!base.Owner, (LandscapeType)base.Data.Form.BaseVal2);
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].DEF = base.Data.Val1 * num;
	}

	public override void Floop()
	{
		TargetList.Add(this);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		int num = base.GameInstance.LandscapeCount(!base.Owner, (LandscapeType)base.Data.Form.BaseVal2);
		(target as CreatureScript).DEFMod += base.Data.Val1 * num;
		return true;
	}
}
