using UnityEngine;

public class SplashScript : MonoBehaviour
{
	private ParticleSystem ps;

	private void Start()
	{
		base.transform.eulerAngles = new Vector3(-90f, -180f, 0f);
		ps = GetComponent<ParticleSystem>();
	}

	private void Update()
	{
		if (!ps.IsAlive())
		{
			Object.Destroy(base.gameObject);
		}
	}
}
