public class ActionOnFloop : BuildingScript
{
	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		int result = GameState.Instance.ScoreBoard() - item.Form.DetermineCost(player) * 1;
		if (lane.HasCreature())
		{
			CreatureScript creature = lane.GetCreature();
			if (creature.CanFloop())
			{
				result = creature.EvaluateAbility();
				result -= item.Form.DetermineCost(player) * 1;
				result += item.Val1 * 1;
			}
		}
		return result;
	}

	public override void OnCreatureFlooped()
	{
		base.GameInstance.AddMagicPoints(base.Owner, base.Data.Form.BaseVal1);
		TriggerEffects();
	}
}
