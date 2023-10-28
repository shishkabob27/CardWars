using System.Runtime.CompilerServices;
using UnityEngine;

[AddComponentMenu("FingerGestures/Finger Events/Finger Up Detector")]
public class FingerUpDetector : FingerEventDetector<FingerUpEvent>
{
	public string MessageName = "OnFingerUp";

	[method: MethodImpl(32)]
	public event FingerEventHandler OnFingerUp;

	protected override void ProcessFinger(FingerGestures.Finger finger)
	{
		if (!finger.IsDown && finger.WasDown)
		{
			FingerUpEvent @event = GetEvent(finger);
			@event.Name = MessageName;
			@event.TimeHeldDown = Mathf.Max(0f, Time.time - finger.StarTime);
			UpdateSelection(@event);
			if (this.OnFingerUp != null)
			{
				this.OnFingerUp(@event);
			}
			TrySendMessage(@event);
		}
	}
}
