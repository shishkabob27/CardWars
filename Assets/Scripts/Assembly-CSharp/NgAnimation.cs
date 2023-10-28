using UnityEngine;

public class NgAnimation
{
	public static AnimationClip SetAnimation(Animation tarAnimation, int tarIndex, AnimationClip srcClip)
	{
		int num = 0;
		AnimationClip[] array = new AnimationClip[tarAnimation.GetClipCount() - tarIndex + 1];
		foreach (AnimationState item in tarAnimation)
		{
			if (tarIndex == num)
			{
				tarAnimation.RemoveClip(item.clip);
			}
			if (tarIndex < num)
			{
				array[num - tarIndex - 1] = item.clip;
				tarAnimation.RemoveClip(item.clip);
			}
		}
		tarAnimation.AddClip(srcClip, srcClip.name);
		for (int i = 0; i < array.Length; i++)
		{
			tarAnimation.AddClip(array[i], array[i].name);
		}
		return srcClip;
	}

	public static AnimationState GetAnimationByIndex(Animation anim, int nIndex)
	{
		int num = 0;
		foreach (AnimationState item in anim)
		{
			if (num == nIndex)
			{
				return item;
			}
			num++;
		}
		return null;
	}
}
