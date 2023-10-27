using UnityEngine;

public class UIFastGrid : MonoBehaviour
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
}
