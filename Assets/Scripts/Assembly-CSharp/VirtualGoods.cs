using System.Collections.Generic;

public class VirtualGoods
{
	public string ProductID;

	public int Gems;

	public int Hearts;

	public int Coins;

	public int Inventory;

	public List<string> OccuranceStrings;

	public List<string> Cards;

	public VirtualGoods()
	{
		Cards = new List<string>();
		OccuranceStrings = new List<string>();
	}
}
