public class Protection : CreatureScript
{
	public override bool CanFloop()
	{
		if (base.CurrentLane.OpponentLane.HasCreature() && !base.Enemy.CantAttack)
		{
			return true;
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].DEF = base.Enemy.ATK;
	}

	public override void Floop()
	{
		TargetList.Add(base.CurrentLane.OpponentLane.GetCreature());
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		creatureScript.CantAttack = true;
		return true;
	}
}
