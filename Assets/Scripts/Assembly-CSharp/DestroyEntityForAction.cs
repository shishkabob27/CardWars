public class DestroyEntityForAction : SpellScript
{
	private static bool Locked;

	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		if (Locked)
		{
			return false;
		}
		bool flag = CardScript.CanPlay(player, lane, card);
		if (flag)
		{
			int num = GameState.Instance.CardCount(player, (CardType)card.BaseVal1);
			if (num <= 0)
			{
				flag = false;
			}
			if (player == PlayerType.Opponent && flag)
			{
				GameState.Instance.AddMagicPoints(player, 4);
				Locked = true;
				if (!GameState.Instance.HasLegalMove(player))
				{
					flag = false;
				}
				Locked = false;
				GameState.Instance.AddMagicPoints(player, -4);
			}
		}
		return flag;
	}

	private static CWList<Lane> AIStaticSelection(PlayerType player, CardItem item)
	{
		CWList<Lane> cWList = new CWList<Lane>();
		GameState instance = GameState.Instance;
		for (int i = 0; i < 4; i++)
		{
			Lane lane = instance.GetLane(player, i);
			if (lane.HasCard((CardType)item.Form.BaseVal1))
			{
				cWList.Add(lane);
			}
		}
		return cWList;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		CWList<Lane> candidates = AIStaticSelection(player, item);
		CardScript cardScript = null;
		CardScript.ResetMods();
		int num = -item.Form.DetermineCost(player) * 1 + 4;
		if (item.Form.BaseVal1 == 0)
		{
			cardScript = CardScript.BestSacrifice(candidates);
			CardScript.LaneMods[(int)player, cardScript.CurrentLane.Index].DEF = -((CreatureScript)cardScript).Health;
			return CardScript.ScoreBoard(player) + num;
		}
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
		if (candidate.HasCard((CardType)base.Data.Form.BaseVal1))
		{
			return true;
		}
		return false;
	}

	private void SacrificeTarget(Lane target)
	{
		CardScript script = target.GetScript((CardType)base.Data.Form.BaseVal1);
		TargetList.Add(script);
		DoEffect();
		CWFloopActionManager.GetInstance().DoEffect(this, base.Owner);
	}

	public override void OnTargetSelected(Lane target)
	{
		SacrificeTarget(target);
		EndTargetSelection();
	}

	public override void Cast()
	{
		if (base.Owner == PlayerType.User)
		{
			StartTargetSelection(base.Owner, (SelectionType)base.Data.Form.BaseVal1, KFFLocalization.Get("!!PICK_A_SACRIFICE"));
			return;
		}
		CWList<Lane> candidates = AITargetSelection(base.Owner, (SelectionType)base.Data.Form.BaseVal1);
		CardScript cardScript = null;
		cardScript = ((base.Data.Form.BaseVal1 != 0) ? ((CardScript)CardScript.BestBuildingSacrifice(candidates)) : ((CardScript)CardScript.BestSacrifice(candidates)));
		SacrificeTarget(cardScript.CurrentLane);
	}

	public override bool DoResult(CardScript target)
	{
		if (target != null)
		{
			base.GameInstance.RemoveCardFromPlay(base.Owner, target.CurrentLane.Index, (CardType)base.Data.Form.BaseVal1);
			base.GameInstance.DiscardCard(base.Owner, target.Data);
		}
		else
		{
			base.GameInstance.AddMagicPoints(base.Owner, 4);
		}
		return true;
	}
}
