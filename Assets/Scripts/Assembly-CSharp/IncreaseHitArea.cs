public class IncreaseHitArea : CreatureScript
{
	public override bool CanFloop()
	{
		return (int)GameState.Instance.HitAreaModifier == 0;
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
		base.GameInstance.HitAreaModifier = (float)base.Data.Val1 / 100f;
		return true;
	}
}
