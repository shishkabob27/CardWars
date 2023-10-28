using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Drag Object")]
public class UIDragObject : IgnoreTimeScale
{
	public enum DragEffect
	{
		None,
		Momentum,
		MomentumAndSpring
	}

	public Transform target;

	public Vector3 scale = Vector3.one;

	public float scrollWheelFactor;

	public bool restrictWithinPanel;

	public DragEffect dragEffect = DragEffect.MomentumAndSpring;

	public float momentumAmount = 35f;

	private Plane mPlane;

	private Vector3 mLastPos;

	private UIPanel mPanel;

	private bool mPressed;

	private Vector3 mMomentum = Vector3.zero;

	private float mScroll;

	private Bounds mBounds;

	private int mTouchID;

	private bool mStarted;

	private void FindPanel()
	{
		mPanel = ((!(target != null)) ? null : UIPanel.Find(target.transform, false));
		if (mPanel == null)
		{
			restrictWithinPanel = false;
		}
	}

	private void OnPress(bool pressed)
	{
		if (!base.enabled || !NGUITools.GetActive(base.gameObject) || !(target != null))
		{
			return;
		}
		if (pressed)
		{
			if (!mPressed)
			{
				mTouchID = UICamera.currentTouchID;
				mMomentum = Vector3.zero;
				mPressed = true;
				mStarted = false;
				mScroll = 0f;
				if (restrictWithinPanel && mPanel == null)
				{
					FindPanel();
				}
				if (restrictWithinPanel)
				{
					mBounds = NGUIMath.CalculateRelativeWidgetBounds(mPanel.cachedTransform, target);
				}
				SpringPosition component = target.GetComponent<SpringPosition>();
				if (component != null)
				{
					component.enabled = false;
				}
				Transform transform = UICamera.currentCamera.transform;
				mPlane = new Plane(((!(mPanel != null)) ? transform.rotation : mPanel.cachedTransform.rotation) * Vector3.back, UICamera.lastHit.point);
			}
		}
		else if (mPressed && mTouchID == UICamera.currentTouchID)
		{
			mPressed = false;
			if (restrictWithinPanel && mPanel.clipping != 0 && dragEffect == DragEffect.MomentumAndSpring)
			{
				mPanel.ConstrainTargetToBounds(target, ref mBounds, false);
			}
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (!mPressed || mTouchID != UICamera.currentTouchID || !base.enabled || !NGUITools.GetActive(base.gameObject) || !(target != null))
		{
			return;
		}
		UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
		Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);
		float enter = 0f;
		if (!mPlane.Raycast(ray, out enter))
		{
			return;
		}
		Vector3 point = ray.GetPoint(enter);
		Vector3 vector = point - mLastPos;
		mLastPos = point;
		if (!mStarted)
		{
			mStarted = true;
			vector = Vector3.zero;
		}
		if (vector.x != 0f || vector.y != 0f)
		{
			vector = target.InverseTransformDirection(vector);
			vector.Scale(scale);
			vector = target.TransformDirection(vector);
		}
		if (dragEffect != 0)
		{
			mMomentum = Vector3.Lerp(mMomentum, mMomentum + vector * (0.01f * momentumAmount), 0.67f);
		}
		if (restrictWithinPanel)
		{
			Vector3 localPosition = target.localPosition;
			target.position += vector;
			mBounds.center += target.localPosition - localPosition;
			if (dragEffect != DragEffect.MomentumAndSpring && mPanel.clipping != 0 && mPanel.ConstrainTargetToBounds(target, ref mBounds, true))
			{
				mMomentum = Vector3.zero;
				mScroll = 0f;
			}
		}
		else
		{
			target.position += vector;
		}
	}

	private void LateUpdate()
	{
		float deltaTime = UpdateRealTimeDelta();
		if (target == null)
		{
			return;
		}
		if (mPressed)
		{
			SpringPosition component = target.GetComponent<SpringPosition>();
			if (component != null)
			{
				component.enabled = false;
			}
			mScroll = 0f;
		}
		else
		{
			mMomentum += scale * ((0f - mScroll) * 0.05f);
			mScroll = NGUIMath.SpringLerp(mScroll, 0f, 20f, deltaTime);
			if (mMomentum.magnitude > 0.0001f)
			{
				if (mPanel == null)
				{
					FindPanel();
				}
				if (mPanel != null)
				{
					target.position += NGUIMath.SpringDampen(ref mMomentum, 9f, deltaTime);
					if (!restrictWithinPanel || mPanel.clipping == UIDrawCall.Clipping.None)
					{
						return;
					}
					mBounds = NGUIMath.CalculateRelativeWidgetBounds(mPanel.cachedTransform, target);
					if (!mPanel.ConstrainTargetToBounds(target, ref mBounds, dragEffect == DragEffect.None))
					{
						SpringPosition component2 = target.GetComponent<SpringPosition>();
						if (component2 != null)
						{
							component2.enabled = false;
						}
					}
					return;
				}
			}
			else
			{
				mScroll = 0f;
			}
		}
		NGUIMath.SpringDampen(ref mMomentum, 9f, deltaTime);
	}

	private void OnScroll(float delta)
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
}
