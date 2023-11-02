//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Unity doesn't keep the values of static variables after scripts change get recompiled. One way around this
/// is to store the references in EditorPrefs -- retrieve them at start, and save them whenever something changes.
/// </summary>

public class NGUISettings
{
	static bool mLoaded = false;
	static UIFont mFont;
	static UIAtlas mAtlas;
	static UIWidget.Pivot mPivot = UIWidget.Pivot.Center;
	static TextAsset mFontData;
	static Texture2D mFontTexture;
	static string mPartial = "";
	static string mFontName = "New Font";
	static string mAtlasName = "New Atlas";
	static string mSpriteName;
	static int mAtlasPadding = 1;
	static public bool mAtlasTrimming = true;
	static public bool mAtlasPMA = false;
	static bool mUnityPacking = true;
	static bool mForceSquare = true;
	static bool mAllow4096 = false;
	static Color mColor = Color.white;
	static int mLayer = 0;
	static Font mDynFont;
	static int mDynFontSize = 16;
	static FontStyle mDynFontStyle = FontStyle.Normal;

	static Object GetObject (string name)
	{
		int assetID = EditorPrefs.GetInt(name, -1);
		return (assetID != -1) ? EditorUtility.InstanceIDToObject(assetID) : null;
	}

	static void Load ()
	{
		mLoaded			= true;
		mPartial		= EditorPrefs.GetString("NGUI Partial");
		mFontName		= EditorPrefs.GetString("NGUI Font Name");
		mAtlasName		= EditorPrefs.GetString("NGUI Atlas Name");
		mSpriteName		= EditorPrefs.GetString("NGUI Selected Sprite");
		mFontData		= GetObject("NGUI Font Asset") as TextAsset;
		mFontTexture	= GetObject("NGUI Font Texture") as Texture2D;
		mFont			= GetObject("NGUI Font") as UIFont;
		mAtlas			= GetObject("NGUI Atlas") as UIAtlas;
		mAtlasPadding	= EditorPrefs.GetInt("NGUI Atlas Padding", 1);
		mAtlasTrimming	= EditorPrefs.GetBool("NGUI Atlas Trimming", true);
		mAtlasPMA		= EditorPrefs.GetBool("NGUI Atlas PMA", true);
		mUnityPacking	= EditorPrefs.GetBool("NGUI Unity Packing", true);
		mForceSquare	= EditorPrefs.GetBool("NGUI Force Square Atlas", true);
		mPivot			= (UIWidget.Pivot)EditorPrefs.GetInt("NGUI Pivot", (int)mPivot);
		mLayer			= EditorPrefs.GetInt("NGUI Layer", -1);
		mDynFont		= GetObject("NGUI DynFont") as Font;
		mDynFontSize	= EditorPrefs.GetInt("NGUI DynFontSize", 16);
		mDynFontStyle	= (FontStyle)EditorPrefs.GetInt("NGUI DynFontStyle", (int)FontStyle.Normal);

		if (mLayer < 0 || string.IsNullOrEmpty(LayerMask.LayerToName(mLayer))) mLayer = -1;

		if (mLayer == -1) mLayer = LayerMask.NameToLayer("UI");
		if (mLayer == -1) mLayer = LayerMask.NameToLayer("GUI");
		if (mLayer == -1) mLayer = 5;

		EditorPrefs.SetInt("UI Layer", mLayer);

		LoadColor();
	}

