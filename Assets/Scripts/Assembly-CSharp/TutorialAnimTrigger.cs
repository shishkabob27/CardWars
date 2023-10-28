using UnityEngine;

public class TutorialAnimTrigger : MonoBehaviour
{
	public TutorialTrigger Trigger;

	public bool triggerOnStart;

	private void Start()
	{
		if (triggerOnStart)
		{
			TriggerTutorial();
		}
	}

	private void TriggerTutorial()
	{
		if (base.gameObject.activeInHierarchy && base.enabled && TutorialManager.Instance != null)
		{
			TutorialMonitor.Instance.TriggerTutorial(Trigger);
		}
	}

	public bool WillTriggerTutorial()
	{
		if (base.gameObject.activeInHierarchy && base.enabled && TutorialManager.Instance != null)
		{
			return TutorialMonitor.Instance.ShouldTriggerTutorial(Trigger);
		}
		return false;
	}
}
