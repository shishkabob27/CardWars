using UnityEngine;

public class UISlider : IgnoreTimeScale
{
	public enum Direction
	{
		Horizontal = 0,
		Vertical = 1,
	}

	public Transform foreground;
	public Transform thumb;
	public Direction direction;
	public GameObject eventReceiver;
	public string functionName;
	public int numberOfSteps;
	[SerializeField]
	private float rawValue;
}
