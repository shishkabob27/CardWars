using System.Collections.Generic;
using UnityEngine;

public class CWDeckRemoveButton : MonoBehaviour
{
	public List<GameObject> tables;

	public void OnClick()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		PlayerDeckManager deckManager = instance.DeckManager;
		CWDeckController instance2 = CWDeckController.GetInstance();
		Deck deck = deckManager.GetDeck(instance2.currentDeck);
		foreach (GameObject table in tables)
		{
			CWDeckCard[] componentsInChildren = table.GetComponentsInChildren<CWDeckCard>();
			CWDeckCard[] array = componentsInChildren;
			foreach (CWDeckCard cWDeckCard in array)
			{
				if (cWDeckCard.Xed)
				{
					deck.RemoveCard(cWDeckCard.card);
				}
			}
		}
		instance2.SetLandscapes();
		instance2.ChangeDeck();
		CWDeckBuildDeckController instance3 = CWDeckBuildDeckController.GetInstance();
		if (instance3 != null)
		{
			instance3.UpdateUI();
		}
	}
}
