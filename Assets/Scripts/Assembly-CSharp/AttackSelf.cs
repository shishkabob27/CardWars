using System;

public class AttackSelf : SpellScript
{
	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		bool flag = CardScript.CanPlay(player, lane, card);
		if (flag)
		{
			if (GameState.Instance.HasCreaturesInPlay(!player))
			{
				return true;
			}
			return false;
		}
		return flag;
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
				cWList.Add(lane);
			}
		}
		return cWList;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		CWList<Lane> candidates = AIStaticSelection(!player);
		CreatureScript creatureScript = CardScript.BestCandidateForAttack(candidates, -2);
		CardScript.ResetMods();
		CardScript.LaneMods[(int)(!player), creatureScript.CurrentLane.Index].DEF = -Math.Min(creatureScript.Health, creatureScript.ATK);
		CardScript.APMods[(int)player] = -item.Form.DetermineCost(player);
		return CardScript.ScoreBoard(player);
	}

	public override bool SelectionFilter(Lane candidate)
	{
		if (candidate.HasCreature())
		{
			return true;
		}
		return false;
	}

	private void ReturnTarget(Lane target)
	{
		CreatureScript creature = target.GetCreature();
		TargetList.Add(creature);
		DoEffect();
	}

	public override void OnTargetSelected(Lane target)
	{
		ReturnTarget(target);
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
		CreatureScript creatureScript = CardScript.BestCandidateForAttack(candidates, -2);
		ReturnTarget(creatureScript.CurrentLane);
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		creatureScript.TakeDamage(this, creatureScript.ATK);
		return true;
	}
}
