using UnityEngine;
using AnimationOrTween;

public class UIButtonTween : MonoBehaviour
{
	public string label;
	public GameObject tweenTarget;
	public int tweenGroup;
	public Trigger trigger;
	public Direction playDirection;
	public bool resetOnPlay;
	public EnableCondition ifDisabledOnPlay;
	public DisableCondition disableWhenFinished;
	public bool includeChildren;
	public GameObject eventReceiver;
	public string callWhenFinished;
}
