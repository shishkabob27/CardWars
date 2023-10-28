using AnimationOrTween;
using UnityEngine;

[RequireComponent(typeof(Animation))]
[AddComponentMenu("NGUI/Internal/Active Animation")]
public class ActiveAnimation : IgnoreTimeScale
{
	public delegate void OnFinished(ActiveAnimation anim);

	public OnFinished onFinished;

	public GameObject eventReceiver;

	public string callWhenFinished;

	private Animation mAnim;

	private Direction mLastDirection;

	private Direction mDisableDirection;

	private bool mNotify;

	public bool isPlaying
	{
		get
		{
			if (mAnim == null)
			{
				return false;
			}
			foreach (AnimationState item in mAnim)
			{
				if (!mAnim.IsPlaying(item.name))
				{
					continue;
				}
				if (mLastDirection == Direction.Forward)
				{
					if (!(item.time < item.length))
					{
						continue;
					}
					return true;
				}
				if (mLastDirection == Direction.Reverse)
				{
					if (!(item.time > 0f))
					{
						continue;
					}
					return true;
				}
				return true;
			}
			return false;
		}
	}

	public void Reset()
	{
		if (!(mAnim != null))
		{
			return;
		}
		foreach (AnimationState item in mAnim)
		{
			if (mLastDirection == Direction.Reverse)
			{
				item.time = item.length;
			}
			else if (mLastDirection == Direction.Forward)
			{
				item.time = 0f;
			}
		}
	}

	private void Update()
	{
		float num = UpdateRealTimeDelta();
		if (num == 0f)
		{
			return;
		}
		if (mAnim != null)
		{
			bool flag = false;
			foreach (AnimationState item in mAnim)
			{
				if (!mAnim.IsPlaying(item.name))
				{
					continue;
				}
				float num2 = item.speed * num;
				item.time += num2;
				if (num2 < 0f)
				{
					if (item.time > 0f)
					{
						flag = true;
					}
					else
					{
						item.time = 0f;
					}
				}
				else if (item.time < item.length)
				{
					flag = true;
				}
				else
				{
					item.time = item.length;
				}
			}
			mAnim.Sample();
			if (flag)
			{
				return;
			}
			base.enabled = false;
			if (mNotify)
			{
				mNotify = false;
				if (onFinished != null)
				{
					onFinished(this);
				}
				if (eventReceiver != null && !string.IsNullOrEmpty(callWhenFinished))
				{
					eventReceiver.SendMessage(callWhenFinished, this, SendMessageOptions.DontRequireReceiver);
				}
				if (mDisableDirection != 0 && mLastDirection == mDisableDirection)
				{
					NGUITools.SetActive(base.gameObject, false);
				}
			}
		}
		else
		{
			base.enabled = false;
		}
	}

	private void Play(string clipName, Direction playDirection)
	{
		if (!(mAnim != null))
		{
			return;
		}
		base.enabled = true;
		mAnim.enabled = false;
		if (playDirection == Direction.Toggle)
		{
			playDirection = ((mLastDirection != Direction.Forward) ? Direction.Forward : Direction.Reverse);
		}
		if (string.IsNullOrEmpty(clipName))
		{
			if (!mAnim.isPlaying)
			{
				mAnim.Play();
			}
		}
		else if (!mAnim.IsPlaying(clipName))
		{
			mAnim.Play(clipName);
		}
		foreach (AnimationState item in mAnim)
		{
			if (string.IsNullOrEmpty(clipName) || item.name == clipName)
			{
				float num = Mathf.Abs(item.speed);
				item.speed = num * (float)playDirection;
				if (playDirection == Direction.Reverse && item.time == 0f)
				{
					item.time = item.length;
				}
				else if (playDirection == Direction.Forward && item.time == item.length)
				{
					item.time = 0f;
				}
			}
		}
		mLastDirection = playDirection;
		mNotify = true;
		mAnim.Sample();
	}

	public static ActiveAnimation Play(Animation anim, string clipName, Direction playDirection, EnableCondition enableBeforePlay, DisableCondition disableCondition)
	{
		if (!NGUITools.GetActive(anim.gameObject))
		{
			if (enableBeforePlay != EnableCondition.EnableThenPlay)
			{
				return null;
			}
			NGUITools.SetActive(anim.gameObject, true);
			UIPanel[] componentsInChildren = anim.gameObject.GetComponentsInChildren<UIPanel>();
			int i = 0;
			for (int num = componentsInChildren.Length; i < num; i++)
			{
				componentsInChildren[i].Refresh();
			}
		}
		ActiveAnimation activeAnimation = anim.GetComponent<ActiveAnimation>();
		if (activeAnimation == null)
		{
			activeAnimation = anim.gameObject.AddComponent<ActiveAnimation>();
		}
		activeAnimation.mAnim = anim;
		activeAnimation.mDisableDirection = (Direction)disableCondition;
		activeAnimation.eventReceiver = null;
		activeAnimation.callWhenFinished = null;
		activeAnimation.onFinished = null;
		activeAnimation.Play(clipName, playDirection);
		return activeAnimation;
	}

	public static ActiveAnimation Play(Animation anim, string clipName, Direction playDirection)
	{
		return Play(anim, clipName, playDirection, EnableCondition.DoNothing, DisableCondition.DoNotDisable);
	}

	public static ActiveAnimation Play(Animation anim, Direction playDirection)
	{
		return Play(anim, null, playDirection, EnableCondition.DoNothing, DisableCondition.DoNotDisable);
	}
}
