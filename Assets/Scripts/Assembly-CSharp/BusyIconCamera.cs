using UnityEngine;

public class BusyIconCamera : MonoBehaviour
{
	public enum Placement
	{
		TopLeft = 0,
		TopRight = 1,
		BottomLeft = 2,
		BottomRight = 3,
	}

	public Placement placement;
}
