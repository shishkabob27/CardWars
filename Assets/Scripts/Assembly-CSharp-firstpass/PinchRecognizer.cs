using UnityEngine;

[AddComponentMenu("FingerGestures/Gestures/Pinch Recognizer")]
public class PinchRecognizer : ContinuousGestureRecognizer<PinchGesture>
{
	public float MinDOT = -0.7f;

	public float MinDistance = 5f;

	public float DeltaScale = 1f;

	public override int RequiredFingerCount
	{
		get
		{
			return 2;
		}
		set
		{
		}
	}

	public override bool SupportFingerClustering
	{
		get
		{
			return false;
		}
	}

	public override string GetDefaultEventMessageName()
	{
		return "OnPinch";
	}

	protected override GameObject GetDefaultSelectionForSendMessage(PinchGesture gesture)
	{
		return gesture.StartSelection;
	}

	public override GestureResetMode GetDefaultResetMode()
	{
		return GestureResetMode.NextFrame;
	}

	protected override bool CanBegin(PinchGesture gesture, FingerGestures.IFingerList touches)
	{
		if (!base.CanBegin(gesture, touches))
		{
			return false;
		}
		return CanBegin2(gesture, touches);
	}

	protected virtual bool CanBegin2(PinchGesture gesture, FingerGestures.IFingerList touches)
	{
		FingerGestures.Finger finger = touches[0];
		FingerGestures.Finger finger2 = touches[1];
		if (!FingerGestures.AllFingersMoving(finger, finger2))
		{
			return false;
		}
		if (!FingersMovedInOppositeDirections(finger, finger2))
		{
			return false;
		}
		float num = Vector2.SqrMagnitude(finger.StartPosition - finger2.StartPosition);
		float num2 = Vector2.SqrMagnitude(finger.Position - finger2.Position);
		if (FingerGestures.GetAdjustedPixelDistance(Mathf.Abs(num - num2)) < MinDistance * MinDistance)
		{
			return false;
		}
		return true;
	}

	protected override void OnBegin(PinchGesture gesture, FingerGestures.IFingerList touches)
	{
		FingerGestures.Finger finger = touches[0];
		FingerGestures.Finger finger2 = touches[1];
		gesture.StartPosition = 0.5f * (finger.StartPosition + finger2.StartPosition);
		gesture.Position = 0.5f * (finger.Position + finger2.Position);
		gesture.Gap = Vector2.Distance(finger.StartPosition, finger2.StartPosition);
		float num = Vector2.Distance(finger.Position, finger2.Position);
		gesture.Delta = FingerGestures.GetAdjustedPixelDistance(DeltaScale * (num - gesture.Gap));
		gesture.Gap = num;
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
		if (!FingerGestures.AllFingersMoving(finger, finger2))
		{
			return GestureRecognitionState.InProgress;
		}
		float num = Vector2.Distance(finger.Position, finger2.Position);
		float adjustedPixelDistance = FingerGestures.GetAdjustedPixelDistance(DeltaScale * (num - gesture.Gap));
		gesture.Gap = num;
		if (Mathf.Abs(adjustedPixelDistance) > 0.001f)
		{
			if (!FingersMovedInOppositeDirections(finger, finger2))
			{
				return GestureRecognitionState.InProgress;
			}
			gesture.Delta = adjustedPixelDistance;
			RaiseEvent(gesture);
		}
		return GestureRecognitionState.InProgress;
	}

	protected bool FingersMovedInOppositeDirections(FingerGestures.Finger finger0, FingerGestures.Finger finger1)
	{
		return FingerGestures.FingersMovedInOppositeDirections(finger0, finger1, MinDOT);
	}
}
