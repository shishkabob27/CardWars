using System.Collections.Generic;
using UnityEngine;

public class CWDeckSellCards : MonoBehaviour
{
	public GameObject DeckCardPrefab;

	public CardType panelType;

	public CWDeckCardList cardList;

	private CWSellButton sellButtonScript;

	public void OnEnable()
	{
		if (sellButtonScript == null)
		{
			sellButtonScript = base.transform.parent.parent.parent.Find("LeftPane/SellButton").GetComponent<CWSellButton>();
		}
		cardList.Clear();
		PlayerDeckManager.ResetSort();
		Sort();
	}

	public void Sort()
	{
		FillTable();
	}

	private void FillTable()
	{
		UIFastGrid component = base.gameObject.GetComponent<UIFastGrid>();
		PlayerDeckManager deckManager = PlayerInfoScript.GetInstance().DeckManager;
		List<CardItem> sortedInventory = deckManager.GetSortedInventory();
		deckManager.DetermineMembership();
		List<object> list = new List<object>();
		foreach (CardItem item in sortedInventory)
		{
			if (item.Form.Type == panelType)
			{
				list.Add(item);
			}
		}
		component.Initialize(list, pickPrefab, fillCard);
	}

	private GameObject pickPrefab(object data)
	{
		return DeckCardPrefab;
	}

	private bool clickedCallback(CWDeckCard deckCard)
	{
		List<int> membership = deckCard.card.membership;
		if (membership != null && membership.Count > 0)
		{
			sellButtonScript.errorText.text = string.Format(KFFLocalization.Get("!!ERROR_SELLING_DECK_CARD"), membership[0] + 1);
			sellButtonScript.showError.Play(true);
			return true;
		}
		return false;
	}

	private void fillCard(GameObject cardObj, object data)
	{
		CardItem cardItem = data as CardItem;
		CWDeckCardList cWDeckCardList = cardList;
		CWDeckDeckCards.FillCard(cardItem, cardObj, 0.7f, false, true, cWDeckCardList);
		CWDeckCard component = cardObj.GetComponent<CWDeckCard>();
		component.InUse = cardItem.membership != null && cardItem.membership.Count > 0;
		component.GrayIfTooManySelected();
		component.clickedCallback = clickedCallback;
	}
}
