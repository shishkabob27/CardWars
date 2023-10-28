public class DisableLane : SpellScript
{
	private static Lane BestCandidate(CWList<Lane> Candidates)
	{
		CWList<Lane> cWList = new CWList<Lane>();
		CWList<Lane> cWList2 = new CWList<Lane>();
		CWList<Lane> cWList3 = new CWList<Lane>();
		CWList<Lane> cWList4 = new CWList<Lane>();
		CWList<Lane> cWList5 = new CWList<Lane>();
		CWList<Lane> cWList6 = null;
		foreach (Lane Candidate in Candidates)
		{
			if (!Candidate.HasCreature())
			{
				if (Candidate.OpponentLane.HasCreature())
				{
					cWList2.Add(Candidate);
				}
				else
				{
					cWList.Add(Candidate);
				}
			}
			else if (Candidate.OpponentLane.HasCreature())
			{
				if (!Candidate.HasBuilding())
				{
					if (Candidate.GetCreature().InDanger)
					{
						cWList3.Add(Candidate);
					}
					else
					{
						cWList4.Add(Candidate);
					}
				}
			}
			else if (!Candidate.HasBuilding())
			{
				cWList5.Add(Candidate);
			}
		}
		cWList6 = ((cWList2.Count > 0) ? cWList2 : ((cWList.Count > 0) ? cWList : ((cWList3.Count > 0) ? cWList3 : ((cWList5.Count > 0) ? cWList5 : ((cWList4.Count <= 0) ? Candidates : cWList4)))));
		Lane lane = null;
		foreach (Lane item in cWList6)
		{
			if (lane == null)
			{
				lane = item;
			}
			else if (item.HasCreature() && item.GetCreature().ATK > lane.GetCreature().ATK)
			{
				lane = item;
			}
		}
		return lane;
	}

	private static CWList<Lane> AIStaticSelection(PlayerType player, SelectionType targetType)
	{
		CWList<Lane> cWList = new CWList<Lane>();
		GameState instance = GameState.Instance;
		for (int i = 0; i < 4; i++)
		{
			Lane lane = instance.GetLane(player, i);
			if (!lane.Disabled)
			{
				cWList.Add(lane);
			}
		}
		return cWList;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		CWList<Lane> candidates = AIStaticSelection(!player, SelectionType.Landscape);
		Lane lane2 = BestCandidate(candidates);
		int num = GameState.Instance.ScoreBoard();
		int num2 = GameState.Instance.ScoreLane(lane2.OpponentLane);
		int num3 = GameState.Instance.ScoreLane(lane2);
		int num4 = num2 - num3;
		return num + num4;
	}

	public override bool SelectionFilter(Lane candidate)
	{
		return true;
	}

	private void ReturnTarget(Lane target)
	{
		target.Disabled = true;
		CWFloopActionManager.GetInstance().DoEffect(this, target.Index);
	}

	public override void OnTargetSelected(Lane target)
	{
		ReturnTarget(target);
		EndTargetSelection();
	}

	public override void Cast()
	{
		if (base.Owner == PlayerType.User)
		{
			StartTargetSelection(!base.Owner, SelectionType.Creature, KFFLocalization.Get("!!PICK_A_CREATURE"));
			return;
		}
		CWList<Lane> candidates = AITargetSelection(!base.Owner, SelectionType.Landscape);
		Lane target = BestCandidate(candidates);
		ReturnTarget(target);
	}
}
