using UnityEngine;

public class FingerEffectScript : MonoBehaviour
{
	public float Scale;

	public Camera uiCamera;

	private void Update()
	{
		if (Input.GetMouseButton(0))
		{
			if (Scale < 1f)
			{
				Scale += Time.deltaTime * 10f;
				if (Scale > 1f)
				{
					Scale = 1f;
				}
			}
			base.transform.position = uiCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
		}
		else if (Scale > 0f)
		{
			Scale -= Time.deltaTime * 10f;
			if (Scale < 0.0001f)
			{
				Scale = 0.0001f;
			}
		}
		base.transform.localScale = new Vector3(Scale * 150f, Scale * 150f, 1f);
	}
}
