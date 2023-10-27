using UnityEngine;

public class TBTwistToRotate : MonoBehaviour
{
	public enum RotationAxis
	{
		WorldX = 0,
		WorldY = 1,
		WorldZ = 2,
		ObjectX = 3,
		ObjectY = 4,
		ObjectZ = 5,
		CameraX = 6,
		CameraY = 7,
		CameraZ = 8,
	}

	public float Sensitivity;
	public RotationAxis Axis;
	public Camera ReferenceCamera;
}
