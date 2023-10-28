using UnityEngine;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("NGUI/Interaction/Draggable Camera")]
public class UIDraggableCamera : IgnoreTimeScale
{
	public Transform rootForBounds;

	public Vector2 scale = Vector2.one;

	public float scrollWheelFactor;

	public UIDragObject.DragEffect dragEffect = UIDragObject.DragEffect.MomentumAndSpring;

	public bool smoothDragStart = true;

	public float momentumAmount = 35f;

	private Camera mCam;

	private Transform mTrans;

	private bool mPressed;

	private Vector2 mMomentum = Vector2.zero;

	private Bounds mBounds;

	private float mScroll;

	private UIRoot mRoot;

	private bool mDragStarted;

	public Vector2 currentMomentum
	{
		get
		{
			return mMomentum;
		}
		set
		{
			mMomentum = value;
		}
	}

	private void Awake()
	{
		mCam = GetComponent<Camera>();
		mTrans = base.transform;
		if (rootForBounds == null)
		{
			base.enabled = false;
		}
	}

	private void Start()
	{
		mRoot = NGUITools.FindInParents<UIRoot>(base.gameObject);
	}

	private Vector3 CalculateConstrainOffset()
	{
		if (rootForBounds == null || rootForBounds.childCount == 0)
		{
			return Vector3.zero;
		}
		Vector3 position = new Vector3(mCam.rect.xMin * (float)Screen.width, mCam.rect.yMin * (float)Screen.height, 0f);
		Vector3 position2 = new Vector3(mCam.rect.xMax * (float)Screen.width, mCam.rect.yMax * (float)Screen.height, 0f);
		position = mCam.ScreenToWorldPoint(position);
		position2 = mCam.ScreenToWorldPoint(position2);
		Vector2 minRect = new Vector2(mBounds.min.x, mBounds.min.y);
		Vector2 maxRect = new Vector2(mBounds.max.x, mBounds.max.y);
		return NGUIMath.ConstrainRect(minRect, maxRect, position, position2);
	}

	public bool ConstrainToBounds(bool immediate)
	{
		if (mTrans != null && rootForBounds != null)
		{
			Vector3 vector = CalculateConstrainOffset();
			if (vector.magnitude > 0f)
			{
				if (immediate)
				{
					mTrans.position -= vector;
				}
				else
				{
					SpringPosition springPosition = SpringPosition.Begin(base.gameObject, mTrans.position - vector, 13f);
					springPosition.ignoreTimeScale = true;
					springPosition.worldSpace = true;
				}
				return true;
			}
		}
		return false;
	}

	public void Press(bool isPressed)
	{
		if (isPressed)
		{
			mDragStarted = false;
		}
		if (!(rootForBounds != null))
		{
			return;
		}
		mPressed = isPressed;
		if (isPressed)
		{
			mBounds = NGUIMath.CalculateAbsoluteWidgetBounds(rootForBounds);
			mMomentum = Vector2.zero;
			mScroll = 0f;
			SpringPosition component = GetComponent<SpringPosition>();
			if (component != null)
			{
				component.enabled = false;
			}
		}
		else if (dragEffect == UIDragObject.DragEffect.MomentumAndSpring)
		{
			ConstrainToBounds(false);
		}
	}

	public void Drag(Vector2 delta)
	{
		if (smoothDragStart && !mDragStarted)
		{
			mDragStarted = true;
			return;
		}
		UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
		if (mRoot != null)
		{
			delta *= mRoot.pixelSizeAdjustment;
		}
		Vector2 vector = Vector2.Scale(delta, -scale);
		mTrans.localPosition += (Vector3)vector;
		mMomentum = Vector2.Lerp(mMomentum, mMomentum + vector * (0.01f * momentumAmount), 0.67f);
		if (dragEffect != UIDragObject.DragEffect.MomentumAndSpring && ConstrainToBounds(true))
		{
			mMomentum = Vector2.zero;
			mScroll = 0f;
		}
	}

	public void Scroll(float delta)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject))
		{
			if (Mathf.Sign(mScroll) != Mathf.Sign(delta))
			{
				mScroll = 0f;
			}
			mScroll += delta * scrollWheelFactor;
		}
	}

	private void Update()
	{
		float deltaTime = UpdateRealTimeDelta();
		if (mPressed)
		{
			SpringPosition component = GetComponent<SpringPosition>();
			if (component != null)
			{
				component.enabled = false;
			}
			mScroll = 0f;
		}
		else
		{
			mMomentum += scale * (mScroll * 20f);
			mScroll = NGUIMath.SpringLerp(mScroll, 0f, 20f, deltaTime);
			if (mMomentum.magnitude > 0.01f)
			{
				mTrans.localPosition += (Vector3)NGUIMath.SpringDampen(ref mMomentum, 9f, deltaTime);
				mBounds = NGUIMath.CalculateAbsoluteWidgetBounds(rootForBounds);
				if (!ConstrainToBounds(dragEffect == UIDragObject.DragEffect.None))
				{
					SpringPosition component2 = GetComponent<SpringPosition>();
					if (component2 != null)
					{
						component2.enabled = false;
					}
				}
				return;
			}
			mScroll = 0f;
		}
		NGUIMath.SpringDampen(ref mMomentum, 9f, deltaTime);
	}
}
