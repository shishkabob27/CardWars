using UnityEngine;

public class CWTriggerBuildingAbility : MonoBehaviour
{
	public GameObject abilityFX;

	public AudioClip abilitySound;

	private void OnEnable()
	{
	}

	public void TriggerBuildingAbility()
	{
		if (abilityFX != null)
		{
			SpawnObject(abilityFX, base.transform.parent);
		}
		Animation component = GetComponent<Animation>();
		if (component.GetClip("Floop") != null)
		{
			component.Play("Floop");
		}
		if (abilitySound != null)
		{
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayOneShot(abilitySound);
		}
	}

	private void SpawnObject(GameObject prefab, Transform parentTr)
	{
		GameObject gameObject = null;
		gameObject = SLOTGame.InstantiateFX(prefab, parentTr.position, parentTr.rotation) as GameObject;
		gameObject.transform.parent = parentTr;
	}

	private void Update()
	{
	}
}
