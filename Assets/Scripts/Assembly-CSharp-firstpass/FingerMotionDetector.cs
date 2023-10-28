using System.Runtime.CompilerServices;
using UnityEngine;

[AddComponentMenu("FingerGestures/Finger Events/Finger Motion Detector")]
public class FingerMotionDetector : FingerEventDetector<FingerMotionEvent>
{
	private enum EventType
	{
		Move,
		Stationary
	}

	public string MoveMessageName = "OnFingerMove";

	public string StationaryMessageName = "OnFingerStationary";

	public bool TrackMove = true;

	public bool TrackStationary = true;

	[method: MethodImpl(32)]
	public event FingerEventHandler OnFingerMove;

	[method: MethodImpl(32)]
	public event FingerEventHandler OnFingerStationary;

	private bool FireEvent(FingerMotionEvent e, EventType eventType, FingerMotionPhase phase, Vector2 position, bool updateSelection)
	{
		if ((!TrackMove && eventType == EventType.Move) || (!TrackStationary && eventType == EventType.Stationary))
		{
			return false;
		}
		e.Phase = phase;
		e.Position = position;
		if (e.Phase == FingerMotionPhase.Started)
		{
			e.StartTime = Time.time;
		}
		if (updateSelection)
		{
			UpdateSelection(e);
		}
		switch (eventType)
		{
		case EventType.Move:
			e.Name = MoveMessageName;
			if (this.OnFingerMove != null)
			{
				this.OnFingerMove(e);
			}
			TrySendMessage(e);
			break;
		case EventType.Stationary:
			e.Name = StationaryMessageName;
			if (this.OnFingerStationary != null)
			{
				this.OnFingerStationary(e);
			}
			TrySendMessage(e);
			break;
		default:
			return false;
		}
		return true;
	}

	protected override void ProcessFinger(FingerGestures.Finger finger)
	{
		FingerMotionEvent @event = GetEvent(finger);
		bool flag = false;
		if (finger.Phase != finger.PreviousPhase)
		{
			switch (finger.PreviousPhase)
			{
			case FingerGestures.FingerPhase.Moving:
				flag |= FireEvent(@event, EventType.Move, FingerMotionPhase.Ended, finger.Position, !flag);
				break;
			case FingerGestures.FingerPhase.Stationary:
				flag |= FireEvent(@event, EventType.Stationary, FingerMotionPhase.Ended, finger.PreviousPosition, !flag);
				break;
			}
			switch (finger.Phase)
			{
			case FingerGestures.FingerPhase.Moving:
				flag |= FireEvent(@event, EventType.Move, FingerMotionPhase.Started, finger.PreviousPosition, !flag);
				flag |= FireEvent(@event, EventType.Move, FingerMotionPhase.Updated, finger.Position, !flag);
				break;
			case FingerGestures.FingerPhase.Stationary:
				flag |= FireEvent(@event, EventType.Stationary, FingerMotionPhase.Started, finger.Position, !flag);
				flag |= FireEvent(@event, EventType.Stationary, FingerMotionPhase.Updated, finger.Position, !flag);
				break;
			}
		}
		else
		{
			switch (finger.Phase)
			{
			case FingerGestures.FingerPhase.Moving:
				flag |= FireEvent(@event, EventType.Move, FingerMotionPhase.Updated, finger.Position, !flag);
				break;
			case FingerGestures.FingerPhase.Stationary:
				flag |= FireEvent(@event, EventType.Stationary, FingerMotionPhase.Updated, finger.Position, !flag);
				break;
			}
		}
	}
}
