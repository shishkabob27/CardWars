using UnityEngine;

public class SurrenderButton : MonoBehaviour
{
	public GameObject onClickTarget;

	private void OnEnable()
	{
		if (!GlobalFlags.Instance.InMPMode)
		{
			NGUITools.SetActive(base.gameObject, false);
		}
	}

	private void OnClick()
	{
		if (onClickTarget != null && !TutorialMonitor.Instance.PopupActive)
		{
			onClickTarget.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
	}
}
