using UnityEngine;

public class ScreenRaycaster : MonoBehaviour
{
	public Camera[] Cameras;
	public LayerMask IgnoreLayerMask;
	public float RayThickness;
	public bool VisualizeRaycasts;
}
