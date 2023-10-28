using UnityEngine;

public class ParticleDeathScript : MonoBehaviour
{
	private ParticleSystem ps;

	private void Start()
	{
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
