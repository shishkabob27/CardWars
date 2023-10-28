using System.Collections.Generic;

public class HealAllFactionCreaturesLeader : LeaderScript
{
	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		bool flag = LeaderScript.CanPlay(player, lane, card);
		if (flag)
		{
			List<Lane> lanes = GameState.Instance.GetLanes(player);
			LeaderForm form = GameState.Instance.GetLeader(player).Form;
			foreach (Lane item in lanes)
			{
				if (item.HasCreature() && item.GetCreature().Data.Form.Faction == form.forFaction)
				{
					CreatureScript creature = item.GetCreature();
					if (creature.Damage > 0)
					{
						return true;
					}
				}
			}
			return false;
		}
		return flag;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		List<Lane> lanes = GameState.Instance.GetLanes(player);
		LeaderForm form = GameState.Instance.GetLeader(player).Form;
		CardScript.ResetMods();
		foreach (Lane item2 in lanes)
		{
			if (item2.HasCreature() && item2.GetCreature().Data.Form.Faction == form.forFaction)
			{
				CreatureScript creature = item2.GetCreature();
				CardScript.LaneMods[(int)player, creature.CurrentLane.Index].DEF = creature.Damage;
			}
		}
		return CardScript.ScoreBoard(player);
	}

	public override void Cast()
	{
		List<Lane> lanes = base.GameInstance.GetLanes(base.Owner);
		foreach (Lane item in lanes)
		{
			if (item.HasCreature() && item.GetCreature().Data.Form.Faction == base.Leader.Form.forFaction)
			{
				CreatureScript creature = item.GetCreature();
				if (creature.Damage > 0)
				{
					TargetList.Add(creature);
				}
			}
		}
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		creatureScript.Heal(creatureScript.Damage);
		return true;
	}
}
