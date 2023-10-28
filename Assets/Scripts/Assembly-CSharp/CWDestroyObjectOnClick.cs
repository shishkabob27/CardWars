using UnityEngine;

public class CWDestroyObjectOnClick : MonoBehaviour
{
	public GameObject obj;

	public float timer;

	private void OnClick()
	{
		if (obj != null)
		{
			Object.Destroy(obj, timer);
		}
	}

	private void Update()
	{
	}
}
