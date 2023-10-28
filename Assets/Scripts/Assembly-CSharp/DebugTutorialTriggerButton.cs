using UnityEngine;

public class DebugTutorialTriggerButton : MonoBehaviour
{
	public string tutorialID;

	private TutorialMonitor tMonitor;

	private void Start()
	{
		tMonitor = TutorialMonitor.Instance;
	}

	private void OnClick()
	{
		tMonitor.StartTutorial(tutorialID);
	}

	private void Update()
	{
	}
}