	static void Save ()
	{
		EditorPrefs.SetString("NGUI Partial", mPartial);
		EditorPrefs.SetString("NGUI Font Name", mFontName);
		EditorPrefs.SetString("NGUI Atlas Name", mAtlasName);
		EditorPrefs.SetString("NGUI Selected Sprite", mSpriteName);
		EditorPrefs.SetInt("NGUI Font Asset", (mFontData != null) ? mFontData.GetInstanceID() : -1);
		EditorPrefs.SetInt("NGUI Font Texture", (mFontTexture != null) ? mFontTexture.GetInstanceID() : -1);
		EditorPrefs.SetInt("NGUI Font", (mFont != null) ? mFont.GetInstanceID() : -1);
		EditorPrefs.SetInt("NGUI Atlas", (mAtlas != null) ? mAtlas.GetInstanceID() : -1);
		EditorPrefs.SetInt("NGUI Atlas Padding", mAtlasPadding);
		EditorPrefs.SetBool("NGUI Atlas Trimming", mAtlasTrimming);
		EditorPrefs.SetBool("NGUI Atlas PMA", mAtlasPMA);
		EditorPrefs.SetBool("NGUI Unity Packing", mUnityPacking);
		EditorPrefs.SetBool("NGUI Force Square Atlas", mForceSquare);
		EditorPrefs.SetInt("NGUI Pivot", (int)mPivot);
		EditorPrefs.SetInt("NGUI Layer", mLayer);
		EditorPrefs.SetInt("NGUI DynFont", (mDynFont != null) ? mDynFont.GetInstanceID() : -1);
		EditorPrefs.SetInt("NGUI DynFontSize", mDynFontSize);
		EditorPrefs.SetInt("NGUI DynFontStyle", (int)mDynFontStyle);

		SaveColor();
	}

	static void LoadColor ()
	{
		string sc = EditorPrefs.GetString("NGUI Color");

		if (!string.IsNullOrEmpty(sc))
		{
			string[] colors = sc.Split(' ');

			if (colors.Length == 4)
			{
				float.TryParse(colors[0], out mColor.r);
				float.TryParse(colors[1], out mColor.g);
				float.TryParse(colors[2], out mColor.b);
				float.TryParse(colors[3], out mColor.a);
			}
		}
	}

	static void SaveColor ()
	{
		EditorPrefs.SetString("NGUI Color", mColor.r + " " + mColor.g + " " + mColor.b + " " + mColor.a);
	}

	/// <summary>
	/// Color is used to easily copy/paste the widget's color value.
	/// </summary>

	static public Color color
	{
		get
		{
			if (!mLoaded) Load();
			return mColor;
		}
		set
		{
			if (mColor != value)
			{
				mColor = value;
				SaveColor();
			}
		}
	}

	/// <summary>
	/// Default bitmap font used by NGUI.
	/// </summary>

	static public UIFont font
	{
		get
		{
			if (!mLoaded) Load();
			return mFont;
		}
		set
		{
			if (mFont != value)
			{
				mFont = value;
				mFontName = (mFont != null) ? mFont.name : "New Font";
				Save();
			}
		}
	}

	/// <summary>
	/// Default dynamic font used by NGUI.
	/// </summary>

	static public Font dynamicFont
	{
		get
		{
			if (!mLoaded) Load();
			return mDynFont;
		}
		set
		{
			if (mDynFont != value)
			{
				mDynFont = value;
				mFontName = (mDynFont != null) ? mDynFont.name : "New Font";
				Save();
			}
		}
	}

	/// <summary>
	/// Default atlas used by NGUI.
	/// </summary>

	static public UIAtlas atlas
	{
		get
		{
			if (!mLoaded) Load();
			return mAtlas;
		}
		set
		{
			if (mAtlas != value)
			{
				mAtlas = value;
				mAtlasName = (mAtlas != null) ? mAtlas.name : "New Atlas";
				Save();
			}
		}
	}

	/// <summary>
	/// Currently selected sprite.
	/// </summary>

	static public string selectedSprite
	{
		get
		{
			if (!mLoaded) Load();
			return mSpriteName;
		}
		set
		{
			if (mSpriteName != value)
			{
				mSpriteName = value;
				Save();
			}
		}
	}

	/// <summary>
	/// Default pivot point used by sprites.
	/// </summary>

	static public UIWidget.Pivot pivot
	{
		get
		{
			if (!mLoaded) Load();
			return mPivot;
		}
		set
		{
			if (mPivot != value)
			{
				mPivot = value;
				Save();
			}
		}
	}

