using AnimationOrTween;
using UnityEngine;

public class TutorialTweenTrigger : MonoBehaviour
{
	public enum TriggerDirection
	{
		Forward,
		Reverse,
		Both
	}

	public TutorialTrigger Trigger;

	public TriggerDirection triggerDirection = TriggerDirection.Both;

	private void TriggerTutorial(UITweener tweener)
	{
		bool flag = false;
		switch (triggerDirection)
		{
		case TriggerDirection.Forward:
			if (tweener != null)
			{
				flag = tweener.direction == Direction.Forward;
			}
			break;
		case TriggerDirection.Reverse:
			if (tweener != null)
			{
				flag = tweener.direction == Direction.Reverse;
			}
			break;
		case TriggerDirection.Both:
			flag = true;
			break;
		}
		if (flag && TutorialManager.Instance != null)
		{
			TutorialMonitor.Instance.TriggerTutorial(Trigger);
		}
	}
}
