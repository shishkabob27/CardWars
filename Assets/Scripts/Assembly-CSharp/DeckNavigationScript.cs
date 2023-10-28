using UnityEngine;

public class DeckNavigationScript : MonoBehaviour
{
	public string NavPoint;

	private void OnClick()
	{
		DeckManagerNavigation instance = DeckManagerNavigation.GetInstance();
		if (instance != null)
		{
			if (NavPoint == "BuildDeck" && instance.BuildDeck != null)
			{
				instance.Back();
				instance.NavBuildDeck();
			}
			if (NavPoint == "FuseDeck" && instance.FuseDeck != null)
			{
				instance.Back();
				instance.NavFuseDeck();
			}
			if (NavPoint == "Inventory" && instance.Inventory != null)
			{
				instance.Back();
				instance.NavInventory();
			}
			if (NavPoint == "SellCard" && instance.SellCard != null)
			{
				instance.Back();
				instance.NavSellCard();
			}
		}
	}
}
