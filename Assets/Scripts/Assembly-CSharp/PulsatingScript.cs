using UnityEngine;

public class PulsatingScript : MonoBehaviour
{
	private bool Grow;

	private void Update()
	{
		if (Grow)
		{
			base.transform.localScale = new Vector3(base.transform.localScale.x + Time.deltaTime * 0.1f, base.transform.localScale.y + Time.deltaTime * 0.1f, base.transform.localScale.z + Time.deltaTime * 0.1f);
			if (base.transform.localScale.x > 1f)
			{
				base.transform.localScale = new Vector3(1f, 1f, 1f);
				Grow = false;
			}
		}
		else
		{
			base.transform.localScale = new Vector3(base.transform.localScale.x - Time.deltaTime * 0.1f, base.transform.localScale.y - Time.deltaTime * 0.1f, base.transform.localScale.z - Time.deltaTime * 0.1f);
			if ((double)base.transform.localScale.x < 0.9)
			{
				base.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
				Grow = true;
			}
		}
	}
}
