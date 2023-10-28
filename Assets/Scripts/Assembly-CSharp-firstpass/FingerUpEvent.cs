public class FingerUpEvent : FingerEvent
{
	private float timeHeldDown;

	public float TimeHeldDown
	{
		get
		{
			return timeHeldDown;
		}
		internal set
		{
			timeHeldDown = value;
		}
	}
}
