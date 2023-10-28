using UnityEngine;

public class CWMapPan : TBPan
{
	public float swipeMagnitude = 5f;

	public float swipeSensitivity = 1f;

	public float scrollDamping = 0.97f;

	private int lastDragFrame = -1;

	private Vector3 worldSpaceMove = Vector3.zero;

	private void OnEnable()
	{
		ResetMomentum();
	}

	public override Vector3 ConstrainToMoveArea(Vector3 p)
	{
		if ((bool)moveArea)
		{
			Vector3 min = moveArea.bounds.min;
			Vector3 max = moveArea.bounds.max;
			p.x = Mathf.Clamp(p.x, min.x, max.x);
			p.y = 50f;
			p.z = Mathf.Clamp(p.z, min.z, max.z);
		}
		return p;
	}

	private void OnDrag(DragGesture gesture)
	{
		if (!GlobalFlags.Instance.enableMapDrag)
		{
			return;
		}
		if (gesture.State == GestureRecognitionState.Ended)
		{
			GlobalFlags.Instance.lastQuestMapCameraIdealPos = idealPos;
			if (gesture.DeltaMove.SqrMagnitude() > swipeMagnitude * swipeMagnitude && Time.frameCount - lastDragFrame <= 1)
			{
				Vector2 vector = swipeSensitivity * dragGesture.DeltaMove;
				if (Time.deltaTime > 0f)
				{
					vector *= 1f / Time.deltaTime;
				}
				worldSpaceMove = vector.x * cachedTransform.right + vector.y * cachedTransform.up;
			}
			else
			{
				ResetMomentum();
			}
		}
		else
		{
			ResetMomentum();
		}
		dragGesture = ((gesture.State == GestureRecognitionState.InProgress) ? gesture : null);
		if (gesture.State == GestureRecognitionState.InProgress)
		{
			lastDragFrame = Time.frameCount;
		}
	}

	private void FixedUpdate()
	{
		if (worldSpaceMove.x != 0f)
		{
			idealPos.x -= worldSpaceMove.x * Time.deltaTime;
			worldSpaceMove.x *= scrollDamping;
		}
		if (worldSpaceMove.z != 0f)
		{
			idealPos.z -= worldSpaceMove.z * Time.deltaTime;
			worldSpaceMove.z *= scrollDamping;
		}
	}

	public void ResetMomentum()
	{
		worldSpaceMove = Vector3.zero;
		lastDragFrame = -1;
	}
}
