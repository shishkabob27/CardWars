using System.Collections.Generic;
using UnityEngine;

public class CWConfirmSellButton : MonoBehaviour
{
	public GameObject tableObject;

	public CWDeckCardList cardList;

	private void OnClick()
	{
		if (!(tableObject != null))
		{
			return;
		}
		int num = 0;
		List<CardItem> chosenList = cardList.chosenList;
		foreach (CardItem item in chosenList)
		{
			num += item.SalePrice;
		}
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		instance.Coins += num;
		foreach (CardItem item2 in chosenList)
		{
			instance.DeckManager.RemoveCard(item2);
		}
		instance.Save();
		foreach (CardItem item3 in chosenList)
		{
			Singleton<AnalyticsManager>.Instance.LogCardSold(item3.Form.ID);
		}
		cardList.Clear();
		CWDeckSellCards componentInChildren = tableObject.GetComponentInChildren<CWDeckSellCards>();
		if (componentInChildren != null)
		{
			componentInChildren.Sort();
		}
		CWSellCardsController.GetInstance().ResetUpdate();
	}
}
