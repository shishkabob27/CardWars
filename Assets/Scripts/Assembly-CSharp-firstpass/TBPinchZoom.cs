using UnityEngine;

public class TBPinchZoom : MonoBehaviour
{
	public enum ZoomMethod
	{
		Position = 0,
		FOV = 1,
	}

	public ZoomMethod zoomMethod;
	public float zoomSpeed;
	public float minZoomAmount;
	public float maxZoomAmount;
}
