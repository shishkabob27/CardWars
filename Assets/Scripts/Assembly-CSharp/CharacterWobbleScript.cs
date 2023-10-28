using UnityEngine;

public class CharacterWobbleScript : MonoBehaviour
{
	private float Wobble = 1f;

	private bool WobbleBack = true;

	private void Update()
	{
		if (!(base.transform.position.y < 1000f))
		{
			return;
		}
		if (WobbleBack)
		{
			Wobble -= Time.deltaTime;
			if (Wobble < -1f)
			{
				WobbleBack = false;
			}
		}
		else
		{
			Wobble += Time.deltaTime;
			if (Wobble > 1f)
			{
				WobbleBack = true;
			}
		}
		base.transform.eulerAngles = new Vector3(Wobble, base.transform.eulerAngles.y, base.transform.eulerAngles.z);
	}
}
