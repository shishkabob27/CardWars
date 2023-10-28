using UnityEngine;

[AddComponentMenu("NGUI/UI/Texture")]
[ExecuteInEditMode]
public class UITexture : UIWidget
{
	[HideInInspector]
	[SerializeField]
	private Rect mRect = new Rect(0f, 0f, 1f, 1f);

	[HideInInspector]
	[SerializeField]
	private Shader mShader;

	[HideInInspector]
	[SerializeField]
	private Texture mTexture;

	[HideInInspector]
	[SerializeField]
	private Material mMat;

	private bool mCreatingMat;

	private Material mDynamicMat;

	private int mPMA = -1;

	public Rect uvRect
	{
		get
		{
			return mRect;
		}
		set
		{
			if (mRect != value)
			{
				mRect = value;
				MarkAsChanged();
			}
		}
	}

	public Shader shader
	{
		get
		{
			if (mShader == null)
			{
				Material material = this.material;
				if (material != null)
				{
					mShader = material.shader;
				}
				if (mShader == null)
				{
					mShader = Shader.Find("Unlit/Transparent Colored");
				}
			}
			return mShader;
		}
		set
		{
			if (mShader != value)
			{
				mShader = value;
				Material material = this.material;
				if (material != null)
				{
					material.shader = value;
				}
				mPMA = -1;
			}
		}
	}

	public bool hasDynamicMaterial
	{
		get
		{
			return mDynamicMat != null;
		}
	}

	public override Material material
	{
		get
		{
			if (mMat != null)
			{
				return mMat;
			}
			if (mDynamicMat != null)
			{
				return mDynamicMat;
			}
			if (!mCreatingMat && mDynamicMat == null)
			{
				mCreatingMat = true;
				if (mShader == null)
				{
					mShader = Shader.Find("Unlit/Texture");
				}
				Cleanup();
				mDynamicMat = new Material(mShader);
				mDynamicMat.hideFlags = HideFlags.DontSave;
				mDynamicMat.mainTexture = mTexture;
				mPMA = 0;
				mCreatingMat = false;
			}
			return mDynamicMat;
		}
		set
		{
			if (mMat != value)
			{
				Cleanup();
				mMat = value;
				mPMA = -1;
				MarkAsChanged();
			}
		}
	}

	public bool premultipliedAlpha
	{
		get
		{
			if (mPMA == -1)
			{
				Material material = this.material;
				mPMA = ((material != null && material.shader != null && material.shader.name.Contains("Premultiplied")) ? 1 : 0);
			}
			return mPMA == 1;
		}
	}

	public override Texture mainTexture
	{
		get
		{
			if (mMat != null)
			{
				return mMat.mainTexture;
			}
			if (mTexture != null)
			{
				return mTexture;
			}
			return null;
		}
		set
		{
			RemoveFromPanel();
			Material material = this.material;
			if (material != null)
			{
				mPanel = null;
				mTexture = value;
				material.mainTexture = value;
				if (base.enabled)
				{
					CreatePanel();
				}
			}
		}
	}

	private void OnDestroy()
	{
		Cleanup();
	}

	private void Cleanup()
	{
		if (mDynamicMat != null)
		{
			NGUITools.Destroy(mDynamicMat);
			mDynamicMat = null;
		}
	}

	public override void MakePixelPerfect()
	{
		Texture texture = mainTexture;
		if (texture != null)
		{
			Vector3 localScale = base.cachedTransform.localScale;
			localScale.x = (float)texture.width * uvRect.width;
			localScale.y = (float)texture.height * uvRect.height;
			localScale.z = 1f;
			base.cachedTransform.localScale = localScale;
		}
		base.MakePixelPerfect();
	}

	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Color color = base.color;
		color.a *= mPanel.alpha;
		Color32 item = ((!premultipliedAlpha) ? color : NGUITools.ApplyPMA(color));
		verts.Add(new Vector3(1f, 0f, 0f));
		verts.Add(new Vector3(1f, -1f, 0f));
		verts.Add(new Vector3(0f, -1f, 0f));
		verts.Add(new Vector3(0f, 0f, 0f));
		uvs.Add(new Vector2(mRect.xMax, mRect.yMax));
		uvs.Add(new Vector2(mRect.xMax, mRect.yMin));
		uvs.Add(new Vector2(mRect.xMin, mRect.yMin));
		uvs.Add(new Vector2(mRect.xMin, mRect.yMax));
		cols.Add(item);
		cols.Add(item);
		cols.Add(item);
		cols.Add(item);
	}
}
