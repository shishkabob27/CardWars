using UnityEngine;
using AnimationOrTween;

public class UIButtonPlayAnimation : MonoBehaviour
{
	public Animation target;
	public string clipName;
	public Trigger trigger;
	public Direction playDirection;
	public bool resetOnPlay;
	public bool clearSelection;
	public EnableCondition ifDisabledOnPlay;
	public DisableCondition disableWhenFinished;
	public GameObject eventReceiver;
	public string callWhenFinished;
}
