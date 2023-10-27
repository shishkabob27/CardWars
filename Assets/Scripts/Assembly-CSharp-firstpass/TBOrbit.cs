using UnityEngine;

public class TBOrbit : MonoBehaviour
{
	public Transform target;
	public float initialDistance;
	public float minDistance;
	public float maxDistance;
	public float yawSensitivity;
	public float pitchSensitivity;
	public bool clampYawAngle;
	public float minYaw;
	public float maxYaw;
	public bool clampPitchAngle;
	public float minPitch;
	public float maxPitch;
	public bool allowPinchZoom;
	public float pinchZoomSensitivity;
	public bool smoothMotion;
	public float smoothZoomSpeed;
	public float smoothOrbitSpeed;
	public bool allowPanning;
	public bool invertPanningDirections;
	public float panningSensitivity;
	public Transform panningPlane;
	public bool smoothPanning;
	public float smoothPanningSpeed;
}
