using UnityEngine;

public class CWMapZoom : TBPinchZoom
{
	private Camera cam;

	private CWMapPan pan;

	private void Awake()
	{
		cam = base.gameObject.GetComponent<Camera>();
		pan = base.gameObject.GetComponent(typeof(CWMapPan)) as CWMapPan;
	}

	private void OnPinch(PinchGesture gesture)
	{
		if (GlobalFlags.Instance.enableMapDrag && cam != null && pan != null && gesture.Fingers.Count >= 2)
		{
			GlobalFlags.Instance.lastQuestMapCameraFOV = GetComponent<Camera>().orthographicSize;
			FingerGestures.Finger finger = gesture.Fingers[0];
			FingerGestures.Finger finger2 = gesture.Fingers[1];
			Vector2 position = finger.Position;
			Vector2 position2 = finger2.Position;
			Vector2 previousPosition = finger.PreviousPosition;
			Vector2 previousPosition2 = finger2.PreviousPosition;
			Vector2 vector = previousPosition + (previousPosition2 - previousPosition) * 0.5f;
			Vector3 vector2 = cam.ScreenToWorldPoint(vector);
			base.ZoomAmount += zoomSpeed * gesture.Delta;
			Vector2 vector3 = position2 - position;
			Vector2 vector4 = position + vector3 * 0.5f;
			Vector3 vector5 = cam.ScreenToWorldPoint(vector4);
			Vector3 vector6 = vector5 - vector2;
			Vector3 worldPos = pan.ConstrainToMoveArea(pan.idealPos - vector6);
			pan.TeleportTo(worldPos);
		}
	}
}
