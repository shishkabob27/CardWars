using System.Collections.Generic;

public class FlatFloopCost : SpellScript
{
	private static bool Locked;

	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		if (Locked)
		{
			return false;
		}
		bool flag = CardScript.CanPlay(player, lane, card);
		if (flag && player == PlayerType.Opponent && flag)
		{
			GameState.Instance.SetFlatFloopCost(player, card.BaseVal1);
			Locked = true;
			if (!GameState.Instance.HasLegalMove(player))
			{
				flag = false;
			}
			Locked = false;
			GameState.Instance.SetFlatFloopCost(player, 0);
		}
		return flag;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		List<Lane> lanes = GameState.Instance.GetLanes(player);
		int num = 0;
		foreach (Lane item2 in lanes)
		{
			if (item2.HasCreature())
			{
				CreatureScript creature = item2.GetCreature();
				int num2 = creature.DetermineFloopCost();
				int num3 = creature.Data.Form.BaseVal1 - num2;
				num += num3;
			}
		}
		return GameState.Instance.ScoreBoard() + num * 1 - item.Form.DetermineCost(player) * 1;
	}

	public override void Cast()
	{
		CWFloopActionManager.GetInstance().DoEffect(this, base.Owner);
	}

	public override bool DoResult(CardScript target)
	{
		GameState.Instance.SetFlatFloopCost(base.Owner, base.Data.Form.BaseVal1);
		return true;
	}
}
