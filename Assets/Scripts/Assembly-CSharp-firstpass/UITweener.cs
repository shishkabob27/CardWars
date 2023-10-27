using UnityEngine;

public class UITweener : IgnoreTimeScale
{
	public enum Method
	{
		Linear = 0,
		EaseIn = 1,
		EaseOut = 2,
		EaseInOut = 3,
		BounceIn = 4,
		BounceOut = 5,
	}

	public enum Style
	{
		Once = 0,
		Loop = 1,
		PingPong = 2,
	}

	public string label;
	public Method method;
	public Style style;
	public AnimationCurve animationCurve;
	public bool ignoreTimeScale;
	public float delay;
	public float duration;
	public bool steeperCurves;
	public int tweenGroup;
	public bool DisableInputWhileTweening;
	public GameObject eventReceiver;
	public string callWhenFinished;
}
