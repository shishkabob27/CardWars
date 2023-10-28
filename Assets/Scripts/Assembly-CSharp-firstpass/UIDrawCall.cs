using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Internal/Draw Call")]
public class UIDrawCall : MonoBehaviour
{
	public enum Clipping
	{
		None,
		HardClip,
		AlphaClip,
		SoftClip
	}

	private Transform mTrans;

	private Material mSharedMat;

	private Mesh mMesh0;

	private Mesh mMesh1;

	private MeshFilter mFilter;

	private MeshRenderer mRen;

	private Clipping mClipping;

	private Vector4 mClipRange;

	private Vector2 mClipSoft;

	private Material mMat;

	private Material mDepthMat;

	private int[] mIndices;

	private bool mUseDepth;

	private bool mReset = true;

	private bool mEven = true;

	private int mDepth;

	public bool depthPass
	{
		get
		{
			return mUseDepth;
		}
		set
		{
			if (mUseDepth != value)
			{
				mUseDepth = value;
				mReset = true;
			}
		}
	}

	public int depth
	{
		get
		{
			return mDepth;
		}
		set
		{
			if (mDepth != value)
			{
				mDepth = value;
				if (mMat != null && mSharedMat != null)
				{
					mMat.renderQueue = mSharedMat.renderQueue + value;
				}
			}
		}
	}

	public Transform cachedTransform
	{
		get
		{
			if (mTrans == null)
			{
				mTrans = base.transform;
			}
			return mTrans;
		}
	}

	public Material material
	{
		get
		{
			return mSharedMat;
		}
		set
		{
			mSharedMat = value;
		}
	}

	public Texture mainTexture
	{
		get
		{
			return mMat.mainTexture;
		}
		set
		{
			mMat.mainTexture = value;
		}
	}

	public int triangles
	{
		get
		{
			Mesh mesh = ((!mEven) ? mMesh1 : mMesh0);
			return (mesh != null) ? (mesh.vertexCount >> 1) : 0;
		}
	}

	public bool isClipped
	{
		get
		{
			return mClipping != Clipping.None;
		}
	}

	public Clipping clipping
	{
		get
		{
			return mClipping;
		}
		set
		{
			if (mClipping != value)
			{
				mClipping = value;
				mReset = true;
			}
		}
	}

	public Vector4 clipRange
	{
		get
		{
			return mClipRange;
		}
		set
		{
			mClipRange = value;
		}
	}

	public Vector2 clipSoftness
	{
		get
		{
			return mClipSoft;
		}
		set
		{
			mClipSoft = value;
		}
	}

	private Mesh GetMesh(ref bool rebuildIndices, int vertexCount)
	{
		mEven = !mEven;
		if (mEven)
		{
			if (mMesh0 == null)
			{
				mMesh0 = new Mesh();
				mMesh0.hideFlags = HideFlags.DontSave;
				mMesh0.name = "Mesh0 for " + mSharedMat.name;
				mMesh0.MarkDynamic();
				rebuildIndices = true;
			}
			else if (rebuildIndices || mMesh0.vertexCount != vertexCount)
			{
				rebuildIndices = true;
				mMesh0.Clear();
			}
			return mMesh0;
		}
		if (mMesh1 == null)
		{
			mMesh1 = new Mesh();
			mMesh1.hideFlags = HideFlags.DontSave;
			mMesh1.name = "Mesh1 for " + mSharedMat.name;
			mMesh1.MarkDynamic();
			rebuildIndices = true;
		}
		else if (rebuildIndices || mMesh1.vertexCount != vertexCount)
		{
			rebuildIndices = true;
			mMesh1.Clear();
		}
		return mMesh1;
	}

