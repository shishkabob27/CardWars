using UnityEngine;

public interface ULAnimModelInterface
{
	bool HasAnimation(string animName);

	AnimationClip AnimClip(string animName);

	AnimationBlendMode AnimBlendMode(string animName);

	WrapMode AnimWrapMode(string animName);

	PlayMode AnimPlayMode(string animName);

	int AnimLayer(string animName);

	void ApplyAnimationSettings(Animation targetAnimation);

	void UnapplyAnimationSettings(Animation targetAnimation);
}
