using System.Collections.Generic;
using UnityEngine;

public class QuestEarningManager : MonoBehaviour
{
	public List<CardItem> earnedCards = new List<CardItem>();

	public List<string> earnedCardsName = new List<string>();

	public List<bool> hasCardFlag = new List<bool>();

	public List<string> cardHistory = new List<string>();

	public Dictionary<int, int> sideQuestEarnedItem = new Dictionary<int, int>();

	public int earnedCoin;

	public int earnedGem;

	public bool dropedThisBattle;

	private static QuestEarningManager g_earningManager;

	private void Awake()
	{
		g_earningManager = this;
	}

	public static QuestEarningManager GetInstance()
	{
		return g_earningManager;
	}

	public void InitCardHistory(PlayerInfoScript pInfo)
	{
		cardHistory = new List<string>();
		List<CardItem> sortedInventory = pInfo.DeckManager.GetSortedInventory();
		foreach (CardItem item in sortedInventory)
		{
			if (!cardHistory.Contains(item.Form.ID))
			{
				cardHistory.Add(item.Form.ID);
			}
		}
	}

	public int GetNumEarnedItems(SideQuestData sqd)
	{
		try
		{
			return sideQuestEarnedItem[sqd.iQuestID];
		}
		catch (KeyNotFoundException)
		{
		}
		return 0;
	}

	public void IncNumEarnedItems(SideQuestData sqd)
	{
		if (sqd != null)
		{
			if (!sideQuestEarnedItem.ContainsKey(sqd.iQuestID))
			{
				sideQuestEarnedItem[sqd.iQuestID] = 0;
			}
			Dictionary<int, int> dictionary;
			Dictionary<int, int> dictionary2 = (dictionary = sideQuestEarnedItem);
			int iQuestID;
			int key = (iQuestID = sqd.iQuestID);
			iQuestID = dictionary[iQuestID];
			dictionary2[key] = iQuestID + 1;
		}
	}

	public void ResetEarnedItems()
	{
		sideQuestEarnedItem = new Dictionary<int, int>();
	}
}
