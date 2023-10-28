using System.Collections.Generic;
using UnityEngine;

public class CWMapPinchRecognizer : PinchRecognizer
{
	public LayerMask layers;

	private CWScreenRaycaster rayCaster;

	private void CreateRaycaster()
	{
		if (rayCaster == null)
		{
			rayCaster = base.gameObject.GetComponent(typeof(CWScreenRaycaster)) as CWScreenRaycaster;
			if (rayCaster == null)
			{
				rayCaster = base.gameObject.AddComponent(typeof(CWScreenRaycaster)) as CWScreenRaycaster;
			}
		}
		if (!(rayCaster != null))
		{
			return;
		}
		Object[] array = Object.FindObjectsOfType(typeof(Camera));
		List<Camera> list = new List<Camera>();
		Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Camera camera = (Camera)array2[i];
			if (camera != null && (layers.value & (1 << camera.gameObject.layer)) != 0)
			{
				list.Add(camera);
			}
		}
		rayCaster.Cameras = list.ToArray();
	}

	protected override bool CanBegin2(PinchGesture gesture, FingerGestures.IFingerList touches)
	{
		FingerGestures.Finger finger = touches[0];
		FingerGestures.Finger finger2 = touches[1];
		float num = Vector2.SqrMagnitude(finger.StartPosition - finger2.StartPosition);
		float num2 = Vector2.SqrMagnitude(finger.Position - finger2.Position);
		if (FingerGestures.GetAdjustedPixelDistance(Mathf.Abs(num - num2)) < MinDistance * MinDistance)
		{
			return false;
		}
		if (rayCaster == null)
		{
			CreateRaycaster();
		}
		if (rayCaster != null)
		{
			int count = touches.Count;
			for (int i = 0; i < count; i++)
			{
				FingerGestures.Finger finger3 = touches[i];
				if (finger3 == null)
				{
					continue;
				}
				GameObject gameObject = gesture.PickObject(rayCaster, finger3.StartPosition);
				if (gameObject != null)
				{
					CWCameraCollider cWCameraCollider = gameObject.GetComponent(typeof(CWCameraCollider)) as CWCameraCollider;
					if (cWCameraCollider == null)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	protected override GestureRecognitionState OnRecognize(PinchGesture gesture, FingerGestures.IFingerList touches)
	{
		if (touches.Count != RequiredFingerCount)
		{
			gesture.Delta = 0f;
			if (touches.Count < RequiredFingerCount)
			{
				return GestureRecognitionState.Ended;
			}
			return GestureRecognitionState.Failed;
		}
		FingerGestures.Finger finger = touches[0];
		FingerGestures.Finger finger2 = touches[1];
		gesture.Position = 0.5f * (finger.Position + finger2.Position);
		float num = Vector2.Distance(finger.Position, finger2.Position);
		float adjustedPixelDistance = FingerGestures.GetAdjustedPixelDistance(DeltaScale * (num - gesture.Gap));
		gesture.Gap = num;
		gesture.Delta = adjustedPixelDistance;
		RaiseEvent(gesture);
		return GestureRecognitionState.InProgress;
	}
}
