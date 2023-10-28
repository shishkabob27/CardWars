using System.Collections.Generic;

public class KillLoneOpponent : SpellScript
{
	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		bool flag = CardScript.CanPlay(player, lane, card);
		if (flag)
		{
			List<Lane> lanes = GameState.Instance.GetLanes(!player);
			int num = 0;
			foreach (Lane item in lanes)
			{
				if (item.HasCreature())
				{
					num++;
				}
			}
			if (num == 1)
			{
				return true;
			}
			return false;
		}
		return flag;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		List<Lane> lanes = GameState.Instance.GetLanes(!player);
		int num = GameState.Instance.ScoreBoard() - item.Form.DetermineCost(player) * 1;
		CardScript.ResetMods();
		foreach (Lane item2 in lanes)
		{
			if (item2.HasCreature())
			{
				CreatureScript creature = item2.GetCreature();
				CardScript.LaneMods[(int)(!player), creature.CurrentLane.Index].DEF = -item2.GetCreature().Health;
			}
		}
		CardScript.APMods[(int)player] = -item.Form.DetermineCost(player);
		return CardScript.ScoreBoard(player);
	}

	public override void Cast()
	{
		List<Lane> lanes = GameState.Instance.GetLanes(!base.Owner);
		foreach (Lane item in lanes)
		{
			if (item.HasCreature())
			{
				CreatureScript creature = item.GetCreature();
				TargetList.Add(creature);
			}
		}
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		creatureScript.TakeDamage(this, creatureScript.Health);
		return true;
	}
}
