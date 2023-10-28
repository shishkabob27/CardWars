using System.Collections.Generic;

public class MoveBuildingSelf : SpellScript
{
	private static Lane Source;

	private static Lane Destination;

	private static bool SelectingBuilding;

	private static int StoredScore;

	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		GameState instance = GameState.Instance;
		bool flag = CardScript.CanPlay(player, lane, card);
		if (flag)
		{
			bool flag2 = true;
			bool flag3 = true;
			for (int i = 0; i < 4; i++)
			{
				if (instance.LaneHasBuilding(player, i))
				{
					flag2 = false;
				}
				else
				{
					flag3 = false;
				}
			}
			if (flag2 || flag3)
			{
				flag = false;
			}
		}
		return flag;
	}

	public static void SetSourceAndDestination(CWList<Lane> BuildingCandidates, CWList<Lane> LaneCandidates, PlayerType player)
	{
		List<BuildingScript> list = CardScript.LanesToBuildings(BuildingCandidates);
		int num = 0;
		Source = null;
		Destination = null;
		foreach (BuildingScript item in list)
		{
			item.StopTriggerEffects();
			item.OnCardLeftPlay(item);
			foreach (Lane LaneCandidate in LaneCandidates)
			{
				int num2 = item.Data.EvaluateLanePlacement(player, LaneCandidate);
				if (Source == null)
				{
					Source = item.CurrentLane;
					Destination = LaneCandidate;
					num = num2;
				}
				else if (num2 > num)
				{
					Source = item.CurrentLane;
					Destination = LaneCandidate;
					num = num2;
				}
			}
			item.OnCardEnterPlay(item);
			item.ResumeTriggerEffects();
		}
		StoredScore = num;
	}

	private static CWList<Lane> AIStaticSelection(PlayerType player, SelectionType targetType)
	{
		CWList<Lane> cWList = new CWList<Lane>();
		GameState instance = GameState.Instance;
		for (int i = 0; i < 4; i++)
		{
			Lane lane = instance.GetLane(player, i);
			if (SelectingBuilding)
			{
				if (lane.HasBuilding())
				{
					cWList.Add(lane);
				}
			}
			else if (!lane.HasBuilding())
			{
				cWList.Add(lane);
			}
		}
		return cWList;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		SelectingBuilding = true;
		CWList<Lane> buildingCandidates = AIStaticSelection(player, SelectionType.Building);
		SelectingBuilding = false;
		CWList<Lane> laneCandidates = AIStaticSelection(player, SelectionType.Landscape);
		SetSourceAndDestination(buildingCandidates, laneCandidates, player);
		return StoredScore - item.Form.DetermineCost(player) * 1;
	}

	public override bool SelectionFilter(Lane candidate)
	{
		if (SelectingBuilding)
		{
			if (candidate.HasBuilding())
			{
				return true;
			}
		}
		else if (!candidate.HasBuilding())
		{
			return true;
		}
		return false;
	}

	public override void OnTargetSelected(Lane target)
	{
		if (SelectingBuilding)
		{
			Source = target;
			TargetList.Add(target.GetBuilding());
			EndTargetSelection();
			SelectingBuilding = false;
			StartTargetSelection(base.Owner, SelectionType.Landscape, KFFLocalization.Get("!!PICK_AN_EMPTY_LANE"));
		}
		else
		{
			Destination = target;
			EndTargetSelection();
			DoEffect();
		}
	}

	public override bool DoResult(CardScript target)
	{
		base.GameInstance.MoveCard(base.Owner, Source.Index, Destination.Index, CardType.Building);
		return true;
	}

	public override void Cast()
	{
		if (base.Owner == PlayerType.User)
		{
			SelectingBuilding = true;
			StartTargetSelection(base.Owner, SelectionType.Building, KFFLocalization.Get("!!PICK_A_BUILDING_TO_MOVE"));
			return;
		}
		SelectingBuilding = true;
		CWList<Lane> buildingCandidates = AITargetSelection(base.Owner, SelectionType.Building);
		SelectingBuilding = false;
		CWList<Lane> laneCandidates = AITargetSelection(base.Owner, SelectionType.Landscape);
		SetSourceAndDestination(buildingCandidates, laneCandidates, base.Owner);
		DoEffect();
	}
}
