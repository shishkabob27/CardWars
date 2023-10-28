using UnityEngine;

public class CWDestroyObject : MonoBehaviour
{
	public GameObject obj;

	public float timer;

	private void Start()
	{
		if (obj == null)
		{
			obj = base.gameObject;
		}
		Object.Destroy(obj, timer);
	}

	private void Update()
	{
	}
}
