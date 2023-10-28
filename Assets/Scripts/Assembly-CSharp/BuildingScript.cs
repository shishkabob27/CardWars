using UnityEngine;

public class BuildingScript : CardScript
{
	protected bool trigger = true;

	public virtual void OnCreatureFlooped()
	{
	}

	public virtual void OnCreatureWon()
	{
	}

	public override void OnCreatureDied(CardItem creature)
	{
	}

	public void StopTriggerEffects()
	{
		trigger = false;
	}

	public void ResumeTriggerEffects()
	{
		trigger = true;
	}

	protected void ModifyATK(CreatureScript Script, int value)
	{
		Script.ATKMod += value;
	}

	protected void ModifyDEF(CreatureScript Script, int value)
	{
		Script.DEFMod += value;
	}

	protected void TriggerEffects()
	{
		if (trigger)
		{
			GameObject gameObject = CreatureManagerScript.GetInstance().Spawn_Points[(int)base.Owner, base.CurrentLane.Index, 1].gameObject;
			CWTriggerBuildingAbility componentInChildren = gameObject.GetComponentInChildren<CWTriggerBuildingAbility>();
			if (componentInChildren != null)
			{
				componentInChildren.TriggerBuildingAbility();
			}
		}
	}
}
