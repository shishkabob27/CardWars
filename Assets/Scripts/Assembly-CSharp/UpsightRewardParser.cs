using System;
using System.Collections.Generic;

public class UpsightRewardParser
{
	public delegate void ReceiveReward(string aName, int aQuantity);

	private Dictionary<string, ReceiveReward> _TokenDict = new Dictionary<string, ReceiveReward>();

	private static List<_UpsightReward> RewardList = new List<_UpsightReward>();

	public void Init()
	{
		_TokenDict.Add("coin", GiveCoin);
		_TokenDict.Add("gem", GiveGem);
		_TokenDict.Add("heart", GiveHeart);
		_TokenDict.Add("key", GiveKey);
		_TokenDict.Add("card", GiveCard);
	}

	public void Parse(string aRewards)
	{
		RewardList.Clear();
		string[] array = aRewards.Split('_');
		int num = 0;
		while (num < array.Length)
		{
			ReceiveReward value = null;
			if (_TokenDict.TryGetValue(array[num], out value))
			{
				if (array[num] == "key")
				{
					value(array[++num], Convert.ToInt32(array[++num]));
				}
				else
				{
					value(array[num++], Convert.ToInt32(array[num++]));
				}
			}
		}
	}

	private void GiveCoin(string aName, int aQuantity)
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (!(instance == null))
		{
			instance.Coins += aQuantity;
			instance.Save();
			RewardList.Add(new _UpsightReward(aName, aQuantity));
		}
	}

	private void GiveGem(string aName, int aQuantity)
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (!(instance == null))
		{
			instance.Gems += aQuantity;
			instance.Save();
			RewardList.Add(new _UpsightReward(aName, aQuantity));
		}
	}

	private void GiveHeart(string aName, int aQuantity)
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (!(instance == null))
		{
			instance.Stamina += aQuantity;
			instance.Save();
			RewardList.Add(new _UpsightReward(aName, aQuantity));
		}
	}

	private void GiveKey(string aName, int aQuantity)
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (!(instance == null))
		{
			for (int i = 0; i < aQuantity; i++)
			{
				instance.GachaKeys.AddKey(aName);
			}
			instance.Save();
			RewardList.Add(new _UpsightReward(aName, aQuantity));
		}
	}

	private void GiveCard(string aName, int aQuantity)
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (!(instance == null))
		{
			for (int i = 0; i < aQuantity; i++)
			{
				CardItem card = new CardItem(CardDataManager.Instance.GetCard(aName));
				instance.DeckManager.AddCard(card);
			}
			instance.Save();
			RewardList.Add(new _UpsightReward(aName, aQuantity));
		}
	}
}
