using UnityEngine;

public class CWDeckSubMenuController : MonoBehaviour
{
	public GameObject BuildDeck;

	public GameObject FuseDeck;

	public GameObject Inventory;

	public GameObject SellCard;

	private static CWDeckSubMenuController controller;

	private void Awake()
	{
		controller = this;
	}

	public static CWDeckSubMenuController GetInstance()
	{
		return controller;
	}
}
