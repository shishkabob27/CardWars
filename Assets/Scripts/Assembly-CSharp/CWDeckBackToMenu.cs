using UnityEngine;

public class CWDeckBackToMenu : MonoBehaviour
{
	public UILabel errorText;

	public UIButtonTween showError;

	public GameObject tweenController2;

	public GameObject loadingManager;

	private void OnClick()
	{
		DeckManagerNavigation instance = DeckManagerNavigation.GetInstance();
		if (instance != null && instance.DeckManager != null && !NGUITools.GetActive(instance.DeckManager))
		{
			CloseDeckManager();
			return;
		}
		PlayerInfoScript instance2 = PlayerInfoScript.GetInstance();
		string text = instance2.DeckManager.CheckValidity();
		if (text != null)
		{
			errorText.text = text;
			showError.Play(true);
		}
		else
		{
			instance2.Save();
			CloseDeckManager();
		}
	}

	private void CloseDeckManager()
	{
		DeckManagerNavigation instance = DeckManagerNavigation.GetInstance();
		if (instance != null)
		{
			instance.gameObject.transform.Find("TweenController_Build").gameObject.SendMessage("OnClick");
			instance.gameObject.transform.Find("TweenController_Fuse").gameObject.SendMessage("OnClick");
			instance.gameObject.transform.Find("TweenController_Inventory").gameObject.SendMessage("OnClick");
			instance.gameObject.transform.Find("TweenController_Sell").gameObject.SendMessage("OnClick");
			instance.gameObject.transform.Find("TweenController_SellConfirm").gameObject.SendMessage("OnClick");
			instance.gameObject.transform.Find("B_2_ZoomCard").gameObject.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			instance.gameObject.transform.Find("B_2_ZoomLeader").gameObject.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
		if (tweenController2 != null)
		{
			tweenController2.SendMessage("OnClick");
		}
		if (loadingManager != null)
		{
			loadingManager.SendMessage("DeckManagerDisable");
		}
	}
}
