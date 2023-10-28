using System;
using UnityEngine;

public class FingerEvent
{
	private FingerEventDetector detector;

	private FingerGestures.Finger finger;

	private string name = string.Empty;

	private GameObject selection;

	private RaycastHit hit = default(RaycastHit);

	public string Name
	{
		get
		{
			return name;
		}
		internal set
		{
			name = value;
		}
	}

	public FingerEventDetector Detector
	{
		get
		{
			return detector;
		}
		internal set
		{
			detector = value;
		}
	}

	public FingerGestures.Finger Finger
	{
		get
		{
			return finger;
		}
		internal set
		{
			finger = value;
		}
	}

	public virtual Vector2 Position
	{
		get
		{
			return finger.Position;
		}
		internal set
		{
			throw new NotSupportedException("Setting position is not supported on " + GetType());
		}
	}

	public GameObject Selection
	{
		get
		{
			return selection;
		}
		internal set
		{
			selection = value;
		}
	}

	public RaycastHit Hit
	{
		get
		{
			return hit;
		}
		internal set
		{
			hit = value;
		}
	}
}
