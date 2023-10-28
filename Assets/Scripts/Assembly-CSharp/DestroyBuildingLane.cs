public class DestroyBuildingLane : SpellScript
{
	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		bool flag = CardScript.CanPlay(player, lane, card);
		if (flag)
		{
			if (GameState.Instance.BuildingCount(!player) > 0 || GameState.Instance.CreatureCount(!player) > 0)
			{
				return true;
			}
			return false;
		}
		return flag;
	}

	private static CWList<Lane> AIStaticSelection(PlayerType player)
	{
		CWList<Lane> cWList = new CWList<Lane>();
		GameState instance = GameState.Instance;
		for (int i = 0; i < 4; i++)
		{
			Lane lane = instance.GetLane(player, i);
			if (lane.HasBuilding() || lane.HasCreature())
			{
				cWList.Add(lane);
			}
		}
		return cWList;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		CWList<Lane> candidates = AIStaticSelection(!player);
		Lane lane2 = CardScript.BestDoubleBuildingSacrifice(player, candidates);
		CardScript.ResetMods();
		CardScript.APMods[(int)player] = -(item.Form.DetermineCost(player) * 1);
		if (lane2.HasBuilding())
		{
			BuildingScript building = lane2.GetBuilding();
			building.OnCardLeftPlay(building);
		}
		if (lane2.OpponentLane.HasBuilding())
		{
			BuildingScript building2 = lane2.OpponentLane.GetBuilding();
			building2.OnCardLeftPlay(building2);
		}
		if (lane2.HasCreature())
		{
			CardScript.LaneMods[(int)(!player), lane2.Index].DEF = -lane2.GetCreature().Health;
		}
		if (lane2.OpponentLane.HasCreature())
		{
			CardScript.LaneMods[(int)player, lane2.Index].DEF = -lane2.OpponentLane.GetCreature().Health;
		}
		int result = CardScript.ScoreBoard(player);
		if (lane2.HasBuilding())
		{
			BuildingScript building3 = lane2.GetBuilding();
			building3.StopTriggerEffects();
			building3.OnCardEnterPlay(building3);
			building3.ResumeTriggerEffects();
		}
		if (lane2.OpponentLane.HasBuilding())
		{
			BuildingScript building4 = lane2.OpponentLane.GetBuilding();
			building4.StopTriggerEffects();
			building4.OnCardEnterPlay(building4);
			building4.ResumeTriggerEffects();
		}
		return result;
	}

	public override bool SelectionFilter(Lane candidate)
	{
		if (candidate.HasBuilding() || candidate.HasCreature())
		{
			return true;
		}
		return false;
	}

	private void DestroyTarget(Lane target)
	{
		if (target.HasBuilding())
		{
			BuildingScript building = target.GetBuilding();
			TargetList.Add(building);
		}
		if (target.HasCreature())
		{
			CreatureScript creature = target.GetCreature();
			TargetList.Add(creature);
		}
		if (target.OpponentLane.HasBuilding())
		{
			BuildingScript building2 = target.OpponentLane.GetBuilding();
			TargetList.Add(building2);
		}
		if (target.OpponentLane.HasCreature())
		{
			CreatureScript creature2 = target.OpponentLane.GetCreature();
			TargetList.Add(creature2);
		}
		CWFloopActionManager.GetInstance().DoEffectNeutral(this, TargetList.ToArray());
		TargetList.Clear();
	}

	public override void OnTargetSelected(Lane target)
	{
		DestroyTarget(target);
		EndTargetSelection();
	}

	public override void Cast()
	{
		if (base.Owner == PlayerType.User)
		{
			StartTargetSelection(!base.Owner, SelectionType.Landscape, KFFLocalization.Get("!!PICK_A_TARGET"));
			return;
		}
		CWList<Lane> candidates = AITargetSelection(!base.Owner, SelectionType.Landscape);
		Lane target = CardScript.BestDoubleBuildingSacrifice(base.Owner, candidates);
		DestroyTarget(target);
	}

	public override bool DoResult(CardScript target)
	{
		base.GameInstance.RemoveCardFromPlay(target.Owner, target.CurrentLane.Index, target.Data.Form.Type);
		base.GameInstance.DiscardCard(target.Owner, target.Data);
		if (target.Data.Form.Type == CardType.Creature)
		{
			VOManager.Instance.PlayEvent(target.Owner, VOEvent.CreatureDestroyed);
		}
		else
		{
			VOManager.Instance.PlayEvent(target.Owner, VOEvent.BuildingDestroyed);
		}
		return true;
	}
}
