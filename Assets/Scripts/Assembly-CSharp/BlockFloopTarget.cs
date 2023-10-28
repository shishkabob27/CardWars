using System.Collections.Generic;

public class BlockFloopTarget : CreatureScript
{
	private int StoredScore;

	public override bool CanFloop()
	{
		List<Lane> lanes = GameState.Instance.GetLanes(!base.Owner);
		foreach (Lane item in lanes)
		{
			if (item.HasCreature())
			{
				CreatureScript creature = item.GetCreature();
				if (!creature.FloopBlocked && (base.Owner == PlayerType.User || creature.CanFloop()))
				{
					return true;
				}
			}
		}
		return false;
	}

	private CreatureScript SelectTarget()
	{
		CWList<Lane> lanes = AITargetSelection(!base.Owner, SelectionType.Creature);
		List<CreatureScript> list = CardScript.LanesToCreatures(lanes);
		int num = int.MinValue;
		CreatureScript creatureScript = null;
		foreach (CreatureScript item in list)
		{
			if (item.FloopBlocked)
			{
				continue;
			}
			if (item is BlockFloopTarget || (item is BlockFloop && item.Enemy is BlockFloopTarget))
			{
				int num2 = base.GameInstance.ScoreBoard();
				if (creatureScript == null)
				{
					creatureScript = item;
					num = num2;
				}
				else if (num2 > num)
				{
					creatureScript = item;
					num = num2;
				}
				continue;
			}
			int num3 = base.GameInstance.ScoreBoard();
			if (item.CanFloop())
			{
				num3 = item.EvaluateAbility();
			}
			if (creatureScript == null)
			{
				creatureScript = item;
				num = num3;
			}
			else if (num3 > num)
			{
				creatureScript = item;
				num = num3;
			}
		}
		StoredScore = num;
		return creatureScript;
	}

	public override int EvaluateAbility()
	{
		SelectTarget();
		int num = base.GameInstance.ScoreBoard();
		int num2 = StoredScore - num;
		return num + num2;
	}

	public override bool SelectionFilter(Lane candidate)
	{
		if (candidate.HasCreature())
		{
			CreatureScript creature = candidate.GetCreature();
			if (!creature.FloopBlocked)
			{
				return true;
			}
		}
		return false;
	}

	private void Block(Lane target)
	{
		CreatureScript creature = target.GetCreature();
		TargetList.Add(creature);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		creatureScript.FloopBlocked = true;
		return true;
	}

	public override void OnTargetSelected(Lane target)
	{
		Block(target);
		EndTargetSelection();
	}

	public override void Floop()
	{
		if (base.Owner == PlayerType.User)
		{
			StartTargetSelection(!base.Owner, SelectionType.Creature, KFFLocalization.Get("!!TAP_CREATURE_TO_BLOCK_FLOOP"));
			return;
		}
		CreatureScript creatureScript = SelectTarget();
		Block(creatureScript.CurrentLane);
	}
}
