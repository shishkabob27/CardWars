using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Stretch")]
public class UIStretch : MonoBehaviour
{
	public enum Style
	{
		None,
		Horizontal,
		Vertical,
		Both,
		BasedOnHeight,
		FillKeepingRatio,
		FitInternalKeepingRatio
	}

	public Camera uiCamera;

	public UIWidget widgetContainer;

	public UIPanel panelContainer;

	public Style style;

	public bool runOnlyOnce;

	public Vector2 relativeSize = Vector2.one;

	public Vector2 initialSize = Vector2.one;

	private Transform mTrans;

	private UIRoot mRoot;

	private Animation mAnim;

	private Rect mRect;

	private void Awake()
	{
		mAnim = GetComponent<Animation>();
		mRect = default(Rect);
		mTrans = base.transform;
	}

	private void Start()
	{
		if (uiCamera == null)
		{
			uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
		}
		mRoot = NGUITools.FindInParents<UIRoot>(base.gameObject);
		Update();
	}

	private void Update()
	{
		if ((mAnim != null && mAnim.isPlaying) || style == Style.None)
		{
			return;
		}
		float num = 1f;
		if (panelContainer != null)
		{
			if (panelContainer.clipping == UIDrawCall.Clipping.None)
			{
				mRect.xMin = (float)(-Screen.width) * 0.5f;
				mRect.yMin = (float)(-Screen.height) * 0.5f;
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
			mRect = uiCamera.pixelRect;
			if (mRoot != null)
			{
				num = mRoot.pixelSizeAdjustment;
			}
		}
		float num2 = mRect.width;
		float num3 = mRect.height;
		if (num != 1f && num3 > 1f)
		{
			float num4 = (float)mRoot.activeHeight / num3;
			num2 *= num4;
			num3 *= num4;
		}
		Vector3 localScale2 = mTrans.localScale;
		if (style == Style.BasedOnHeight)
		{
			localScale2.x = relativeSize.x * num3;
			localScale2.y = relativeSize.y * num3;
		}
		else if (style == Style.FillKeepingRatio)
		{
			float num5 = num2 / num3;
			float num6 = initialSize.x / initialSize.y;
			if (num6 < num5)
			{
				float num7 = num2 / initialSize.x;
				localScale2.x = num2;
				localScale2.y = initialSize.y * num7;
			}
			else
			{
				float num8 = num3 / initialSize.y;
				localScale2.x = initialSize.x * num8;
				localScale2.y = num3;
			}
		}
		else if (style == Style.FitInternalKeepingRatio)
		{
			float num9 = num2 / num3;
			float num10 = initialSize.x / initialSize.y;
			if (num10 > num9)
			{
				float num11 = num2 / initialSize.x;
				localScale2.x = num2;
				localScale2.y = initialSize.y * num11;
			}
			else
			{
				float num12 = num3 / initialSize.y;
				localScale2.x = initialSize.x * num12;
				localScale2.y = num3;
			}
		}
		else
		{
			if (style == Style.Both || style == Style.Horizontal)
			{
				localScale2.x = relativeSize.x * num2;
			}
			if (style == Style.Both || style == Style.Vertical)
			{
				localScale2.y = relativeSize.y * num3;
			}
		}
		if (mTrans.localScale != localScale2)
		{
			mTrans.localScale = localScale2;
		}
		if (runOnlyOnce && Application.isPlaying)
		{
			Object.Destroy(this);
		}
	}
}
