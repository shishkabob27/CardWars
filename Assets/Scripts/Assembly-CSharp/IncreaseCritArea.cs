public class IncreaseCritArea : CreatureScript
{
	public override bool CanFloop()
	{
		return (int)GameState.Instance.CritAreaModifier == 0;
	}

	public override int EvaluateAbility()
	{
		return int.MinValue;
	}

	public override void Floop()
	{
		TargetList.Add(this);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		base.GameInstance.CritAreaModifier = (float)base.Data.Val1 / 100f;
		return true;
	}
}
