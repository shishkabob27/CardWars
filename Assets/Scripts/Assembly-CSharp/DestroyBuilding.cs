public class DestroyBuilding : SpellScript
{
	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		bool flag = CardScript.CanPlay(player, lane, card);
		if (flag)
		{
			if (GameState.Instance.BuildingCount(!player) > 0)
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
			if (lane.HasBuilding())
			{
				cWList.Add(lane);
			}
		}
		return cWList;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		CWList<Lane> candidates = AIStaticSelection(!player);
		CardScript cardScript = null;
		int num = -item.Form.DetermineCost(player) * 1;
		BuildingScript buildingScript = CardScript.BestBuildingSacrifice(candidates);
		cardScript = buildingScript;
		buildingScript.StopTriggerEffects();
		cardScript.OnCardLeftPlay(cardScript);
		int result = GameState.Instance.ScoreBoard() + num;
		cardScript.OnCardEnterPlay(cardScript);
		buildingScript.ResumeTriggerEffects();
		return result;
	}

	public override bool SelectionFilter(Lane candidate)
	{
		if (candidate.HasBuilding())
		{
			return true;
		}
		return false;
	}

	private void DestroyTarget(Lane target)
	{
		BuildingScript building = target.GetBuilding();
		TargetList.Add(building);
		DoEffect();
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
			StartTargetSelection(!base.Owner, SelectionType.Building, KFFLocalization.Get("!!PICK_A_BUILDING"));
			return;
		}
		CWList<Lane> candidates = AITargetSelection(!base.Owner, SelectionType.Building);
		BuildingScript buildingScript = CardScript.BestBuildingSacrifice(candidates);
		DestroyTarget(buildingScript.CurrentLane);
	}

	public override bool DoResult(CardScript target)
	{
		base.GameInstance.RemoveCardFromPlay(!base.Owner, target.CurrentLane.Index, CardType.Building);
		base.GameInstance.DiscardCard(!base.Owner, target.Data);
		VOManager.Instance.PlayEvent(target.Owner, VOEvent.BuildingDestroyed);
		return true;
	}
}
