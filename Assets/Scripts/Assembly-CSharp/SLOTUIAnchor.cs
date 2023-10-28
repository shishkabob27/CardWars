using UnityEngine;

public class SLOTUIAnchor : UIAnchor
{
	public Vector2 screenOffset = Vector2.zero;

	public float depthOffset;

	public bool useDepthOffset;

	public GameObject parentObject;

	public bool useParentObjectPosition;

	public bool useCameraRectIfNotUsingParentObjectPosition = true;

	public bool anchorEnabled;

	public bool syncWidth;

	public float syncWidthOffset;

	public float syncWidthScale = 1f;

	public bool syncHeight;

	public float syncHeightOffset;

	public float syncHeightScale = 1f;

	public bool syncClippingWidth;

	public float syncClippingWidthOffset;

	public float syncClippingWidthScale = 1f;

	public bool syncClippingHeight;

	public float syncClippingHeightOffset;

	public float syncClippingHeightScale = 1f;

	public bool syncColliderWidth;

	public float syncColliderWidthOffset;

	public float syncColliderWidthScale = 1f;

	public bool syncColliderHeight;

	public float syncColliderHeightOffset;

	public float syncColliderHeightScale = 1f;

	protected bool mIsWindows;

	private SLOTUICamera slotUICamera;

	private UIPanel uiPanel;

	private UIDraggablePanel draggablePanel;

	protected override void Start()
	{
		mIsWindows = Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor;
		uiPanel = base.gameObject.GetComponent(typeof(UIPanel)) as UIPanel;
		draggablePanel = base.gameObject.GetComponent(typeof(UIDraggablePanel)) as UIDraggablePanel;
		base.Start();
		if (uiCamera != null)
		{
			slotUICamera = uiCamera.gameObject.GetComponent(typeof(SLOTUICamera)) as SLOTUICamera;
		}
		UpdateAnchor();
	}

	protected override void Update()
	{
		UpdateAnchor();
	}

	private void UpdateAnchor(GameObject obj)
	{
		if (obj != null)
		{
			SLOTUIAnchor sLOTUIAnchor = obj.GetComponent(typeof(SLOTUIAnchor)) as SLOTUIAnchor;
			if (sLOTUIAnchor != null)
			{
				sLOTUIAnchor.UpdateAnchor();
			}
		}
	}

