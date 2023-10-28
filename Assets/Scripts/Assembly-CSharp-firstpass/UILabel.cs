using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Label")]
public class UILabel : UIWidget
{
	public enum Effect
	{
		None,
		Shadow,
		Outline
	}

	[HideInInspector]
	[SerializeField]
	private UIFont mFont;

	[HideInInspector]
	[SerializeField]
	private string mText = string.Empty;

	[HideInInspector]
	[SerializeField]
	private int mMaxLineWidth;

	[HideInInspector]
	[SerializeField]
	private int mMaxLineHeight;

	[HideInInspector]
	[SerializeField]
	private bool mEncoding = true;

	[HideInInspector]
	[SerializeField]
	private int mMaxLineCount;

	[HideInInspector]
	[SerializeField]
	private bool mPassword;

	[SerializeField]
	[HideInInspector]
	private bool mShowLastChar;

	[HideInInspector]
	[SerializeField]
	private Effect mEffectStyle;

	[HideInInspector]
	[SerializeField]
	private Color mEffectColor = Color.black;

	[SerializeField]
	[HideInInspector]
	private UIFont.SymbolStyle mSymbols = UIFont.SymbolStyle.Uncolored;

	[HideInInspector]
	[SerializeField]
	private Vector2 mEffectDistance = Vector2.one;

	[HideInInspector]
	[SerializeField]
	private bool mShrinkToFit;

	[HideInInspector]
	[SerializeField]
	private int mMaxFontSize = 128;

	[SerializeField]
	[HideInInspector]
	private float mLineWidth;

	[SerializeField]
	[HideInInspector]
	private bool mMultiline = true;

	private bool mShouldBeProcessed = true;

	private string mProcessedText;

	private Vector3 mLastScale = Vector3.one;

	private Vector2 mSize = Vector2.zero;

	private bool mPremultiply;

	private static LinkedList<WeakReference> fontCallbacks;

	private object fontCallbackRef;

	private bool hasChanged
	{
		get
		{
			return mShouldBeProcessed;
		}
		set
		{
			if (value)
			{
				mChanged = true;
				mShouldBeProcessed = true;
			}
			else
			{
				mShouldBeProcessed = false;
			}
		}
	}

	public override Material material
	{
		get
		{
			return (!(mFont != null)) ? null : mFont.material;
		}
	}

	public UIFont font
	{
		get
		{
			return mFont;
		}
		set
		{
			if (!(mFont != value))
			{
				return;
			}
			RemoveFromPanel();
			mFont = value;
			hasChanged = true;
			if (mFont != null && mFont.dynamicFont != null)
			{
				mFont.Request(mText);
				if (fontCallbackRef == null)
				{
					fontCallbackRef = RegisterFontCallback(this);
				}
			}
			else if (fontCallbackRef != null)
			{
				UnregisterFontCallback(fontCallbackRef);
				fontCallbackRef = null;
			}
			MarkAsChanged();
		}
	}

