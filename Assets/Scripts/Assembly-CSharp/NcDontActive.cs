using UnityEngine;

public class NcDontActive : NcEffectBehaviour
{
	private void Awake()
	{
		Object.Destroy(base.gameObject);
	}

	private void OnEnable()
	{
	}
}
