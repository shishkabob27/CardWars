using System;

public class LowerATKBonusATK : CreatureScript
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
		CardScript.LaneMods[(int)(!base.Owner), base.CurrentLane.OpponentLane.Index].ATK = -Math.Min(base.Enemy.ATK, base.Data.Val1);
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].ATK = base.Data.Val2;
	}

	public override void Floop()
	{
		CreatureScript creature = base.CurrentLane.OpponentLane.GetCreature();
		TargetList.Add(creature);
		TargetList.Add(this);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		if (creatureScript == this)
		{
			creatureScript.ATKMod += base.Data.Val2;
		}
		else
		{
			creatureScript.ATKMod -= base.Data.Val1;
		}
		return true;
	}
}
