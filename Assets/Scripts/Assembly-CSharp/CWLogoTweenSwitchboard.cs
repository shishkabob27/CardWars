using UnityEngine;

public class CWLogoTweenSwitchboard : MonoBehaviour
{
	public TweenScale Tween;

	public float ForwardDuration;

	public float ReverseDuration;

	public float StartX;

	public float StartY;

	public float StartZ;

	public float EndX;

	public float EndY;

	public float EndZ;

	public void ForwardTween()
	{
		TweenScale.Begin(scale: new Vector3(EndX, EndY, EndZ), go: base.gameObject, duration: ForwardDuration);
	}

	public void ReverseTween()
	{
		TweenScale.Begin(scale: new Vector3(StartX, StartY, StartZ), go: base.gameObject, duration: ReverseDuration);
	}
}
