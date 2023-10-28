public class LandscapeATKBonus : CreatureScript
{
	public override bool CanFloop()
	{
		int num = base.GameInstance.LandscapeCount(base.Owner, (LandscapeType)base.Data.Form.BaseVal2);
		if (num > 0)
		{
			return true;
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		int num = base.GameInstance.LandscapeCount(base.Owner, (LandscapeType)base.Data.Form.BaseVal2);
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].ATK = base.Data.Val1 * num;
	}

	public override void Floop()
	{
		TargetList.Add(this);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		int num = base.GameInstance.LandscapeCount(base.Owner, (LandscapeType)base.Data.Form.BaseVal2);
		creatureScript.ATKMod += num * base.Data.Val1;
		return true;
	}
}
