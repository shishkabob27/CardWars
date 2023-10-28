using System.Collections.Generic;

public class HealAllSacrifice : CreatureScript
{
	public override bool CanFloop()
	{
		List<Lane> lanes = GameState.Instance.GetLanes(base.Owner);
		foreach (Lane item in lanes)
		{
			if (item.HasCreature())
			{
				CreatureScript creature = item.GetCreature();
				if (creature != this && creature.Damage > 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		List<Lane> lanes = GameState.Instance.GetLanes(base.Owner);
		foreach (Lane item in lanes)
		{
			if (item.HasCreature())
			{
				CreatureScript creature = item.GetCreature();
				CardScript.LaneMods[(int)base.Owner, item.Index].DEF = creature.Damage;
			}
		}
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].DEF = -base.Health;
	}

	public override void Floop()
	{
		List<Lane> lanes = base.GameInstance.GetLanes(base.Owner);
		foreach (Lane item in lanes)
		{
			if (item.HasCreature())
			{
				CreatureScript creature = item.GetCreature();
				if (creature != this)
				{
					TargetList.Add(creature);
				}
			}
		}
		TargetList.Add(this);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		if (creatureScript != this)
		{
			creatureScript.Heal(creatureScript.Damage);
		}
		else
		{
			base.GameInstance.RemoveCardFromPlay(target.Owner, target.CurrentLane.Index, CardType.Creature);
			base.GameInstance.DiscardCard(target.Owner, target.Data);
		}
		return true;
	}
}
