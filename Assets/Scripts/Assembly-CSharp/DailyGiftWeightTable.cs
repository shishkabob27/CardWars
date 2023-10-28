using System.Collections.Generic;
using UnityEngine;

internal class DailyGiftWeightTable
{
	public int totalWeight;

	public List<DailyGiftWeightPair> weights = new List<DailyGiftWeightPair>();

	public void Add(int id, int weight)
	{
		totalWeight += weight;
		weights.Add(new DailyGiftWeightPair(id, totalWeight));
	}

	public int Pick()
	{
		int rnum = Random.Range(1, totalWeight);
		DailyGiftWeightPair dailyGiftWeightPair = weights.Find((DailyGiftWeightPair p) => rnum <= p.weight);
		if (dailyGiftWeightPair == null)
		{
			return 0;
		}
		return dailyGiftWeightPair.id;
	}
}
