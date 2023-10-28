using UnityEngine;

public class DeckManagerNavigation : MonoBehaviour
{
	private static DeckManagerNavigation dm_navigation;

	public GameObject DeckManager;

	public GameObject BuildDeck;

	public GameObject FuseDeck;

	public GameObject Inventory;

	public GameObject SellCard;

	public GameObject BuildBack;

	public GameObject FuseBack;

	public GameObject InventoryBack;

	public GameObject SellBack;

	public GameObject SellConfirmBack;

	private void Awake()
	{
		dm_navigation = this;
	}

	public static DeckManagerNavigation GetInstance()
	{
		return dm_navigation;
	}

	public void NavBuildDeck()
	{
		if (BuildDeck != null)
		{
			BuildDeck.SendMessage("OnClick");
		}
	}

	public void NavFuseDeck()
	{
		if (FuseDeck != null)
		{
			FuseDeck.SendMessage("OnClick");
		}
	}

	public void NavInventory()
	{
		if (Inventory != null)
		{
			Inventory.SendMessage("OnClick");
		}
	}

	public void NavSellCard()
	{
		if (SellCard != null)
		{
			SellCard.SendMessage("OnClick");
		}
	}

	public void Back()
	{
		if (BuildBack != null && NGUITools.GetActive(BuildBack))
		{
			BuildBack.SendMessage("OnClick");
		}
		if (FuseBack != null && NGUITools.GetActive(FuseBack))
		{
			FuseBack.SendMessage("OnClick");
		}
		if (InventoryBack != null && NGUITools.GetActive(InventoryBack))
		{
			InventoryBack.SendMessage("OnClick");
		}
		if (SellBack != null && NGUITools.GetActive(SellBack))
		{
			SellBack.SendMessage("OnClick");
		}
	}

	private void Update()
	{
		if (BuildBack != null && !NGUITools.GetActive(BuildBack))
		{
			NGUITools.SetActive(BuildBack, true);
		}
		if (FuseBack != null && !NGUITools.GetActive(FuseBack))
		{
			NGUITools.SetActive(FuseBack, true);
		}
		if (InventoryBack != null && !NGUITools.GetActive(InventoryBack))
		{
			NGUITools.SetActive(InventoryBack, true);
		}
		if (SellBack != null && !NGUITools.GetActive(SellBack))
		{
			NGUITools.SetActive(SellBack, true);
		}
		if (SellConfirmBack != null && !NGUITools.GetActive(SellConfirmBack))
		{
			NGUITools.SetActive(SellConfirmBack, true);
		}
	}
}
