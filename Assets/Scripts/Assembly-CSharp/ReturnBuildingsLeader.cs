using System.Collections.Generic;

public class ReturnBuildingsLeader : LeaderScript
{
	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		bool flag = LeaderScript.CanPlay(player, lane, card);
		if (flag)
		{
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
		int result = GameState.Instance.ScoreBoard();
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
		return result;
	}

	public override void Cast()
	{
		List<Lane> lanes = base.GameInstance.GetLanes(!base.Owner);
		foreach (Lane item in lanes)
		{
			if (item.HasBuilding())
			{
				BuildingScript building = item.GetBuilding();
				TargetList.Add(building);
			}
		}
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		base.GameInstance.RemoveCardFromPlay(target.Owner, target.CurrentLane.Index, CardType.Building);
		if (base.GameInstance.GetCardsInHand(target.Owner) < 7)
		{
			base.GameInstance.PlaceCardInHand(target.Owner, target.Data);
		}
		else
		{
			base.GameInstance.DiscardCard(target.Owner, target.Data);
		}
		return true;
	}
}
