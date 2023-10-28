using System;

public class LowerATKOpponentCards : CreatureScript
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
		int cardsInHand = base.GameInstance.GetCardsInHand(!base.Owner);
		CardScript.LaneMods[(int)(!base.Owner), base.CurrentLane.OpponentLane.Index].ATK = -Math.Min(base.Enemy.ATK, base.Data.Val1 * cardsInHand);
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
		int cardsInHand = base.GameInstance.GetCardsInHand(!base.Owner);
		creatureScript.ATKMod -= base.Data.Val1 * cardsInHand;
		return true;
	}
}
