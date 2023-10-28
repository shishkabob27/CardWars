using System.Runtime.CompilerServices;
using UnityEngine;

[AddComponentMenu("FingerGestures/Finger Events/Finger Hover Detector")]
public class FingerHoverDetector : FingerEventDetector<FingerHoverEvent>
{
	public string MessageName = "OnFingerHover";

	[method: MethodImpl(32)]
	public event FingerEventHandler OnFingerHover;

	protected override void Start()
	{
		base.Start();
		if ((bool)Raycaster)
		{
		}
	}

	private bool FireEvent(FingerHoverEvent e, FingerHoverPhase phase)
	{
		e.Name = MessageName;
		e.Phase = phase;
		if (this.OnFingerHover != null)
		{
			this.OnFingerHover(e);
		}
		TrySendMessage(e);
		return true;
	}

	protected override void ProcessFinger(FingerGestures.Finger finger)
	{
		FingerHoverEvent @event = GetEvent(finger);
		GameObject previousSelection = @event.PreviousSelection;
		GameObject gameObject = ((!finger.IsDown) ? null : PickObject(finger.Position));
		if (gameObject != previousSelection)
		{
			if ((bool)previousSelection)
			{
				FireEvent(@event, FingerHoverPhase.Exit);
			}
			if ((bool)gameObject)
			{
				@event.Selection = gameObject;
				@event.Hit = base.LastHit;
				FireEvent(@event, FingerHoverPhase.Enter);
			}
		}
		@event.PreviousSelection = gameObject;
	}
}
