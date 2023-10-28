using System.Runtime.CompilerServices;
using UnityEngine;

[AddComponentMenu("FingerGestures/Finger Events/Finger Down Detector")]
public class FingerDownDetector : FingerEventDetector<FingerDownEvent>
{
	public string MessageName = "OnFingerDown";

	[method: MethodImpl(32)]
	public event FingerEventHandler OnFingerDown;

	protected override void ProcessFinger(FingerGestures.Finger finger)
	{
		if (finger.IsDown && !finger.WasDown)
		{
			FingerDownEvent @event = GetEvent(finger.Index);
			@event.Name = MessageName;
			UpdateSelection(@event);
			if (this.OnFingerDown != null)
			{
				this.OnFingerDown(@event);
			}
			TrySendMessage(@event);
		}
	}
}
