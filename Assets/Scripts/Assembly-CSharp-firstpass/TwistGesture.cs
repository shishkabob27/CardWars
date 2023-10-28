public class TwistGesture : ContinuousGesture
{
	private float deltaRotation;

	private float totalRotation;

	public float DeltaRotation
	{
		get
		{
			return deltaRotation;
		}
		internal set
		{
			deltaRotation = value;
		}
	}

	public float TotalRotation
	{
		get
		{
			return totalRotation;
		}
		internal set
		{
			totalRotation = value;
		}
	}
}
