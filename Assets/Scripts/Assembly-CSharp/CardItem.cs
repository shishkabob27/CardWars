using System;
using System.Collections.Generic;
using System.Reflection;

public class CardItem
{
	public const int DEFAULT_LEVEL = 1;

	public const float DEFAULT_DROP_RATE = 0f;

	public const int DEFAULT_DROP_LEVEL = 1;

	public const int DEFAULT_STATIC_DROP_WEIGHT = 0;

	public const float DEFAULT_COIN_DROP_RATE = 0f;

	public const int DEFAULT_COIN_REWARD = 0;

	public CardForm Form;

	public int Level;

	public List<int> membership;

	public int ATK
	{
		get
		{
			return Level * Form.BaseATK;
		}
	}

	public int DEF
	{
		get
		{
			return Level * Form.BaseDEF;
		}
	}

	public int Val1
	{
		get
		{
			return Level * Form.BaseVal1;
		}
	}

	public int Val2
	{
		get
		{
			return Level * Form.BaseVal2;
		}
	}

	public int SalePrice
	{
		get
		{
			return Level * Form.BaseSalePrice;
		}
	}

	public string Description
	{
		get
		{
			return Form.RawDescription.Replace("<val1>", Val1.ToString()).Replace("<val2>", Val2.ToString());
		}
	}

	public float DropRate { get; set; }

	public int DropLevel { get; set; }

	public int StaticDropWeight { get; set; }

	public float CoinDropRate { get; set; }

	public int CoinReward { get; set; }

	public bool IsNew { get; set; }

	public CardItem(CardForm frm, int lvl = 1, bool isnew = true)
	{
		Form = frm;
		Level = lvl;
		IsNew = isnew;
		DropRate = 0f;
		DropLevel = 1;
		StaticDropWeight = 0;
		CoinDropRate = 0f;
		CoinReward = 0;
	}

	public int EvaluateLanePlacement(PlayerType player, Lane lane)
	{
		Type scriptClass = Form.GetScriptClass();
		MethodInfo method = scriptClass.GetMethod("EvaluateLanePlacement", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
		return (int)method.Invoke(null, new object[3] { player, lane, this });
	}
}
