using UnityEngine;

public class DragGesture : ContinuousGesture
{
	private Vector2 deltaMove = Vector2.zero;

	public Vector2 LastPos = Vector2.zero;

	public Vector2 LastDelta = Vector2.zero;

	public Vector2 DeltaMove
	{
		get
		{
			return deltaMove;
		}
		set
		{
			deltaMove = value;
		}
	}

	public Vector2 TotalMove
	{
		get
		{
			return base.Position - base.StartPosition;
		}
	}
}
