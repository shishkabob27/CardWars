using System;

public class DamageOpponentIfDamagedLastTurn : CreatureScript
{
	public override bool CanFloop()
	{
		return base.DamageLastTurn > 0 && GameState.Instance.LaneHasCreature(!base.Owner, base.CurrentLane.OpponentLane.Index);
	}

	public override void PopulateLaneMods()
	{
		CreatureScript creature = base.CurrentLane.OpponentLane.GetCreature();
		CardScript.LaneMods[(int)(!base.Owner), creature.CurrentLane.Index].DEF = -Math.Min(creature.Health, GetDamageToDeal());
	}

	public override void Floop()
	{
		TargetList.Add(base.CurrentLane.OpponentLane.GetCreature());
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		(target as CreatureScript).TakeDamage(this, GetDamageToDeal());
		return true;
	}

	private int GetDamageToDeal()
	{
		float num = (float)base.Data.Val1 / 100f;
		float num2 = (float)base.DamageLastTurn * num;
		return (int)num2;
	}
}
