using UnityEngine;

//[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Panel")]
public class UIPanel : MonoBehaviour
{
	public enum DebugInfo
	{
		None,
		Gizmos,
		Geometry
	}

	public delegate void OnChangeDelegate();

	public OnChangeDelegate onChange;

	public bool sortByDepth;

	public bool showInPanelTool = true;

	public bool generateNormals;

	public bool depthPass;

	public bool widgetsAreStatic;

	public bool cullWhileDragging;

	[HideInInspector]
	public Matrix4x4 worldToLocal = Matrix4x4.identity;

	[HideInInspector]
	[SerializeField]
	private float mAlpha = 1f;

	[HideInInspector]
	[SerializeField]
	private DebugInfo mDebugInfo = DebugInfo.Gizmos;

	[SerializeField]
	[HideInInspector]
	private UIDrawCall.Clipping mClipping;

	[SerializeField]
	[HideInInspector]
	private Vector4 mClipRange = Vector4.zero;

	[SerializeField]
	[HideInInspector]
	private Vector2 mClipSoftness = new Vector2(40f, 40f);

	private BetterList<UIWidget> mWidgets = new BetterList<UIWidget>();

	private BetterList<Material> mChanged = new BetterList<Material>();

	private BetterList<UIDrawCall> mDrawCalls = new BetterList<UIDrawCall>();

	private BetterList<Vector3> mVerts = new BetterList<Vector3>();

	private BetterList<Vector3> mNorms = new BetterList<Vector3>();

	private BetterList<Vector4> mTans = new BetterList<Vector4>();

	private BetterList<Vector2> mUvs = new BetterList<Vector2>();

	private BetterList<Color32> mCols = new BetterList<Color32>();

	private GameObject mGo;

	private Transform mTrans;

	private Camera mCam;

	private int mLayer = -1;

	private bool mDepthChanged;

	private float mCullTime;

	private float mUpdateTime;

	private float mMatrixTime;

	private static float[] mTemp = new float[4];

	private Vector2 mMin = Vector2.zero;

	private Vector2 mMax = Vector2.zero;

	private UIPanel[] mChildPanels;

	public GameObject cachedGameObject
	{
		get
		{
			if (mGo == null)
			{
				mGo = base.gameObject;
			}
			return mGo;
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

	public float alpha
	{
		get
		{
			return mAlpha;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if (mAlpha != num)
			{
				mAlpha = num;
				for (int i = 0; i < mDrawCalls.size; i++)
				{
					UIDrawCall uIDrawCall = mDrawCalls[i];
					MarkMaterialAsChanged(uIDrawCall.material, false);
				}
				for (int j = 0; j < mWidgets.size; j++)
				{
					mWidgets[j].MarkAsChangedLite();
				}
			}
		}
	}

	public DebugInfo debugInfo
	{
		get
		{
			return mDebugInfo;
		}
		set
		{
			if (mDebugInfo != value)
			{
				mDebugInfo = value;
				BetterList<UIDrawCall> betterList = drawCalls;
				HideFlags hideFlags = ((mDebugInfo != DebugInfo.Geometry) ? HideFlags.HideAndDontSave : (HideFlags.DontSave | HideFlags.NotEditable));
				int i = 0;
				for (int size = betterList.size; i < size; i++)
				{
					UIDrawCall uIDrawCall = betterList[i];
					GameObject gameObject = uIDrawCall.gameObject;
					NGUITools.SetActiveSelf(gameObject, false);
					gameObject.hideFlags = hideFlags;
					NGUITools.SetActiveSelf(gameObject, true);
				}
			}
		}
	}

	public UIDrawCall.Clipping clipping
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
				mMatrixTime = 0f;
				UpdateDrawcalls();
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
			if (mClipRange != value)
			{
				mCullTime = ((mCullTime != 0f) ? (Time.realtimeSinceStartup + 0.15f) : 0.001f);
				mClipRange = value;
				mMatrixTime = 0f;
				UpdateDrawcalls();
			}
		}
	}

	public Vector2 clipSoftness
	{
		get
		{
			return mClipSoftness;
		}
		set
		{
			if (mClipSoftness != value)
			{
				mClipSoftness = value;
				UpdateDrawcalls();
			}
		}
	}

	public BetterList<UIWidget> widgets
	{
		get
		{
			return mWidgets;
		}
	}

	public BetterList<UIDrawCall> drawCalls
	{
		get
		{
			int num = mDrawCalls.size;
			while (num > 0)
			{
				UIDrawCall uIDrawCall = mDrawCalls[--num];
				if (uIDrawCall == null)
				{
					mDrawCalls.RemoveAt(num);
				}
			}
			return mDrawCalls;
		}
	}

