public class EmptyLaneATKBonus : CreatureScript
{
	public override bool CanFloop()
	{
		int num = base.GameInstance.EmptyLaneCount(base.Owner);
		if (num > 0)
		{
			return true;
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		int num = base.GameInstance.EmptyLaneCount(base.Owner);
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].ATK = base.Data.Val1 * num;
	}

	public override void Floop()
	{
		int num = base.GameInstance.EmptyLaneCount(base.Owner);
		base.ATKMod += num * base.Data.Val1;
	}
}
