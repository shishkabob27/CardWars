using System.Collections.Generic;

public class ATKBonusFactionCreaturesLeader : LeaderScript
{
	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		bool flag = LeaderScript.CanPlay(player, lane, card);
		if (flag)
		{
			LeaderForm form = GameState.Instance.GetLeader(player).Form;
			GameState instance = GameState.Instance;
			Faction? forFaction = form.forFaction;
			int num = instance.CreatureFactionCount(player, forFaction.Value);
			if (num > 0)
			{
				return true;
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
				CardScript.LaneMods[(int)player, creature.CurrentLane.Index].ATK = form.BaseVal1;
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
				TargetList.Add(creature);
			}
		}
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		(target as CreatureScript).ATKMod += base.Leader.Form.BaseVal1;
		return true;
	}
}
