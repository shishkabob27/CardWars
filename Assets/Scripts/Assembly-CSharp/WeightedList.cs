using System.Collections.Generic;
using UnityEngine;

public class WeightedList<T>
{
	private List<WeightedItem<T>> ItemCollection = new List<WeightedItem<T>>();

	private int TotalWeight;

	public void Add(T item, int weight)
	{
		WeightedItem<T> weightedItem = new WeightedItem<T>();
		if (weight < 0)
		{
			weight = 0;
		}
		weightedItem.Weight = TotalWeight + weight;
		weightedItem.Item = item;
		TotalWeight += weight;
		ItemCollection.Add(weightedItem);
	}

	public void AddSorted(T item, int weight)
	{
		bool flag = false;
		int num = 0;
		WeightedItem<T> weightedItem = new WeightedItem<T>();
		weightedItem.Weight = weight;
		weightedItem.Item = item;
		while (!flag)
		{
			if (num < ItemCollection.Count)
			{
				WeightedItem<T> weightedItem2 = ItemCollection[num];
				if (weightedItem.Weight >= weightedItem2.Weight)
				{
					ItemCollection.Insert(num, weightedItem);
					flag = true;
				}
				num++;
			}
			else
			{
				ItemCollection.Add(weightedItem);
				flag = true;
			}
		}
	}

	public T TopCandidate()
	{
		T result = default(T);
		if (ItemCollection.Count > 0)
		{
			bool flag = false;
			int num = 0;
			CWList<T> cWList = new CWList<T>();
			int weight = ItemCollection[0].Weight;
			while (!flag && num < ItemCollection.Count)
			{
				WeightedItem<T> weightedItem = ItemCollection[num];
				if (weightedItem.Weight == weight)
				{
					cWList.Add(weightedItem.Item);
					num++;
				}
				else
				{
					flag = true;
				}
			}
			result = cWList.RandomItem();
		}
		return result;
	}

	public int TopWeight()
	{
		int result = 0;
		if (ItemCollection.Count > 0)
		{
			result = ItemCollection[0].Weight;
		}
		return result;
	}

	public bool IsEmpty()
	{
		return ItemCollection.Count <= 0;
	}

	public void Clear()
	{
		TotalWeight = 0;
		ItemCollection.Clear();
	}

	public int GetTotalWeight()
	{
		return TotalWeight;
	}

	public T RandomItem()
	{
		T result = default(T);
		bool flag = false;
		int num = 0;
		if (ItemCollection.Count <= 0)
		{
			return default(T);
		}
		int num2 = Random.Range(0, TotalWeight);
		while (!flag)
		{
			if (num < ItemCollection.Count)
			{
				WeightedItem<T> weightedItem = ItemCollection[num];
				if (num2 < weightedItem.Weight)
				{
					result = weightedItem.Item;
					flag = true;
				}
				num++;
			}
			else
			{
				flag = true;
			}
		}
		return result;
	}
}
