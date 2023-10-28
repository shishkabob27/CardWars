using UnityEngine;

[AddComponentMenu("FingerGestures/Components/Screen Raycaster")]
public class ScreenRaycaster : MonoBehaviour
{
	public Camera[] Cameras;

	public LayerMask IgnoreLayerMask;

	public float RayThickness;

	public bool VisualizeRaycasts = true;

	private void Start()
	{
		if (Cameras == null || Cameras.Length == 0)
		{
			Cameras = new Camera[1] { Camera.main };
		}
	}

	public bool Raycast(Vector2 screenPos, out RaycastHit hit)
	{
		Camera[] cameras = Cameras;
		foreach (Camera cam in cameras)
		{
			if (Raycast(cam, screenPos, out hit))
			{
				return true;
			}
		}
		hit = default(RaycastHit);
		return false;
	}

	protected virtual bool Raycast(Camera cam, Vector2 screenPos, out RaycastHit hit)
	{
		Ray ray = cam.ScreenPointToRay(screenPos);
		bool flag = false;
		if (RayThickness > 0f)
		{
			return Physics.SphereCast(ray, 0.5f * RayThickness, out hit, float.PositiveInfinity, ~(int)IgnoreLayerMask);
		}
		return Physics.Raycast(ray, out hit, float.PositiveInfinity, ~(int)IgnoreLayerMask);
	}
}
