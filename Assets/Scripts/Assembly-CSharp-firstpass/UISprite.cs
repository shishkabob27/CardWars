using UnityEngine;

public class UISprite : UIWidget
{
	public enum Type
	{
		Simple = 0,
		Sliced = 1,
		Tiled = 2,
		Filled = 3,
	}

	public enum FillDirection
	{
		Horizontal = 0,
		Vertical = 1,
		Radial90 = 2,
		Radial180 = 3,
		Radial360 = 4,
	}

	[SerializeField]
	private UIAtlas mAtlas;
	[SerializeField]
	private string mSpriteName;
	[SerializeField]
	private bool mFillCenter;
	[SerializeField]
	private Type mType;
	[SerializeField]
	private FillDirection mFillDirection;
	[SerializeField]
	private float mFillAmount;
	[SerializeField]
	private bool mInvert;
}
