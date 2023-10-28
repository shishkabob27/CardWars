using System;
using UnityEngine;

public class DebugFloopActionCamera : MonoBehaviour
{
	public bool SpawnVolcano;

	public bool SpawnBloodstorm;

	public bool SpawnGeneric;

	public GameObject Volcano;

	public GameObject Bloodstorm;

	public GameObject Generic;

	private void Update()
	{
		if (SpawnVolcano)
		{
			SpawnVolcano = false;
			SpawnVFX(Volcano);
		}
		if (SpawnBloodstorm)
		{
			SpawnBloodstorm = false;
			SpawnVFX(Bloodstorm);
		}
		if (SpawnGeneric)
		{
			SpawnGeneric = false;
			SpawnVFX(Generic);
		}
	}

	private void SpawnVFX(GameObject obj)
	{
		int num = new System.Random().Next(0, 4);
		GameObject gameObject = CWFloopActionManager.GetInstance().PlayerNeutralPoints[num].gameObject;
		if (obj != null)
		{
			UnityEngine.Object.Instantiate(obj, gameObject.transform.position, gameObject.transform.rotation);
		}
	}
}
