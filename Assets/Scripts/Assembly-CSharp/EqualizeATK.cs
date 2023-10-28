public class EqualizeATK : CreatureScript
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
		int aTK = base.ATK - base.Enemy.ATK;
		CardScript.LaneMods[(int)(!base.Owner), base.Enemy.CurrentLane.Index].ATK = aTK;
	}

	public override void Floop()
	{
		TargetList.Add(base.CurrentLane.OpponentLane.GetCreature());
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		int baseATK = creatureScript.Data.Form.BaseATK;
		int aTKMod = base.ATK - baseATK;
		creatureScript.ATKMod = aTKMod;
		return true;
	}
}
