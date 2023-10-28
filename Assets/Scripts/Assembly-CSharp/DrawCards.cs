using System;

public class DrawCards : CreatureScript
{
	public override bool CanFloop()
	{
		if (GameState.Instance.GetCardsInHand(base.Owner) < 7 && base.GameInstance.GetDeck(base.Owner).CardCount() > 0)
		{
			return true;
		}
		return false;
	}

	public override int EvaluateAbility()
	{
		int num = base.GameInstance.ScoreBoard();
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
		DoEffect();
	}
}
