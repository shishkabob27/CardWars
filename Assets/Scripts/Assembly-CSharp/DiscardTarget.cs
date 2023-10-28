public class DiscardTarget : SpellScript
{
	public override void Cast()
	{
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				Lane lane = GameState.Instance.GetLane(base.Owner, j);
				if (lane.HasCard((CardType)i))
				{
					CardScript script = lane.GetScript((CardType)i);
					TargetList.Add(script);
				}
			}
		}
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		base.GameInstance.RemoveCardFromPlay(target.Owner, target.CurrentLane.Index, target.Data.Form.Type);
		base.GameInstance.DiscardCard(target.Owner, target.Data);
		return true;
	}
}
