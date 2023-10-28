public class ATKHealBonus : CreatureScript
{
	public override bool CanFloop()
	{
		return true;
	}

	public override void PopulateLaneMods()
	{
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].ATK = base.Data.Val1;
	}

	public override void Floop()
	{
		TargetList.Add(this);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		creatureScript.ATKMod += base.Data.Val1;
		creatureScript.Heal(base.Data.Val2);
		return true;
	}
}
