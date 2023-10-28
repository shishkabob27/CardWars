using System.Collections;
using UnityEngine;

public class ULAnimModel : ULAnimModelInterface
{
	protected Hashtable animationHashtable;

	public ULAnimModel(ULAnimationSetting[] animationSettings)
	{
		animationHashtable = new Hashtable();
		foreach (ULAnimationSetting uLAnimationSetting in animationSettings)
		{
			animationHashtable.Add(uLAnimationSetting.clip.name, uLAnimationSetting);
		}
	}

	public ULAnimModel(Hashtable hashtable)
	{
		animationHashtable = hashtable;
	}

	public ULAnimModel()
	{
		animationHashtable = new Hashtable();
	}

	public void AddAnimationSetting(string key, ULAnimationSetting setting)
	{
		animationHashtable.Add(key, setting);
	}

	public bool HasAnimation(string animName)
	{
		return animationHashtable.ContainsKey(animName);
	}

	public AnimationClip AnimClip(string animName)
	{
		return ((ULAnimationSetting)animationHashtable[animName]).clip;
	}

	public AnimationBlendMode AnimBlendMode(string animName)
	{
		return ((ULAnimationSetting)animationHashtable[animName]).blendMode;
	}

	public WrapMode AnimWrapMode(string animName)
	{
		return ((ULAnimationSetting)animationHashtable[animName]).wrapMode;
	}

	public PlayMode AnimPlayMode(string animName)
	{
		return ((ULAnimationSetting)animationHashtable[animName]).playMode;
	}

	public int AnimLayer(string animName)
	{
		return ((ULAnimationSetting)animationHashtable[animName]).layer;
	}

	public void ApplyAnimationSettings(Animation targetAnimation)
	{
		foreach (string key in animationHashtable.Keys)
		{
			ULAnimationSetting uLAnimationSetting = (ULAnimationSetting)animationHashtable[key];
			targetAnimation.AddClip(uLAnimationSetting.clip, key);
			targetAnimation[key].blendMode = uLAnimationSetting.blendMode;
			targetAnimation[key].wrapMode = uLAnimationSetting.wrapMode;
			targetAnimation[key].layer = uLAnimationSetting.layer;
		}
	}

	public void UnapplyAnimationSettings(Animation targetAnimation)
	{
		foreach (string key in animationHashtable.Keys)
		{
			targetAnimation.RemoveClip(key);
		}
	}
}
