public class ATKOnFloop : BuildingScript
{
	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		int num = GameState.Instance.ScoreBoard() - item.Form.DetermineCost(player) * 1;
		if (lane.HasCreature())
		{
			CreatureScript creature = lane.GetCreature();
			if (creature.CanFloop())
			{
				num = creature.EvaluateAbility();
				int num2 = creature.ATK * 2;
				int num3 = (creature.ATK + item.Val1) * 2;
				num = num - num2 + num3;
			}
			num -= item.Form.DetermineCost(player) * 1;
		}
		return num;
	}

	public override void OnCreatureFlooped()
	{
		CreatureScript creature = base.CurrentLane.GetCreature();
		ModifyATK(creature, base.Data.Val1);
		TriggerEffects();
	}
}
