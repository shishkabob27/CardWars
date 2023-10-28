public class PinchGesture : ContinuousGesture
{
	private float delta;

	private float gap;

	public float Delta
	{
		get
		{
			return delta;
		}
		set
		{
			delta = value;
		}
	}

	public float Gap
	{
		get
		{
			return gap;
		}
		set
		{
			gap = value;
		}
	}
}
