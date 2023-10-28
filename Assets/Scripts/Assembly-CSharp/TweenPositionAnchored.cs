using AnimationOrTween;
using UnityEngine;

public class TweenPositionAnchored : TweenPosition
{
	public bool tweenAnchorScreenOffset;

	public string onTweenEnableForwardMessage;

	public GameObject onTweenEnableForwardMessageTarget;

	public string onTweenEnableReverseMessage;

	public GameObject onTweenEnableReverseMessageTarget;

	private SLOTUIAnchor uiAnchor;

	public override void Start()
	{
		base.Start();
		uiAnchor = base.gameObject.GetComponent(typeof(SLOTUIAnchor)) as SLOTUIAnchor;
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		if (tweenAnchorScreenOffset)
		{
			if (uiAnchor == null)
			{
				uiAnchor = base.gameObject.GetComponent(typeof(SLOTUIAnchor)) as SLOTUIAnchor;
			}
			if (uiAnchor != null && Application.isPlaying)
			{
				uiAnchor.screenOffset = from * (1f - factor) + to * factor;
			}
		}
		else
		{
			base.OnUpdate(factor, isFinished);
		}
	}

	protected override void OnEnable()
	{
		if (base.amountPerDelta != 0f && base.enabled && base.gameObject.activeInHierarchy)
		{
			Camera camera = NGUITools.FindCameraForLayer(base.gameObject.layer);
			if (camera != null && camera.enabled && camera.gameObject.activeInHierarchy)
			{
				switch (base.direction)
				{
				case Direction.Forward:
					if (!string.IsNullOrEmpty(onTweenEnableForwardMessage) && onTweenEnableForwardMessageTarget != null)
					{
						onTweenEnableForwardMessageTarget.SendMessage(onTweenEnableForwardMessage, this, SendMessageOptions.DontRequireReceiver);
					}
					break;
				case Direction.Reverse:
					if (!string.IsNullOrEmpty(onTweenEnableReverseMessage) && onTweenEnableReverseMessageTarget != null)
					{
						onTweenEnableReverseMessageTarget.SendMessage(onTweenEnableReverseMessage, this, SendMessageOptions.DontRequireReceiver);
					}
					break;
				}
			}
		}
		base.OnEnable();
	}
}
