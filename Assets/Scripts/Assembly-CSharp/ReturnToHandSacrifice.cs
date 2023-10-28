public class ReturnToHandSacrifice : BuildingScript
{
	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		int result = GameState.Instance.ScoreBoard() - item.Form.DetermineCost(player) * 1;
		if (lane.HasCreature())
		{
			CreatureScript creature = lane.GetCreature();
			result = creature.Data.EvaluateLanePlacement(player, lane);
		}
		return result;
	}

	public override void OnCreatureDied(CardItem creature)
	{
		base.GameInstance.PlaceCardInHand(base.Owner, creature);
		base.GameInstance.RemoveCardFromDiscardPile(base.Owner, creature);
		TriggerEffects();
		base.GameInstance.DiscardCard(base.Owner, base.Data);
		base.GameInstance.RemoveCardFromPlay(base.Owner, base.CurrentLane.Index, CardType.Building);
	}
}
