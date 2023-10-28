using System.Collections.Generic;

public class DestroyHighRarityTarget : SpellScript
{
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
					if (creature.Data.Form.Rarity >= card.BaseVal1)
					{
						return true;
					}
				}
			}
			return false;
		}
		return flag;
	}

	private static CWList<Lane> AIStaticSelection(PlayerType player, CardItem item)
	{
		CWList<Lane> cWList = new CWList<Lane>();
		GameState instance = GameState.Instance;
		for (int i = 0; i < 4; i++)
		{
			Lane lane = instance.GetLane(player, i);
			if (lane.HasCreature() && lane.GetCreature().Data.Form.Rarity >= item.Form.BaseVal1)
			{
				cWList.Add(lane);
			}
		}
		return cWList;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		CWList<Lane> candidates = AIStaticSelection(!player, item);
		CreatureScript creatureScript = CardScript.BestCandidateForAttack(candidates, -1);
		CardScript.ResetMods();
		CardScript.LaneMods[(int)(!player), creatureScript.CurrentLane.Index].DEF = -creatureScript.Health;
		CardScript.APMods[(int)player] = -item.Form.DetermineCost(player);
		return CardScript.ScoreBoard(player);
	}

	public override bool SelectionFilter(Lane candidate)
	{
		if (candidate.HasCreature() && candidate.GetCreature().Data.Form.Rarity >= base.Data.Form.BaseVal1)
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
		CreatureScript creatureScript = CardScript.BestCandidateForAttack(candidates, -1);
		ReturnTarget(creatureScript.CurrentLane);
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		creatureScript.TakeDamage(this, creatureScript.Health);
		return true;
	}
}
