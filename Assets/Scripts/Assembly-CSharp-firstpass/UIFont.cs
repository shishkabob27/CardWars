using UnityEngine;
using System.Collections.Generic;

public class UIFont : MonoBehaviour
{
	public enum SymbolStyle
	{
		None = 0,
		Uncolored = 1,
		Colored = 2,
	}

	[SerializeField]
	private Material mMat;
	[SerializeField]
	private Rect mUVRect;
	[SerializeField]
	private BMFont mFont;
	[SerializeField]
	private int mSpacingX;
	[SerializeField]
	private int mSpacingY;
	[SerializeField]
	private UIAtlas mAtlas;
	[SerializeField]
	private UIFont mReplacement;
	[SerializeField]
	private float mPixelSize;
	[SerializeField]
	private List<BMSymbol> mSymbols;
	[SerializeField]
	private Font mDynamicFont;
	[SerializeField]
	private int mDynamicFontSize;
	[SerializeField]
	private FontStyle mDynamicFontStyle;
	[SerializeField]
	private float mDynamicFontOffset;
}
