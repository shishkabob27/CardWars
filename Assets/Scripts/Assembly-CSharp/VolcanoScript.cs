using UnityEngine;

public class VolcanoScript : MonoBehaviour
{
	public GameObject Smoke;

	private float Destination = -1.9f;

	private float Timer;

	public bool Rise;

	private void Update()
	{
		if (Rise)
		{
			GetComponent<Animation>().Play();
			Timer += Time.deltaTime;
			if (Timer > 6f)
			{
				Smoke.transform.localPosition = new Vector3(0f, 3f, 0f);
				Destination = -11f;
			}
			else if (Smoke.transform.localPosition.y < 10f)
			{
				Smoke.transform.localPosition = new Vector3(0f, Smoke.transform.localPosition.y + Time.deltaTime * 4f, 0f);
			}
			base.transform.position = new Vector3(base.transform.position.x, Mathf.Lerp(base.transform.position.y, Destination, Time.deltaTime), base.transform.position.z);
			if (Timer > 8f)
			{
				Smoke.transform.localPosition = new Vector3(0f, 3f, 0f);
				base.transform.position = new Vector3(0f, -11f, 0f);
				Destination = -1.9f;
				Rise = false;
				Timer = 0f;
				GetComponent<Animation>().Stop();
			}
		}
	}
}
