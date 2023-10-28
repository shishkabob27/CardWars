using UnityEngine;

[AddComponentMenu("FingerGestures/Gestures/Long Press Recognizer")]
public class LongPressRecognizer : DiscreteGestureRecognizer<LongPressGesture>
{
	public float Duration = 1f;

	public float MoveTolerance = 5f;

	public override string GetDefaultEventMessageName()
	{
		return "OnLongPress";
	}

	protected override void OnBegin(LongPressGesture gesture, FingerGestures.IFingerList touches)
	{
		gesture.Position = touches.GetAveragePosition();
		gesture.StartPosition = gesture.Position;
	}

	protected override GestureRecognitionState OnRecognize(LongPressGesture gesture, FingerGestures.IFingerList touches)
	{
		if (touches.Count != RequiredFingerCount)
		{
			return GestureRecognitionState.Failed;
		}
		if (gesture.ElapsedTime >= Duration)
		{
			return GestureRecognitionState.Ended;
		}
		if (touches.GetAverageDistanceFromStart() > MoveTolerance)
		{
			return GestureRecognitionState.Failed;
		}
		return GestureRecognitionState.InProgress;
	}
}
