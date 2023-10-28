using UnityEngine;

public class ConfirmPopupController : MonoBehaviour
{
	public delegate void ClickCallback(bool yes);

	public UILabel popupText;

	public ClickCallback OnSelect;

	public string Label
	{
		get
		{
			return popupText.text;
		}
		set
		{
			popupText.text = KFFLocalization.Get(value);
		}
	}

	private void OnClickYes()
	{
		if (OnSelect != null)
		{
			OnSelect(true);
		}
	}

	private void OnClickNo()
	{
		if (OnSelect != null)
		{
			OnSelect(false);
		}
	}
}