	private void UpdateMaterials()
	{
		bool flag = mClipping != Clipping.None;
		if (mMat == null)
		{
			mMat = new Material(mSharedMat);
			mMat.hideFlags = HideFlags.DontSave;
			mMat.CopyPropertiesFromMaterial(mSharedMat);
			mMat.renderQueue = mSharedMat.renderQueue + mDepth;
		}
		if (flag && mClipping != 0)
		{
			Shader shader = null;
			string text = mSharedMat.shader.name;
			text = text.Replace(" (AlphaClip)", string.Empty);
			text = text.Replace(" (SoftClip)", string.Empty);
			if (mClipping == Clipping.HardClip || mClipping == Clipping.AlphaClip)
			{
				shader = Shader.Find(text + " (AlphaClip)");
			}
			else if (mClipping == Clipping.SoftClip)
			{
				shader = Shader.Find(text + " (SoftClip)");
			}
			if (shader != null)
			{
				mMat.shader = shader;
			}
			else
			{
				mClipping = Clipping.None;
			}
		}
		if (mUseDepth)
		{
			if (mDepthMat == null)
			{
				Shader shader2 = Shader.Find("Unlit/Depth Cutout");
				mDepthMat = new Material(shader2);
				mDepthMat.hideFlags = HideFlags.DontSave;
			}
			mDepthMat.mainTexture = mSharedMat.mainTexture;
		}
		else if (mDepthMat != null)
		{
			NGUITools.Destroy(mDepthMat);
			mDepthMat = null;
		}
		if (mDepthMat != null)
		{
			if (mRen.sharedMaterials == null || mRen.sharedMaterials.Length != 2 || !(mRen.sharedMaterials[1] == mMat))
			{
				mRen.sharedMaterials = new Material[2] { mDepthMat, mMat };
			}
		}
		else if (mRen.sharedMaterial != mMat)
		{
			mRen.sharedMaterials = new Material[1] { mMat };
		}
	}

	public void Set(BetterList<Vector3> verts, BetterList<Vector3> norms, BetterList<Vector4> tans, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		int size = verts.size;
		if (size > 0 && size == uvs.size && size == cols.size && size % 4 == 0)
		{
			if (mFilter == null)
			{
				mFilter = base.gameObject.GetComponent<MeshFilter>();
			}
			if (mFilter == null)
			{
				mFilter = base.gameObject.AddComponent<MeshFilter>();
			}
			if (mRen == null)
			{
				mRen = base.gameObject.GetComponent<MeshRenderer>();
			}
			if (mRen == null)
			{
				mRen = base.gameObject.AddComponent<MeshRenderer>();
				UpdateMaterials();
			}
			else if (mMat != null && mMat.mainTexture != mSharedMat.mainTexture)
			{
				UpdateMaterials();
			}
			if (verts.size < 65000)
			{
				int num = (size >> 1) * 3;
				bool rebuildIndices = mIndices == null || mIndices.Length != num;
				if (rebuildIndices)
				{
					mIndices = new int[num];
					int num2 = 0;
					for (int i = 0; i < size; i += 4)
					{
						mIndices[num2++] = i;
						mIndices[num2++] = i + 1;
						mIndices[num2++] = i + 2;
						mIndices[num2++] = i + 2;
						mIndices[num2++] = i + 3;
						mIndices[num2++] = i;
					}
				}
				Mesh mesh = GetMesh(ref rebuildIndices, verts.size);
				mesh.vertices = verts.ToArray();
				if (norms != null)
				{
					mesh.normals = norms.ToArray();
				}
				if (tans != null)
				{
					mesh.tangents = tans.ToArray();
				}
				mesh.uv = uvs.ToArray();
				mesh.colors32 = cols.ToArray();
				if (rebuildIndices)
				{
					mesh.triangles = mIndices;
				}
				mesh.RecalculateBounds();
				mFilter.mesh = mesh;
			}
			else if (mFilter.mesh != null)
			{
				mFilter.mesh.Clear();
			}
		}
		else if (mFilter.mesh != null)
		{
			mFilter.mesh.Clear();
		}
	}

	private void OnWillRenderObject()
	{
		if (mReset)
		{
			mReset = false;
			UpdateMaterials();
		}
		if (mMat != null && isClipped)
		{
			mMat.mainTextureOffset = new Vector2((0f - mClipRange.x) / mClipRange.z, (0f - mClipRange.y) / mClipRange.w);
			mMat.mainTextureScale = new Vector2(1f / mClipRange.z, 1f / mClipRange.w);
			Vector2 vector = new Vector2(1000f, 1000f);
			if (mClipSoft.x > 0f)
			{
				vector.x = mClipRange.z / mClipSoft.x;
			}
			if (mClipSoft.y > 0f)
			{
				vector.y = mClipRange.w / mClipSoft.y;
			}
			mMat.SetVector("_ClipSharpness", vector);
		}
	}

	private void OnDestroy()
	{
		NGUITools.DestroyImmediate(mMesh0);
		NGUITools.DestroyImmediate(mMesh1);
		NGUITools.DestroyImmediate(mMat);
		NGUITools.DestroyImmediate(mDepthMat);
	}
}
