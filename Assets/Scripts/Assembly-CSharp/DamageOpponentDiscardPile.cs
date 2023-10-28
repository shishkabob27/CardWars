using System;

public class DamageOpponentDiscardPile : CreatureScript
{
	public override bool CanFloop()
	{
		int count = base.GameInstance.GetDiscardPile(base.Owner).Count;
		if (count >= base.Data.Form.BaseVal2 && base.CurrentLane.OpponentLane.HasCreature())
		{
			return true;
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		int num = base.GameInstance.GetDiscardPile(base.Owner).Count / base.Data.Form.BaseVal2;
		CreatureScript creature = base.CurrentLane.OpponentLane.GetCreature();
		CardScript.LaneMods[(int)(!base.Owner), creature.CurrentLane.Index].DEF = -Math.Min(creature.Health, base.Data.Val1 * num);
	}

	public override void Floop()
	{
		CreatureScript creature = base.CurrentLane.OpponentLane.GetCreature();
		TargetList.Add(creature);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		int num = base.GameInstance.GetDiscardPile(base.Owner).Count / base.Data.Form.BaseVal2;
		creatureScript.TakeDamage(this, base.Data.Val1 * num);
		return true;
	}
}
