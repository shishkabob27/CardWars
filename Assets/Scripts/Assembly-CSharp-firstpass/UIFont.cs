using System.Collections.Generic;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Font")]
public class UIFont : MonoBehaviour
{
	public enum Alignment
	{
		Left,
		Center,
		Right
	}

	public enum SymbolStyle
	{
		None,
		Uncolored,
		Colored
	}

	[HideInInspector]
	[SerializeField]
	private Material mMat;

	[HideInInspector]
	[SerializeField]
	private Rect mUVRect = new Rect(0f, 0f, 1f, 1f);

	[HideInInspector]
	[SerializeField]
	private BMFont mFont = new BMFont();

	[HideInInspector]
	[SerializeField]
	private int mSpacingX;

	[SerializeField]
	[HideInInspector]
	private int mSpacingY;

	[SerializeField]
	[HideInInspector]
	private UIAtlas mAtlas;

	[HideInInspector]
	[SerializeField]
	private UIFont mReplacement;

	[HideInInspector]
	[SerializeField]
	private float mPixelSize = 1f;

	[SerializeField]
	[HideInInspector]
	private List<BMSymbol> mSymbols = new List<BMSymbol>();

	[SerializeField]
	[HideInInspector]
	private Font mDynamicFont;

	[SerializeField]
	[HideInInspector]
	private int mDynamicFontSize = 16;

	[HideInInspector]
	[SerializeField]
	private FontStyle mDynamicFontStyle;

	[HideInInspector]
	[SerializeField]
	private float mDynamicFontOffset;

	private UIAtlas.Sprite mSprite;

	private int mPMA = -1;

	private bool mSpriteSet;

	private List<Color> mColors = new List<Color>();

	private static CharacterInfo mTemp;

	private static CharacterInfo mChar;

	public BMFont bmFont
	{
		get
		{
			return (!(mReplacement != null)) ? mFont : mReplacement.bmFont;
		}
	}

	public int texWidth
	{
		get
		{
			return (mReplacement != null) ? mReplacement.texWidth : ((mFont == null) ? 1 : mFont.texWidth);
		}
	}

	public int texHeight
	{
		get
		{
			return (mReplacement != null) ? mReplacement.texHeight : ((mFont == null) ? 1 : mFont.texHeight);
		}
	}

	public bool hasSymbols
	{
		get
		{
			return (!(mReplacement != null)) ? (mSymbols.Count != 0) : mReplacement.hasSymbols;
		}
	}

	public List<BMSymbol> symbols
	{
		get
		{
			return (!(mReplacement != null)) ? mSymbols : mReplacement.symbols;
		}
	}

