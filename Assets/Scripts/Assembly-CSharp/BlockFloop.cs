public class BlockFloop : CreatureScript
{
	public override bool CanFloop()
	{
		if (base.CurrentLane.OpponentLane.HasCreature())
		{
			CreatureScript creature = base.CurrentLane.OpponentLane.GetCreature();
			if (!creature.FloopBlocked)
			{
				return true;
			}
		}
		return false;
	}

	public override int EvaluateAbility()
	{
		if (base.Enemy is BlockFloop)
		{
			return int.MinValue;
		}
		int num = base.GameInstance.ScoreBoard();
		int num2 = 0;
		if (base.Enemy.CanFloop())
		{
			int num3 = base.Enemy.EvaluateAbility();
			num2 = num3 - num;
		}
		return num + num2;
	}

	public override void Floop()
	{
		CreatureScript creature = base.CurrentLane.OpponentLane.GetCreature();
		TargetList.Add(creature);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		creatureScript.FloopBlocked = true;
		return true;
	}
}
