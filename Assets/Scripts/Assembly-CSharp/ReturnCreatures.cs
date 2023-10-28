using System.Collections.Generic;

public class ReturnCreatures : SpellScript
{
	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		bool flag = CardScript.CanPlay(player, lane, card);
		if (flag)
		{
			if (GameState.Instance.CreatureCount(player) > 0)
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
		int num = 0;
		CardScript.ResetMods();
		foreach (Lane item2 in lanes)
		{
			if (item2.HasCreature())
			{
				CreatureScript creature = item2.GetCreature();
				CardScript.LaneMods[(int)player, creature.CurrentLane.Index].DEF = -creature.Health;
				num++;
			}
		}
		CardScript.APMods[(int)player] = -item.Form.DetermineCost(player);
		return CardScript.ScoreBoard(player) + num * 3;
	}

	public override void Cast()
	{
		List<Lane> lanes = base.GameInstance.GetLanes(base.Owner);
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
		base.GameInstance.RemoveCardFromPlay(base.Owner, target.CurrentLane.Index, CardType.Creature);
		if (base.GameInstance.GetCardsInHand(base.Owner) < 7)
		{
			base.GameInstance.PlaceCardInHand(base.Owner, target.Data);
		}
		else
		{
			base.GameInstance.DiscardCard(base.Owner, target.Data);
		}
		return true;
	}
}
