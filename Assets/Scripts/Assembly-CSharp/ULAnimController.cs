using System.Collections;
using UnityEngine;

public class ULAnimController : ULAnimControllerInterface
{
	protected bool enabled;

	protected Animation animation;

	protected ULAnimModelInterface animationModel;

	public Animation UnityAnimation
	{
		get
		{
			return animation;
		}
		set
		{
			animation = value;
		}
	}

	public ULAnimModelInterface AnimationModel
	{
		get
		{
			return animationModel;
		}
		set
		{
			animationModel = value;
		}
	}

	public static IEnumerator PlaySequence(Animation tgtAnimation, string[] sequence)
	{
		float totalTime = 0f;
		foreach (string s in sequence)
		{
			if (totalTime == 0f)
			{
				tgtAnimation.Play(s, PlayMode.StopSameLayer);
			}
			else
			{
				tgtAnimation.PlayQueued(s, QueueMode.CompleteOthers);
			}
			totalTime += tgtAnimation[s].length;
		}
		yield return new WaitForSeconds(totalTime);
	}

	public static IEnumerator PlayRandom(Animation tgtAnimation, string[] domain)
	{
		string s = domain[(int)(Random.value * (float)domain.Length)];
		tgtAnimation.Play(s, PlayMode.StopSameLayer);
		yield return new WaitForSeconds(tgtAnimation[s].length);
	}

	public bool HasAnimation(string animationName)
	{
		return animationModel.HasAnimation(animationName);
	}

	public bool AnimationEnabled()
	{
		return enabled;
	}

	public void EnableAnimation(bool toEnabled)
	{
		enabled = toEnabled;
		if (!enabled)
		{
			animation.Stop();
		}
	}

	public void PlayAnimation(string animationName)
	{
		if (enabled)
		{
			PlayMode mode = animationModel.AnimPlayMode(animationName);
			animation.Play(animationName, mode);
		}
	}

	public void StopAnimation(string animationName)
	{
		animation.Stop(animationName);
	}

	public void StopAnimations()
	{
		animation.Stop();
	}

	public void Sample(string animationName, float time)
	{
		AnimationState animationState = animation[animationName];
		animationState.enabled = true;
		animationState.time = time;
		animationState.weight = 1f;
		animation.Sample();
		animationState.enabled = false;
	}

	public void SampleWithNormalizedTime(string animationName, float normalizedTime)
	{
		AnimationState animationState = animation[animationName];
		animationState.enabled = true;
		animationState.normalizedTime = normalizedTime;
		animationState.weight = 1f;
		animation.Sample();
		animationState.enabled = false;
	}

	public float GetFrameRate(string animationName)
	{
		return animation[animationName].clip.frameRate;
	}

	public float GetLength(string animationName)
	{
		return animation[animationName].clip.length;
	}

	public float NormalizedTimePerFrame(string animationName)
	{
		AnimationState animationState = animation[animationName];
		float frameRate = animationState.clip.frameRate;
		float num = frameRate / ((Application.targetFrameRate >= 0) ? ((float)Application.targetFrameRate) : 60f);
		return 1f / frameRate / animationState.clip.length * num;
	}
}
