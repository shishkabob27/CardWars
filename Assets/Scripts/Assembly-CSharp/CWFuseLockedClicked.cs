using UnityEngine;

public class CWFuseLockedClicked : MonoBehaviour
{
	public void OnClick()
	{
		TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.MoreRecipes);
	}
}
