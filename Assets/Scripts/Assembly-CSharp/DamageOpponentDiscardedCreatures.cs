using System;

public class DamageOpponentDiscardedCreatures : CreatureScript
{
	public override bool CanFloop()
	{
		if (base.CurrentLane.OpponentLane.HasCreature() && base.GameInstance.DiscardPileContains(!base.Owner, CardType.Creature))
		{
			return true;
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		CreatureScript creature = base.CurrentLane.OpponentLane.GetCreature();
		int num = base.GameInstance.DiscardCount(!base.Owner, CardType.Creature);
		CardScript.LaneMods[(int)(!base.Owner), creature.CurrentLane.Index].DEF = -Math.Min(creature.Health, base.Data.Val1 * num);
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
		int num = base.GameInstance.DiscardCount(!base.Owner, CardType.Creature);
		creatureScript.TakeDamage(this, base.Data.Val1 * num);
		return true;
	}
}