	/// <summary>
	/// Default layer used by the UI.
	/// </summary>

	static public int layer
	{
		get
		{
			if (!mLoaded) Load();
			return mLayer;
		}
		set
		{
			if (mLayer != value)
			{
				mLayer = value;
				Save();
			}
		}
	}

	/// <summary>
	/// Name of the font, used by the Font Maker.
	/// </summary>

	static public string fontName { get { if (!mLoaded) Load(); return mFontName; } set { if (mFontName != value) { mFontName = value; Save(); } } }

	/// <summary>
	/// Data used to create the font, used by the Font Maker.
	/// </summary>

	static public TextAsset fontData { get { if (!mLoaded) Load(); return mFontData; } set { if (mFontData != value) { mFontData = value; Save(); } } }

	/// <summary>
	/// Texture used to create the font, used by the Font Maker.
	/// </summary>

	static public Texture2D fontTexture { get { if (!mLoaded) Load(); return mFontTexture; } set { if (mFontTexture != value) { mFontTexture = value; Save(); } } }

	/// <summary>
	/// Name of the atlas, used by the Atlas maker.
	/// </summary>

	static public string atlasName { get { if (!mLoaded) Load(); return mAtlasName; } set { if (mAtlasName != value) { mAtlasName = value; Save(); } } }

	/// <summary>
	/// Size of the dynamic font.
	/// </summary>

	static public int dynamicFontSize { get { if (!mLoaded) Load(); return mDynFontSize; } set { if (mDynFontSize != value) { mDynFontSize = value; Save(); } } }

	/// <summary>
	/// Dynamic font's style.
	/// </summary>

	static public FontStyle dynamicFontStyle { get { if (!mLoaded) Load(); return mDynFontStyle; } set { if (mDynFontStyle != value) { mDynFontStyle = value; Save(); } } }

	/// <summary>
	/// Name of the partial sprite name, used to filter sprites.
	/// </summary>

	static public string partialSprite
	{
		get
		{
			if (!mLoaded) Load();
			return mPartial;
		}
		set
		{
			if (mPartial != value)
			{
				mPartial = value;
				EditorPrefs.SetString("NGUI Partial", mPartial);
			}
		}
	}

	/// <summary>
	/// Added padding in-between of sprites when creating an atlas.
	/// </summary>

	static public int atlasPadding { get { if (!mLoaded) Load(); return mAtlasPadding; } set { if (mAtlasPadding != value) { mAtlasPadding = value; Save(); } } }

	/// <summary>
	/// Whether the transparent pixels will be trimmed away when creating an atlas.
	/// </summary>

	static public bool atlasTrimming { get { if (!mLoaded) Load(); return mAtlasTrimming; } set { if (mAtlasTrimming != value) { mAtlasTrimming = value; Save(); } } }

	/// <summary>
	/// Whether the transparent pixels will affect the color.
	/// </summary>

	static public bool atlasPMA { get { if (!mLoaded) Load(); return mAtlasPMA; } set { if (mAtlasPMA != value) { mAtlasPMA = value; Save(); } } }

	/// <summary>
	/// Whether Unity's method or MaxRectBinPack will be used when creating an atlas
	/// </summary>

	static public bool unityPacking { get { if (!mLoaded) Load(); return mUnityPacking; } set { if (mUnityPacking != value) { mUnityPacking = value; Save(); } } }
	
	/// <summary>
	/// Whether the Atlas Maker will force a square atlas texture when creating an atlas
	/// </summary>
	
	static public bool forceSquareAtlas { get { if (!mLoaded) Load(); return mForceSquare; } set { if (mForceSquare != value) { mForceSquare = value; Save(); } } }

	/// <summary>
	/// Whether the atlas maker will allow 4096 width/height textures on mobiles.
	/// </summary>

	static public bool allow4096 { get { if (!mLoaded) Load(); return mAllow4096; } set { if (mAllow4096 != value) { mAllow4096 = value; Save(); } } }
}
