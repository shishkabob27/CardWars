using System;

public class DrainOpponentATKLowerSelfDEF : CreatureScript
{
	public override bool CanFloop()
	{
		if (base.CurrentLane.OpponentLane.HasCreature() && base.Enemy.ATK > 0)
		{
			return true;
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		CardScript.LaneMods[(int)(!base.Owner), base.CurrentLane.OpponentLane.Index].ATK = -Math.Min(base.Enemy.ATK, base.Enemy.ATK / 2);
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].ATK = base.Enemy.ATK / 2;
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].DEF = -Math.Min(base.Enemy.ATK, base.Enemy.ATK / 2);
	}

	public override void Floop()
	{
		TargetList.Add(this);
		CreatureScript creature = base.CurrentLane.OpponentLane.GetCreature();
		TargetList.Add(creature);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		if (creatureScript == this)
		{
			if (base.Enemy.ATK == 1)
			{
				creatureScript.ATKMod++;
				creatureScript.DEFMod--;
			}
			else
			{
				creatureScript.ATKMod += base.Enemy.ATK / 2;
				creatureScript.DEFMod -= base.Enemy.ATK / 2;
			}
		}
		else if (base.Enemy.ATK == 1)
		{
			creatureScript.ATKMod--;
		}
		else
		{
			creatureScript.ATKMod -= base.Enemy.ATK / 2;
		}
		return true;
	}
}
