using UnityEngine;

public class CWDisableLaneCollider : MonoBehaviour
{
	private void Start()
	{
	}

	public void leaderAbilityCollidersOn()
	{
		ActivateColliders(true, string.Empty);
	}

	public void leaderAbilityCollidersOff()
	{
		ActivateColliders(false, string.Empty);
	}

	public void P1FlippedCollidersOn()
	{
		ActivateColliders(false, string.Empty);
		ActivateColliders(true, "P1Flipped");
	}

	public void P2CollidersOn()
	{
		ActivateColliders(false, string.Empty);
		ActivateColliders(true, "P2Lane");
	}

	public void P1CollidersOn()
	{
		ActivateColliders(false, string.Empty);
		ActivateColliders(true, "P1Lane");
		ActivateColliders(true, "FLOOPButton");
	}

	public void AllCollidersOff()
	{
		ActivateColliders(false, string.Empty);
	}

	public void CardAndChestCollidersOn()
	{
		ActivateColliders(true, "LootChest");
	}

	private void AllCollidersOn()
	{
		ActivateColliders(true, string.Empty);
	}

	private void AllLaneCollidersOn()
	{
		ActivateColliders(false, string.Empty);
		ActivateColliders(true, "Lane");
	}

	private void AllActiveCreatureColliderOn()
	{
		ActivateColliders(false, string.Empty);
		ActivateCreatureColliders();
	}

	public void ActivateColliders(bool enable, string str)
	{
		BoxCollider[] componentsInChildren = GetComponentsInChildren<BoxCollider>(true);
		BoxCollider[] array = componentsInChildren;
		foreach (BoxCollider boxCollider in array)
		{
			if (boxCollider.name.Contains(str))
			{
				boxCollider.enabled = enable;
			}
		}
	}

	public void ActivateCreatureColliders()
	{
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				for (int k = 0; k < 2; k++)
				{
					CreatureManagerScript instance = CreatureManagerScript.GetInstance();
					BoxCollider component = instance.Spawn_Points[i, j, k].GetComponent<BoxCollider>();
					component.enabled = ((instance.Instances[i, j, k] != null) ? true : false);
				}
			}
		}
		UICamera.useInputEnabler = false;
	}

	private void Update()
	{
	}
}
