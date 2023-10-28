using UnityEngine;

public class FingerMotionEvent : FingerEvent
{
	private FingerMotionPhase phase;

	private Vector2 position = Vector2.zero;

	internal float StartTime;

	public override Vector2 Position
	{
		get
		{
			return position;
		}
		internal set
		{
			position = value;
		}
	}

	public FingerMotionPhase Phase
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

	public float ElapsedTime
	{
		get
		{
			return Mathf.Max(0f, Time.time - StartTime);
		}
	}
}
