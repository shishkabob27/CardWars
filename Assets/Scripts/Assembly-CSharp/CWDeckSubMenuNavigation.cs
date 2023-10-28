using UnityEngine;

public class CWDeckSubMenuNavigation : MonoBehaviour
{
	public ScreenDimmer questMapDimmer;

	public bool NavToBuildDeck;

	public bool NavToSell;

	public bool NavToInventory;

	public bool NavToCrafting;

	private void OnClick()
	{
		MapControllerBase.GetInstance().HideMap();
		PanelManager instance = PanelManager.GetInstance();
		if (instance != null)
		{
			CameraManager.ActivateCamera(instance.uiCamera);
			CameraManager.ActivateCamera(instance.newCamera.GetComponent<Camera>());
		}
		MenuController instance2 = MenuController.GetInstance();
		if (instance2 != null)
		{
			instance2.SwitchToMainMenu();
			instance2.SwitchToDeck();
		}
		CrossSceneNavScript instance3 = CrossSceneNavScript.GetInstance();
		if (instance3 != null)
		{
			instance3.gameObject.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
		if (questMapDimmer == null)
		{
			GameObject gameObject = GameObject.Find("QuestMapDimmer");
			if (gameObject != null)
			{
				questMapDimmer = gameObject.GetComponent(typeof(ScreenDimmer)) as ScreenDimmer;
			}
		}
		if (questMapDimmer != null)
		{
			questMapDimmer.FadeOut();
		}
		CWDeckManagerAdditiveLoad instance4 = CWDeckManagerAdditiveLoad.GetInstance();
		if (!(instance4 != null))
		{
			return;
		}
		instance4.DeckManagerEnable();
		CWDeckSubMenuController instance5 = CWDeckSubMenuController.GetInstance();
		if (instance5 != null)
		{
			if (NavToBuildDeck && (bool)instance5.BuildDeck)
			{
				instance5.BuildDeck.SendMessage("OnClick");
			}
			else if (NavToCrafting)
			{
				instance5.FuseDeck.SendMessage("OnClick");
			}
			else if (NavToInventory)
			{
				instance5.Inventory.SendMessage("OnClick");
			}
			else if (NavToSell)
			{
				instance5.SellCard.SendMessage("OnClick");
			}
		}
	}
}
