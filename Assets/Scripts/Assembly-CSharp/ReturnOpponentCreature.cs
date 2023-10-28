public class ReturnOpponentCreature : SpellScript
{
	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		bool flag = CardScript.CanPlay(player, lane, card);
		if (flag)
		{
			if (GameState.Instance.HasCreaturesInPlay(!player))
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
			if (lane.HasCreature())
			{
				cWList.Add(lane);
			}
		}
		return cWList;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		CWList<Lane> candidates = AIStaticSelection(!player);
		CreatureScript creatureScript = CardScript.BestCandidateForAttack(candidates, -1);
		CardScript.ResetMods();
		CardScript.LaneMods[(int)(!player), creatureScript.CurrentLane.Index].DEF = -creatureScript.Health;
		CardScript.APMods[(int)player] = -item.Form.DetermineCost(player);
		return CardScript.ScoreBoard(player);
	}

	public override bool SelectionFilter(Lane candidate)
	{
		if (candidate.HasCreature())
		{
			return true;
		}
		return false;
	}

	private void ReturnTarget(Lane target)
	{
		CreatureScript creature = target.GetCreature();
		TargetList.Add(creature);
		DoEffect();
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
		CWList<Lane> candidates = AITargetSelection(!base.Owner, SelectionType.Creature);
		CreatureScript creatureScript = CardScript.BestCandidateForAttack(candidates, -1);
		ReturnTarget(creatureScript.CurrentLane);
	}

	public override bool DoResult(CardScript target)
	{
		base.GameInstance.RemoveCardFromPlay(!base.Owner, target.CurrentLane.Index, CardType.Creature);
		if (base.GameInstance.GetCardsInHand(!base.Owner) < 7)
		{
			base.GameInstance.PlaceCardInHand(!base.Owner, target.Data);
		}
		else
		{
			base.GameInstance.DiscardCard(!base.Owner, target.Data);
		}
		return true;
	}
}
