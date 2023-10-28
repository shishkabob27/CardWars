using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("FingerGestures/Toolbox/Camera/Orbit")]
public class TBOrbit : MonoBehaviour
{
	public enum PanMode
	{
		Disabled,
		OneFinger,
		TwoFingers
	}

	public Transform target;

	public float initialDistance = 10f;

	public float minDistance = 1f;

	public float maxDistance = 20f;

	public float yawSensitivity = 80f;

	public float pitchSensitivity = 80f;

	public bool clampYawAngle;

	public float minYaw = -75f;

	public float maxYaw = 75f;

	public bool clampPitchAngle = true;

	public float minPitch = -20f;

	public float maxPitch = 80f;

	public bool allowPinchZoom = true;

	public float pinchZoomSensitivity = 2f;

	public bool smoothMotion = true;

	public float smoothZoomSpeed = 3f;

	public float smoothOrbitSpeed = 4f;

	public bool allowPanning;

	public bool invertPanningDirections;

	public float panningSensitivity = 1f;

	public Transform panningPlane;

	public bool smoothPanning = true;

	public float smoothPanningSpeed = 8f;

	private float distance = 10f;

	private float yaw;

	private float pitch;

	private float idealDistance;

	private float idealYaw;

	private float idealPitch;

	private Vector3 idealPanOffset = Vector3.zero;

	private Vector3 panOffset = Vector3.zero;

	private float nextDragTime;

	public float Distance
	{
		get
		{
			return distance;
		}
	}

	public float IdealDistance
	{
		get
		{
			return idealDistance;
		}
		set
		{
			idealDistance = Mathf.Clamp(value, minDistance, maxDistance);
		}
	}

	public float Yaw
	{
		get
		{
			return yaw;
		}
	}

	public float IdealYaw
	{
		get
		{
			return idealYaw;
		}
		set
		{
			idealYaw = ((!clampYawAngle) ? value : ClampAngle(value, minYaw, maxYaw));
		}
	}

	public float Pitch
	{
		get
		{
			return pitch;
		}
	}

	public float IdealPitch
	{
		get
		{
			return idealPitch;
		}
		set
		{
			idealPitch = ((!clampPitchAngle) ? value : ClampAngle(value, minPitch, maxPitch));
		}
	}

	public Vector3 IdealPanOffset
	{
		get
		{
			return idealPanOffset;
		}
		set
		{
			idealPanOffset = value;
		}
	}

	public Vector3 PanOffset
	{
		get
		{
			return panOffset;
		}
	}

	private void InstallGestureRecognizers()
	{
		List<GestureRecognizer> list = new List<GestureRecognizer>(GetComponents<GestureRecognizer>());
		DragRecognizer dragRecognizer = list.Find((GestureRecognizer r) => r.EventMessageName == "OnDrag") as DragRecognizer;
		DragRecognizer dragRecognizer2 = list.Find((GestureRecognizer r) => r.EventMessageName == "OnTwoFingerDrag") as DragRecognizer;
		PinchRecognizer pinchRecognizer = list.Find((GestureRecognizer r) => r.EventMessageName == "OnPinch") as PinchRecognizer;
		if (!dragRecognizer)
		{
			dragRecognizer = base.gameObject.AddComponent<DragRecognizer>();
			dragRecognizer.RequiredFingerCount = 1;
			dragRecognizer.IsExclusive = true;
			dragRecognizer.MaxSimultaneousGestures = 1;
			dragRecognizer.SendMessageToSelection = GestureRecognizer.SelectionType.None;
		}
		if (!pinchRecognizer)
		{
			pinchRecognizer = base.gameObject.AddComponent<PinchRecognizer>();
		}
		if (!dragRecognizer2)
		{
			dragRecognizer2 = base.gameObject.AddComponent<DragRecognizer>();
			dragRecognizer2.RequiredFingerCount = 2;
			dragRecognizer2.IsExclusive = true;
			dragRecognizer2.MaxSimultaneousGestures = 1;
			dragRecognizer2.ApplySameDirectionConstraint = false;
			dragRecognizer2.EventMessageName = "OnTwoFingerDrag";
		}
	}

	private void Start()
	{
		InstallGestureRecognizers();
		if (!panningPlane)
		{
			panningPlane = base.transform;
		}
		Vector3 eulerAngles = base.transform.eulerAngles;
		float num2 = (IdealDistance = initialDistance);
		distance = num2;
		num2 = (IdealYaw = eulerAngles.y);
		yaw = num2;
		num2 = (IdealPitch = eulerAngles.x);
		pitch = num2;
		if ((bool)GetComponent<Rigidbody>())
		{
			GetComponent<Rigidbody>().freezeRotation = true;
		}
		Apply();
	}

	private void OnDrag(DragGesture gesture)
	{
		if (!(Time.time < nextDragTime) && (bool)target)
		{
			IdealYaw += gesture.DeltaMove.x * yawSensitivity * 0.02f;
			IdealPitch -= gesture.DeltaMove.y * pitchSensitivity * 0.02f;
		}
	}

	private void OnPinch(PinchGesture gesture)
	{
		if (allowPinchZoom)
		{
			IdealDistance -= gesture.Delta * pinchZoomSensitivity;
			nextDragTime = Time.time + 0.25f;
		}
	}

	private void OnTwoFingerDrag(DragGesture gesture)
	{
		if (allowPanning)
		{
			Vector3 vector = -0.02f * panningSensitivity * (panningPlane.right * gesture.DeltaMove.x + panningPlane.up * gesture.DeltaMove.y);
			if (invertPanningDirections)
			{
				IdealPanOffset -= vector;
			}
			else
			{
				IdealPanOffset += vector;
			}
			nextDragTime = Time.time + 0.25f;
		}
	}

	private void Apply()
	{
		if (smoothMotion)
		{
			distance = Mathf.Lerp(distance, IdealDistance, Time.deltaTime * smoothZoomSpeed);
			yaw = Mathf.Lerp(yaw, IdealYaw, Time.deltaTime * smoothOrbitSpeed);
			pitch = Mathf.LerpAngle(pitch, IdealPitch, Time.deltaTime * smoothOrbitSpeed);
		}
		else
		{
			distance = IdealDistance;
			yaw = IdealYaw;
			pitch = IdealPitch;
		}
		if (smoothPanning)
		{
			panOffset = Vector3.Lerp(panOffset, idealPanOffset, Time.deltaTime * smoothPanningSpeed);
		}
		else
		{
			panOffset = idealPanOffset;
		}
		base.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
		base.transform.position = target.position + panOffset - distance * base.transform.forward;
	}

	private void LateUpdate()
	{
		Apply();
	}

	private static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}

	public void ResetPanning()
	{
		IdealPanOffset = Vector3.zero;
	}
}