	public void SetAlphaRecursive(float val, bool rebuildList)
	{
		if (rebuildList || mChildPanels == null)
		{
			mChildPanels = GetComponentsInChildren<UIPanel>(true);
		}
		int i = 0;
		for (int num = mChildPanels.Length; i < num; i++)
		{
			mChildPanels[i].alpha = val;
		}
	}

	private bool IsVisible(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
	{
		UpdateTransformMatrix();
		a = worldToLocal.MultiplyPoint3x4(a);
		b = worldToLocal.MultiplyPoint3x4(b);
		c = worldToLocal.MultiplyPoint3x4(c);
		d = worldToLocal.MultiplyPoint3x4(d);
		mTemp[0] = a.x;
		mTemp[1] = b.x;
		mTemp[2] = c.x;
		mTemp[3] = d.x;
		float num = Mathf.Min(mTemp);
		float num2 = Mathf.Max(mTemp);
		mTemp[0] = a.y;
		mTemp[1] = b.y;
		mTemp[2] = c.y;
		mTemp[3] = d.y;
		float num3 = Mathf.Min(mTemp);
		float num4 = Mathf.Max(mTemp);
		if (num2 < mMin.x)
		{
			return false;
		}
		if (num4 < mMin.y)
		{
			return false;
		}
		if (num > mMax.x)
		{
			return false;
		}
		if (num3 > mMax.y)
		{
			return false;
		}
		return true;
	}

	public bool IsVisible(Vector3 worldPos)
	{
		if (mAlpha < 0.001f)
		{
			return false;
		}
		if (mClipping == UIDrawCall.Clipping.None)
		{
			return true;
		}
		UpdateTransformMatrix();
		Vector3 vector = worldToLocal.MultiplyPoint3x4(worldPos);
		if (vector.x < mMin.x)
		{
			return false;
		}
		if (vector.y < mMin.y)
		{
			return false;
		}
		if (vector.x > mMax.x)
		{
			return false;
		}
		if (vector.y > mMax.y)
		{
			return false;
		}
		return true;
	}

	public bool IsVisible(UIWidget w)
	{
		if (mAlpha < 0.001f)
		{
			return false;
		}
		if (!w.enabled || !NGUITools.GetActive(w.cachedGameObject) || w.alpha < 0.001f)
		{
			return false;
		}
		if (mClipping == UIDrawCall.Clipping.None)
		{
			return true;
		}
		Vector2 relativeSize = w.relativeSize;
		Vector2 vector = Vector2.Scale(w.pivotOffset, relativeSize);
		Vector2 vector2 = vector;
		vector.x += relativeSize.x;
		vector.y -= relativeSize.y;
		Transform transform = w.cachedTransform;
		Vector3 a = transform.TransformPoint(vector);
		Vector3 b = transform.TransformPoint(new Vector2(vector.x, vector2.y));
		Vector3 c = transform.TransformPoint(new Vector2(vector2.x, vector.y));
		Vector3 d = transform.TransformPoint(vector2);
		return IsVisible(a, b, c, d);
	}

	public void MarkMaterialAsChanged(Material mat, bool sort)
	{
		if (mat != null)
		{
			if (sort)
			{
				mDepthChanged = true;
			}
			if (!mChanged.Contains(mat))
			{
				mChanged.Add(mat);
			}
		}
	}

	public void AddWidget(UIWidget w)
	{
		if (w != null && !mWidgets.Contains(w))
		{
			mWidgets.Add(w);
			if (!mChanged.Contains(w.material))
			{
				mChanged.Add(w.material);
			}
			mDepthChanged = true;
		}
	}

	public void RemoveWidget(UIWidget w)
	{
		if (w != null && w != null && mWidgets.Remove(w) && w.material != null)
		{
			mChanged.Add(w.material);
		}
	}

	private UIDrawCall GetDrawCall(Material mat, bool createIfMissing)
	{
		int i = 0;
		for (int size = drawCalls.size; i < size; i++)
		{
			UIDrawCall uIDrawCall = drawCalls.buffer[i];
			if (uIDrawCall.material == mat)
			{
				return uIDrawCall;
			}
		}
		UIDrawCall uIDrawCall2 = null;
		if (createIfMissing)
		{
			GameObject gameObject = new GameObject("_UIDrawCall [" + mat.name + "]");
			Object.DontDestroyOnLoad(gameObject);
			gameObject.layer = cachedGameObject.layer;
			uIDrawCall2 = gameObject.AddComponent<UIDrawCall>();
			uIDrawCall2.material = mat;
			mDrawCalls.Add(uIDrawCall2);
		}
		return uIDrawCall2;
	}

	private void Awake()
	{
		mGo = base.gameObject;
		mTrans = base.transform;
	}

	private void Start()
	{
		mLayer = mGo.layer;
		UICamera uICamera = UICamera.FindCameraForLayer(mLayer);
		mCam = ((!(uICamera != null)) ? NGUITools.FindCameraForLayer(mLayer) : uICamera.cachedCamera);
	}

	private void OnEnable()
	{
		int num = 0;
		while (num < mWidgets.size)
		{
			UIWidget uIWidget = mWidgets.buffer[num];
			if (uIWidget != null)
			{
				MarkMaterialAsChanged(uIWidget.material, true);
				num++;
			}
			else
			{
				mWidgets.RemoveAt(num);
			}
		}
	}

	private void OnDisable()
	{
		int num = mDrawCalls.size;
		while (num > 0)
		{
			UIDrawCall uIDrawCall = mDrawCalls.buffer[--num];
			if (uIDrawCall != null)
			{
				NGUITools.DestroyImmediate(uIDrawCall.gameObject);
			}
		}
		mDrawCalls.Release();
		mChanged.Release();
	}

	private void UpdateTransformMatrix()
	{
		if (mUpdateTime != 0f && mMatrixTime == mUpdateTime)
		{
			return;
		}
		mMatrixTime = mUpdateTime;
		worldToLocal = cachedTransform.worldToLocalMatrix;
		if (mClipping != 0)
		{
			Vector2 vector = new Vector2(mClipRange.z, mClipRange.w);
			if (vector.x == 0f)
			{
				vector.x = ((!(mCam == null)) ? mCam.pixelWidth : Screen.width);
			}
			if (vector.y == 0f)
			{
				vector.y = ((!(mCam == null)) ? mCam.pixelHeight : Screen.height);
			}
			vector *= 0.5f;
			mMin.x = mClipRange.x - vector.x;
			mMin.y = mClipRange.y - vector.y;
			mMax.x = mClipRange.x + vector.x;
			mMax.y = mClipRange.y + vector.y;
		}
	}

	public void UpdateDrawcalls()
	{
		Vector4 vector = Vector4.zero;
		if (mClipping != 0)
		{
			vector = new Vector4(mClipRange.x, mClipRange.y, mClipRange.z * 0.5f, mClipRange.w * 0.5f);
		}
		if (vector.z == 0f)
		{
			vector.z = (float)Screen.width * 0.5f;
		}
		if (vector.w == 0f)
		{
			vector.w = (float)Screen.height * 0.5f;
		}
		RuntimePlatform platform = Application.platform;
		if (platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsEditor)
		{
			vector.x -= 0.5f;
			vector.y += 0.5f;
		}
		Transform transform = cachedTransform;
		int i = 0;
		for (int size = mDrawCalls.size; i < size; i++)
		{
			UIDrawCall uIDrawCall = mDrawCalls.buffer[i];
			uIDrawCall.clipping = mClipping;
			uIDrawCall.clipRange = vector;
			uIDrawCall.clipSoftness = mClipSoftness;
			uIDrawCall.depthPass = depthPass && mClipping == UIDrawCall.Clipping.None;
			Transform transform2 = uIDrawCall.transform;
			transform2.position = transform.position;
			transform2.rotation = transform.rotation;
			transform2.localScale = transform.lossyScale;
		}
	}

	private void Fill(Material mat)
	{
		int num = -100;
		int num2 = 0;
		while (num2 < mWidgets.size)
		{
			UIWidget uIWidget = mWidgets.buffer[num2];
			if (uIWidget == null)
			{
				mWidgets.RemoveAt(num2);
				continue;
			}
			if (uIWidget.material == mat && uIWidget.isVisible)
			{
				if (!(uIWidget.panel == this))
				{
					mWidgets.RemoveAt(num2);
					continue;
				}
				int depth = uIWidget.depth;
				if (depth > num)
				{
					num = depth;
				}
				if (generateNormals)
				{
					uIWidget.WriteToBuffers(mVerts, mUvs, mCols, mNorms, mTans);
				}
				else
				{
					uIWidget.WriteToBuffers(mVerts, mUvs, mCols, null, null);
				}
			}
			num2++;
		}
		if (mVerts.size > 0)
		{
			UIDrawCall drawCall = GetDrawCall(mat, true);
			drawCall.depthPass = depthPass && mClipping == UIDrawCall.Clipping.None;
			drawCall.depth = (sortByDepth ? num : 0);
			drawCall.Set(mVerts, (!generateNormals) ? null : mNorms, (!generateNormals) ? null : mTans, mUvs, mCols);
			drawCall.mainTexture = mat.mainTexture;
		}
		else
		{
			UIDrawCall drawCall2 = GetDrawCall(mat, false);
			if (drawCall2 != null)
			{
				mDrawCalls.Remove(drawCall2);
				NGUITools.DestroyImmediate(drawCall2.gameObject);
			}
		}
		mVerts.Clear();
		mNorms.Clear();
		mTans.Clear();
		mUvs.Clear();
		mCols.Clear();
	}

	private void LateUpdate()
	{
		mUpdateTime = Time.realtimeSinceStartup;
		UpdateTransformMatrix();
		if (mLayer != cachedGameObject.layer)
		{
			mLayer = mGo.layer;
			UICamera uICamera = UICamera.FindCameraForLayer(mLayer);
			mCam = ((!(uICamera != null)) ? NGUITools.FindCameraForLayer(mLayer) : uICamera.cachedCamera);
			SetChildLayer(cachedTransform, mLayer);
			int i = 0;
			for (int size = drawCalls.size; i < size; i++)
			{
				mDrawCalls.buffer[i].gameObject.layer = mLayer;
			}
		}
		bool forceVisible = !cullWhileDragging && (clipping == UIDrawCall.Clipping.None || mCullTime > mUpdateTime);
		int j = 0;
		for (int size2 = mWidgets.size; j < size2; j++)
		{
			UIWidget uIWidget = mWidgets[j];
			if (uIWidget.UpdateGeometry(this, forceVisible) && !mChanged.Contains(uIWidget.material))
			{
				mChanged.Add(uIWidget.material);
			}
		}
		if (mChanged.size != 0 && onChange != null)
		{
			onChange();
		}
		if (mDepthChanged)
		{
			mDepthChanged = false;
			mWidgets.Sort(UIWidget.CompareFunc);
		}
		int k = 0;
		for (int size3 = mChanged.size; k < size3; k++)
		{
			Fill(mChanged.buffer[k]);
		}
		UpdateDrawcalls();
		mChanged.Clear();
	}

	public void Refresh()
	{
		UIWidget[] componentsInChildren = GetComponentsInChildren<UIWidget>();
		int i = 0;
		for (int num = componentsInChildren.Length; i < num; i++)
		{
			componentsInChildren[i].Update();
		}
		LateUpdate();
	}

	public Vector3 CalculateConstrainOffset(Vector2 min, Vector2 max)
	{
		float num = clipRange.z * 0.5f;
		float num2 = clipRange.w * 0.5f;
		Vector2 minRect = new Vector2(min.x, min.y);
		Vector2 maxRect = new Vector2(max.x, max.y);
		Vector2 minArea = new Vector2(clipRange.x - num, clipRange.y - num2);
		Vector2 maxArea = new Vector2(clipRange.x + num, clipRange.y + num2);
		if (clipping == UIDrawCall.Clipping.SoftClip)
		{
			minArea.x += clipSoftness.x;
			minArea.y += clipSoftness.y;
			maxArea.x -= clipSoftness.x;
			maxArea.y -= clipSoftness.y;
		}
		return NGUIMath.ConstrainRect(minRect, maxRect, minArea, maxArea);
	}

	public bool ConstrainTargetToBounds(Transform target, ref Bounds targetBounds, bool immediate)
	{
		Vector3 vector = CalculateConstrainOffset(targetBounds.min, targetBounds.max);
		if (vector.magnitude > 0f)
		{
			if (immediate)
			{
				target.localPosition += vector;
				targetBounds.center += vector;
				SpringPosition component = target.GetComponent<SpringPosition>();
				if (component != null)
				{
					component.enabled = false;
				}
			}
			else
			{
				SpringPosition springPosition = SpringPosition.Begin(target.gameObject, target.localPosition + vector, 13f);
				springPosition.ignoreTimeScale = true;
				springPosition.worldSpace = false;
			}
			return true;
		}
		return false;
	}

	public bool ConstrainTargetToBounds(Transform target, bool immediate)
	{
		Bounds targetBounds = NGUIMath.CalculateRelativeWidgetBounds(cachedTransform, target);
		return ConstrainTargetToBounds(target, ref targetBounds, immediate);
	}

	private static void SetChildLayer(Transform t, int layer)
	{
		for (int i = 0; i < t.childCount; i++)
		{
			Transform child = t.GetChild(i);
			if (child.GetComponent<UIPanel>() == null)
			{
				if (child.GetComponent<UIWidget>() != null)
				{
					child.gameObject.layer = layer;
				}
				SetChildLayer(child, layer);
			}
		}
	}

	public static UIPanel Find(Transform trans, bool createIfMissing)
	{
		Transform transform = trans;
		UIPanel uIPanel = null;
		while (uIPanel == null && trans != null)
		{
			uIPanel = trans.GetComponent<UIPanel>();
			if (uIPanel != null || trans.parent == null)
			{
				break;
			}
			trans = trans.parent;
		}
		if (createIfMissing && uIPanel == null && trans != transform)
		{
			uIPanel = trans.gameObject.AddComponent<UIPanel>();
			uIPanel.sortByDepth = true;
			SetChildLayer(uIPanel.cachedTransform, uIPanel.cachedGameObject.layer);
		}
		return uIPanel;
	}

	public static UIPanel Find(Transform trans)
	{
		return Find(trans, true);
	}
}
