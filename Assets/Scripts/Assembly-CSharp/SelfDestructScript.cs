using UnityEngine;

public class SelfDestructScript : MonoBehaviour
{
	public float DestroyTime = 1.3f;

	private void Start()
	{
		Object.Destroy(base.gameObject, DestroyTime);
	}
}
