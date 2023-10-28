using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(DragRecognizer))]
[AddComponentMenu("FingerGestures/Toolbox/Camera/Pan")]
public class TBPan : MonoBehaviour
{
	public delegate void PanEventHandler(TBPan source, Vector3 move);

	protected Transform cachedTransform;

	public float sensitivity = 1f;

	public float smoothSpeed = 10f;

	public BoxCollider moveArea;

	public Vector3 idealPos;

	protected DragGesture dragGesture;

	[method: MethodImpl(32)]
	public event PanEventHandler OnPan;

	private void Awake()
	{
		cachedTransform = base.transform;
	}

	private void Start()
	{
		idealPos = cachedTransform.position;
		if (!GetComponent<DragRecognizer>())
		{
			base.enabled = false;
		}
	}

	private void OnDrag(DragGesture gesture)
	{
		dragGesture = ((gesture.State != GestureRecognitionState.Ended) ? gesture : null);
	}

	private void Update()
	{
		if (dragGesture != null && dragGesture.DeltaMove.SqrMagnitude() > 0f)
		{
			Vector2 vector = sensitivity * dragGesture.DeltaMove;
			Vector3 vector2 = vector.x * cachedTransform.right + vector.y * cachedTransform.up;
			idealPos -= vector2;
			if (this.OnPan != null)
			{
				this.OnPan(this, vector2);
			}
		}
		idealPos = ConstrainToMoveArea(idealPos);
		if (smoothSpeed > 0f)
		{
			cachedTransform.position = Vector3.Lerp(cachedTransform.position, idealPos, Time.deltaTime * smoothSpeed);
		}
		else
		{
			cachedTransform.position = idealPos;
		}
	}

	public Vector3 ConstrainToPanningPlane(Vector3 p)
	{
		Vector3 position = cachedTransform.InverseTransformPoint(p);
		position.z = 0f;
		return cachedTransform.TransformPoint(position);
	}

	public void TeleportTo(Vector3 worldPos)
	{
		cachedTransform.position = (idealPos = ConstrainToPanningPlane(worldPos));
	}

	public void FlyTo(Vector3 worldPos)
	{
		idealPos = ConstrainToPanningPlane(worldPos);
	}

	public virtual Vector3 ConstrainToMoveArea(Vector3 p)
	{
		if ((bool)moveArea)
		{
			Vector3 min = moveArea.bounds.min;
			Vector3 max = moveArea.bounds.max;
			p.x = Mathf.Clamp(p.x, min.x, max.x);
			p.y = Mathf.Clamp(p.y, min.y, max.y);
			p.z = Mathf.Clamp(p.z, min.z, max.z);
		}
		return p;
	}
}
