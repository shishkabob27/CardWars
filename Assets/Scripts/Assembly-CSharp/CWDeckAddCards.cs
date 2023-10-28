using System;
using System.Collections.Generic;
using UnityEngine;

public class CWDeckAddCards : MonoBehaviour
{
	public GameObject DeckCardPrefab;

	public HashSet<CardItem> chosenList = new HashSet<CardItem>();

	public int[] chosenCounts;

	private CardType filter;

	private GameObject anyCardObj;

	public CardType Filter
	{
		get
		{
			return filter;
		}
		set
		{
			filter = value;
			Sort();
		}
	}

	public void OnEnable()
	{
		filter = CardType.None;
		PlayerDeckManager.ResetSort();
		ResetChosenList();
		Sort();
	}

	public void OnDisable()
	{
		chosenList.Clear();
	}

	public void Sort()
	{
		FillTable();
	}

	private void ResetChosenList()
	{
		PlayerDeckManager deckManager = PlayerInfoScript.GetInstance().DeckManager;
		List<CardItem> sortedInventory = deckManager.GetSortedInventory();
		int currentDeck = CWDeckController.GetInstance().currentDeck;
		deckManager.DetermineMembership();
		int num = Enum.GetNames(typeof(CardType)).Length;
		if (chosenCounts == null || chosenCounts.Length == 0)
		{
			chosenCounts = new int[num];
		}
		Array.Clear(chosenCounts, 0, chosenCounts.Length);
		chosenList.Clear();
		foreach (CardItem item in sortedInventory)
		{
			if (item.membership != null && item.membership.Contains(currentDeck))
			{
				chosenList.Add(item);
				chosenCounts[(int)item.Form.Type]++;
			}
		}
	}

	private bool cardClickedHandler(CWDeckCard deckCard)
	{
		CardItem card = deckCard.card;
		if (chosenList.Contains(card))
		{
			chosenList.Remove(card);
			chosenCounts[(int)card.Form.Type]--;
			deckCard.InUse = false;
		}
		else
		{
			chosenList.Add(card);
			chosenCounts[(int)card.Form.Type]++;
			deckCard.InUse = true;
		}
		return true;
	}

	private void FillTable()
	{
		UIFastGrid component = base.gameObject.GetComponent<UIFastGrid>();
		PlayerDeckManager deckManager = PlayerInfoScript.GetInstance().DeckManager;
		List<CardItem> sortedInventory = deckManager.GetSortedInventory();
		deckManager.DetermineMembership();
		if (filter != CardType.None)
		{
			sortedInventory.RemoveAll((CardItem p) => p.Form.Type != filter);
		}
		List<CardItem> list = new List<CardItem>();
		List<CardItem> list2 = new List<CardItem>();
		foreach (CardItem item in sortedInventory)
		{
			if (chosenList.Contains(item))
			{
				list.Add(item);
			}
			else
			{
				list2.Add(item);
			}
		}
		sortedInventory = list;
		sortedInventory.AddRange(list2);
		List<object> list3 = new List<object>();
		foreach (CardItem item2 in sortedInventory)
		{
			list3.Add(item2);
		}
		component.Initialize(list3, pickPrefab, fillCard);
	}

	private GameObject pickPrefab(object data)
	{
		return DeckCardPrefab;
	}

	private void fillCard(GameObject cardObj, object data)
	{
		CardItem cardItem = data as CardItem;
		CWDeckDeckCards.FillCard(cardItem, cardObj, 0.7f, false);
		CWDeckCard component = cardObj.GetComponent<CWDeckCard>();
		component.InUse = chosenList.Contains(cardItem);
		component.clickedCallback = cardClickedHandler;
		anyCardObj = cardObj;
	}

	public GameObject GetAnyCard()
	{
		return anyCardObj;
	}
}
