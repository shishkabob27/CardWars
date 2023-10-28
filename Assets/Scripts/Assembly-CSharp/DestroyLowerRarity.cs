using System.Collections.Generic;

public class DestroyLowerRarity : SpellScript
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
					if (creature.Data.Form.Rarity <= card.BaseVal1)
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
		List<Lane> lanes = GameState.Instance.GetLanes(!player);
		int num = GameState.Instance.ScoreBoard() - item.Form.DetermineCost(player) * 1;
		CardScript.ResetMods();
		foreach (Lane item2 in lanes)
		{
			if (item2.HasCreature())
			{
				CreatureScript creature = item2.GetCreature();
				if (creature.Data.Form.Rarity <= item.Form.BaseVal1)
				{
					CardScript.LaneMods[(int)(!player), item2.Index].DEF = -creature.Health;
				}
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
				if (creature.Data.Form.Rarity <= base.Data.Form.BaseVal1)
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
		creatureScript.TakeDamage(this, creatureScript.Health);
		return true;
	}
}
