using System;

public class DamageOpponentCards : CreatureScript
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
		int cardsInHand = base.GameInstance.GetCardsInHand(base.Owner);
		CreatureScript creature = base.CurrentLane.OpponentLane.GetCreature();
		CardScript.LaneMods[(int)(!base.Owner), base.CurrentLane.OpponentLane.Index].DEF = -Math.Min(creature.Health, base.Data.Val1 * cardsInHand);
	}

	public override void Floop()
	{
		CreatureScript creature = base.CurrentLane.OpponentLane.GetCreature();
		TargetList.Add(creature);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		int cardsInHand = base.GameInstance.GetCardsInHand(base.Owner);
		CreatureScript creatureScript = target as CreatureScript;
		creatureScript.TakeDamage(this, base.Data.Val1 * cardsInHand);
		return true;
	}
}
