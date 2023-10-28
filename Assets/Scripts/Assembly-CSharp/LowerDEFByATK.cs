using System;

public class LowerDEFByATK : CreatureScript
{
	public override bool CanFloop()
	{
		if (base.CurrentLane.OpponentLane.HasCreature())
		{
			return true;
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		CardScript.LaneMods[(int)(!base.Owner), base.CurrentLane.OpponentLane.Index].DEF = -Math.Min(base.Enemy.Health, base.ATK);
	}

	public override void Floop()
	{
		CreatureScript creature = base.CurrentLane.OpponentLane.GetCreature();
		TargetList.Add(creature);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		(target as CreatureScript).DEFMod -= base.ATK;
		return true;
	}
}
