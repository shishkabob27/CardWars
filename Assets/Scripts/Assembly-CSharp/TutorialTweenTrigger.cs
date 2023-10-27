using UnityEngine;

public class TutorialTweenTrigger : MonoBehaviour
{
	public enum TriggerDirection
	{
		Forward = 0,
		Reverse = 1,
		Both = 2,
	}

	public TutorialTrigger Trigger;
	public TriggerDirection triggerDirection;
}
