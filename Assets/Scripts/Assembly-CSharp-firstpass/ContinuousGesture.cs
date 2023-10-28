public abstract class ContinuousGesture : Gesture
{
	public ContinuousGesturePhase Phase
	{
		get
		{
			switch (base.State)
			{
			case GestureRecognitionState.Started:
				return ContinuousGesturePhase.Started;
			case GestureRecognitionState.InProgress:
				return ContinuousGesturePhase.Updated;
			case GestureRecognitionState.Failed:
			case GestureRecognitionState.Ended:
				return ContinuousGesturePhase.Ended;
			default:
				return ContinuousGesturePhase.None;
			}
		}
	}
}
