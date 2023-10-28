using System;

public class ConvertOpponentAttToHP : CreatureScript
{
	public override bool CanFloop()
	{
		return base.CurrentLane.OpponentLane.HasCreature();
	}

	public override void PopulateLaneMods()
	{
		CreatureScript creature = base.CurrentLane.OpponentLane.GetCreature();
		CardScript.LaneMods[(int)base.Owner, base.CurrentLane.Index].DEF = Math.Min(base.Damage, creature.ATK);
	}

	public override void Floop()
	{
		TargetList.Add(this);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		creatureScript.Heal(target.CurrentLane.OpponentLane.GetCreature().ATK);
		return true;
	}
}
