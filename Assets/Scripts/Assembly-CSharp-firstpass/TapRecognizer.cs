public class TapRecognizer : DiscreteGestureRecognizer<TapGesture>
{
	public int RequiredTaps;
	public float MoveTolerance;
	public float MaxDuration;
	public float MaxDelayBetweenTaps;
}
