public class DestroyOpponentBuilding : CreatureScript
{
	public override bool CanFloop()
	{
		if (base.CurrentLane.OpponentLane.HasBuilding())
		{
			return true;
		}
		return false;
	}

	public override int EvaluateAbility()
	{
		BuildingScript building = base.CurrentLane.OpponentLane.GetBuilding();
		building.StopTriggerEffects();
		building.OnCardLeftPlay(building);
		building.ResumeTriggerEffects();
		int result = base.GameInstance.ScoreBoard();
		building.StopTriggerEffects();
		building.OnCardEnterPlay(building);
		building.ResumeTriggerEffects();
		return result;
	}

	public override void Floop()
	{
		BuildingScript building = base.CurrentLane.OpponentLane.GetBuilding();
		TargetList.Add(building);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		base.GameInstance.RemoveCardFromPlay(target.Owner, target.CurrentLane.Index, target.Data.Form.Type);
		base.GameInstance.DiscardCard(target.Owner, target.Data);
		return true;
	}
}
