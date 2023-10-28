using System;

public class DrawCardsSacrifice : CreatureScript
{
	public override bool CanFloop()
	{
		if (base.GameInstance.GetCardsInHand(base.Owner) < 7 && base.GameInstance.GetDeck(base.Owner).CardCount() > 0)
		{
			return true;
		}
		return false;
	}

	public override int EvaluateAbility()
	{
		CardScript.ResetMods();
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].DEF = -base.Health;
		CardScript.APMods[(int)base.Owner] = -DetermineFloopCost();
		int num = CardScript.ScoreBoard(base.Owner);
		int num2 = Math.Min(7 - base.GameInstance.GetCardsInHand(base.Owner), base.Data.Val1);
		return num + num2 * 3;
	}

	public override void Floop()
	{
		int baseVal = base.Data.Form.BaseVal1;
		for (int i = 0; i < baseVal; i++)
		{
			base.GameInstance.DrawCard(base.Owner);
		}
		TargetList.Add(this);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		base.GameInstance.RemoveCardFromPlay(target.Owner, target.CurrentLane.Index, CardType.Creature);
		base.GameInstance.DiscardCard(target.Owner, target.Data);
		return true;
	}
}
