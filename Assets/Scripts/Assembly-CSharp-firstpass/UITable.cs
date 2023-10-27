using UnityEngine;

public class UITable : MonoBehaviour
{
	public enum Direction
	{
		Down = 0,
		Up = 1,
	}

	public int columns;
	public Direction direction;
	public Vector2 padding;
	public bool sorted;
	public bool hideInactive;
	public bool repositionNow;
	public bool keepWithinPanel;
}
