using UnityEngine;

[AddComponentMenu("NIS/Tween/Prompter")]
public class NisPrompter : NisTweenAlpha
{
	protected override void OnEnable()
	{
		base.OnEnable();
		Play(true);
		Reset();
	}
}
