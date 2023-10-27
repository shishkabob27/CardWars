using UnityEngine;

public class SpringPosition : IgnoreTimeScale
{
	public Vector3 target;
	public float strength;
	public bool worldSpace;
	public bool ignoreTimeScale;
	public GameObject eventReceiver;
	public string callWhenFinished;
}
