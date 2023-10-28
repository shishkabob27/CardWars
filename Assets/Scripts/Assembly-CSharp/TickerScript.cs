using UnityEngine;

public class TickerScript : MonoBehaviour
{
	public UILabel Label;

	public float Speed = 1f;

	private void Start()
	{
		base.transform.position = new Vector3(3.375f + 0.25f * (float)Label.text.Length, base.transform.position.y, base.transform.position.z);
	}

	private void Update()
	{
		base.transform.position = new Vector3(base.transform.position.x - Speed * Time.deltaTime, base.transform.position.y, base.transform.position.z);
		if (base.transform.position.x < -3.375f - 0.25f * (float)Label.text.Length)
		{
			base.transform.position = new Vector3(3.375f + 0.25f * (float)Label.text.Length, base.transform.position.y, base.transform.position.z);
		}
	}
}
