using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	public string pausedMessage;

	public string cantPauseDuringTutorialMessage;

	public UILabel messageLabel;

	public GameObject yesButton;

	public GameObject noButton;

	public GameObject okButton;

	private float savedTimeScale;

	public static bool pauseMenuShown;

	private void OnEnable()
	{
		pauseMenuShown = true;
		savedTimeScale = Time.timeScale;
		Time.timeScale = 0f;
		bool flag = TutorialManager.Instance.isTutorialCompleted(TutorialTrigger.Won) || DebugFlagsScript.GetInstance().stopTutorial;
		if (messageLabel != null)
		{
			messageLabel.text = KFFLocalization.Get((!flag) ? cantPauseDuringTutorialMessage : pausedMessage);
		}
		if (yesButton != null)
		{
			yesButton.SetActive(flag);
		}
		if (noButton != null)
		{
			noButton.SetActive(flag);
		}
		if (okButton != null)
		{
			okButton.SetActive(!flag);
		}
	}

	private void PauseMenuDone()
	{
		Time.timeScale = savedTimeScale;
		pauseMenuShown = false;
	}

	private void TutorialPopupShown()
	{
		if (okButton != null && okButton.activeInHierarchy)
		{
			okButton.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
		else if (noButton != null && noButton.activeInHierarchy)
		{
			noButton.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
	}
}
