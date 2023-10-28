using System;
using System.Collections.Generic;

public class AttackCreaturePlains : SpellScript
{
	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		bool flag = CardScript.CanPlay(player, lane, card);
		if (flag)
		{
			List<Lane> lanes = GameState.Instance.GetLanes(player);
			foreach (Lane item in lanes)
			{
				if (item.HasCreature() && item.GetCreature().Data.Form.Faction == (Faction)card.BaseVal1 && item.OpponentLane.HasCreature())
				{
					return true;
				}
			}
			return false;
		}
		return flag;
	}

	private static CreatureScript BestCreatureToAttack(CWList<Lane> Candidates)
	{
		List<CreatureScript> list = CardScript.LanesToCreatures(Candidates);
		List<CreatureScript> list2 = new List<CreatureScript>();
		List<CreatureScript> list3 = new List<CreatureScript>();
		List<CreatureScript> list4 = new List<CreatureScript>();
		List<CreatureScript> list5 = null;
		foreach (CreatureScript item in list)
		{
			if (item.Enemy == null)
			{
				continue;
			}
			if (item.InDanger && !item.Enemy.InDanger)
			{
				if (item.ATK * 2 >= item.Enemy.Health)
				{
					list2.Add(item);
				}
				else
				{
					list3.Add(item);
				}
			}
			else if (item.Enemy.InDanger)
			{
				list4.Add(item);
			}
		}
		list5 = ((list2.Count > 0) ? list2 : ((list4.Count <= 0) ? list : list4));
		CreatureScript creatureScript = null;
		CreatureScript creatureScript2 = null;
		foreach (CreatureScript item2 in list5)
		{
			if (creatureScript == null)
			{
				creatureScript = item2;
			}
			else if (!list3.Contains(item2) && item2.Enemy.ATK > creatureScript.Enemy.ATK)
			{
				creatureScript = item2;
			}
			if (creatureScript2 == null)
			{
				creatureScript2 = item2;
			}
			else if (creatureScript2.Enemy.ATK < item2.Enemy.ATK)
			{
				creatureScript2 = item2;
			}
		}
		if (creatureScript == null)
		{
			creatureScript = creatureScript2;
		}
		return creatureScript;
	}

	private static CWList<Lane> AIStaticSelection(PlayerType player, CardItem item)
	{
		CWList<Lane> cWList = new CWList<Lane>();
		GameState instance = GameState.Instance;
		for (int i = 0; i < 4; i++)
		{
			Lane lane = instance.GetLane(player, i);
			if (lane.HasCreature() && lane.OpponentLane.HasCreature() && lane.GetCreature().Data.Form.Faction == (Faction)item.Form.BaseVal1)
			{
				cWList.Add(lane);
			}
		}
		return cWList;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		CWList<Lane> candidates = AIStaticSelection(player, item);
		CreatureScript creatureScript = BestCreatureToAttack(candidates);
		CardScript.ResetMods();
		CardScript.LaneMods[(int)player, creatureScript.CurrentLane.OpponentLane.Index].DEF = -Math.Min(creatureScript.Enemy.Health, creatureScript.ATK);
		CardScript.APMods[(int)player] = -item.Form.DetermineCost(player);
		return CardScript.ScoreBoard(player);
	}

	public override bool SelectionFilter(Lane candidate)
	{
		if (candidate.HasCreature() && candidate.GetCreature().Data.Form.Faction == (Faction)base.Data.Form.BaseVal1 && candidate.OpponentLane.HasCreature())
		{
			return true;
		}
		return false;
	}

	private void HealTarget(Lane target)
	{
		CreatureScript creature = target.GetCreature();
		TargetList.Add(creature.CurrentLane.OpponentLane.GetCreature());
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
		CreatureScript creatureScript = BestCreatureToAttack(candidates);
		HealTarget(creatureScript.CurrentLane);
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		CreatureScript creature = target.CurrentLane.OpponentLane.GetCreature();
		creatureScript.TakeDamage(creature, creature.ATK);
		return true;
	}
}
