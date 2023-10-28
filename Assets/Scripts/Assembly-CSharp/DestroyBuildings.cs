using System.Collections.Generic;

public class DestroyBuildings : SpellScript
{
	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		bool flag = CardScript.CanPlay(player, lane, card);
		if (flag)
		{
			int num = GameState.Instance.BuildingCount(player);
			int num2 = GameState.Instance.BuildingCount(!player);
			if (num2 > 0 || num > 0)
			{
				return true;
			}
			return false;
		}
		return flag;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		List<Lane> lanes = GameState.Instance.GetLanes(player);
		foreach (Lane item2 in lanes)
		{
			if (item2.HasBuilding())
			{
				BuildingScript building = item2.GetBuilding();
				building.OnCardLeftPlay(building);
			}
			if (item2.OpponentLane.HasBuilding())
			{
				BuildingScript building2 = item2.OpponentLane.GetBuilding();
				building2.OnCardLeftPlay(building2);
			}
		}
		int result = GameState.Instance.ScoreBoard() - item.Form.DetermineCost(player) * 1;
		foreach (Lane item3 in lanes)
		{
			if (item3.HasBuilding())
			{
				BuildingScript building3 = item3.GetBuilding();
				building3.StopTriggerEffects();
				building3.OnCardEnterPlay(building3);
				building3.ResumeTriggerEffects();
			}
			if (item3.OpponentLane.HasBuilding())
			{
				BuildingScript building4 = item3.OpponentLane.GetBuilding();
				building4.StopTriggerEffects();
				building4.OnCardEnterPlay(building4);
				building4.ResumeTriggerEffects();
			}
		}
		return result;
	}

	public override void Cast()
	{
		List<Lane> lanes = GameState.Instance.GetLanes(base.Owner);
		foreach (Lane item in lanes)
		{
			if (item.HasBuilding())
			{
				BuildingScript building = item.GetBuilding();
				TargetList.Add(building);
			}
			if (item.OpponentLane.HasBuilding())
			{
				BuildingScript building2 = item.OpponentLane.GetBuilding();
				TargetList.Add(building2);
			}
		}
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		base.GameInstance.DiscardCard(target.Owner, target.Data);
		base.GameInstance.RemoveCardFromPlay(target.Owner, target.CurrentLane.Index, CardType.Building);
		VOManager.Instance.PlayEvent(target.Owner, VOEvent.BuildingDestroyed);
		return true;
	}
}
