using UnityEngine;

public class UIGrid : MonoBehaviour
{
	public enum Arrangement
	{
		Horizontal = 0,
		Vertical = 1,
	}

	public Arrangement arrangement;
	public int maxPerLine;
	public float cellWidth;
	public float cellHeight;
	public bool repositionNow;
	public bool sorted;
	public bool hideInactive;
}
