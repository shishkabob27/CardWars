using System;
using System.Collections;
using System.Collections.Generic;

public class CardBoxManager : ILoadable
{
	private static CardBoxManager ms_instance;

	private List<CardBoxTier> BoxEntries = new List<CardBoxTier>();

	private string dbJsonFileName = "db_CardBox.json";

	public static CardBoxManager Instance
	{
		get
		{
			if (ms_instance == null)
			{
				ms_instance = new CardBoxManager();
			}
			return ms_instance;
		}
	}

	public int MaxBoxCapacity { get; private set; }

	public IEnumerator Load()
	{
		Dictionary<string, object>[] data = SQUtils.ReadJSONData(dbJsonFileName, false);
		if (LoadingManager.ShouldYield())
		{
			yield return null;
		}
		MaxBoxCapacity = 0;
		Dictionary<string, object>[] array = data;
		foreach (Dictionary<string, object> row in array)
		{
			CardBoxTier entry = CardBoxTier.CreateEntry(row);
			BoxEntries.Add(entry);
			MaxBoxCapacity = Math.Max(MaxBoxCapacity, entry.TierMax);
		}
		BoxEntries.Sort((CardBoxTier a, CardBoxTier b) => a.TierMax.CompareTo(b.TierMax));
	}

	public void Destroy()
	{
		ms_instance = null;
	}

	public CardBoxTier GetTier(int curBoxCapacity)
	{
		CardBoxTier result = null;
		for (int num = BoxEntries.Count - 1; num >= 0; num--)
		{
			CardBoxTier cardBoxTier = BoxEntries[num];
			if (curBoxCapacity >= cardBoxTier.TierMax)
			{
				if (cardBoxTier.CoinPrice > 0 || cardBoxTier.GemPrice > 0)
				{
					result = cardBoxTier;
				}
				break;
			}
		}
		return result;
	}
}
