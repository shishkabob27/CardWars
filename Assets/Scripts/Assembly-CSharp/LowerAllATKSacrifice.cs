using System;
using System.Collections.Generic;

public class LowerAllATKSacrifice : CreatureScript
{
	public override bool CanFloop()
	{
		if (base.GameInstance.HasCreaturesInPlay(!base.Owner))
		{
			return true;
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		List<Lane> lanes = base.GameInstance.GetLanes(!base.Owner);
		foreach (Lane item in lanes)
		{
			if (item.HasCreature())
			{
				CardScript.LaneMods[(int)(!base.Owner), item.Index].ATK = -Math.Min(base.Enemy.ATK, base.Data.Val1);
			}
		}
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].DEF = -base.Health;
	}

	public override void Floop()
	{
		List<Lane> lanes = base.GameInstance.GetLanes(!base.Owner);
		foreach (Lane item in lanes)
		{
			if (item.HasCreature())
			{
				CreatureScript creature = item.GetCreature();
				TargetList.Add(creature);
			}
		}
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
