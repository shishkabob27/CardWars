public class ReturnTargetToHand : CreatureScript
{
	public override bool CanFloop()
	{
		if (base.GameInstance.HasCardTypeInPlay(!base.Owner, (CardType)base.Data.Form.BaseVal1))
		{
			return true;
		}
		return false;
	}

	public override int EvaluateAbility()
	{
		CWList<Lane> candidates = AITargetSelection(!base.Owner, (SelectionType)base.Data.Form.BaseVal1);
		CardScript cardScript = null;
		int num = 0;
		CardScript.ResetMods();
		CardScript.APMods[(int)base.Owner] = -DetermineFloopCost();
		if (base.Data.Form.BaseVal1 == 0)
		{
			cardScript = CardScript.BestCandidateForAttack(candidates, -1);
			CardScript.LaneMods[(int)(!base.Owner), cardScript.CurrentLane.Index].DEF = -((CreatureScript)cardScript).Health;
			num = CardScript.ScoreBoard(base.Owner);
		}
		else
		{
			cardScript = CardScript.BestBuildingSacrifice(candidates);
			((BuildingScript)cardScript).StopTriggerEffects();
			cardScript.OnCardLeftPlay(cardScript);
			num = CardScript.ScoreBoard(base.Owner);
			cardScript.OnCardEnterPlay(cardScript);
			((BuildingScript)cardScript).ResumeTriggerEffects();
		}
		return num;
	}

	public override bool SelectionFilter(Lane candidate)
	{
		if (candidate.HasCard((CardType)base.Data.Form.BaseVal1))
		{
			return true;
		}
		return false;
	}

	private void ReturnToHand(Lane target)
	{
		CardScript script = target.GetScript((CardType)base.Data.Form.BaseVal1);
		TargetList.Add(script);
		DoEffect();
	}

	public override void OnTargetSelected(Lane target)
	{
		ReturnToHand(target);
		EndTargetSelection();
	}

	public override void Floop()
	{
		if (base.Owner == PlayerType.User)
		{
			StartTargetSelection(!base.Owner, (SelectionType)base.Data.Form.BaseVal1, KFFLocalization.Get("!!PICK_A_TARGET"));
			return;
		}
		CWList<Lane> candidates = AITargetSelection(!base.Owner, (SelectionType)base.Data.Form.BaseVal1);
		CreatureScript creatureScript = CardScript.BestCandidateForAttack(candidates, -1);
		ReturnToHand(creatureScript.CurrentLane);
	}

	public override bool DoResult(CardScript target)
	{
		base.GameInstance.RemoveCardFromPlay(target.Owner, target.CurrentLane.Index, target.Data.Form.Type);
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
