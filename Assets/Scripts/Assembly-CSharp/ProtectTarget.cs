using System.Collections.Generic;

public class ProtectTarget : CreatureScript
{
	public override bool CanFloop()
	{
		int num = base.GameInstance.CreatureCount(base.Owner);
		if (num > 0)
		{
			return true;
		}
		return false;
	}

	private CreatureScript SelectTarget()
	{
		CWList<Lane> cWList = AITargetSelection(base.Owner, SelectionType.Creature);
		List<CreatureScript> list = CardScript.LanesToCreatures(cWList);
		List<CreatureScript> creaturesInDanger = base.GameInstance.GetCreaturesInDanger(base.Owner);
		CreatureScript creatureScript = null;
		CreatureScript creatureScript2 = null;
		foreach (CreatureScript item in list)
		{
			if (creaturesInDanger.Contains(item) && (creatureScript == null || item.ATK > creatureScript.ATK))
			{
				creatureScript = item;
			}
			if (item.Enemy != null && (creatureScript2 == null || creatureScript2.Enemy.ATK < item.Enemy.ATK))
			{
				creatureScript2 = item;
			}
		}
		if (creatureScript == null)
		{
			creatureScript = creatureScript2;
		}
		if (creatureScript == null)
		{
			creatureScript = cWList.RandomItem().GetCreature();
		}
		return creatureScript;
	}

	public override void PopulateLaneMods()
	{
		CreatureScript creatureScript = SelectTarget();
		if (creatureScript.Enemy != null)
		{
			CardScript.LaneMods[(int)base.Owner, creatureScript.CurrentLane.Index].DEF = creatureScript.Enemy.ATK;
		}
	}

	public override bool SelectionFilter(Lane candidate)
	{
		if (candidate.HasCreature())
		{
			return true;
		}
		return false;
	}

	private void Protect(Lane target)
	{
		CreatureScript creature = target.GetCreature();
		TargetList.Add(creature);
		DoEffect();
	}

	public override bool DoResult(CardScript target)
	{
		CreatureScript creatureScript = target as CreatureScript;
		creatureScript.Protected = true;
		return true;
	}

	public override void OnTargetSelected(Lane target)
	{
		Protect(target);
		EndTargetSelection();
	}

	public override void Floop()
	{
		if (base.Owner == PlayerType.User)
		{
			StartTargetSelection(base.Owner, SelectionType.Creature, KFFLocalization.Get("!!TAP_CREATURE_TO_PROTECT"));
			return;
		}
		CreatureScript creatureScript = SelectTarget();
		Lane currentLane = creatureScript.CurrentLane;
		Protect(currentLane);
	}
}
