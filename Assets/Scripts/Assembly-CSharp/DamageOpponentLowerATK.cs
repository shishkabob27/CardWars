using System;

public class DamageOpponentLowerATK : CreatureScript
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
		CreatureScript creature = base.CurrentLane.OpponentLane.GetCreature();
		CardScript.LaneMods[(int)(!base.Owner), creature.CurrentLane.Index].DEF = -Math.Min(creature.Health, base.Data.Val1);
		CardScript.LaneMods[(int)(!base.Owner), base.CurrentLane.OpponentLane.Index].ATK = -Math.Min(base.Enemy.ATK, base.Data.Val2);
	}

	public override void Floop()
	{
		CreatureScript creature = base.CurrentLane.OpponentLane.GetCreature();
		TargetList.Add(creature);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		creatureScript.TakeDamage(this, base.Data.Val1);
		creatureScript.ATKMod -= base.Data.Val2;
		return true;
	}
}
