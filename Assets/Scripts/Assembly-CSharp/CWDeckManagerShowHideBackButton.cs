using UnityEngine;

public class CWDeckManagerShowHideBackButton : MonoBehaviour
{
	public bool show = true;

	private static GameObject backbutton;

	private void OnEnable()
	{
		if (backbutton == null)
		{
			backbutton = GameObject.Find("B_1_BackToMenu");
		}
	}

	public void OnClick()
	{
		if (backbutton == null)
		{
			backbutton = GameObject.Find("B_1_BackToMenu");
		}
		if (base.enabled && backbutton != null)
		{
			backbutton.SetActive(show);
		}
	}
}
