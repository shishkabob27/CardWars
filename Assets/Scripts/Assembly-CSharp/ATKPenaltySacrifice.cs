using System;

public class ATKPenaltySacrifice : CreatureScript
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
		CardScript.LaneMods[(int)(!base.Owner), base.Enemy.CurrentLane.Index].ATK = -Math.Min(base.Enemy.ATK, base.Data.Val1);
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].DEF = -base.Health;
	}

	public override void Floop()
	{
		TargetList.Add(base.CurrentLane.OpponentLane.GetCreature());
		TargetList.Add(this);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		if (creatureScript == this)
		{
			base.GameInstance.RemoveCardFromPlay(target.Owner, target.CurrentLane.Index, CardType.Creature);
			base.GameInstance.DiscardCard(target.Owner, target.Data);
		}
		else
		{
			creatureScript.ATKMod -= base.Data.Val1;
		}
		return true;
	}
}
