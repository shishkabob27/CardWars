using System;

public class DamageOpponentTheirBuildings : CreatureScript
{
	public override bool CanFloop()
	{
		int num = base.GameInstance.BuildingCount(!base.Owner);
		if (base.CurrentLane.OpponentLane.HasCreature() && num > 0)
		{
			return true;
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		int num = base.GameInstance.BuildingCount(!base.Owner);
		CardScript.LaneMods[(int)(!base.Owner), base.Enemy.CurrentLane.Index].DEF = -Math.Min(base.Enemy.Health, base.Data.Val1 * num);
	}

	public override void Floop()
	{
		CreatureScript creature = base.CurrentLane.OpponentLane.GetCreature();
		TargetList.Add(creature);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		int num = base.GameInstance.BuildingCount(!base.Owner);
		CreatureScript creatureScript = target as CreatureScript;
		creatureScript.TakeDamage(this, base.Data.Val1 * num);
		return true;
	}
}
