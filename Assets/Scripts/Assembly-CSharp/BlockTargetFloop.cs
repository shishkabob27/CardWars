using System.Collections.Generic;

public class BlockTargetFloop : SpellScript
{
	private static int StoredScore;

	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		bool flag = CardScript.CanPlay(player, lane, card);
		if (flag)
		{
			List<Lane> lanes = GameState.Instance.GetLanes(!player);
			foreach (Lane item in lanes)
			{
				if (item.HasCreature())
				{
					CreatureScript creature = item.GetCreature();
					if (!creature.FloopBlocked && (player == PlayerType.User || creature.CanFloop()))
					{
						return true;
					}
				}
			}
			return false;
		}
		return flag;
	}

	private static CreatureScript SelectTarget(CWList<Lane> Candidates)
	{
		List<CreatureScript> list = CardScript.LanesToCreatures(Candidates);
		int num = 0;
		CreatureScript creatureScript = null;
		foreach (CreatureScript item in list)
		{
			int num2 = GameState.Instance.ScoreBoard();
			if (item.CanFloop())
			{
				num2 = item.EvaluateAbility();
			}
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
		}
		StoredScore = num;
		return creatureScript;
	}

	private static CWList<Lane> AIStaticSelection(PlayerType player)
	{
		CWList<Lane> cWList = new CWList<Lane>();
		GameState instance = GameState.Instance;
		for (int i = 0; i < 4; i++)
		{
			Lane lane = instance.GetLane(player, i);
			if (lane.HasCreature())
			{
				CreatureScript creature = lane.GetCreature();
				if (!creature.FloopBlocked && creature.CanFloop())
				{
					cWList.Add(lane);
				}
			}
		}
		return cWList;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		CWList<Lane> candidates = AIStaticSelection(!player);
		SelectTarget(candidates);
		int num = GameState.Instance.ScoreBoard();
		int num2 = StoredScore - num;
		return num + num2 - item.Form.DetermineCost(player) * 1;
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

	private void ReturnTargetToHand(Lane target)
	{
		CreatureScript creature = target.GetCreature();
		TargetList.Add(creature);
		DoEffect();
	}

	public override void OnTargetSelected(Lane target)
	{
		ReturnTargetToHand(target);
		EndTargetSelection();
	}

	public override void Cast()
	{
		if (base.Owner == PlayerType.User)
		{
			StartTargetSelection(!base.Owner, SelectionType.Creature, KFFLocalization.Get("!!PICK_A_CREATURE"));
			return;
		}
		CWList<Lane> candidates = AITargetSelection(!base.Owner, SelectionType.Creature);
		CreatureScript creatureScript = SelectTarget(candidates);
		ReturnTargetToHand(creatureScript.CurrentLane);
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		creatureScript.FloopBlocked = true;
		return true;
	}
}
