public abstract class ContinuousGestureRecognizer<T> : GestureRecognizer<T> where T : ContinuousGesture, new()
{
	protected override void Reset(T gesture)
	{
		base.Reset(gesture);
	}

	protected override void OnStateChanged(Gesture sender)
	{
		base.OnStateChanged(sender);
		T gesture = (T)sender;
		switch (gesture.State)
		{
		case GestureRecognitionState.Started:
			RaiseEvent(gesture);
			break;
		case GestureRecognitionState.Ended:
			RaiseEvent(gesture);
			break;
		case GestureRecognitionState.Failed:
			if (gesture.PreviousState != 0)
			{
				RaiseEvent(gesture);
			}
			break;
		case GestureRecognitionState.InProgress:
			break;
		}
	}
}
