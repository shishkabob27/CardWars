using UnityEngine;

public class UIDraggablePanel : IgnoreTimeScale
{
	public enum PanelRestrictionOrientation
	{
		AllDirections = 0,
		HorizontalOnly = 1,
		VerticalOnly = 2,
	}

	public enum DragEffect
	{
		None = 0,
		Momentum = 1,
		MomentumAndSpring = 2,
	}

	public enum ShowCondition
	{
		Always = 0,
		OnlyIfNeeded = 1,
		WhenDragging = 2,
	}

	public bool restrictWithinPanel;
	public PanelRestrictionOrientation restrictionOrientation;
	public bool disableDragIfFits;
	public DragEffect dragEffect;
	public bool smoothDragStart;
	public Vector3 scale;
	public float scrollWheelFactor;
	public float momentumAmount;
	public Vector2 relativePositionOnReset;
	public bool repositionClipping;
	public bool iOSDragEmulation;
	public UIScrollBar horizontalScrollBar;
	public UIScrollBar verticalScrollBar;
	public ShowCondition showScrollBars;
}
