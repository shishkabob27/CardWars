using System.Collections.Generic;

public class HealCreatureLeader : LeaderScript
{
	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		bool flag = LeaderScript.CanPlay(player, lane, card);
		if (flag)
		{
			List<Lane> lanes = GameState.Instance.GetLanes(player);
			foreach (Lane item in lanes)
			{
				if (item.HasCreature() && item.GetCreature().Damage > 0)
				{
					return true;
				}
			}
			return false;
		}
		return flag;
	}

	private static CWList<Lane> AIStaticSelection(PlayerType player, SelectionType targetType)
	{
		CWList<Lane> cWList = new CWList<Lane>();
		GameState instance = GameState.Instance;
		for (int i = 0; i < 4; i++)
		{
			Lane lane = instance.GetLane(player, i);
			if (lane.HasCreature() && lane.GetCreature().Damage > 0)
			{
				cWList.Add(lane);
			}
		}
		return cWList;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		CWList<Lane> candidates = AIStaticSelection(player, SelectionType.Creature);
		CreatureScript creatureScript = CardScript.BestCandidateForHealing(candidates, -1, true);
		CardScript.ResetMods();
		CardScript.LaneMods[(int)player, creatureScript.CurrentLane.Index].DEF = creatureScript.Damage;
		return CardScript.ScoreBoard(player);
	}

	public override bool SelectionFilter(Lane candidate)
	{
		if (candidate.HasCreature() && candidate.GetCreature().Damage > 0)
		{
			return true;
		}
		return false;
	}

	private void HealTarget(Lane target)
	{
		CreatureScript creature = target.GetCreature();
		TargetList.Add(creature);
		DoEffect();
	}

	public override void OnTargetSelected(Lane target)
	{
		HealTarget(target);
		EndTargetSelection();
	}

	public override void Cast()
	{
		if (base.Owner == PlayerType.User)
		{
			StartTargetSelection(base.Owner, SelectionType.Creature, KFFLocalization.Get("!!PICK_A_CREATURE"));
			return;
		}
		CWList<Lane> candidates = AITargetSelection(base.Owner, SelectionType.Creature);
		CreatureScript creatureScript = CardScript.BestCandidateForHealing(candidates, -1, true);
		HealTarget(creatureScript.CurrentLane);
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		creatureScript.Heal(creatureScript.Damage);
		return true;
	}
}
