using UnityEngine;

[AddComponentMenu("NGUI/UI/Anchor")]
[ExecuteInEditMode]
public class UIAnchor : MonoBehaviour
{
	public enum Side
	{
		BottomLeft,
		Left,
		TopLeft,
		Top,
		TopRight,
		Right,
		BottomRight,
		Bottom,
		Center
	}

	private bool mNeedsHalfPixelOffset;

	public Camera uiCamera;

	public UIWidget widgetContainer;

	public UIPanel panelContainer;

	public Side side = Side.Center;

	public bool halfPixelOffset = true;

	public bool runOnlyOnce;

	public Vector2 relativeOffset = Vector2.zero;

	public Vector2 pixelOffset = Vector2.zero;

	private Transform mTrans;

	protected Animation mAnim;

	private Rect mRect = default(Rect);

	private UIRoot mRoot;

	private void Awake()
	{
		mTrans = base.transform;
		mAnim = GetComponent<Animation>();
	}

	protected virtual void Start()
	{
		mRoot = NGUITools.FindInParents<UIRoot>(base.gameObject);
		mNeedsHalfPixelOffset = Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor;
		if (mNeedsHalfPixelOffset)
		{
			mNeedsHalfPixelOffset = SystemInfo.graphicsShaderLevel < 40;
		}
		if (uiCamera == null)
		{
			uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
		}
		Update();
	}

	protected virtual void Update()
	{
		if (mAnim != null && mAnim.enabled && mAnim.isPlaying)
		{
			return;
		}
		bool flag = false;
		if (panelContainer != null)
		{
			if (panelContainer.clipping == UIDrawCall.Clipping.None)
			{
				float num = ((!(mRoot != null)) ? 0.5f : ((float)mRoot.activeHeight / (float)Screen.height * 0.5f));
				mRect.xMin = (float)(-Screen.width) * num;
				mRect.yMin = (float)(-Screen.height) * num;
				mRect.xMax = 0f - mRect.xMin;
				mRect.yMax = 0f - mRect.yMin;
			}
			else
			{
				Vector4 clipRange = panelContainer.clipRange;
				mRect.x = clipRange.x - clipRange.z * 0.5f;
				mRect.y = clipRange.y - clipRange.w * 0.5f;
				mRect.width = clipRange.z;
				mRect.height = clipRange.w;
			}
		}
		else if (widgetContainer != null)
		{
			Transform cachedTransform = widgetContainer.cachedTransform;
			Vector3 localScale = cachedTransform.localScale;
			Vector3 localPosition = cachedTransform.localPosition;
			Vector3 vector = widgetContainer.relativeSize;
			Vector3 vector2 = widgetContainer.pivotOffset;
			vector2.y -= 1f;
			vector2.x *= widgetContainer.relativeSize.x * localScale.x;
			vector2.y *= widgetContainer.relativeSize.y * localScale.y;
			mRect.x = localPosition.x + vector2.x;
			mRect.y = localPosition.y + vector2.y;
			mRect.width = vector.x * localScale.x;
			mRect.height = vector.y * localScale.y;
		}
		else
		{
			if (!(uiCamera != null))
			{
				return;
			}
			flag = true;
			mRect = uiCamera.pixelRect;
		}
		float x = (mRect.xMin + mRect.xMax) * 0.5f;
		float y = (mRect.yMin + mRect.yMax) * 0.5f;
		Vector3 vector3 = new Vector3(x, y, 0f);
		if (side != Side.Center)
		{
			if (side == Side.Right || side == Side.TopRight || side == Side.BottomRight)
			{
				vector3.x = mRect.xMax;
			}
			else if (side == Side.Top || side == Side.Center || side == Side.Bottom)
			{
				vector3.x = x;
			}
			else
			{
				vector3.x = mRect.xMin;
			}
			if (side == Side.Top || side == Side.TopRight || side == Side.TopLeft)
			{
				vector3.y = mRect.yMax;
			}
			else if (side == Side.Left || side == Side.Center || side == Side.Right)
			{
				vector3.y = y;
			}
			else
			{
				vector3.y = mRect.yMin;
			}
		}
		float width = mRect.width;
		float height = mRect.height;
		vector3.x += relativeOffset.x * width;
		vector3.y += relativeOffset.y * height;
		if (flag)
		{
			if (uiCamera.orthographic)
			{
				vector3.x = Mathf.Round(vector3.x);
				vector3.y = Mathf.Round(vector3.y);
				vector3.x += pixelOffset.x;
				vector3.y += pixelOffset.y;
				if (halfPixelOffset && mNeedsHalfPixelOffset)
				{
					vector3.x -= 0.5f;
					vector3.y += 0.5f;
				}
			}
			vector3.z = uiCamera.WorldToScreenPoint(mTrans.position).z;
			vector3 = uiCamera.ScreenToWorldPoint(vector3);
		}
		else
		{
			vector3.x = Mathf.Round(vector3.x);
			vector3.y = Mathf.Round(vector3.y);
			vector3.x += pixelOffset.x;
			vector3.y += pixelOffset.y;
			if (panelContainer != null)
			{
				vector3 = panelContainer.cachedTransform.TransformPoint(vector3);
			}
			else if (widgetContainer != null)
			{
				Transform parent = widgetContainer.cachedTransform.parent;
				if (parent != null)
				{
					vector3 = parent.TransformPoint(vector3);
				}
			}
			vector3.z = mTrans.position.z;
		}
		if (mTrans.position != vector3)
		{
			mTrans.position = vector3;
		}
		if (runOnlyOnce && Application.isPlaying)
		{
			Object.Destroy(this);
		}
	}
}
