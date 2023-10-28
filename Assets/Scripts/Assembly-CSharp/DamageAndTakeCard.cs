using System;

public class DamageAndTakeCard : CreatureScript
{
	private CardItem card;

	public override bool CanFloop()
	{
		return base.GameInstance.GetCardsInHand(base.Owner) < 7 && GameState.Instance.LaneHasCreature(!base.Owner, base.CurrentLane.OpponentLane.Index);
	}

	public override void Floop()
	{
		card = null;
		TargetList.Add(base.CurrentLane.OpponentLane.GetCreature());
		DoEffect();
	}

	public override void PopulateLaneMods()
	{
		CreatureScript creature = base.CurrentLane.OpponentLane.GetCreature();
		CardScript.LaneMods[(int)(!base.Owner), creature.CurrentLane.Index].DEF = -Math.Min(creature.Health, base.Data.Val1);
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		if (creatureScript.Health <= base.Data.Val1)
		{
			card = target.Data;
		}
		creatureScript.TakeDamage(this, base.Data.Val1);
		return true;
	}

	public override void OnCreatureDied(CardItem deadCard)
	{
		if (deadCard == card)
		{
			base.GameInstance.PlaceCardInHand(base.Owner, deadCard);
			base.GameInstance.RemoveCardFromDiscardPile(!base.Owner, deadCard);
			card = null;
		}
	}
}
