using UnityEngine;

public class FingerHoverEvent : FingerEvent
{
	private FingerHoverPhase phase;

	internal GameObject PreviousSelection;

	public FingerHoverPhase Phase
	{
		get
		{
			return phase;
		}
		internal set
		{
			phase = value;
		}
	}
}
