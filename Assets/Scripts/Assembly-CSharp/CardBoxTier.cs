using System.Collections.Generic;

public class CardBoxTier
{
	public int CoinPrice { get; private set; }

	public int GemPrice { get; private set; }

	public int Gain { get; private set; }

	public int TierMax { get; private set; }

	public static CardBoxTier CreateEntry(Dictionary<string, object> dict)
	{
		CardBoxTier cardBoxTier = new CardBoxTier();
		cardBoxTier.TierMax = TFUtils.LoadInt(dict, "TierMax", 0);
		cardBoxTier.Gain = TFUtils.LoadInt(dict, "Gain", 0);
		cardBoxTier.CoinPrice = TFUtils.LoadInt(dict, "CoinPrice", 0);
		cardBoxTier.GemPrice = TFUtils.LoadInt(dict, "GemPrice", 0);
		return cardBoxTier;
	}
}
