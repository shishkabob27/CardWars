using System.Collections.Generic;

public class DropProfile
{
	public string DropProfileID;

	public List<float> ChestDropPercentages;

	public List<int> CardWeights;

	public List<float> CoinDropPercentages;

	public List<int> CoinAmounts;

	public List<int> CoinWeights;

	public List<float> ItemDropPercentages;

	public DropProfile()
	{
		DropProfileID = string.Empty;
		ChestDropPercentages = new List<float>();
		CardWeights = new List<int>();
		CoinDropPercentages = new List<float>();
		CoinAmounts = new List<int>();
		CoinWeights = new List<int>();
		ItemDropPercentages = new List<float>();
	}

	public int GetCardWeightAtIndex(int index)
	{
		if (CardWeights.Count > index)
		{
			return CardWeights[index];
		}
		return 0;
	}
}
