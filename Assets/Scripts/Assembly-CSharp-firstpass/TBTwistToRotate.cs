using UnityEngine;

[AddComponentMenu("FingerGestures/Toolbox/Twist To Rotate")]
public class TBTwistToRotate : MonoBehaviour
{
	public enum RotationAxis
	{
		WorldX,
		WorldY,
		WorldZ,
		ObjectX,
		ObjectY,
		ObjectZ,
		CameraX,
		CameraY,
		CameraZ
	}

	public float Sensitivity = 1f;

	public RotationAxis Axis = RotationAxis.WorldY;

	public Camera ReferenceCamera;

	private void Start()
	{
		if (!ReferenceCamera)
		{
			ReferenceCamera = Camera.main;
		}
	}

	public Vector3 GetRotationAxis()
	{
		switch (Axis)
		{
		case RotationAxis.WorldX:
			return Vector3.right;
		case RotationAxis.WorldY:
			return Vector3.up;
		case RotationAxis.WorldZ:
			return Vector3.forward;
		case RotationAxis.ObjectX:
			return base.transform.right;
		case RotationAxis.ObjectY:
			return base.transform.up;
		case RotationAxis.ObjectZ:
			return base.transform.forward;
		case RotationAxis.CameraX:
			return ReferenceCamera.transform.right;
		case RotationAxis.CameraY:
			return ReferenceCamera.transform.up;
		case RotationAxis.CameraZ:
			return ReferenceCamera.transform.forward;
		default:
			return Vector3.forward;
		}
	}

	private void OnTwist(TwistGesture gesture)
	{
		Quaternion quaternion = Quaternion.AngleAxis(Sensitivity * gesture.DeltaRotation, GetRotationAxis());
		base.transform.rotation = quaternion * base.transform.rotation;
	}
}
