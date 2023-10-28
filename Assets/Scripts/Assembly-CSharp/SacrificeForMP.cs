public class SacrificeForMP : CreatureScript
{
	private bool Locked;

	public override bool CanFloop()
	{
		bool flag = true;
		if (Locked)
		{
			return false;
		}
		if (base.Owner == PlayerType.Opponent && flag)
		{
			int baseVal = base.Data.Form.BaseVal1;
			GameState.Instance.AddMagicPoints(base.Owner, baseVal);
			Locked = true;
			if (!GameState.Instance.HasLegalMove(base.Owner))
			{
				flag = false;
			}
			Locked = false;
			GameState.Instance.AddMagicPoints(base.Owner, -baseVal);
		}
		return flag;
	}

	public override int EvaluateAbility()
	{
		CardScript.ResetMods();
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].DEF = -base.Health;
		CardScript.APMods[(int)base.Owner] = base.Data.Form.BaseVal1 - DetermineFloopCost();
		return CardScript.ScoreBoard(base.Owner);
	}

	public override void Floop()
	{
		TargetList.Add(this);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		base.GameInstance.AddMagicPoints(base.Owner, base.Data.Form.BaseVal1);
		base.GameInstance.RemoveCardFromPlay(target.Owner, target.CurrentLane.Index, CardType.Creature);
		base.GameInstance.DiscardCard(target.Owner, target.Data);
		return true;
	}
}
