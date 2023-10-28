using UnityEngine;

public class CWTutorialActionStart : MonoBehaviour
{
	public GameObject triggerNode;

	public bool useOnClick = true;

	private void OnPress(bool isDown)
	{
		if (isDown && !useOnClick)
		{
			DisableInput();
		}
	}

	private void OnClick()
	{
		if (useOnClick)
		{
			DisableInput();
		}
	}

	private void DisableInput()
	{
		if (!(triggerNode != null))
		{
			return;
		}
		Component[] components = triggerNode.GetComponents(typeof(TutorialAnimTrigger));
		Component[] array = components;
		foreach (Component component in array)
		{
			TutorialAnimTrigger tutorialAnimTrigger = component as TutorialAnimTrigger;
			if (tutorialAnimTrigger != null && tutorialAnimTrigger.WillTriggerTutorial())
			{
				UICamera.useInputEnabler = true;
				break;
			}
		}
	}
}
