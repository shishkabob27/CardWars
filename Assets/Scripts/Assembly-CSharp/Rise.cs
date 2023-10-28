using UnityEngine;

public class Rise : MonoBehaviour
{
	public float StartHeight;

	private void Start()
	{
		base.transform.position = new Vector3(base.transform.position.x, StartHeight, base.transform.position.z);
	}

	private void Update()
	{
		base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + Time.deltaTime * 10f, base.transform.position.z);
		if (base.transform.position.y > 0f)
		{
			base.transform.position = new Vector3(0f, 0f, 0f);
			Object.Destroy(this);
		}
	}
}
