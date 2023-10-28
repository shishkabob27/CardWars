using UnityEngine;

public class CWBackButtonSetScript : MonoBehaviour
{
	public GameObject tweenController;

	public GameObject panelToHide;

	public GameObject panelToShow;

	public bool hideBottom = true;

	private void OnClick()
	{
		UIButtonTween[] components = tweenController.GetComponents<UIButtonTween>();
		if (panelToHide != null)
		{
			components[0].tweenTarget = panelToHide;
		}
		if (panelToShow != null)
		{
			components[1].tweenTarget = panelToShow;
		}
		components[2].enabled = hideBottom;
	}

	private void Update()
	{
	}
}
