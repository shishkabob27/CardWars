using UnityEngine;

public class TutorialButtonTrigger : MonoBehaviour
{
	public TutorialTrigger Trigger;

	private void OnClick()
	{
		if (TutorialManager.Instance != null)
		{
			TutorialMonitor.Instance.TriggerTutorial(Trigger);
		}
	}
}
