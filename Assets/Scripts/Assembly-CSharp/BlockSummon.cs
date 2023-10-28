public class BlockSummon : CreatureScript
{
	public override bool CanFloop()
	{
		if (!base.CurrentLane.OpponentLane.Disabled)
		{
			return true;
		}
		return false;
	}

	public override int EvaluateAbility()
	{
		int num = base.GameInstance.ScoreBoard();
		int num2 = base.GameInstance.ScoreLane(base.CurrentLane);
		int num3 = base.GameInstance.ScoreLane(base.CurrentLane.OpponentLane);
		int num4 = num2 - num3;
		return num + num4;
	}

	public override void Floop()
	{
		base.CurrentLane.OpponentLane.Disabled = true;
		CWFloopActionManager.GetInstance().DoEffect(this, base.CurrentLane.OpponentLane.Index);
		TargetList.Clear();
	}
}
