using System;

public class DrainOpponentHealth : SpellScript
{
	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		bool flag = CardScript.CanPlay(player, lane, card);
		if (flag)
		{
			return true;
		}
		return flag;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		GameState instance = GameState.Instance;
		CardScript.HealthMods[(int)player] = Math.Min(instance.GetMaxHealth(player) - instance.GetHealth(player), item.Form.BaseVal1);
		CardScript.HealthMods[(int)(!player)] = -Math.Min(instance.GetHealth(!player), item.Form.BaseVal1);
		return CardScript.ScoreBoard(player);
	}

	public override void Cast()
	{
		CWFloopActionManager.GetInstance().DoEffect(this, base.Owner);
	}

	public override bool DoResult(CardScript target)
	{
		base.GameInstance.DealDamage(base.Owner, -base.Data.Val1);
		base.GameInstance.DealDamage(!base.Owner, base.Data.Val1);
		return true;
	}
}
