using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class BMFont
{
	[SerializeField]
	private int mSize;
	[SerializeField]
	private int mBase;
	[SerializeField]
	private int mWidth;
	[SerializeField]
	private int mHeight;
	[SerializeField]
	private string mSpriteName;
	[SerializeField]
	private List<BMGlyph> mSaved;
}
