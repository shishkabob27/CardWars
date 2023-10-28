using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPManager : ILoadable
{
	private class LevelUpScheme
	{
		public float coefficent;

		public List<int> xpTable;
	}

	private Dictionary<string, LevelUpScheme> schemes;

	private static XPManager instance;

	public static XPManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new XPManager();
			}
			return instance;
		}
	}

	public IEnumerator Load()
	{
		Dictionary<string, object>[] data = SQUtils.ReadJSONData("db_LvUpScheme.json");
		if (LoadingManager.ShouldYield())
		{
			yield return null;
		}
		schemes = new Dictionary<string, LevelUpScheme>();
		Dictionary<string, object>[] array = data;
		foreach (Dictionary<string, object> dict in array)
		{
			string key = TFUtils.TryLoadString(dict, "LvUpSchemeID");
			if (!string.IsNullOrEmpty(key))
			{
				InitializeScheme(key, TFUtils.LoadFloat(dict, "LvIndex"));
			}
			if (LoadingManager.ShouldYield())
			{
				yield return null;
			}
		}
	}

	public void Destroy()
	{
		instance = null;
	}

	private void InitializeScheme(string key, float coefficent)
	{
		LevelUpScheme levelUpScheme = new LevelUpScheme();
		levelUpScheme.coefficent = coefficent;
		levelUpScheme.xpTable = new List<int>();
		ExpandTableToLevel(levelUpScheme, 100);
		schemes.Add(key, levelUpScheme);
	}

	private void ExpandTableToLevel(LevelUpScheme scheme, int target)
	{
		target = (target & -128) + 128;
		scheme.xpTable.Capacity = target;
		int count = scheme.xpTable.Count;
		for (int i = count; i < target; i++)
		{
			scheme.xpTable.Add(Mathf.CeilToInt(scheme.coefficent * Mathf.Pow((float)i * 1.02f, 2.5f)));
		}
	}

	private void ExpandTableToXP(LevelUpScheme scheme, int target)
	{
		int target2 = Mathf.FloorToInt(Mathf.Pow((float)target / scheme.coefficent, 0.4f) * 0.98f) + 1;
		ExpandTableToLevel(scheme, target2);
	}

	public int FindLevel(string lvlUpScheme, int xp)
	{
		if (lvlUpScheme == null)
		{
			return 1;
		}
		if (!schemes.ContainsKey(lvlUpScheme))
		{
			return 1;
		}
		LevelUpScheme levelUpScheme = schemes[lvlUpScheme];
		int num = levelUpScheme.xpTable.BinarySearch(xp);
		if (num >= 0)
		{
			return num + 1;
		}
		num = ~num;
		if (num < levelUpScheme.xpTable.Count)
		{
			return num;
		}
		ExpandTableToXP(levelUpScheme, xp);
		return FindLevel(lvlUpScheme, xp);
	}

	public int FindRequiredXP(string lvlUpScheme, int level)
	{
		LevelUpScheme levelUpScheme = schemes[lvlUpScheme];
		if (level >= levelUpScheme.xpTable.Count)
		{
			ExpandTableToLevel(levelUpScheme, level - 1);
		}
		return levelUpScheme.xpTable[level - 1];
	}
}
