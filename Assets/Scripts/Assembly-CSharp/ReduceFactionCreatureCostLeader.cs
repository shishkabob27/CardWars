using System;
using System.Collections.Generic;

public class ReduceFactionCreatureCostLeader : LeaderScript
{
	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		bool flag = LeaderScript.CanPlay(player, lane, card);
		if (flag)
		{
			List<CardItem> hand = GameState.Instance.GetHand(player);
			LeaderForm form = GameState.Instance.GetLeader(player).Form;
			foreach (CardItem item in hand)
			{
				if (item.Form.Type == form.forCardType)
				{
					return true;
				}
			}
			return false;
		}
		return flag;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		List<CardItem> hand = GameState.Instance.GetHand(player);
		LeaderForm form = GameState.Instance.GetLeader(player).Form;
		return GameState.Instance.ScoreBoard() + hand.Count * 1;
	}

	public override void Cast()
	{
		CWFloopActionManager.GetInstance().DoEffect(this, base.Owner);
	}

	public override bool DoResult(CardScript target)
	{
		foreach (int value in Enum.GetValues(typeof(Faction)))
		{
			if ((int?)value == (int?)base.Leader.Form.forFaction)
			{
				GameState gameInstance = base.GameInstance;
				PlayerType owner = base.Owner;
				CardType? forCardType = base.Leader.Form.forCardType;
				gameInstance.SetDiscount(owner, forCardType.Value, base.Leader.Form.BaseVal1, true, (Faction)value);
			}
			else
			{
				GameState gameInstance2 = base.GameInstance;
				PlayerType owner2 = base.Owner;
				CardType? forCardType2 = base.Leader.Form.forCardType;
				gameInstance2.SetDiscount(owner2, forCardType2.Value, base.Leader.Form.BaseVal2, true, (Faction)value);
			}
		}
		return true;
	}
}
