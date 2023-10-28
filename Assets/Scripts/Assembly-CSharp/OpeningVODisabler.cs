using UnityEngine;

public class OpeningVODisabler : MonoBehaviour
{
	public TutorialAnimTrigger tutorialTrigger;

	private VOButtonTrigger voTrigger;

	private void Start()
	{
		voTrigger = base.gameObject.GetComponent(typeof(VOButtonTrigger)) as VOButtonTrigger;
		if (voTrigger != null)
		{
			voTrigger.enabled = false;
		}
	}

	private void OnClick()
	{
		if (voTrigger != null && (bool)tutorialTrigger && !tutorialTrigger.WillTriggerTutorial())
		{
			voTrigger.enabled = true;
			voTrigger.OnClick();
			voTrigger.enabled = false;
		}
	}
}
