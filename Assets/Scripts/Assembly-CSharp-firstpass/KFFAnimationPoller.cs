using UnityEngine;

public class KFFAnimationPoller : MonoBehaviour
{
	public enum PollType
	{
		Playing,
		NotPlaying,
		WaitForTime
	}

	public Animation anim;

	public PollType pollType;

	public string animName;

	public float targetTime;

	public string message = "OnClick";

	public GameObject messageTarget;

	public bool destroyWhenDone = true;

	private bool started;

	private bool done;

	private void Awake()
	{
		if (Application.isPlaying && anim == null)
		{
			anim = GetComponent<Animation>();
		}
	}

	private void Update()
	{
		if (!Application.isPlaying || !base.enabled || done || !(anim != null))
		{
			return;
		}
		if (!started && (string.IsNullOrEmpty(animName) || anim.IsPlaying(animName)))
		{
			started = true;
		}
		if (!started)
		{
			return;
		}
		switch (pollType)
		{
		case PollType.Playing:
			if (!string.IsNullOrEmpty(animName) && anim.enabled && anim.IsPlaying(animName))
			{
				done = true;
				OnConditionMet();
			}
			else if (anim.isPlaying)
			{
				done = true;
				OnConditionMet();
			}
			break;
		case PollType.NotPlaying:
			if (!string.IsNullOrEmpty(animName) && !anim.IsPlaying(animName))
			{
				done = true;
				OnConditionMet();
			}
			else if (!anim.isPlaying)
			{
				done = true;
				OnConditionMet();
			}
			else if (CheckElapsedTime(anim, animName, -1f))
			{
				done = true;
				OnConditionMet();
			}
			break;
		case PollType.WaitForTime:
			if (CheckElapsedTime(anim, animName, targetTime))
			{
				done = true;
				OnConditionMet();
			}
			break;
		}
	}

	private float GetAnimEndTime(AnimationState state, float elapsedTime)
	{
		float result = elapsedTime;
		if (elapsedTime < 0f)
		{
			AnimationClip animationClip = ((!(state != null)) ? null : state.clip);
			if (animationClip != null)
			{
				result = state.length - ((!(animationClip.frameRate > 0f)) ? 0.1f : (1f / animationClip.frameRate));
			}
		}
		return result;
	}

	private bool CheckElapsedTime(Animation anim, string animname, float elapsedTime)
	{
		if (anim == null)
		{
			return true;
		}
		if (!anim.isPlaying)
		{
			return true;
		}
		if (string.IsNullOrEmpty(animname))
		{
			AnimationState animationState = anim[animname];
			float animEndTime = GetAnimEndTime(animationState, elapsedTime);
			if (animationState != null && animationState.enabled && animationState.length > 0f && animationState.clip != null && (animationState.speed == 0f || animationState.weight <= 0f || animationState.time >= animEndTime))
			{
				return true;
			}
		}
		else
		{
			foreach (AnimationState item in anim)
			{
				float animEndTime2 = GetAnimEndTime(item, elapsedTime);
				if (item.enabled && item.length > 0f && item.clip != null && (item.speed == 0f || item.weight <= 0f || item.time >= animEndTime2))
				{
					return true;
				}
			}
		}
		return false;
	}

	private void OnConditionMet()
	{
		bool flag = destroyWhenDone;
		if (messageTarget != null && !string.IsNullOrEmpty(message))
		{
			messageTarget.SendMessage(message, this, SendMessageOptions.DontRequireReceiver);
		}
		if (flag)
		{
			Object.Destroy(this);
		}
	}

	public void Reset()
	{
		done = false;
	}
}
