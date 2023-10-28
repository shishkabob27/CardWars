using System.Collections.Generic;

public class SwapATKDEFOpponent : SpellScript
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

	private static CreatureScript BestCandidateForSwap(CWList<Lane> Candidates, PlayerType player)
	{
		List<CreatureScript> list = CardScript.LanesToCreatures(Candidates);
		List<CreatureScript> list2 = new List<CreatureScript>();
		List<CreatureScript> list3 = new List<CreatureScript>();
		List<CreatureScript> list4 = new List<CreatureScript>();
		List<CreatureScript> list5 = new List<CreatureScript>();
		List<CreatureScript> list6 = new List<CreatureScript>();
		List<CreatureScript> list7 = null;
		foreach (CreatureScript item in list)
		{
			if (item.Enemy == null)
			{
				if (item.CanWin)
				{
					if (item.DEF < GameState.Instance.GetHealth(player))
					{
						list2.Add(item);
					}
				}
				else if (item.DEF < item.ATK)
				{
					list3.Add(item);
				}
			}
			else if (item.Enemy.InDanger)
			{
				if (item.DEF < item.Enemy.Health)
				{
					list4.Add(item);
				}
				else
				{
					list6.Add(item);
				}
			}
			else if (item.ATK < item.Enemy.ATK)
			{
				list5.Add(item);
			}
		}
		list7 = ((list2.Count > 0) ? list2 : ((list3.Count > 0) ? list3 : ((list4.Count > 0) ? list4 : ((list5.Count <= 0) ? list : list5))));
		CreatureScript creatureScript = null;
		CreatureScript creatureScript2 = null;
		foreach (CreatureScript item2 in list7)
		{
			if (!list6.Contains(item2))
			{
				if (creatureScript == null)
				{
					creatureScript = item2;
				}
				else if (item2.DEF < creatureScript.DEF)
				{
					creatureScript = item2;
				}
			}
			if (creatureScript2 == null)
			{
				creatureScript2 = item2;
			}
			if (item2.DEF < creatureScript2.DEF)
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
		CreatureScript creatureScript = BestCandidateForSwap(candidates, player);
		CardScript.ResetMods();
		CardScript.LaneMods[(int)(!player), creatureScript.CurrentLane.Index].ATK = creatureScript.DEF - creatureScript.ATK;
		CardScript.LaneMods[(int)(!player), creatureScript.CurrentLane.Index].DEF = creatureScript.ATK - creatureScript.DEF;
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
		CreatureScript creatureScript = BestCandidateForSwap(candidates, base.Owner);
		ReturnTarget(creatureScript.CurrentLane);
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		int aTK = target.Data.ATK;
		int dEF = target.Data.DEF;
		int aTK2 = creatureScript.ATK;
		int aTKMod = creatureScript.DEF - aTK;
		creatureScript.ATKMod = aTKMod;
		aTKMod = aTK2 - dEF;
		creatureScript.DEFMod = aTKMod;
		return true;
	}
}
