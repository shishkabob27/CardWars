using UnityEngine;

[AddComponentMenu("FingerGestures/Toolbox/Camera/Drag View")]
[RequireComponent(typeof(DragRecognizer))]
public class TBDragView : MonoBehaviour
{
	public bool allowUserInput = true;

	public float sensitivity = 8f;

	public float dragAcceleration = 40f;

	public float dragDeceleration = 10f;

	public bool reverseControls;

	public float minPitchAngle = -60f;

	public float maxPitchAngle = 60f;

	public float idealRotationSmoothingSpeed = 7f;

	private Transform cachedTransform;

	private Vector2 angularVelocity = Vector2.zero;

	private Quaternion idealRotation;

	private bool useAngularVelocity;

	private DragGesture dragGesture;

	public bool Dragging
	{
		get
		{
			return dragGesture != null;
		}
	}

	public Quaternion IdealRotation
	{
		get
		{
			return idealRotation;
		}
		set
		{
			idealRotation = value;
			useAngularVelocity = false;
		}
	}

	private void Awake()
	{
		cachedTransform = base.transform;
	}

	private void Start()
	{
		IdealRotation = cachedTransform.rotation;
		if (!GetComponent<DragRecognizer>())
		{
			base.enabled = false;
		}
	}

	private void OnDrag(DragGesture gesture)
	{
		if (gesture.Phase != ContinuousGesturePhase.Ended)
		{
			dragGesture = gesture;
		}
		else
		{
			dragGesture = null;
		}
	}

	private void Update()
	{
		if (Dragging && allowUserInput)
		{
			useAngularVelocity = true;
		}
		if (useAngularVelocity)
		{
			Vector3 localEulerAngles = base.transform.localEulerAngles;
			Vector2 b = Vector2.zero;
			float num = dragDeceleration;
			if (Dragging)
			{
				b = sensitivity * dragGesture.DeltaMove;
				num = dragAcceleration;
			}
			angularVelocity = Vector2.Lerp(angularVelocity, b, Time.deltaTime * num);
			Vector2 vector = Time.deltaTime * angularVelocity;
			if (reverseControls)
			{
				vector = -vector;
			}
			localEulerAngles.x = Mathf.Clamp(NormalizePitch(localEulerAngles.x + vector.y), minPitchAngle, maxPitchAngle);
			localEulerAngles.y -= vector.x;
			base.transform.localEulerAngles = localEulerAngles;
		}
		else if (idealRotationSmoothingSpeed > 0f)
		{
			cachedTransform.rotation = Quaternion.Slerp(cachedTransform.rotation, IdealRotation, Time.deltaTime * idealRotationSmoothingSpeed);
		}
		else
		{
			cachedTransform.rotation = idealRotation;
		}
	}

	private static float NormalizePitch(float angle)
	{
		if (angle > 180f)
		{
			angle -= 360f;
		}
		return angle;
	}

	public void LookAt(Vector3 pos)
	{
		IdealRotation = Quaternion.LookRotation(pos - cachedTransform.position);
	}
}
