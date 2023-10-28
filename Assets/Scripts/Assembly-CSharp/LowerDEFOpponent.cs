using System;

public class LowerDEFOpponent : CreatureScript
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
		CardScript.LaneMods[(int)(!base.Owner), base.CurrentLane.OpponentLane.Index].DEF = -Math.Min(base.Enemy.Health, base.Data.Val1);
	}

	public override void Floop()
	{
		CreatureScript creature = base.CurrentLane.OpponentLane.GetCreature();
		TargetList.Add(creature);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		(target as CreatureScript).DEFMod -= base.Data.Val1;
		return true;
	}
}
