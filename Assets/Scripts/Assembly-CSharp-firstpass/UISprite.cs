using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Sprite")]
[ExecuteInEditMode]
public class UISprite : UIWidget
{
	public enum Type
	{
		Simple,
		Sliced,
		Tiled,
		Filled
	}

	public enum FillDirection
	{
		Horizontal,
		Vertical,
		Radial90,
		Radial180,
		Radial360
	}

	[HideInInspector]
	[SerializeField]
	private UIAtlas mAtlas;

	[SerializeField]
	[HideInInspector]
	private string mSpriteName;

	[SerializeField]
	[HideInInspector]
	private bool mFillCenter = true;

	[SerializeField]
	[HideInInspector]
	private Type mType;

	[HideInInspector]
	[SerializeField]
	private FillDirection mFillDirection = FillDirection.Radial360;

	[SerializeField]
	[HideInInspector]
	private float mFillAmount = 1f;

	[SerializeField]
	[HideInInspector]
	private bool mInvert;

	protected UIAtlas.Sprite mSprite;

	protected Rect mInner;

	protected Rect mInnerUV;

	protected Rect mOuter;

	protected Rect mOuterUV;

	protected Vector3 mScale = Vector3.one;

	private bool mSpriteSet;

	public virtual Type type
	{
		get
		{
			return mType;
		}
		set
		{
			if (mType != value)
			{
				mType = value;
				MarkAsChanged();
			}
		}
	}

	public override Material material
	{
		get
		{
			return (!(mAtlas != null)) ? null : mAtlas.spriteMaterial;
		}
	}

	public UIAtlas atlas
	{
		get
		{
			return mAtlas;
		}
		set
		{
			if (mAtlas != value)
			{
				RemoveFromPanel();
				mAtlas = value;
				mSpriteSet = false;
				mSprite = null;
				if (string.IsNullOrEmpty(mSpriteName) && mAtlas != null && mAtlas.spriteList.Count > 0)
				{
					SetAtlasSprite(mAtlas.spriteList[0]);
					mSpriteName = mSprite.name;
				}
				if (!string.IsNullOrEmpty(mSpriteName))
				{
					string text = mSpriteName;
					mSpriteName = string.Empty;
					spriteName = text;
					MarkAsChanged();
					UpdateUVs(true);
				}
			}
		}
	}

