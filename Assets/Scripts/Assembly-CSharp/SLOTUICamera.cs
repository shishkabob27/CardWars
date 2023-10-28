using UnityEngine;

public class SLOTUICamera : MonoBehaviour
{
	private Camera targetCamera;

	private static float IPHONE_ORTHO_SIZE = 5.8f;

	private static float IPHONE5_ORTHO_SIZE = 5.8f;

	private static float IPAD_ORTHO_SIZE = 5.8f;

	private static float IPHONE_FOV = 60f;

	private static float IPHONE5_FOV = 75f;

	private static float IPAD_FOV = 60f;

	private void Start()
	{
		targetCamera = base.gameObject.GetComponent(typeof(Camera)) as Camera;
		DetermineScale();
	}

	private void DetermineScale()
	{
		if (0 == 0)
		{
			float num = (float)Screen.width / (float)Screen.height;
			float num2 = 0.01f;
			if (Mathf.Abs(num - 1.3333334f) < num2)
			{
				targetCamera.orthographicSize = IPAD_ORTHO_SIZE;
				targetCamera.fieldOfView = IPAD_FOV;
			}
			else if (Mathf.Abs(num - 1.5f) < num2)
			{
				targetCamera.orthographicSize = IPHONE_ORTHO_SIZE;
				targetCamera.fieldOfView = IPHONE_FOV;
			}
			else if (Mathf.Abs(num - 1.7777778f) < num2)
			{
				targetCamera.orthographicSize = IPHONE5_ORTHO_SIZE;
				targetCamera.fieldOfView = IPHONE5_FOV;
			}
			else
			{
				targetCamera.orthographicSize = IPAD_ORTHO_SIZE;
				targetCamera.fieldOfView = IPAD_FOV;
			}
		}
	}

	public float GetScale()
	{
		return 1f;
	}
}
