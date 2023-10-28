using UnityEngine;

public class CWFloopCameraScript : MonoBehaviour
{
	public float Delay;

	public Transform CameraLocation;

	public Transform CameraTarget;

	public Camera FloopCamera;

	private bool tracking;

	private float counter;

	private void OnEnable()
	{
		counter = 0f;
		tracking = true;
	}

	private void Update()
	{
		if (tracking)
		{
			if (counter >= Delay)
			{
				tracking = false;
				CWFloopActionManager.GetInstance().SetCameraPosition(FloopCamera, CameraLocation, CameraTarget);
			}
			else
			{
				counter += Time.deltaTime;
			}
		}
	}
}
