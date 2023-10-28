using UnityEngine;

public class DeckNavigationBack : MonoBehaviour
{
	public GameObject TweenController_Build;

	public GameObject TweenController_Fuse;

	public GameObject TweenController_Inventory;

	public GameObject TweenController_Sell;

	public GameObject TweenController_SellConfirm;

	private void OnClick()
	{
		DeckManagerNavigation instance = DeckManagerNavigation.GetInstance();
		if (instance != null)
		{
			try
			{
				TweenController_Build = instance.gameObject.transform.Find("TweenController_Build").gameObject;
			}
			catch
			{
				TweenController_Build = null;
			}
			try
			{
				TweenController_Fuse = instance.gameObject.transform.Find("TweenController_Fuse").gameObject;
			}
			catch
			{
				TweenController_Fuse = null;
			}
			try
			{
				TweenController_Inventory = instance.gameObject.transform.Find("TweenController_Inventory").gameObject;
			}
			catch
			{
				TweenController_Inventory = null;
			}
			try
			{
				TweenController_Sell = instance.gameObject.transform.Find("TweenController_Sell").gameObject;
			}
			catch
			{
				TweenController_Sell = null;
			}
			try
			{
				TweenController_SellConfirm = instance.gameObject.transform.Find("TweenController_SellConfirm").gameObject;
			}
			catch
			{
				TweenController_SellConfirm = null;
			}
		}
		if (TweenController_Build != null)
		{
			TweenController_Build.SendMessage("OnClick");
		}
		if (TweenController_Fuse != null)
		{
			TweenController_Fuse.SendMessage("OnClick");
		}
		if (TweenController_Inventory != null)
		{
			TweenController_Inventory.SendMessage("OnClick");
		}
		if (TweenController_Sell != null)
		{
			TweenController_Sell.SendMessage("OnClick");
		}
		if (TweenController_SellConfirm != null)
		{
			TweenController_SellConfirm.SendMessage("OnClick");
		}
	}
}
