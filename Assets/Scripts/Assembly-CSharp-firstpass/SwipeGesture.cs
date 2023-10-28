using System;
using UnityEngine;

[Serializable]
public class SwipeGesture : DiscreteGesture
{
	private Vector2 move = Vector2.zero;

	private float velocity;

	private FingerGestures.SwipeDirection direction;

	internal int MoveCounter;

	internal float Deviation;

	public Vector2 Move
	{
		get
		{
			return move;
		}
		internal set
		{
			move = value;
		}
	}

	public float Velocity
	{
		get
		{
			return velocity;
		}
		internal set
		{
			velocity = value;
		}
	}

	public FingerGestures.SwipeDirection Direction
	{
		get
		{
			return direction;
		}
		internal set
		{
			direction = value;
		}
	}
}
