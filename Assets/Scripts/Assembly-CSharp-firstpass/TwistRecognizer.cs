using UnityEngine;

[AddComponentMenu("FingerGestures/Gestures/Twist Recognizer")]
public class TwistRecognizer : ContinuousGestureRecognizer<TwistGesture>
{
	public float MinDOT = -0.7f;

	public float MinRotation = 1f;

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
		return "OnTwist";
	}

	public override GestureResetMode GetDefaultResetMode()
	{
		return GestureResetMode.NextFrame;
	}

	protected override GameObject GetDefaultSelectionForSendMessage(TwistGesture gesture)
	{
		return gesture.StartSelection;
	}

	protected override bool CanBegin(TwistGesture gesture, FingerGestures.IFingerList touches)
	{
		if (!base.CanBegin(gesture, touches))
		{
			return false;
		}
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
		float f = SignedAngularGap(finger, finger2, finger.StartPosition, finger2.StartPosition);
		if (Mathf.Abs(f) < MinRotation)
		{
			return false;
		}
		return true;
	}

	protected override void OnBegin(TwistGesture gesture, FingerGestures.IFingerList touches)
	{
		FingerGestures.Finger finger = touches[0];
		FingerGestures.Finger finger2 = touches[1];
		gesture.StartPosition = 0.5f * (finger.Position + finger2.Position);
		gesture.Position = gesture.StartPosition;
		gesture.TotalRotation = 0f;
		gesture.DeltaRotation = 0f;
	}

	protected override GestureRecognitionState OnRecognize(TwistGesture gesture, FingerGestures.IFingerList touches)
	{
		if (touches.Count != RequiredFingerCount)
		{
			gesture.DeltaRotation = 0f;
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
		gesture.DeltaRotation = SignedAngularGap(finger, finger2, finger.PreviousPosition, finger2.PreviousPosition);
		if (Mathf.Abs(gesture.DeltaRotation) > Mathf.Epsilon)
		{
			gesture.TotalRotation += gesture.DeltaRotation;
			RaiseEvent(gesture);
		}
		return GestureRecognitionState.InProgress;
	}

	private bool FingersMovedInOppositeDirections(FingerGestures.Finger finger0, FingerGestures.Finger finger1)
	{
		return FingerGestures.FingersMovedInOppositeDirections(finger0, finger1, MinDOT);
	}

	private static float SignedAngularGap(FingerGestures.Finger finger0, FingerGestures.Finger finger1, Vector2 refPos0, Vector2 refPos1)
	{
		Vector2 normalized = (finger0.Position - finger1.Position).normalized;
		Vector2 normalized2 = (refPos0 - refPos1).normalized;
		return 57.29578f * FingerGestures.SignedAngle(normalized2, normalized);
	}
}
