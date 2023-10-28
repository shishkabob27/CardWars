public class HealSelf : CreatureScript
{
	public override bool CanFloop()
	{
		if (base.Damage > 0)
		{
			return true;
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].DEF = base.Damage;
	}

	public override void Floop()
	{
		TargetList.Add(this);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		creatureScript.Heal(creatureScript.Damage);
		return true;
	}
}
