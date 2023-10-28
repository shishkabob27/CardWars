using System;
using System.Reflection;
using UnityEngine;

public class LeaderItem
{
	public LeaderForm Form;

	private int xp;

	private int rank;

	private RankManager.RankEntry rankValues;

	public int XP
	{
		get
		{
			return xp;
		}
		set
		{
			xp = Mathf.Min(value, Form.MaxXP);
			rank = XPManager.Instance.FindLevel(Form.LvUpSchemeID, xp);
			rankValues = RankManager.Instance.FindRank(rank);
		}
	}

	public int ToNextRank
	{
		get
		{
			if (xp >= Form.MaxXP)
			{
				return 0;
			}
			return XPManager.Instance.FindRequiredXP(Form.LvUpSchemeID, rank + 1) - xp;
		}
	}

	public int HP
	{
		get
		{
			return rank * 5 + Form.BaseHP;
		}
	}

	public int Rank
	{
		get
		{
			return rank;
		}
	}

	public RankManager.RankEntry RankValues
	{
		get
		{
			return rankValues;
		}
	}

	public string Description
	{
		get
		{
			string text = Form.Desc.Replace("<val1>", Form.BaseVal1.ToString());
			return text.Replace("<val2>", Form.BaseVal2.ToString());
		}
	}

	public LeaderItem(LeaderForm frm)
	{
		Form = frm;
	}

	public Type GetScriptClass()
	{
		Type type = Type.GetType(Form.ScriptName);
		if (type == null)
		{
			type = Type.GetType("LeaderScript");
		}
		return type;
	}

	public bool CanPlay(PlayerType player)
	{
		Type scriptClass = GetScriptClass();
		MethodInfo method = scriptClass.GetMethod("CanPlay", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
		return (bool)method.Invoke(null, new object[3] { player, -1, null });
	}

	public CardScript InstanceScript()
	{
		Type scriptClass = GetScriptClass();
		return Activator.CreateInstance(scriptClass) as CardScript;
	}

	public int EvaluateLeaderAbility(PlayerType player)
	{
		Type scriptClass = GetScriptClass();
		MethodInfo method = scriptClass.GetMethod("EvaluateLanePlacement", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
		return (int)method.Invoke(null, new object[3] { player, null, null });
	}
}
