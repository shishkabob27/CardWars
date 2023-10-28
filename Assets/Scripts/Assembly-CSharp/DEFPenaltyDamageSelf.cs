using System;

public class DEFPenaltyDamageSelf : CreatureScript
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
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].DEF = -Math.Min(base.Health, base.Data.Val1);
		CardScript.LaneMods[(int)(!base.Owner), base.Enemy.CurrentLane.Index].DEF = -Math.Min(base.Health, base.Data.Val2);
	}

	public override void Floop()
	{
		TargetList.Add(this);
		TargetList.Add(base.CurrentLane.OpponentLane.GetCreature());
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		if (creatureScript == this)
		{
			creatureScript.TakeDamage(this, base.Data.Val1);
		}
		else
		{
			creatureScript.DEFMod -= base.Data.Val2;
		}
		return true;
	}
}
