using UnityEngine;

public class SpawnVFX : MonoBehaviour
{
	public GameObject SpawnEffect;

	public float Scale = 2f;

	public bool Disable;

	private void OnEnable()
	{
		SpawnFX();
	}

	public void SpawnFX()
	{
		if (SpawnEffect != null && !Disable)
		{
			GameObject gameObject = (GameObject)SLOTGame.InstantiateFX(SpawnEffect);
			if (gameObject != null)
			{
				gameObject.transform.position = base.gameObject.transform.position;
				gameObject.transform.localScale = new Vector3(Scale, Scale, Scale);
				SLOTGameSingleton<SLOTAudioManager>.GetInstance().UpdateAudioVolumes(gameObject);
			}
		}
	}
}
