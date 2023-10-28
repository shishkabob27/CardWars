public class ATKBonusFloopTurn : CreatureScript
{
	public override bool CanFloop()
	{
		int floopCountTurn = base.GameInstance.GetFloopCountTurn(base.Owner);
		if (floopCountTurn > 0)
		{
			return true;
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		int floopCountTurn = base.GameInstance.GetFloopCountTurn(base.Owner);
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].ATK = base.Data.Val1 * floopCountTurn;
	}

	public override void Floop()
	{
		TargetList.Add(this);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		int num = base.GameInstance.GetFloopCountTurn(base.Owner) - 1;
		base.ATKMod += num * base.Data.Val1;
		return true;
	}
}
