using UnityEngine;

public class CWQuestMapCamera : MonoBehaviour
{
	private Camera cam;

	private CWMapPan pan;

	private CWMapZoom zoom;

	private float orthographicSize = -1f;

	private float zoomAmount = -1f;

	private void Awake()
	{
		cam = base.gameObject.GetComponent<Camera>();
		pan = base.gameObject.GetComponent(typeof(CWMapPan)) as CWMapPan;
		zoom = base.gameObject.GetComponent(typeof(CWMapZoom)) as CWMapZoom;
		UpdatePanSensitivity();
	}

	private void Update()
	{
		if (cam != null && cam.enabled && orthographicSize != cam.orthographicSize)
		{
			UpdatePanSensitivity();
		}
		if (zoom != null && zoom.ZoomAmount != zoomAmount)
		{
			zoomAmount = zoom.ZoomAmount;
			if (pan != null)
			{
				pan.ResetMomentum();
			}
		}
	}

	private void UpdatePanSensitivity()
	{
		if (pan != null && cam != null)
		{
			Vector3 vector = cam.ScreenToWorldPoint(new Vector3(0f, 0f, 0f));
			Vector3 vector2 = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
			pan.sensitivity = (vector2.x - vector.x) / (float)Screen.width;
		}
		orthographicSize = cam.orthographicSize;
	}
}
