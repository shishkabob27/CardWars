using UnityEngine;

public class UIDragObject : IgnoreTimeScale
{
	public enum DragEffect
	{
		None = 0,
		Momentum = 1,
		MomentumAndSpring = 2,
	}

	public Transform target;
	public Vector3 scale;
	public float scrollWheelFactor;
	public bool restrictWithinPanel;
	public DragEffect dragEffect;
	public float momentumAmount;
}
