using System.Collections.Generic;

public class BlockCardTypeDestroyBuildingsLeader : LeaderScript
{
	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		bool flag = LeaderScript.CanPlay(player, lane, card);
		LeaderItem leader = GameState.Instance.GetLeader(player);
		if (flag)
		{
			List<CardItem> hand = GameState.Instance.GetHand(!player);
			int num = 0;
			foreach (CardItem item in hand)
			{
				CardType type = item.Form.Type;
				CardType? forCardType = leader.Form.forCardType;
				if (type == forCardType.Value)
				{
					num++;
				}
			}
			if (num > 0)
			{
				return true;
			}
			if (GameState.Instance.HasCardTypeInPlay(!player, CardType.Building))
			{
				return true;
			}
			return false;
		}
		return flag;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		List<Lane> lanes = GameState.Instance.GetLanes(!player);
		foreach (Lane item2 in lanes)
		{
			if (item2.HasBuilding())
			{
				BuildingScript building = item2.GetBuilding();
				building.OnCardLeftPlay(building);
			}
		}
		int num = GameState.Instance.ScoreBoard();
		foreach (Lane item3 in lanes)
		{
			if (item3.HasBuilding())
			{
				BuildingScript building2 = item3.GetBuilding();
				building2.StopTriggerEffects();
				building2.OnCardEnterPlay(building2);
				building2.ResumeTriggerEffects();
			}
		}
		List<CardItem> hand = GameState.Instance.GetHand(!player);
		LeaderForm form = GameState.Instance.GetLeader(player).Form;
		int num2 = 0;
		foreach (CardItem item4 in hand)
		{
			CardType type = item4.Form.Type;
			CardType? forCardType = form.forCardType;
			if (type == forCardType.Value)
			{
				num2++;
			}
		}
		if (item != null)
		{
			num = num + num2 * 3 - item.Form.DetermineCost(player) * 1;
		}
		return num;
	}

	public override void Cast()
	{
		List<Lane> lanes = base.GameInstance.GetLanes(!base.Owner);
		int num = 0;
		foreach (Lane item in lanes)
		{
			if (item.HasBuilding())
			{
				num++;
				BuildingScript building = item.GetBuilding();
				TargetList.Add(building);
			}
		}
		List<CardItem> hand = GameState.Instance.GetHand(!base.Owner);
		LeaderForm form = GameState.Instance.GetLeader(base.Owner).Form;
		int num2 = 0;
		foreach (CardItem item2 in hand)
		{
			CardType type = item2.Form.Type;
			CardType? forCardType = form.forCardType;
			if (type == forCardType.Value)
			{
				num2++;
			}
		}
		if (num2 > 0)
		{
			TargetList.Add(null);
			CWFloopActionManager.GetInstance().DoEffect(this, !base.Owner);
		}
		if (num > 0)
		{
			DoEffect();
		}
	}

	public override bool DoResult(CardScript target)
	{
		if (target != null)
		{
			base.GameInstance.DiscardCard(target.Owner, target.Data);
			base.GameInstance.RemoveCardFromPlay(target.Owner, target.CurrentLane.Index, CardType.Building);
			VOManager.Instance.PlayEvent(target.Owner, VOEvent.BuildingDestroyed);
		}
		GameState gameInstance = base.GameInstance;
		PlayerType player = !base.Owner;
		CardType? forCardType = base.Leader.Form.forCardType;
		gameInstance.EnableCasting(player, forCardType.Value, false);
		return true;
	}
}