	protected virtual void UpdateAnchor()
	{
		if (mAnim != null && mAnim.enabled && mAnim.isPlaying)
		{
			return;
		}
		float num = 1f;
		if (slotUICamera != null)
		{
			num = slotUICamera.GetScale();
		}
		Rect rect = default(Rect);
		bool flag = false;
		if (parentObject != null)
		{
			if (useParentObjectPosition)
			{
				Vector3 vector = base.transform.InverseTransformPoint(parentObject.transform.position);
				rect.xMin = vector.x;
				rect.yMin = vector.y;
				rect.xMax = rect.xMin;
				rect.yMax = rect.yMin;
			}
			else if (useCameraRectIfNotUsingParentObjectPosition)
			{
				flag = true;
				rect = uiCamera.pixelRect;
			}
			UpdateAnchor(parentObject);
		}
		else if (panelContainer != null)
		{
			if (panelContainer.clipping == UIDrawCall.Clipping.None)
			{
				rect.xMin = panelContainer.transform.position.x - (float)Screen.width * 0.5f;
				rect.yMin = panelContainer.transform.position.y - (float)Screen.height * 0.5f;
				rect.xMax = 0f - rect.xMin;
				rect.yMax = 0f - rect.yMin;
			}
			else
			{
				Vector4 clipRange = panelContainer.clipRange;
				rect.x = clipRange.x - clipRange.z * 0.5f;
				rect.y = clipRange.y - clipRange.w * 0.5f;
				rect.width = clipRange.z;
				rect.height = clipRange.w;
				UpdateAnchor(panelContainer.gameObject);
			}
		}
		else if (widgetContainer != null)
		{
			Transform cachedTransform = widgetContainer.cachedTransform;
			Vector3 localScale = cachedTransform.localScale;
			Vector3 localPosition = cachedTransform.localPosition;
			Vector3 vector2 = widgetContainer.relativeSize;
			Vector3 vector3 = widgetContainer.pivotOffset;
			vector3.y -= 1f;
			vector3.x *= widgetContainer.relativeSize.x * localScale.x;
			vector3.y *= widgetContainer.relativeSize.y * localScale.y;
			rect.x = localPosition.x + vector3.x;
			rect.y = localPosition.y + vector3.y;
			rect.width = vector2.x * localScale.x;
			rect.height = vector2.y * localScale.y;
			UpdateAnchor(widgetContainer.gameObject);
		}
		else
		{
			if (!(uiCamera != null))
			{
				return;
			}
			flag = true;
			rect = uiCamera.pixelRect;
		}
		rect.xMin /= num;
		rect.yMin /= num;
		rect.xMax /= num;
		rect.yMax /= num;
		float num2 = 640f / (float)Screen.height;
		if (anchorEnabled)
		{
			float x = (rect.xMin + rect.xMax) * 0.5f;
			float y = (rect.yMin + rect.yMax) * 0.5f;
			Vector3 vector4 = new Vector3(x, y, (!useDepthOffset) ? 0f : depthOffset);
			if (side != Side.Center)
			{
				if (side == Side.Right || side == Side.TopRight || side == Side.BottomRight)
				{
					vector4.x = rect.xMax;
				}
				else if (side == Side.Top || side == Side.Center || side == Side.Bottom)
				{
					vector4.x = x;
				}
				else
				{
					vector4.x = rect.xMin;
				}
				if (side == Side.Top || side == Side.TopRight || side == Side.TopLeft)
				{
					vector4.y = rect.yMax;
				}
				else if (side == Side.Left || side == Side.Center || side == Side.Right)
				{
					vector4.y = y;
				}
				else
				{
					vector4.y = rect.yMin;
				}
			}
			float width = rect.width;
			float height = rect.height;
			vector4.x += relativeOffset.x * width;
			vector4.y += relativeOffset.y * height;
			if (parentObject != null && !useParentObjectPosition)
			{
				vector4.x += parentObject.transform.localPosition.x;
				vector4.y += parentObject.transform.localPosition.y;
			}
			if (flag)
			{
				vector4.x += screenOffset.x / num2;
				vector4.y += screenOffset.y / num2;
				if (uiCamera.orthographic)
				{
					vector4.x = Mathf.RoundToInt(vector4.x);
					vector4.y = Mathf.RoundToInt(vector4.y);
					if (halfPixelOffset && mIsWindows)
					{
						vector4.x -= 0.5f;
						vector4.y += 0.5f;
					}
				}
				vector4.z = uiCamera.transform.position.z;
				vector4 = uiCamera.ScreenToWorldPoint(vector4);
				Vector3 position = base.transform.position;
				if (position.x != vector4.x || position.y != vector4.y || (useDepthOffset && base.transform.localPosition.z != depthOffset))
				{
					float z = base.transform.localPosition.z;
					base.transform.position = vector4;
					position = base.transform.localPosition;
					position.z = ((!useDepthOffset) ? z : depthOffset);
					base.transform.localPosition = position;
				}
			}
			else
			{
				vector4.x += screenOffset.x;
				vector4.y += screenOffset.y;
				vector4.x = Mathf.RoundToInt(vector4.x);
				vector4.y = Mathf.RoundToInt(vector4.y);
				if (base.transform.localPosition != vector4)
				{
					base.transform.localPosition = vector4;
				}
			}
		}
		if (syncWidth)
		{
			float x2 = rect.width * syncWidthScale * ((!flag) ? 1f : num2) + syncWidthOffset;
			base.transform.localScale = new Vector3(x2, base.transform.localScale.y, base.transform.localScale.z);
		}
		if (syncHeight)
		{
			float y2 = rect.height * syncWidthScale * ((!flag) ? 1f : num2) + syncHeightOffset;
			base.transform.localScale = new Vector3(base.transform.localScale.x, y2, base.transform.localScale.z);
		}
		if (uiPanel != null)
		{
			if (syncClippingWidth)
			{
				float num3 = rect.width * syncClippingWidthScale + syncClippingWidthOffset;
				Vector4 clipRange2 = uiPanel.clipRange;
				if (clipRange2.z != num3)
				{
					float num4 = num3 - clipRange2.z;
					clipRange2.z = num3;
					uiPanel.clipRange = clipRange2;
					base.transform.localPosition = new Vector3(base.transform.localPosition.x + num4 * 0.5f, base.transform.localPosition.y, base.transform.localPosition.z);
					if (draggablePanel != null)
					{
						draggablePanel.ResetPosition();
					}
				}
			}
			if (syncClippingHeight)
			{
				float num5 = rect.height * syncClippingHeightScale * ((!flag) ? 1f : num2) + syncClippingHeightOffset;
				Vector4 clipRange3 = uiPanel.clipRange;
				if (clipRange3.w != num5)
				{
					float num6 = num5 - clipRange3.w;
					clipRange3.w = num5;
					uiPanel.clipRange = clipRange3;
					base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y + num6 * 0.5f, base.transform.localPosition.z);
					if (draggablePanel != null)
					{
						draggablePanel.ResetPosition();
					}
				}
			}
		}
		if (!(GetComponent<Collider>() != null))
		{
			return;
		}
		BoxCollider boxCollider = GetComponent<Collider>() as BoxCollider;
		if (!(boxCollider != null))
		{
			return;
		}
		if (syncColliderWidth)
		{
			float num7 = rect.width * syncColliderWidthScale * ((!flag) ? 1f : num2) + syncColliderWidthOffset;
			Vector3 size = boxCollider.size;
			if (size.x != num7)
			{
				size.x = num7;
				boxCollider.size = size;
			}
		}
		if (syncColliderHeight)
		{
			float num8 = rect.height * syncColliderHeightScale * ((!flag) ? 1f : num2) + syncColliderHeightOffset;
			Vector3 size2 = boxCollider.size;
			if (size2.y != num8)
			{
				size2.y = num8;
				boxCollider.size = size2;
			}
		}
	}
}
