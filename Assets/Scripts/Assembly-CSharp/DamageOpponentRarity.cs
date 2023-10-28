using System;

public class DamageOpponentRarity : CreatureScript
{
	public override bool CanFloop()
	{
		if (base.CurrentLane.OpponentLane.HasCreature())
		{
			return true;
		}
		return false;
	}

	public override void PopulateLaneMods()
	{
		CreatureScript creature = base.CurrentLane.OpponentLane.GetCreature();
		int rarity = creature.Data.Form.Rarity;
		CardScript.LaneMods[(int)(!base.Owner), creature.CurrentLane.Index].DEF = -Math.Min(creature.Health, base.Data.Val1 * rarity);
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
		int rarity = creatureScript.Data.Form.Rarity;
		creatureScript.TakeDamage(this, base.Data.Val1 * rarity);
		return true;
	}
}