	public string text
	{
		get
		{
			return mText;
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				if (!string.IsNullOrEmpty(mText))
				{
					mText = string.Empty;
				}
				hasChanged = true;
			}
			else if (mText != value)
			{
				mText = value;
				hasChanged = true;
				if (mFont != null)
				{
					mFont.Request(value);
				}
				if (shrinkToFit)
				{
					MakePixelPerfect();
				}
			}
		}
	}

	public bool supportEncoding
	{
		get
		{
			return mEncoding;
		}
		set
		{
			if (mEncoding != value)
			{
				mEncoding = value;
				hasChanged = true;
				if (value)
				{
					mPassword = false;
				}
			}
		}
	}

	public UIFont.SymbolStyle symbolStyle
	{
		get
		{
			return mSymbols;
		}
		set
		{
			if (mSymbols != value)
			{
				mSymbols = value;
				hasChanged = true;
			}
		}
	}

	public int lineWidth
	{
		get
		{
			return mMaxLineWidth;
		}
		set
		{
			if (mMaxLineWidth != value)
			{
				mMaxLineWidth = value;
				hasChanged = true;
				if (shrinkToFit)
				{
					MakePixelPerfect();
				}
			}
		}
	}

	public int lineHeight
	{
		get
		{
			return mMaxLineHeight;
		}
		set
		{
			if (mMaxLineHeight != value)
			{
				mMaxLineHeight = value;
				hasChanged = true;
				if (shrinkToFit)
				{
					MakePixelPerfect();
				}
			}
		}
	}

	public bool multiLine
	{
		get
		{
			return mMaxLineCount != 1;
		}
		set
		{
			if (mMaxLineCount != 1 != value)
			{
				mMaxLineCount = ((!value) ? 1 : 0);
				hasChanged = true;
				if (value)
				{
					mPassword = false;
				}
			}
		}
	}

	public int maxLineCount
	{
		get
		{
			return mMaxLineCount;
		}
		set
		{
			if (mMaxLineCount != value)
			{
				mMaxLineCount = Mathf.Max(value, 0);
				if (value != 1)
				{
					mPassword = false;
				}
				hasChanged = true;
				if (shrinkToFit)
				{
					MakePixelPerfect();
				}
			}
		}
	}

	public bool password
	{
		get
		{
			return mPassword;
		}
		set
		{
			if (mPassword != value)
			{
				if (value)
				{
					mMaxLineCount = 1;
					mEncoding = false;
				}
				mPassword = value;
				hasChanged = true;
			}
		}
	}

	public bool showLastPasswordChar
	{
		get
		{
			return mShowLastChar;
		}
		set
		{
			if (mShowLastChar != value)
			{
				mShowLastChar = value;
				hasChanged = true;
			}
		}
	}

	public Effect effectStyle
	{
		get
		{
			return mEffectStyle;
		}
		set
		{
			if (mEffectStyle != value)
			{
				mEffectStyle = value;
				hasChanged = true;
			}
		}
	}

	public Color effectColor
	{
		get
		{
			return mEffectColor;
		}
		set
		{
			if (!mEffectColor.Equals(value))
			{
				mEffectColor = value;
				if (mEffectStyle != 0)
				{
					hasChanged = true;
				}
			}
		}
	}

	public Vector2 effectDistance
	{
		get
		{
			return mEffectDistance;
		}
		set
		{
			if (mEffectDistance != value)
			{
				mEffectDistance = value;
				hasChanged = true;
			}
		}
	}

	public bool shrinkToFit
	{
		get
		{
			return mShrinkToFit;
		}
		set
		{
			if (mShrinkToFit != value)
			{
				mShrinkToFit = value;
				hasChanged = true;
			}
		}
	}

	public int maxFontSize
	{
		get
		{
			return mMaxFontSize;
		}
		set
		{
			if (mMaxFontSize != value)
			{
				mMaxFontSize = value;
				hasChanged = true;
			}
		}
	}

	public string processedText
	{
		get
		{
			if (mLastScale != base.cachedTransform.localScale)
			{
				mLastScale = base.cachedTransform.localScale;
				mShouldBeProcessed = true;
			}
			if (hasChanged)
			{
				ProcessText();
			}
			return mProcessedText;
		}
	}

	public override Vector2 relativeSize
	{
		get
		{
			if (mFont == null)
			{
				return Vector3.one;
			}
			if (hasChanged)
			{
				ProcessText();
			}
			return mSize;
		}
	}

	static UILabel()
	{
		fontCallbacks = new LinkedList<WeakReference>();
		Font.textureRebuilt += SignalFontChange;
	}

	private static void SignalFontChange(Font targetFont)
	{
		LinkedListNode<WeakReference> linkedListNode = fontCallbacks.First;
		while (linkedListNode != null)
		{
			LinkedListNode<WeakReference> next = linkedListNode.Next;
			UILabel uILabel = linkedListNode.Value.Target as UILabel;
			if (uILabel == null)
			{
				fontCallbacks.Remove(linkedListNode);
			}
			else
			{
				uILabel.OnFontChange(targetFont);
			}
			linkedListNode = next;
		}
	}

	private static object RegisterFontCallback(UILabel inst)
	{
		WeakReference value = new WeakReference(inst);
		return fontCallbacks.AddLast(value);
	}

	private static void UnregisterFontCallback(object cbRef)
	{
		LinkedListNode<WeakReference> linkedListNode = cbRef as LinkedListNode<WeakReference>;
		if (linkedListNode != null)
		{
			fontCallbacks.Remove(linkedListNode);
		}
	}

	protected void OnFontChange(Font targetFont)
	{
		if (!(mFont == null) && !(mFont.dynamicFont != targetFont))
		{
			MarkAsChanged();
		}
	}

	protected override void OnEnable()
	{
		if (mFont != null && mFont.dynamicFont != null && fontCallbackRef == null)
		{
			fontCallbackRef = RegisterFontCallback(this);
		}
		base.OnEnable();
	}

	protected override void OnDisable()
	{
		if (fontCallbackRef != null)
		{
			UnregisterFontCallback(fontCallbackRef);
			fontCallbackRef = null;
		}
		base.OnDisable();
	}

	protected override void OnStart()
	{
		if (Application.isPlaying)
		{
			text = KFFLocalization.Get(text);
		}
		if (mLineWidth > 0f)
		{
			mMaxLineWidth = Mathf.RoundToInt(mLineWidth);
			mLineWidth = 0f;
		}
		if (!mMultiline)
		{
			mMaxLineCount = 1;
			mMultiline = true;
		}
		mPremultiply = font != null && font.material != null && font.material.shader.name.Contains("Premultiplied");
		if (mFont != null)
		{
			mFont.Request(mText);
		}
	}

	public override void MarkAsChanged()
	{
		hasChanged = true;
		base.MarkAsChanged();
	}

	private void ProcessText()
	{
		mChanged = true;
		hasChanged = false;
		float num = Mathf.Abs(base.cachedTransform.localScale.x);
		if (num > 0f)
		{
			while (true)
			{
				bool flag = true;
				if (mPassword)
				{
					mProcessedText = string.Empty;
					if (mShowLastChar)
					{
						int i = 0;
						for (int num2 = mText.Length - 1; i < num2; i++)
						{
							mProcessedText += "*";
						}
						if (mText.Length > 0)
						{
							mProcessedText += mText[mText.Length - 1];
						}
					}
					else
					{
						int j = 0;
						for (int length = mText.Length; j < length; j++)
						{
							mProcessedText += "*";
						}
					}
					flag = mFont.WrapText(mProcessedText, out mProcessedText, (float)mMaxLineWidth / num, (float)mMaxLineHeight / num, mMaxLineCount, false, UIFont.SymbolStyle.None);
				}
				else if (mMaxLineWidth > 0 || mMaxLineHeight > 0)
				{
					flag = mFont.WrapText(mText, out mProcessedText, (float)mMaxLineWidth / num, (float)mMaxLineHeight / num, mMaxLineCount, mEncoding, mSymbols);
				}
				else
				{
					mProcessedText = mText;
				}
				mSize = (string.IsNullOrEmpty(mProcessedText) ? Vector2.one : mFont.CalculatePrintedSize(mProcessedText, mEncoding, mSymbols));
				if (!mShrinkToFit)
				{
					break;
				}
				if (!flag)
				{
					num = Mathf.Round(num - 1f);
					if (num > 1f)
					{
						continue;
					}
				}
				if (mMaxLineWidth > 0)
				{
					float num3 = (float)mMaxLineWidth / num;
					float a = ((!(mSize.x * num > num3)) ? num : (num3 / mSize.x * num));
					num = Mathf.Min(a, num);
				}
				num = Mathf.Round(num);
				base.cachedTransform.localScale = new Vector3(num, num, 1f);
				break;
			}
			mSize.x = Mathf.Max(mSize.x, (!(num > 0f)) ? 1f : ((float)mMaxLineWidth / num));
			mSize.y = Mathf.Max(mSize.y, (!(num > 0f)) ? 1f : ((float)mMaxLineHeight / num));
		}
		else
		{
			mSize.x = 1f;
			mSize.y = 1f;
			num = mFont.size;
			base.cachedTransform.localScale = new Vector3(num, num, 1f);
			mProcessedText = string.Empty;
		}
		mSize.y = Mathf.Max(Mathf.Max(mSize.y, 1f), (float)mMaxLineHeight / num);
	}

	public override void MakePixelPerfect()
	{
		if (mFont != null)
		{
			float pixelSize = font.pixelSize;
			Vector3 localScale = base.cachedTransform.localScale;
			localScale.x = Math.Min((float)mFont.size * pixelSize, mMaxFontSize);
			localScale.y = localScale.x;
			localScale.z = 1f;
			Vector3 localPosition = base.cachedTransform.localPosition;
			localPosition.x = Mathf.CeilToInt(localPosition.x / pixelSize * 4f) >> 2;
			localPosition.y = Mathf.CeilToInt(localPosition.y / pixelSize * 4f) >> 2;
			localPosition.z = Mathf.RoundToInt(localPosition.z);
			localPosition.x *= pixelSize;
			localPosition.y *= pixelSize;
			base.cachedTransform.localPosition = localPosition;
			base.cachedTransform.localScale = localScale;
			if (shrinkToFit)
			{
				ProcessText();
			}
		}
		else
		{
			base.MakePixelPerfect();
		}
	}

	private void ApplyShadow(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols, int start, int end, float x, float y)
	{
		Color color = mEffectColor;
		color.a *= base.alpha * mPanel.alpha;
		Color32 color2 = ((!font.premultipliedAlpha) ? color : NGUITools.ApplyPMA(color));
		for (int i = start; i < end; i++)
		{
			verts.Add(verts.buffer[i]);
			uvs.Add(uvs.buffer[i]);
			cols.Add(cols.buffer[i]);
			Vector3 vector = verts.buffer[i];
			vector.x += x;
			vector.y += y;
			verts.buffer[i] = vector;
			cols.buffer[i] = color2;
		}
	}

	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		if (mFont == null)
		{
			return;
		}
		Pivot pivot = base.pivot;
		int size = verts.size;
		Color color = base.color;
		color.a *= mPanel.alpha;
		if (font.premultipliedAlpha)
		{
			color = NGUITools.ApplyPMA(color);
		}
		switch (pivot)
		{
		case Pivot.TopLeft:
		case Pivot.Left:
		case Pivot.BottomLeft:
			mFont.Print(processedText, color, verts, uvs, cols, mEncoding, mSymbols, UIFont.Alignment.Left, 0, mPremultiply);
			break;
		case Pivot.TopRight:
		case Pivot.Right:
		case Pivot.BottomRight:
			mFont.Print(processedText, color, verts, uvs, cols, mEncoding, mSymbols, UIFont.Alignment.Right, Mathf.RoundToInt(relativeSize.x * (float)mFont.size), mPremultiply);
			break;
		default:
			mFont.Print(processedText, color, verts, uvs, cols, mEncoding, mSymbols, UIFont.Alignment.Center, Mathf.RoundToInt(relativeSize.x * (float)mFont.size), mPremultiply);
			break;
		}
		if (effectStyle != 0)
		{
			int size2 = verts.size;
			float num = 1f / ((float)mFont.size * mFont.pixelSize);
			float num2 = num * mEffectDistance.x;
			float num3 = num * mEffectDistance.y;
			ApplyShadow(verts, uvs, cols, size, size2, num2, 0f - num3);
			if (effectStyle == Effect.Outline)
			{
				size = size2;
				size2 = verts.size;
				ApplyShadow(verts, uvs, cols, size, size2, 0f - num2, num3);
				size = size2;
				size2 = verts.size;
				ApplyShadow(verts, uvs, cols, size, size2, num2, num3);
				size = size2;
				size2 = verts.size;
				ApplyShadow(verts, uvs, cols, size, size2, 0f - num2, 0f - num3);
			}
		}
	}
}
