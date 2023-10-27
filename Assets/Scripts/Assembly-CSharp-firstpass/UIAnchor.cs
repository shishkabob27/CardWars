using UnityEngine;

public class UIAnchor : MonoBehaviour
{
	public enum Side
	{
		BottomLeft = 0,
		Left = 1,
		TopLeft = 2,
		Top = 3,
		TopRight = 4,
		Right = 5,
		BottomRight = 6,
		Bottom = 7,
		Center = 8,
	}

	public Camera uiCamera;
	public UIWidget widgetContainer;
	public UIPanel panelContainer;
	public Side side;
	public bool halfPixelOffset;
	public bool runOnlyOnce;
	public Vector2 relativeOffset;
	public Vector2 pixelOffset;
}
