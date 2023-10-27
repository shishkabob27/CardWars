using UnityEngine;

public class UIWidget : MonoBehaviour
{
	public enum Pivot
	{
		TopLeft = 0,
		Top = 1,
		TopRight = 2,
		Left = 3,
		Center = 4,
		Right = 5,
		BottomLeft = 6,
		Bottom = 7,
		BottomRight = 8,
	}

	[SerializeField]
	private Color mColor;
	[SerializeField]
	private Pivot mPivot;
	[SerializeField]
	private int mDepth;
}
