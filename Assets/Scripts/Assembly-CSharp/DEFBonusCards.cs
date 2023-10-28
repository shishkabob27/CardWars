public class DEFBonusCards : BuildingScript
{
	private int prevCount;

	private bool Initialized;

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		int result = GameState.Instance.ScoreBoard() - item.Form.DetermineCost(player) * 1;
		if (lane.HasCreature())
		{
			int cardsInHand = GameState.Instance.GetCardsInHand(player);
			CardScript.ResetMods();
			CardScript.LaneMods[(int)player, lane.Index].DEF = item.Val1 * cardsInHand;
			CardScript.APMods[(int)player] = -item.Form.DetermineCost(player);
			result = CardScript.ScoreBoard(player);
		}
		return result;
	}

	public override void OnCardEnterPlay(CardScript script)
	{
		int cardsInHand = base.GameInstance.GetCardsInHand(base.Owner);
		if (script == this)
		{
			if (base.CurrentLane.HasCreature())
			{
				CreatureScript creature = base.CurrentLane.GetCreature();
				ModifyDEF(creature, base.Data.Val1 * cardsInHand);
				TriggerEffects();
			}
			Initialized = true;
		}
		else if (script.Data.Form.Type == CardType.Creature && script.CurrentLane == base.CurrentLane)
		{
			CreatureScript script2 = script as CreatureScript;
			ModifyDEF(script2, base.Data.Val1 * cardsInHand);
			TriggerEffects();
		}
		prevCount = cardsInHand;
	}

	public override void Update()
	{
		base.Update();
		if (!Initialized)
		{
			return;
		}
		int cardsInHand = base.GameInstance.GetCardsInHand(base.Owner);
		if (cardsInHand != prevCount)
		{
			int num = cardsInHand - prevCount;
			if (base.CurrentLane.HasCreature())
			{
				CreatureScript creature = base.CurrentLane.GetCreature();
				ModifyDEF(creature, base.Data.Val1 * num);
				TriggerEffects();
			}
			prevCount = cardsInHand;
		}
	}

	public override void OnCardLeftPlay(CardScript script)
	{
		int cardsInHand = base.GameInstance.GetCardsInHand(base.Owner);
		if (script == this && base.CurrentLane.HasCreature())
		{
			CreatureScript creature = base.CurrentLane.GetCreature();
			ModifyDEF(creature, -base.Data.Val1 * cardsInHand);
		}
	}
}
