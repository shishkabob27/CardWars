using System;
using UnityEngine;

[Serializable]
public class ULAnimationSetting
{
	public AnimationClip clip;

	public AnimationBlendMode blendMode;

	public WrapMode wrapMode;

	public PlayMode playMode;

	public int layer;
}
