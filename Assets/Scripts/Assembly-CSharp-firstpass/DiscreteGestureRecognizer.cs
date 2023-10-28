public abstract class DiscreteGestureRecognizer<T> : GestureRecognizer<T> where T : DiscreteGesture, new()
{
	protected override void OnStateChanged(Gesture sender)
	{
		base.OnStateChanged(sender);
		T gesture = (T)sender;
		if (gesture.State == GestureRecognitionState.Ended)
		{
			RaiseEvent(gesture);
		}
	}
}
