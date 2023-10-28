using System.Collections.Generic;
using MiniJSON;

public class UpsightReward
{
	public string name { get; private set; }

	public int quantity { get; private set; }

	public string receipt { get; private set; }

	public static UpsightReward rewardFromJson(string json)
	{
		UpsightReward upsightReward = new UpsightReward();
		Dictionary<string, object> dictionary = Json.Deserialize(json) as Dictionary<string, object>;
		if (dictionary != null)
		{
			if (dictionary.ContainsKey("name"))
			{
				upsightReward.name = dictionary["name"].ToString();
			}
			if (dictionary.ContainsKey("quantity"))
			{
				upsightReward.quantity = int.Parse(dictionary["quantity"].ToString());
			}
			if (dictionary.ContainsKey("receipt"))
			{
				upsightReward.receipt = dictionary["receipt"].ToString();
			}
		}
		return upsightReward;
	}

	public override string ToString()
	{
		return string.Format("[UpsightReward: name={0}, quantity={1}, receipt={2}]", name, quantity, receipt);
	}
}
