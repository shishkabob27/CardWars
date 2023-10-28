public class DestroyEntityForCards : SpellScript
{
	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		bool flag = CardScript.CanPlay(player, lane, card);
		if (flag)
		{
			if (GameState.Instance.CardCount(player, (CardType)card.BaseVal1) > 0 && GameState.Instance.GetDeck(player).CardCount() > 0)
			{
				return true;
			}
			return false;
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
		int num = -item.Form.DetermineCost(player) * 1 + item.Form.BaseVal2 * 3;
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
		base.GameInstance.RemoveCardFromPlay(base.Owner, target.CurrentLane.Index, (CardType)base.Data.Form.BaseVal1);
		base.GameInstance.DiscardCard(base.Owner, target.Data);
		for (int i = 0; i < base.Data.Form.BaseVal2; i++)
		{
			base.GameInstance.DrawCard(base.Owner);
		}
		return true;
	}
}
