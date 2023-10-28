public class CreatureDEFBonus : CreatureScript
{
	public override bool CanFloop()
	{
		int num = base.GameInstance.CreatureCount(base.Owner);
		if (num > 0)
		{
			return true;
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		int num = base.GameInstance.CreatureCount(base.Owner);
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].DEF = base.Data.Val1 * num;
	}

	public override void Floop()
	{
		TargetList.Add(this);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		int num = base.GameInstance.CreatureCount(base.Owner);
		creatureScript.DEFMod += num * base.Data.Val1;
		return true;
	}
}
