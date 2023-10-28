using UnityEngine;

public class ClosePvpInfoPanel : MonoBehaviour
{
	private UIButtonTween HidePVPPanel;

	private void OnEnable()
	{
		HidePVPPanel = null;
		GameObject gameObject = GameObject.Find("TweenControllers/O_1_PVPQuestInfo_Hide");
		if ((bool)gameObject)
		{
			HidePVPPanel = gameObject.GetComponent<UIButtonTween>();
		}
	}

	private void OnClick()
	{
		if (GlobalFlags.Instance.InMPMode && (bool)HidePVPPanel)
		{
			HidePVPPanel.Play(true);
		}
	}
}