	public string spriteName
	{
		get
		{
			return mSpriteName;
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				if (!string.IsNullOrEmpty(mSpriteName))
				{
					mSpriteName = string.Empty;
					mSprite = null;
					mChanged = true;
					mSpriteSet = false;
				}
			}
			else if (mSpriteName != value)
			{
				mSpriteName = value;
				mSprite = null;
				mChanged = true;
				mSpriteSet = false;
				if (isValid)
				{
					UpdateUVs(true);
				}
			}
		}
	}

	public bool isValid
	{
		get
		{
			return GetAtlasSprite() != null;
		}
	}

	public Rect innerUV
	{
		get
		{
			UpdateUVs(false);
			return mInnerUV;
		}
	}

	public Rect outerUV
	{
		get
		{
			UpdateUVs(false);
			return mOuterUV;
		}
	}

	public bool fillCenter
	{
		get
		{
			return mFillCenter;
		}
		set
		{
			if (mFillCenter != value)
			{
				mFillCenter = value;
				MarkAsChanged();
			}
		}
	}

	public FillDirection fillDirection
	{
		get
		{
			return mFillDirection;
		}
		set
		{
			if (mFillDirection != value)
			{
				mFillDirection = value;
				mChanged = true;
			}
		}
	}

	public float fillAmount
	{
		get
		{
			return mFillAmount;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if (mFillAmount != num)
			{
				mFillAmount = num;
				mChanged = true;
			}
		}
	}

	public bool invert
	{
		get
		{
			return mInvert;
		}
		set
		{
			if (mInvert != value)
			{
				mInvert = value;
				mChanged = true;
			}
		}
	}

	public override Vector4 relativePadding
	{
		get
		{
			if (isValid && type == Type.Simple)
			{
				return new Vector4(mSprite.paddingLeft, mSprite.paddingTop, mSprite.paddingRight, mSprite.paddingBottom);
			}
			return base.relativePadding;
		}
	}

	public override Vector4 border
	{
		get
		{
			if (type == Type.Sliced)
			{
				UIAtlas.Sprite atlasSprite = GetAtlasSprite();
				if (atlasSprite == null)
				{
					return Vector2.zero;
				}
				Rect rect = atlasSprite.outer;
				Rect rect2 = atlasSprite.inner;
				Texture texture = mainTexture;
				if (atlas.coordinates == UIAtlas.Coordinates.TexCoords && texture != null)
				{
					rect = NGUIMath.ConvertToPixels(rect, texture.width, texture.height, true);
					rect2 = NGUIMath.ConvertToPixels(rect2, texture.width, texture.height, true);
				}
				return new Vector4(rect2.xMin - rect.xMin, rect2.yMin - rect.yMin, rect.xMax - rect2.xMax, rect.yMax - rect2.yMax) * atlas.pixelSize;
			}
			return base.border;
		}
	}

	public override bool pixelPerfectAfterResize
	{
		get
		{
			return type == Type.Sliced;
		}
	}

	public UIAtlas.Sprite GetAtlasSprite()
	{
		if (!mSpriteSet)
		{
			mSprite = null;
		}
		if (mSprite == null && mAtlas != null)
		{
			if (!string.IsNullOrEmpty(mSpriteName))
			{
				UIAtlas.Sprite sprite = mAtlas.GetSprite(mSpriteName);
				if (sprite == null)
				{
					return null;
				}
				SetAtlasSprite(sprite);
			}
			if (mSprite == null && mAtlas.spriteList.Count > 0)
			{
				UIAtlas.Sprite sprite2 = mAtlas.spriteList[0];
				if (sprite2 == null)
				{
					return null;
				}
				SetAtlasSprite(sprite2);
				if (mSprite == null)
				{
					return null;
				}
				mSpriteName = mSprite.name;
			}
			if (mSprite != null)
			{
				UpdateUVs(true);
			}
		}
		return mSprite;
	}

	protected void SetAtlasSprite(UIAtlas.Sprite sp)
	{
		mChanged = true;
		mSpriteSet = true;
		if (sp != null)
		{
			mSprite = sp;
			mSpriteName = mSprite.name;
		}
		else
		{
			mSpriteName = ((mSprite == null) ? string.Empty : mSprite.name);
			mSprite = sp;
		}
	}

	public virtual void UpdateUVs(bool force)
	{
		if ((type == Type.Sliced || type == Type.Tiled) && base.cachedTransform.localScale != mScale)
		{
			mScale = base.cachedTransform.localScale;
			mChanged = true;
		}
		if (!isValid || !force)
		{
			return;
		}
		Texture texture = mainTexture;
		if (texture != null)
		{
			mInner = mSprite.inner;
			mOuter = mSprite.outer;
			mInnerUV = mInner;
			mOuterUV = mOuter;
			if (atlas.coordinates == UIAtlas.Coordinates.Pixels)
			{
				mOuterUV = NGUIMath.ConvertToTexCoords(mOuterUV, texture.width, texture.height);
				mInnerUV = NGUIMath.ConvertToTexCoords(mInnerUV, texture.width, texture.height);
			}
		}
	}

	public override void MakePixelPerfect()
	{
		if (!isValid)
		{
			return;
		}
		UpdateUVs(false);
		switch (type)
		{
		case Type.Sliced:
		{
			Vector3 localPosition = base.cachedTransform.localPosition;
			localPosition.x = Mathf.RoundToInt(localPosition.x);
			localPosition.y = Mathf.RoundToInt(localPosition.y);
			localPosition.z = Mathf.RoundToInt(localPosition.z);
			base.cachedTransform.localPosition = localPosition;
			Vector3 localScale = base.cachedTransform.localScale;
			localScale.x = Mathf.RoundToInt(localScale.x * 0.5f) << 1;
			localScale.y = Mathf.RoundToInt(localScale.y * 0.5f) << 1;
			localScale.z = 1f;
			base.cachedTransform.localScale = localScale;
			return;
		}
		case Type.Tiled:
		{
			Vector3 localPosition2 = base.cachedTransform.localPosition;
			localPosition2.x = Mathf.RoundToInt(localPosition2.x);
			localPosition2.y = Mathf.RoundToInt(localPosition2.y);
			localPosition2.z = Mathf.RoundToInt(localPosition2.z);
			base.cachedTransform.localPosition = localPosition2;
			Vector3 localScale2 = base.cachedTransform.localScale;
			localScale2.x = Mathf.RoundToInt(localScale2.x);
			localScale2.y = Mathf.RoundToInt(localScale2.y);
			localScale2.z = 1f;
			base.cachedTransform.localScale = localScale2;
			return;
		}
		}
		Texture texture = mainTexture;
		Vector3 localScale3 = base.cachedTransform.localScale;
		if (texture != null)
		{
			Rect rect = NGUIMath.ConvertToPixels(outerUV, texture.width, texture.height, true);
			float pixelSize = atlas.pixelSize;
			localScale3.x = (float)Mathf.RoundToInt(rect.width * pixelSize) * Mathf.Sign(localScale3.x);
			localScale3.y = (float)Mathf.RoundToInt(rect.height * pixelSize) * Mathf.Sign(localScale3.y);
			localScale3.z = 1f;
			base.cachedTransform.localScale = localScale3;
		}
		int num = Mathf.RoundToInt(Mathf.Abs(localScale3.x) * (1f + mSprite.paddingLeft + mSprite.paddingRight));
		int num2 = Mathf.RoundToInt(Mathf.Abs(localScale3.y) * (1f + mSprite.paddingTop + mSprite.paddingBottom));
		Vector3 localPosition3 = base.cachedTransform.localPosition;
		localPosition3.x = Mathf.CeilToInt(localPosition3.x * 4f) >> 2;
		localPosition3.y = Mathf.CeilToInt(localPosition3.y * 4f) >> 2;
		localPosition3.z = Mathf.RoundToInt(localPosition3.z);
		if (num % 2 == 1 && (base.pivot == Pivot.Top || base.pivot == Pivot.Center || base.pivot == Pivot.Bottom))
		{
			localPosition3.x += 0.5f;
		}
		if (num2 % 2 == 1 && (base.pivot == Pivot.Left || base.pivot == Pivot.Center || base.pivot == Pivot.Right))
		{
			localPosition3.y += 0.5f;
		}
		base.cachedTransform.localPosition = localPosition3;
	}

	protected override void OnStart()
	{
		if (mAtlas != null)
		{
			UpdateUVs(true);
		}
	}

	public override void Update()
	{
		base.Update();
		if (mChanged || !mSpriteSet)
		{
			mSpriteSet = true;
			mSprite = null;
			mChanged = true;
			UpdateUVs(true);
		}
		else
		{
			UpdateUVs(false);
		}
	}

	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		switch (type)
		{
		case Type.Simple:
			SimpleFill(verts, uvs, cols);
			break;
		case Type.Sliced:
			SlicedFill(verts, uvs, cols);
			break;
		case Type.Filled:
			FilledFill(verts, uvs, cols);
			break;
		case Type.Tiled:
			TiledFill(verts, uvs, cols);
			break;
		}
	}

	protected void SimpleFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Vector2 item = new Vector2(mOuterUV.xMin, mOuterUV.yMin);
		Vector2 item2 = new Vector2(mOuterUV.xMax, mOuterUV.yMax);
		verts.Add(new Vector3(1f, 0f, 0f));
		verts.Add(new Vector3(1f, -1f, 0f));
		verts.Add(new Vector3(0f, -1f, 0f));
		verts.Add(new Vector3(0f, 0f, 0f));
		uvs.Add(item2);
		uvs.Add(new Vector2(item2.x, item.y));
		uvs.Add(item);
		uvs.Add(new Vector2(item.x, item2.y));
		Color color = base.color;
		color.a *= mPanel.alpha;
		Color32 item3 = ((!atlas.premultipliedAlpha) ? color : NGUITools.ApplyPMA(color));
		cols.Add(item3);
		cols.Add(item3);
		cols.Add(item3);
		cols.Add(item3);
	}

	protected void SlicedFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		if (mOuterUV == mInnerUV)
		{
			SimpleFill(verts, uvs, cols);
			return;
		}
		Vector2[] array = new Vector2[4];
		Vector2[] array2 = new Vector2[4];
		Texture texture = mainTexture;
		array[0] = Vector2.zero;
		array[1] = Vector2.zero;
		array[2] = new Vector2(1f, -1f);
		array[3] = new Vector2(1f, -1f);
		if (texture != null)
		{
			float pixelSize = atlas.pixelSize;
			float num = (mInnerUV.xMin - mOuterUV.xMin) * pixelSize;
			float num2 = (mOuterUV.xMax - mInnerUV.xMax) * pixelSize;
			float num3 = (mInnerUV.yMax - mOuterUV.yMax) * pixelSize;
			float num4 = (mOuterUV.yMin - mInnerUV.yMin) * pixelSize;
			Vector3 localScale = base.cachedTransform.localScale;
			localScale.x = Mathf.Max(0f, localScale.x);
			localScale.y = Mathf.Max(0f, localScale.y);
			Vector2 vector = new Vector2(localScale.x / (float)texture.width, localScale.y / (float)texture.height);
			Vector2 vector2 = new Vector2(num / vector.x, num3 / vector.y);
			Vector2 vector3 = new Vector2(num2 / vector.x, num4 / vector.y);
			Pivot pivot = base.pivot;
			if (pivot == Pivot.Right || pivot == Pivot.TopRight || pivot == Pivot.BottomRight)
			{
				array[0].x = Mathf.Min(0f, 1f - (vector3.x + vector2.x));
				array[1].x = array[0].x + vector2.x;
				array[2].x = array[0].x + Mathf.Max(vector2.x, 1f - vector3.x);
				array[3].x = array[0].x + Mathf.Max(vector2.x + vector3.x, 1f);
			}
			else
			{
				array[1].x = vector2.x;
				array[2].x = Mathf.Max(vector2.x, 1f - vector3.x);
				array[3].x = Mathf.Max(vector2.x + vector3.x, 1f);
			}
			if (pivot == Pivot.Bottom || pivot == Pivot.BottomLeft || pivot == Pivot.BottomRight)
			{
				array[0].y = Mathf.Max(0f, -1f - (vector3.y + vector2.y));
				array[1].y = array[0].y + vector2.y;
				array[2].y = array[0].y + Mathf.Min(vector2.y, -1f - vector3.y);
				array[3].y = array[0].y + Mathf.Min(vector2.y + vector3.y, -1f);
			}
			else
			{
				array[1].y = vector2.y;
				array[2].y = Mathf.Min(vector2.y, -1f - vector3.y);
				array[3].y = Mathf.Min(vector2.y + vector3.y, -1f);
			}
			array2[0] = new Vector2(mOuterUV.xMin, mOuterUV.yMax);
			array2[1] = new Vector2(mInnerUV.xMin, mInnerUV.yMax);
			array2[2] = new Vector2(mInnerUV.xMax, mInnerUV.yMin);
			array2[3] = new Vector2(mOuterUV.xMax, mOuterUV.yMin);
		}
		else
		{
			for (int i = 0; i < 4; i++)
			{
				array2[i] = Vector2.zero;
			}
		}
		Color color = base.color;
		color.a *= mPanel.alpha;
		Color32 item = ((!atlas.premultipliedAlpha) ? color : NGUITools.ApplyPMA(color));
		for (int j = 0; j < 3; j++)
		{
			int num5 = j + 1;
			for (int k = 0; k < 3; k++)
			{
				if (mFillCenter || j != 1 || k != 1)
				{
					int num6 = k + 1;
					verts.Add(new Vector3(array[num5].x, array[k].y, 0f));
					verts.Add(new Vector3(array[num5].x, array[num6].y, 0f));
					verts.Add(new Vector3(array[j].x, array[num6].y, 0f));
					verts.Add(new Vector3(array[j].x, array[k].y, 0f));
					uvs.Add(new Vector2(array2[num5].x, array2[k].y));
					uvs.Add(new Vector2(array2[num5].x, array2[num6].y));
					uvs.Add(new Vector2(array2[j].x, array2[num6].y));
					uvs.Add(new Vector2(array2[j].x, array2[k].y));
					cols.Add(item);
					cols.Add(item);
					cols.Add(item);
					cols.Add(item);
				}
			}
		}
	}

	protected bool AdjustRadial(Vector2[] xy, Vector2[] uv, float fill, bool invert)
	{
		if (fill < 0.001f)
		{
			return false;
		}
		if (!invert && fill > 0.999f)
		{
			return true;
		}
		float num = Mathf.Clamp01(fill);
		if (!invert)
		{
			num = 1f - num;
		}
		num *= (float)Math.PI / 2f;
		float num2 = Mathf.Sin(num);
		float num3 = Mathf.Cos(num);
		if (num2 > num3)
		{
			num3 *= 1f / num2;
			num2 = 1f;
			if (!invert)
			{
				xy[0].y = Mathf.Lerp(xy[2].y, xy[0].y, num3);
				xy[3].y = xy[0].y;
				uv[0].y = Mathf.Lerp(uv[2].y, uv[0].y, num3);
				uv[3].y = uv[0].y;
			}
		}
		else if (num3 > num2)
		{
			num2 *= 1f / num3;
			num3 = 1f;
			if (invert)
			{
				xy[0].x = Mathf.Lerp(xy[2].x, xy[0].x, num2);
				xy[1].x = xy[0].x;
				uv[0].x = Mathf.Lerp(uv[2].x, uv[0].x, num2);
				uv[1].x = uv[0].x;
			}
		}
		else
		{
			num2 = 1f;
			num3 = 1f;
		}
		if (invert)
		{
			xy[1].y = Mathf.Lerp(xy[2].y, xy[0].y, num3);
			uv[1].y = Mathf.Lerp(uv[2].y, uv[0].y, num3);
		}
		else
		{
			xy[3].x = Mathf.Lerp(xy[2].x, xy[0].x, num2);
			uv[3].x = Mathf.Lerp(uv[2].x, uv[0].x, num2);
		}
		return true;
	}

	protected void Rotate(Vector2[] v, int offset)
	{
		for (int i = 0; i < offset; i++)
		{
			Vector2 vector = new Vector2(v[3].x, v[3].y);
			v[3].x = v[2].y;
			v[3].y = v[2].x;
			v[2].x = v[1].y;
			v[2].y = v[1].x;
			v[1].x = v[0].y;
			v[1].y = v[0].x;
			v[0].x = vector.y;
			v[0].y = vector.x;
		}
	}

	protected void FilledFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		float x = 0f;
		float y = 0f;
		float num = 1f;
		float num2 = -1f;
		float num3 = mOuterUV.xMin;
		float num4 = mOuterUV.yMin;
		float num5 = mOuterUV.xMax;
		float num6 = mOuterUV.yMax;
		if (mFillDirection == FillDirection.Horizontal || mFillDirection == FillDirection.Vertical)
		{
			float num7 = (num5 - num3) * mFillAmount;
			float num8 = (num6 - num4) * mFillAmount;
			if (fillDirection == FillDirection.Horizontal)
			{
				if (mInvert)
				{
					x = 1f - mFillAmount;
					num3 = num5 - num7;
				}
				else
				{
					num *= mFillAmount;
					num5 = num3 + num7;
				}
			}
			else if (fillDirection == FillDirection.Vertical)
			{
				if (mInvert)
				{
					num2 *= mFillAmount;
					num4 = num6 - num8;
				}
				else
				{
					y = 0f - (1f - mFillAmount);
					num6 = num4 + num8;
				}
			}
		}
		Vector2[] array = new Vector2[4];
		Vector2[] array2 = new Vector2[4];
		array[0] = new Vector2(num, y);
		array[1] = new Vector2(num, num2);
		array[2] = new Vector2(x, num2);
		array[3] = new Vector2(x, y);
		array2[0] = new Vector2(num5, num6);
		array2[1] = new Vector2(num5, num4);
		array2[2] = new Vector2(num3, num4);
		array2[3] = new Vector2(num3, num6);
		Color color = base.color;
		color.a *= mPanel.alpha;
		Color32 item = ((!atlas.premultipliedAlpha) ? color : NGUITools.ApplyPMA(color));
		if (fillDirection == FillDirection.Radial90)
		{
			if (!AdjustRadial(array, array2, mFillAmount, mInvert))
			{
				return;
			}
		}
		else
		{
			if (fillDirection == FillDirection.Radial180)
			{
				Vector2[] array3 = new Vector2[4];
				Vector2[] array4 = new Vector2[4];
				for (int i = 0; i < 2; i++)
				{
					array3[0] = new Vector2(0f, 0f);
					array3[1] = new Vector2(0f, 1f);
					array3[2] = new Vector2(1f, 1f);
					array3[3] = new Vector2(1f, 0f);
					array4[0] = new Vector2(0f, 0f);
					array4[1] = new Vector2(0f, 1f);
					array4[2] = new Vector2(1f, 1f);
					array4[3] = new Vector2(1f, 0f);
					if (mInvert)
					{
						if (i > 0)
						{
							Rotate(array3, i);
							Rotate(array4, i);
						}
					}
					else if (i < 1)
					{
						Rotate(array3, 1 - i);
						Rotate(array4, 1 - i);
					}
					float a;
					float b;
					if (i == 1)
					{
						a = ((!mInvert) ? 1f : 0.5f);
						b = ((!mInvert) ? 0.5f : 1f);
					}
					else
					{
						a = ((!mInvert) ? 0.5f : 1f);
						b = ((!mInvert) ? 1f : 0.5f);
					}
					array3[1].y = Mathf.Lerp(a, b, array3[1].y);
					array3[2].y = Mathf.Lerp(a, b, array3[2].y);
					array4[1].y = Mathf.Lerp(a, b, array4[1].y);
					array4[2].y = Mathf.Lerp(a, b, array4[2].y);
					float fill = mFillAmount * 2f - (float)i;
					bool flag = i % 2 == 1;
					if (!AdjustRadial(array3, array4, fill, !flag))
					{
						continue;
					}
					if (mInvert)
					{
						flag = !flag;
					}
					if (flag)
					{
						for (int j = 0; j < 4; j++)
						{
							a = Mathf.Lerp(array[0].x, array[2].x, array3[j].x);
							b = Mathf.Lerp(array[0].y, array[2].y, array3[j].y);
							float x2 = Mathf.Lerp(array2[0].x, array2[2].x, array4[j].x);
							float y2 = Mathf.Lerp(array2[0].y, array2[2].y, array4[j].y);
							verts.Add(new Vector3(a, b, 0f));
							uvs.Add(new Vector2(x2, y2));
							cols.Add(item);
						}
						continue;
					}
					for (int num9 = 3; num9 > -1; num9--)
					{
						a = Mathf.Lerp(array[0].x, array[2].x, array3[num9].x);
						b = Mathf.Lerp(array[0].y, array[2].y, array3[num9].y);
						float x3 = Mathf.Lerp(array2[0].x, array2[2].x, array4[num9].x);
						float y3 = Mathf.Lerp(array2[0].y, array2[2].y, array4[num9].y);
						verts.Add(new Vector3(a, b, 0f));
						uvs.Add(new Vector2(x3, y3));
						cols.Add(item);
					}
				}
				return;
			}
			if (fillDirection == FillDirection.Radial360)
			{
				float[] array5 = new float[16]
				{
					0.5f, 1f, 0f, 0.5f, 0.5f, 1f, 0.5f, 1f, 0f, 0.5f,
					0.5f, 1f, 0f, 0.5f, 0f, 0.5f
				};
				Vector2[] array6 = new Vector2[4];
				Vector2[] array7 = new Vector2[4];
				for (int k = 0; k < 4; k++)
				{
					array6[0] = new Vector2(0f, 0f);
					array6[1] = new Vector2(0f, 1f);
					array6[2] = new Vector2(1f, 1f);
					array6[3] = new Vector2(1f, 0f);
					array7[0] = new Vector2(0f, 0f);
					array7[1] = new Vector2(0f, 1f);
					array7[2] = new Vector2(1f, 1f);
					array7[3] = new Vector2(1f, 0f);
					if (mInvert)
					{
						if (k > 0)
						{
							Rotate(array6, k);
							Rotate(array7, k);
						}
					}
					else if (k < 3)
					{
						Rotate(array6, 3 - k);
						Rotate(array7, 3 - k);
					}
					for (int l = 0; l < 4; l++)
					{
						int num10 = ((!mInvert) ? (k * 4) : ((3 - k) * 4));
						float a2 = array5[num10];
						float b2 = array5[num10 + 1];
						float a3 = array5[num10 + 2];
						float b3 = array5[num10 + 3];
						array6[l].x = Mathf.Lerp(a2, b2, array6[l].x);
						array6[l].y = Mathf.Lerp(a3, b3, array6[l].y);
						array7[l].x = Mathf.Lerp(a2, b2, array7[l].x);
						array7[l].y = Mathf.Lerp(a3, b3, array7[l].y);
					}
					float fill2 = mFillAmount * 4f - (float)k;
					bool flag2 = k % 2 == 1;
					if (!AdjustRadial(array6, array7, fill2, !flag2))
					{
						continue;
					}
					if (mInvert)
					{
						flag2 = !flag2;
					}
					if (flag2)
					{
						for (int m = 0; m < 4; m++)
						{
							float x4 = Mathf.Lerp(array[0].x, array[2].x, array6[m].x);
							float y4 = Mathf.Lerp(array[0].y, array[2].y, array6[m].y);
							float x5 = Mathf.Lerp(array2[0].x, array2[2].x, array7[m].x);
							float y5 = Mathf.Lerp(array2[0].y, array2[2].y, array7[m].y);
							verts.Add(new Vector3(x4, y4, 0f));
							uvs.Add(new Vector2(x5, y5));
							cols.Add(item);
						}
						continue;
					}
					for (int num11 = 3; num11 > -1; num11--)
					{
						float x6 = Mathf.Lerp(array[0].x, array[2].x, array6[num11].x);
						float y6 = Mathf.Lerp(array[0].y, array[2].y, array6[num11].y);
						float x7 = Mathf.Lerp(array2[0].x, array2[2].x, array7[num11].x);
						float y7 = Mathf.Lerp(array2[0].y, array2[2].y, array7[num11].y);
						verts.Add(new Vector3(x6, y6, 0f));
						uvs.Add(new Vector2(x7, y7));
						cols.Add(item);
					}
				}
				return;
			}
		}
		for (int n = 0; n < 4; n++)
		{
			verts.Add(array[n]);
			uvs.Add(array2[n]);
			cols.Add(item);
		}
	}

	protected void TiledFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Texture texture = material.mainTexture;
		if (texture == null)
		{
			return;
		}
		Rect rect = mInner;
		if (atlas.coordinates == UIAtlas.Coordinates.TexCoords)
		{
			rect = NGUIMath.ConvertToPixels(rect, texture.width, texture.height, true);
		}
		Vector2 vector = base.cachedTransform.localScale;
		float pixelSize = atlas.pixelSize;
		float num = Mathf.Abs(rect.width / vector.x) * pixelSize;
		float num2 = Mathf.Abs(rect.height / vector.y) * pixelSize;
		if (num * num2 < 0.0001f)
		{
			num = 0.01f;
			num2 = 0.01f;
		}
		Vector2 vector2 = new Vector2(rect.xMin / (float)texture.width, rect.yMin / (float)texture.height);
		Vector2 vector3 = new Vector2(rect.xMax / (float)texture.width, rect.yMax / (float)texture.height);
		Vector2 vector4 = vector3;
		Color color = base.color;
		color.a *= mPanel.alpha;
		Color32 item = ((!atlas.premultipliedAlpha) ? color : NGUITools.ApplyPMA(color));
		for (float num3 = 0f; num3 < 1f; num3 += num2)
		{
			float num4 = 0f;
			vector4.x = vector3.x;
			float num5 = num3 + num2;
			if (num5 > 1f)
			{
				vector4.y = vector2.y + (vector3.y - vector2.y) * (1f - num3) / (num5 - num3);
				num5 = 1f;
			}
			for (; num4 < 1f; num4 += num)
			{
				float num6 = num4 + num;
				if (num6 > 1f)
				{
					vector4.x = vector2.x + (vector3.x - vector2.x) * (1f - num4) / (num6 - num4);
					num6 = 1f;
				}
				verts.Add(new Vector3(num6, 0f - num3, 0f));
				verts.Add(new Vector3(num6, 0f - num5, 0f));
				verts.Add(new Vector3(num4, 0f - num5, 0f));
				verts.Add(new Vector3(num4, 0f - num3, 0f));
				uvs.Add(new Vector2(vector4.x, 1f - vector2.y));
				uvs.Add(new Vector2(vector4.x, 1f - vector4.y));
				uvs.Add(new Vector2(vector2.x, 1f - vector4.y));
				uvs.Add(new Vector2(vector2.x, 1f - vector2.y));
				cols.Add(item);
				cols.Add(item);
				cols.Add(item);
				cols.Add(item);
			}
		}
	}
}
