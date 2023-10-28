using System.Collections.Generic;
using UnityEngine;

public class CWDeckCardList : MonoBehaviour
{
	public List<CardItem> chosenList = new List<CardItem>();

	public void Add(CardItem item)
	{
		chosenList.Add(item);
	}

	public void Remove(CardItem item)
	{
		chosenList.Remove(item);
	}

	public int GetNum(CardItem item)
	{
		return chosenList.IndexOf(item) + 1;
	}

	public void Clear()
	{
		chosenList.Clear();
	}
}
