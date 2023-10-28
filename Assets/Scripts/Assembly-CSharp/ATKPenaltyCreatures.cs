using System;

public class ATKPenaltyCreatures : CreatureScript
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
		int num = base.GameInstance.CreatureCount(!base.Owner);
		CardScript.LaneMods[(int)(!base.Owner), base.Enemy.CurrentLane.Index].ATK = -Math.Min(base.Enemy.ATK, base.Data.Val1 * num);
	}

	public override void Floop()
	{
		TargetList.Add(base.CurrentLane.OpponentLane.GetCreature());
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		int num = base.GameInstance.CreatureCount(!base.Owner);
		(target as CreatureScript).ATKMod -= base.Data.Val1 * num;
		return true;
	}
}
