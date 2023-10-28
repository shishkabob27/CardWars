public class ResetSelf : CreatureScript
{
	public override bool CanFloop()
	{
		if (base.Damage > 0 || base.ATKMod != 0 || base.DEFMod != 0)
		{
			return true;
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].DEF = base.Damage - base.DEFMod;
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].ATK = -base.ATKMod;
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
		creatureScript.ATKMod = 0;
		creatureScript.DEFMod = 0;
		return true;
	}
}