	public UIAtlas atlas
	{
		get
		{
			return (!(mReplacement != null)) ? mAtlas : mReplacement.atlas;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.atlas = value;
			}
			else
			{
				if (!(mAtlas != value))
				{
					return;
				}
				if (value == null)
				{
					if (mAtlas != null)
					{
						mMat = mAtlas.spriteMaterial;
					}
					if (sprite != null)
					{
						mUVRect = uvRect;
					}
				}
				mPMA = -1;
				mAtlas = value;
				MarkAsDirty();
			}
		}
	}

	public Material material
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.material;
			}
			if (mAtlas != null)
			{
				return mAtlas.spriteMaterial;
			}
			if (mMat != null)
			{
				if (mDynamicFont != null && mMat != mDynamicFont.material)
				{
					mMat.mainTexture = mDynamicFont.material.mainTexture;
				}
				return mMat;
			}
			if (mDynamicFont != null)
			{
				return mDynamicFont.material;
			}
			return null;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.material = value;
			}
			else if (mMat != value)
			{
				mPMA = -1;
				mMat = value;
				MarkAsDirty();
			}
		}
	}

	public float pixelSize
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.pixelSize;
			}
			if (mAtlas != null)
			{
				return mAtlas.pixelSize;
			}
			return mPixelSize;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.pixelSize = value;
				return;
			}
			if (mAtlas != null)
			{
				mAtlas.pixelSize = value;
				return;
			}
			float num = Mathf.Clamp(value, 0.25f, 4f);
			if (mPixelSize != num)
			{
				mPixelSize = num;
				MarkAsDirty();
			}
		}
	}

	public bool premultipliedAlpha
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.premultipliedAlpha;
			}
			if (mAtlas != null)
			{
				return mAtlas.premultipliedAlpha;
			}
			if (mPMA == -1)
			{
				Material material = this.material;
				mPMA = ((material != null && material.shader != null && material.shader.name.Contains("Premultiplied")) ? 1 : 0);
			}
			return mPMA == 1;
		}
	}

	public Texture2D texture
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.texture;
			}
			Material material = this.material;
			return (!(material != null)) ? null : (material.mainTexture as Texture2D);
		}
	}

	public Rect uvRect
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.uvRect;
			}
			if (mAtlas != null && mSprite == null && sprite != null)
			{
				Texture texture = mAtlas.texture;
				if (texture != null)
				{
					mUVRect = mSprite.outer;
					if (mAtlas.coordinates == UIAtlas.Coordinates.Pixels)
					{
						mUVRect = NGUIMath.ConvertToTexCoords(mUVRect, texture.width, texture.height);
					}
					if (mSprite.hasPadding)
					{
						Rect rect = mUVRect;
						mUVRect.xMin = rect.xMin - mSprite.paddingLeft * rect.width;
						mUVRect.yMin = rect.yMin - mSprite.paddingBottom * rect.height;
						mUVRect.xMax = rect.xMax + mSprite.paddingRight * rect.width;
						mUVRect.yMax = rect.yMax + mSprite.paddingTop * rect.height;
					}
					if (mSprite.hasPadding)
					{
						Trim();
					}
				}
			}
			return mUVRect;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.uvRect = value;
			}
			else if (sprite == null && mUVRect != value)
			{
				mUVRect = value;
				MarkAsDirty();
			}
		}
	}

	public string spriteName
	{
		get
		{
			return (!(mReplacement != null)) ? mFont.spriteName : mReplacement.spriteName;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.spriteName = value;
			}
			else if (mFont.spriteName != value)
			{
				mFont.spriteName = value;
				MarkAsDirty();
			}
		}
	}

	public int horizontalSpacing
	{
		get
		{
			return (!(mReplacement != null)) ? mSpacingX : mReplacement.horizontalSpacing;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.horizontalSpacing = value;
			}
			else if (mSpacingX != value)
			{
				mSpacingX = value;
				MarkAsDirty();
			}
		}
	}

	public int verticalSpacing
	{
		get
		{
			return (!(mReplacement != null)) ? mSpacingY : mReplacement.verticalSpacing;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.verticalSpacing = value;
			}
			else if (mSpacingY != value)
			{
				mSpacingY = value;
				MarkAsDirty();
			}
		}
	}

	public bool isValid
	{
		get
		{
			return mDynamicFont != null || mFont.isValid;
		}
	}

	public int size
	{
		get
		{
			return (mReplacement != null) ? mReplacement.size : ((!isDynamic) ? mFont.charSize : mDynamicFontSize);
		}
	}

	public UIAtlas.Sprite sprite
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.sprite;
			}
			if (!mSpriteSet)
			{
				mSprite = null;
			}
			if (mSprite == null)
			{
				if (mAtlas != null && !string.IsNullOrEmpty(mFont.spriteName))
				{
					mSprite = mAtlas.GetSprite(mFont.spriteName);
					if (mSprite == null)
					{
						mSprite = mAtlas.GetSprite(base.name);
					}
					mSpriteSet = true;
					if (mSprite == null)
					{
						mFont.spriteName = null;
					}
				}
				int i = 0;
				for (int count = mSymbols.Count; i < count; i++)
				{
					symbols[i].MarkAsDirty();
				}
			}
			return mSprite;
		}
	}

	public UIFont replacement
	{
		get
		{
			return mReplacement;
		}
		set
		{
			UIFont uIFont = value;
			if (uIFont == this)
			{
				uIFont = null;
			}
			if (mReplacement != uIFont)
			{
				if (uIFont != null && uIFont.replacement == this)
				{
					uIFont.replacement = null;
				}
				if (mReplacement != null)
				{
					MarkAsDirty();
				}
				mReplacement = uIFont;
				MarkAsDirty();
			}
		}
	}

	public bool isDynamic
	{
		get
		{
			return mDynamicFont != null;
		}
	}

	public Font dynamicFont
	{
		get
		{
			return (!(mReplacement != null)) ? mDynamicFont : mReplacement.dynamicFont;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.dynamicFont = value;
			}
			else if (mDynamicFont != value)
			{
				if (mDynamicFont != null)
				{
					material = null;
				}
				mDynamicFont = value;
				MarkAsDirty();
			}
		}
	}

	public int dynamicFontSize
	{
		get
		{
			return (!(mReplacement != null)) ? mDynamicFontSize : mReplacement.dynamicFontSize;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.dynamicFontSize = value;
				return;
			}
			value = Mathf.Clamp(value, 4, 128);
			if (mDynamicFontSize != value)
			{
				mDynamicFontSize = value;
				MarkAsDirty();
			}
		}
	}

	public FontStyle dynamicFontStyle
	{
		get
		{
			return (!(mReplacement != null)) ? mDynamicFontStyle : mReplacement.dynamicFontStyle;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.dynamicFontStyle = value;
			}
			else if (mDynamicFontStyle != value)
			{
				mDynamicFontStyle = value;
				MarkAsDirty();
			}
		}
	}

	private Texture dynamicTexture
	{
		get
		{
			if ((bool)mReplacement)
			{
				return mReplacement.dynamicTexture;
			}
			if (isDynamic)
			{
				return mDynamicFont.material.mainTexture;
			}
			return null;
		}
	}

	private void Trim()
	{
		Texture texture = mAtlas.texture;
		if (texture != null && mSprite != null)
		{
			Rect rect = NGUIMath.ConvertToPixels(mUVRect, this.texture.width, this.texture.height, true);
			Rect rect2 = ((mAtlas.coordinates != UIAtlas.Coordinates.TexCoords) ? mSprite.outer : NGUIMath.ConvertToPixels(mSprite.outer, texture.width, texture.height, true));
			int xMin = Mathf.RoundToInt(rect2.xMin - rect.xMin);
			int yMin = Mathf.RoundToInt(rect2.yMin - rect.yMin);
			int xMax = Mathf.RoundToInt(rect2.xMax - rect.xMin);
			int yMax = Mathf.RoundToInt(rect2.yMax - rect.yMin);
			mFont.Trim(xMin, yMin, xMax, yMax);
		}
	}

	private bool References(UIFont font)
	{
		if (font == null)
		{
			return false;
		}
		if (font == this)
		{
			return true;
		}
		return mReplacement != null && mReplacement.References(font);
	}

	public static bool CheckIfRelated(UIFont a, UIFont b)
	{
		if (a == null || b == null)
		{
			return false;
		}
		if (a.isDynamic && b.isDynamic && a.dynamicFont.fontNames[0] == b.dynamicFont.fontNames[0])
		{
			return true;
		}
		return a == b || a.References(b) || b.References(a);
	}

	public void MarkAsDirty()
	{
		if (mReplacement != null)
		{
			mReplacement.MarkAsDirty();
		}
		mSprite = null;
		UILabel[] array = NGUITools.FindActive<UILabel>();
		int i = 0;
		for (int num = array.Length; i < num; i++)
		{
			UILabel uILabel = array[i];
			if (uILabel.enabled && NGUITools.GetActive(uILabel.gameObject) && CheckIfRelated(this, uILabel.font))
			{
				UIFont font = uILabel.font;
				uILabel.font = null;
				uILabel.font = font;
			}
		}
		int j = 0;
		for (int count = mSymbols.Count; j < count; j++)
		{
			symbols[j].MarkAsDirty();
		}
	}

	public void Request(string text)
	{
		if (!string.IsNullOrEmpty(text))
		{
			if (mReplacement != null)
			{
				mReplacement.Request(text);
			}
			else if (mDynamicFont != null)
			{
				mDynamicFont.RequestCharactersInTexture("j", mDynamicFontSize, mDynamicFontStyle);
				mDynamicFont.GetCharacterInfo('j', out mTemp, mDynamicFontSize, mDynamicFontStyle);
				mDynamicFontOffset = mDynamicFontSize + mTemp.minY;
				mDynamicFont.RequestCharactersInTexture(text, mDynamicFontSize, mDynamicFontStyle);
			}
		}
	}

	public Vector2 CalculatePrintedSize(string text, bool encoding, SymbolStyle symbolStyle)
	{
		if (mReplacement != null)
		{
			return mReplacement.CalculatePrintedSize(text, encoding, symbolStyle);
		}
		Vector2 zero = Vector2.zero;
		bool flag = isDynamic;
		if (flag || (mFont != null && mFont.isValid && !string.IsNullOrEmpty(text)))
		{
			if (encoding)
			{
				text = NGUITools.StripSymbols(text);
			}
			if (mDynamicFont != null)
			{
				mDynamicFont.RequestCharactersInTexture(text, mDynamicFontSize, mDynamicFontStyle);
			}
			int length = text.Length;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = size;
			int num6 = num5 + mSpacingY;
			bool flag2 = encoding && symbolStyle != 0 && hasSymbols;
			for (int i = 0; i < length; i++)
			{
				char c = text[i];
				if (c == '\n')
				{
					if (num2 > num)
					{
						num = num2;
					}
					num2 = 0;
					num3 += num6;
					num4 = 0;
				}
				else if (c < ' ')
				{
					num4 = 0;
				}
				else if (!flag)
				{
					BMSymbol bMSymbol = ((!flag2) ? null : MatchSymbol(text, i, length));
					if (bMSymbol == null)
					{
						BMGlyph glyph = mFont.GetGlyph(c);
						if (glyph != null)
						{
							num2 += mSpacingX + ((num4 == 0) ? glyph.advance : (glyph.advance + glyph.GetKerning(num4)));
							num4 = c;
						}
					}
					else
					{
						num2 += mSpacingX + bMSymbol.width;
						i += bMSymbol.length - 1;
						num4 = 0;
					}
				}
				else if (mDynamicFont.GetCharacterInfo(c, out mChar, mDynamicFontSize, mDynamicFontStyle))
				{
					num2 += mSpacingX + mChar.advance;
				}
			}
			float num7 = ((num5 <= 0) ? 1f : (1f / (float)num5));
			zero.x = num7 * (float)((num2 <= num) ? num : num2);
			zero.y = num7 * (float)(num3 + num6);
		}
		return zero;
	}

	private static void EndLine(ref StringBuilder s)
	{
		int num = s.Length - 1;
		if (num > 0 && s[num] == ' ')
		{
			s[num] = '\n';
		}
		else
		{
			s.Append('\n');
		}
	}

	public string GetEndOfLineThatFits(string text, float maxWidth, bool encoding, SymbolStyle symbolStyle)
	{
		if (mReplacement != null)
		{
			return mReplacement.GetEndOfLineThatFits(text, maxWidth, encoding, symbolStyle);
		}
		int num = Mathf.RoundToInt(maxWidth * (float)size);
		if (num < 1)
		{
			return text;
		}
		if (mDynamicFont != null)
		{
			mDynamicFont.RequestCharactersInTexture(text, mDynamicFontSize, mDynamicFontStyle);
		}
		int length = text.Length;
		int num2 = num;
		BMGlyph bMGlyph = null;
		int num3 = length;
		bool flag = encoding && symbolStyle != 0 && hasSymbols;
		bool flag2 = isDynamic;
		while (num3 > 0 && num2 > 0)
		{
			char c = text[--num3];
			BMSymbol bMSymbol = ((!flag) ? null : MatchSymbol(text, num3, length));
			int num4 = mSpacingX;
			if (!flag2)
			{
				if (bMSymbol != null)
				{
					num4 += bMSymbol.advance;
				}
				else
				{
					BMGlyph glyph = mFont.GetGlyph(c);
					if (glyph == null)
					{
						bMGlyph = null;
						continue;
					}
					num4 += glyph.advance + ((bMGlyph != null) ? bMGlyph.GetKerning(c) : 0);
					bMGlyph = glyph;
				}
			}
			else if (mDynamicFont.GetCharacterInfo(c, out mChar, mDynamicFontSize, mDynamicFontStyle))
			{
				num4 += mChar.advance;
			}
			num2 -= num4;
		}
		if (num2 < 0)
		{
			num3++;
		}
		return text.Substring(num3, length - num3);
	}

	public bool WrapText(string text, out string finalText, float width, float height, int lines, bool encoding, SymbolStyle symbolStyle)
	{
		if (mReplacement != null)
		{
			return mReplacement.WrapText(text, out finalText, width, height, lines, encoding, symbolStyle);
		}
		if (width == 0f)
		{
			width = 100000f;
		}
		if (height == 0f)
		{
			height = 100000f;
		}
		int num = Mathf.FloorToInt(width * (float)size);
		int num2 = Mathf.FloorToInt(height * (float)size);
		if (num < 1 || num2 < 1)
		{
			finalText = string.Empty;
			return false;
		}
		int num3 = ((lines <= 0) ? 999999 : lines);
		if (height != 0f)
		{
			num3 = Mathf.Min(num3, Mathf.FloorToInt(height));
			if (num3 == 0)
			{
				finalText = string.Empty;
				return false;
			}
		}
		if (mDynamicFont != null)
		{
			mDynamicFont.RequestCharactersInTexture(text, mDynamicFontSize, mDynamicFontStyle);
		}
		StringBuilder s = new StringBuilder();
		int length = text.Length;
		int num4 = num;
		int num5 = 0;
		int i = 0;
		int j = 0;
		bool flag = true;
		bool flag2 = lines != 1;
		int num6 = 1;
		bool flag3 = encoding && symbolStyle != 0 && hasSymbols;
		bool flag4 = isDynamic;
		for (; j < length; j++)
		{
			char c = text[j];
			if (c == '\n')
			{
				if (!flag2 || num6 == num3)
				{
					break;
				}
				num4 = num;
				if (i < j)
				{
					s.Append(text.Substring(i, j - i + 1));
				}
				else
				{
					s.Append(c);
				}
				flag = true;
				num6++;
				i = j + 1;
				num5 = 0;
				continue;
			}
			if (c == ' ' && num5 != 32 && i < j)
			{
				s.Append(text.Substring(i, j - i + 1));
				flag = false;
				i = j + 1;
				num5 = c;
			}
			if (encoding && c == '[' && j + 2 < length)
			{
				if (text[j + 1] == '-' && text[j + 2] == ']')
				{
					j += 2;
					continue;
				}
				if (j + 7 < length && text[j + 7] == ']' && NGUITools.EncodeColor(NGUITools.ParseColor(text, j + 1)) == text.Substring(j + 1, 6).ToUpper())
				{
					j += 7;
					continue;
				}
			}
			BMSymbol bMSymbol = ((!flag3) ? null : MatchSymbol(text, j, length));
			int num7 = mSpacingX;
			if (!flag4)
			{
				if (bMSymbol != null)
				{
					num7 += bMSymbol.advance;
				}
				else
				{
					BMGlyph bMGlyph = ((bMSymbol != null) ? null : mFont.GetGlyph(c));
					if (bMGlyph == null)
					{
						continue;
					}
					num7 += ((num5 == 0) ? bMGlyph.advance : (bMGlyph.advance + bMGlyph.GetKerning(num5)));
				}
			}
			else if (mDynamicFont.GetCharacterInfo(c, out mChar, mDynamicFontSize, mDynamicFontStyle))
			{
				num7 += Mathf.RoundToInt(mChar.advance);
			}
			num4 -= num7;
			if (num4 < 0)
			{
				if (!flag && flag2 && num6 != num3)
				{
					for (; i < length && text[i] == ' '; i++)
					{
					}
					flag = true;
					num4 = num;
					j = i - 1;
					num5 = 0;
					if (!flag2 || num6 == num3)
					{
						break;
					}
					num6++;
					EndLine(ref s);
					continue;
				}
				s.Append(text.Substring(i, Mathf.Max(0, j - i)));
				if (!flag2 || num6 == num3)
				{
					i = j;
					break;
				}
				EndLine(ref s);
				flag = true;
				num6++;
				if (c == ' ')
				{
					i = j + 1;
					num4 = num;
				}
				else
				{
					i = j;
					num4 = num - num7;
				}
				num5 = 0;
			}
			else
			{
				num5 = c;
			}
			if (!flag4 && bMSymbol != null)
			{
				j += bMSymbol.length - 1;
				num5 = 0;
			}
		}
		if (i < j)
		{
			s.Append(text.Substring(i, j - i));
		}
		finalText = s.ToString();
		return !flag2 || j == length || (lines > 0 && num6 <= lines);
	}

	public bool WrapText(string text, out string finalText, float maxWidth, float maxHeight, int maxLineCount, bool encoding)
	{
		return WrapText(text, out finalText, maxWidth, maxHeight, maxLineCount, encoding, SymbolStyle.None);
	}

	public bool WrapText(string text, out string finalText, float maxWidth, float maxHeight, int maxLineCount)
	{
		return WrapText(text, out finalText, maxWidth, maxHeight, maxLineCount, false, SymbolStyle.None);
	}

	private void Align(BetterList<Vector3> verts, int indexOffset, Alignment alignment, int x, int lineWidth)
	{
		if (alignment == Alignment.Left)
		{
			return;
		}
		int num = size;
		if (num <= 0)
		{
			return;
		}
		float num2 = 0f;
		if (alignment == Alignment.Right)
		{
			num2 = Mathf.RoundToInt(lineWidth - x);
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			num2 /= (float)size;
		}
		else
		{
			num2 = Mathf.RoundToInt((float)(lineWidth - x) * 0.5f);
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			num2 /= (float)size;
			if ((lineWidth & 1) == 1)
			{
				num2 += 0.5f / (float)num;
			}
		}
		for (int i = indexOffset; i < verts.size; i++)
		{
			Vector3 vector = verts.buffer[i];
			vector.x += num2;
			verts.buffer[i] = vector;
		}
	}

	public void Print(string text, Color32 color, BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols, bool encoding, SymbolStyle symbolStyle, Alignment alignment, int lineWidth, bool premultiply)
	{
		if (mReplacement != null)
		{
			mReplacement.Print(text, color, verts, uvs, cols, encoding, symbolStyle, alignment, lineWidth, premultiply);
		}
		else
		{
			if (text == null || !isValid)
			{
				return;
			}
			if (mDynamicFont != null)
			{
				mDynamicFont.RequestCharactersInTexture(text, mDynamicFontSize, mDynamicFontStyle);
			}
			bool flag = isDynamic;
			mColors.Clear();
			mColors.Add(color);
			int num = size;
			Vector2 vector = ((num <= 0) ? Vector2.one : new Vector2(1f / (float)num, 1f / (float)num));
			int num2 = verts.size;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = num + mSpacingY;
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			Vector2 zero3 = Vector2.zero;
			Vector2 zero4 = Vector2.zero;
			float num8 = uvRect.width / (float)mFont.texWidth;
			float num9 = mUVRect.height / (float)mFont.texHeight;
			int length = text.Length;
			bool flag2 = encoding && symbolStyle != 0 && hasSymbols && sprite != null;
			for (int i = 0; i < length; i++)
			{
				char c = text[i];
				if (c == '\n')
				{
					if (num4 > num3)
					{
						num3 = num4;
					}
					if (alignment != 0)
					{
						Align(verts, num2, alignment, num4, lineWidth);
						num2 = verts.size;
					}
					num4 = 0;
					num5 += num7;
					num6 = 0;
					continue;
				}
				if (c < ' ')
				{
					num6 = 0;
					continue;
				}
				if (encoding && c == '[')
				{
					int num10 = NGUITools.ParseSymbol(text, i, mColors, premultiply);
					if (num10 > 0)
					{
						color = mColors[mColors.Count - 1];
						i += num10 - 1;
						continue;
					}
				}
				if (!flag)
				{
					BMSymbol bMSymbol = ((!flag2) ? null : MatchSymbol(text, i, length));
					if (bMSymbol == null)
					{
						BMGlyph glyph = mFont.GetGlyph(c);
						if (glyph == null)
						{
							continue;
						}
						if (num6 != 0)
						{
							num4 += glyph.GetKerning(num6);
						}
						if (c == ' ')
						{
							num4 += mSpacingX + glyph.advance;
							num6 = c;
							continue;
						}
						zero.x = vector.x * (float)(num4 + glyph.offsetX);
						zero.y = (0f - vector.y) * (float)(num5 + glyph.offsetY);
						zero2.x = zero.x + vector.x * (float)glyph.width;
						zero2.y = zero.y - vector.y * (float)glyph.height;
						zero3.x = mUVRect.xMin + num8 * (float)glyph.x;
						zero3.y = mUVRect.yMax - num9 * (float)glyph.y;
						zero4.x = zero3.x + num8 * (float)glyph.width;
						zero4.y = zero3.y - num9 * (float)glyph.height;
						num4 += mSpacingX + glyph.advance;
						num6 = c;
						if (glyph.channel == 0 || glyph.channel == 15)
						{
							for (int j = 0; j < 4; j++)
							{
								cols.Add(color);
							}
						}
						else
						{
							Color color2 = color;
							color2 *= 0.49f;
							switch (glyph.channel)
							{
							case 1:
								color2.b += 0.51f;
								break;
							case 2:
								color2.g += 0.51f;
								break;
							case 4:
								color2.r += 0.51f;
								break;
							case 8:
								color2.a += 0.51f;
								break;
							}
							for (int k = 0; k < 4; k++)
							{
								cols.Add(color2);
							}
						}
					}
					else
					{
						zero.x = vector.x * (float)(num4 + bMSymbol.offsetX);
						zero.y = (0f - vector.y) * (float)(num5 + bMSymbol.offsetY);
						zero2.x = zero.x + vector.x * (float)bMSymbol.width;
						zero2.y = zero.y - vector.y * (float)bMSymbol.height;
						Rect rect = bMSymbol.uvRect;
						zero3.x = rect.xMin;
						zero3.y = rect.yMax;
						zero4.x = rect.xMax;
						zero4.y = rect.yMin;
						num4 += mSpacingX + bMSymbol.advance;
						i += bMSymbol.length - 1;
						num6 = 0;
						if (symbolStyle == SymbolStyle.Colored)
						{
							for (int l = 0; l < 4; l++)
							{
								cols.Add(color);
							}
						}
						else
						{
							Color32 item = Color.white;
							item.a = color.a;
							for (int m = 0; m < 4; m++)
							{
								cols.Add(item);
							}
						}
					}
					verts.Add(new Vector3(zero2.x, zero.y));
					verts.Add(new Vector3(zero2.x, zero2.y));
					verts.Add(new Vector3(zero.x, zero2.y));
					verts.Add(new Vector3(zero.x, zero.y));
					uvs.Add(new Vector2(zero4.x, zero3.y));
					uvs.Add(new Vector2(zero4.x, zero4.y));
					uvs.Add(new Vector2(zero3.x, zero4.y));
					uvs.Add(new Vector2(zero3.x, zero3.y));
				}
				else if (mDynamicFont.GetCharacterInfo(c, out mChar, mDynamicFontSize, mDynamicFontStyle))
				{
					zero.x = vector.x * (float)(num4 + mChar.minX);
					zero.y = (0f - vector.y) * ((float)(num5 - mChar.maxY) + mDynamicFontOffset);
					zero2.x = zero.x + vector.x * (float)mChar.glyphWidth;
					zero2.y = zero.y - vector.y * (float)mChar.glyphHeight;
					num4 += mSpacingX + mChar.advance;
					for (int n = 0; n < 4; n++)
					{
						cols.Add(color);
					}
					uvs.Add(mChar.uvTopRight);
					uvs.Add(mChar.uvTopLeft);
					uvs.Add(mChar.uvBottomLeft);
					uvs.Add(mChar.uvBottomRight);
					verts.Add(new Vector3(zero2.x, zero.y));
					verts.Add(new Vector3(zero.x, zero.y));
					verts.Add(new Vector3(zero.x, zero2.y));
					verts.Add(new Vector3(zero2.x, zero2.y));
				}
			}
			if (alignment != 0 && num2 < verts.size)
			{
				Align(verts, num2, alignment, num4, lineWidth);
				num2 = verts.size;
			}
		}
	}

	private BMSymbol GetSymbol(string sequence, bool createIfMissing)
	{
		int i = 0;
		for (int count = mSymbols.Count; i < count; i++)
		{
			BMSymbol bMSymbol = mSymbols[i];
			if (bMSymbol.sequence == sequence)
			{
				return bMSymbol;
			}
		}
		if (createIfMissing)
		{
			BMSymbol bMSymbol2 = new BMSymbol();
			bMSymbol2.sequence = sequence;
			mSymbols.Add(bMSymbol2);
			return bMSymbol2;
		}
		return null;
	}

	private BMSymbol MatchSymbol(string text, int offset, int textLength)
	{
		int count = mSymbols.Count;
		if (count == 0)
		{
			return null;
		}
		textLength -= offset;
		for (int i = 0; i < count; i++)
		{
			BMSymbol bMSymbol = mSymbols[i];
			int length = bMSymbol.length;
			if (length == 0 || textLength < length)
			{
				continue;
			}
			bool flag = true;
			for (int j = 0; j < length; j++)
			{
				if (text[offset + j] != bMSymbol.sequence[j])
				{
					flag = false;
					break;
				}
			}
			if (flag && bMSymbol.Validate(atlas))
			{
				return bMSymbol;
			}
		}
		return null;
	}

	public void AddSymbol(string sequence, string spriteName)
	{
		BMSymbol symbol = GetSymbol(sequence, true);
		symbol.spriteName = spriteName;
		MarkAsDirty();
	}

	public void RemoveSymbol(string sequence)
	{
		BMSymbol symbol = GetSymbol(sequence, false);
		if (symbol != null)
		{
			symbols.Remove(symbol);
		}
		MarkAsDirty();
	}

	public void RenameSymbol(string before, string after)
	{
		BMSymbol symbol = GetSymbol(before, false);
		if (symbol != null)
		{
			symbol.sequence = after;
		}
		MarkAsDirty();
	}

	public bool UsesSprite(string s)
	{
		if (!string.IsNullOrEmpty(s))
		{
			if (s.Equals(spriteName))
			{
				return true;
			}
			int i = 0;
			for (int count = symbols.Count; i < count; i++)
			{
				BMSymbol bMSymbol = symbols[i];
				if (s.Equals(bMSymbol.spriteName))
				{
					return true;
				}
			}
		}
		return false;
	}
}
