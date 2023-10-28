using UnityEngine;

public class CWCameraCollider : MonoBehaviour
{
	public Camera mainCamera;

	private BoxCollider col;

	private CWMapZoom zoomScript;

	private float currentSize;

	public int longColW = 20;

	public int zoomColW = 70;

	public int longColH = 120;

	public int zoomColH = 140;

	public float longFOV = 38f;

	public float zoomFOV = 18f;

	private void Start()
	{
		col = GetComponent<BoxCollider>();
		zoomScript = mainCamera.GetComponent<CWMapZoom>();
		UpdateBounds();
	}

	private void Update()
	{
		UpdateBounds();
	}

	private void UpdateBounds()
	{
		if (mainCamera.orthographicSize != currentSize)
		{
			longFOV = 22f - zoomScript.minZoomAmount;
			zoomFOV = 22f - zoomScript.maxZoomAmount;
			float num = (float)(longColW - zoomColW) / (longFOV - zoomFOV);
			float num2 = (float)zoomColW - zoomFOV * num;
			float num3 = (float)(longColH - zoomColH) / (longFOV - zoomFOV);
			float num4 = (float)zoomColH - zoomFOV * num3;
			col.size = new Vector3(num * mainCamera.orthographicSize + num2, col.size.y, num3 * mainCamera.orthographicSize + num4);
			currentSize = mainCamera.orthographicSize;
		}
	}
}
