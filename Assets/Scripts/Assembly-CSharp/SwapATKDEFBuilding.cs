using System.Collections.Generic;

public class SwapATKDEFBuilding : BuildingScript
{
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
				if (!item.CanWin)
				{
					if (item.DEF >= GameState.Instance.GetHealth(!player))
					{
						list2.Add(item);
					}
					else if (item.DEF > item.ATK)
					{
						list3.Add(item);
					}
				}
			}
			else if (item.InDanger && !item.Enemy.InDanger)
			{
				if (item.DEF >= item.Enemy.Health || item.ATK - item.Damage > item.Enemy.ATK)
				{
					list4.Add(item);
				}
				else
				{
					list6.Add(item);
				}
			}
			else if (!item.Enemy.InDanger && item.DEF >= item.Enemy.Health)
			{
				list5.Add(item);
			}
		}
		list7 = ((list2.Count > 0) ? list2 : ((list4.Count > 0) ? list4 : ((list5.Count > 0) ? list5 : ((list3.Count <= 0) ? list : list3))));
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
				else if (item2.DEF > creatureScript.DEF)
				{
					creatureScript = item2;
				}
			}
			if (creatureScript2 == null)
			{
				creatureScript2 = item2;
			}
			if (item2.DEF > creatureScript2.DEF)
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
		CWList<Lane> candidates = AIStaticSelection(player);
		CreatureScript creatureScript = BestCandidateForSwap(candidates, player);
		CardScript.ResetMods();
		if (creatureScript != null)
		{
			CardScript.LaneMods[(int)player, creatureScript.CurrentLane.Index].ATK = creatureScript.DEF - creatureScript.ATK;
			CardScript.LaneMods[(int)player, creatureScript.CurrentLane.Index].DEF = creatureScript.ATK - creatureScript.DEF;
		}
		CardScript.APMods[(int)player] = -item.Form.DetermineCost(player);
		return CardScript.ScoreBoard(player);
	}

	public override void OnCardEnterPlay(CardScript script)
	{
		if (script == this)
		{
			if (base.CurrentLane.HasCreature())
			{
				CreatureScript creature = base.CurrentLane.GetCreature();
				int baseATK = creature.Data.Form.BaseATK;
				int baseDEF = creature.Data.Form.BaseDEF;
				int aTK = creature.ATK;
				int aTKMod = creature.DEF - baseATK;
				creature.ATKMod = aTKMod;
				aTKMod = aTK - baseDEF;
				creature.DEFMod = aTKMod;
				TriggerEffects();
			}
		}
		else if (script.Data.Form.Type == CardType.Creature && script.CurrentLane == base.CurrentLane && script.Owner == base.Owner)
		{
			CreatureScript creature2 = base.CurrentLane.GetCreature();
			int baseATK2 = creature2.Data.Form.BaseATK;
			int baseDEF2 = creature2.Data.Form.BaseDEF;
			int aTK2 = creature2.ATK;
			int aTKMod2 = creature2.DEF - baseATK2;
			creature2.ATKMod = aTKMod2;
			aTKMod2 = aTK2 - baseDEF2;
			creature2.DEFMod = aTKMod2;
			TriggerEffects();
		}
	}

	public override void OnCardLeftPlay(CardScript script)
	{
		if (script == this && base.CurrentLane.HasCreature())
		{
			CreatureScript creature = base.CurrentLane.GetCreature();
			int baseATK = script.Data.Form.BaseATK;
			int baseDEF = script.Data.Form.BaseDEF;
			int aTK = creature.ATK;
			int aTKMod = creature.DEF - baseATK;
			creature.ATKMod = aTKMod;
			aTKMod = aTK - baseDEF;
			creature.DEFMod = aTKMod;
		}
	}
}
