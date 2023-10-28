using System;

public class LowerATKByATK : CreatureScript
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
		CardScript.LaneMods[(int)(!base.Owner), base.CurrentLane.OpponentLane.Index].ATK = -Math.Min(base.Enemy.ATK, base.ATK);
	}

	public override void Floop()
	{
		CreatureScript creature = base.CurrentLane.OpponentLane.GetCreature();
		TargetList.Add(creature);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		(target as CreatureScript).ATKMod -= base.ATK;
		return true;
	}
}
