using System;
using UnityEngine;

public class SocialButtonScript : MonoBehaviour
{
	public UILabel buttonLabel;

	public GameObject buttonObj;

	public ConfirmPopupController confirmPopup;

	public UIButtonTween confirmPopupShow;

	private bool waiting;

	private void Start()
	{
		buttonLabel.text = KFFLocalization.Get("!!GOOGLEPLAY");
	}

	private void OnEnable()
	{

	}

	private void OnClick()
	{
		if (!SocialManager.Instance.IsPlayerAuthenticated())
		{
			if (!waiting)
			{
				waiting = true;
				confirmPopup.Label = "!!CONFIRM_GOOGLEPLAYCONNECT";
				ConfirmPopupController confirmPopupController = confirmPopup;
				confirmPopupController.OnSelect = (ConfirmPopupController.ClickCallback)Delegate.Combine(confirmPopupController.OnSelect, new ConfirmPopupController.ClickCallback(OnConfirmGooglePlayConnect));
				confirmPopupShow.Play(true);
			}
		}
		else
		{
			SocialManager.Instance.ShowAchievements();
		}
	}

	private void OnConfirmGooglePlayConnect(bool yes)
	{
		waiting = false;
		ConfirmPopupController confirmPopupController = confirmPopup;
		confirmPopupController.OnSelect = (ConfirmPopupController.ClickCallback)Delegate.Remove(confirmPopupController.OnSelect, new ConfirmPopupController.ClickCallback(OnConfirmGooglePlayConnect));
		if (yes)
		{
			AuthScreenController.SetRetrySocialLoginNextTime();
			PlayerInfoScript.GetInstance().ReloadGame();
		}
	}
}
