using System;

public class DamageAndDiscard : CreatureScript
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
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].DEF = -base.Health;
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
		base.GameInstance.DiscardCard(base.Owner, base.Data);
		base.GameInstance.RemoveCardFromPlay(base.Owner, base.CurrentLane.Index, CardType.Creature);
		return true;
	}
}
