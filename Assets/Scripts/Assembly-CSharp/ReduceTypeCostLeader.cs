using System.Collections.Generic;

public class ReduceTypeCostLeader : LeaderScript
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
		int num = 0;
		foreach (CardItem item2 in hand)
		{
			if (item2.Form.Type == form.forCardType)
			{
				num++;
			}
		}
		return GameState.Instance.ScoreBoard() + num * 1;
	}

	public override void Cast()
	{
		CWFloopActionManager.GetInstance().DoEffect(this, base.Owner);
	}

	public override bool DoResult(CardScript target)
	{
		GameState gameInstance = base.GameInstance;
		PlayerType owner = base.Owner;
		CardType? forCardType = base.Leader.Form.forCardType;
		gameInstance.SetDiscount(owner, forCardType.Value, base.Leader.Form.BaseVal1);
		return true;
	}
}
