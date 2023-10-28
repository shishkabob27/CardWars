using System;
using System.Collections;
using System.Collections.Generic;

public class TipManager : ILoadable
{
	public Dictionary<string, Tip> tips;

	private static TipManager instance;

	public static TipManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new TipManager();
			}
			return instance;
		}
	}

	public IEnumerator Load()
	{
		Dictionary<string, object>[] data = SQUtils.ReadJSONData("db_Tips.json");
		if (LoadingManager.ShouldYield())
		{
			yield return null;
		}
		tips = new Dictionary<string, Tip>();
		Dictionary<string, object>[] array = data;
		foreach (Dictionary<string, object> row in array)
		{
			Tip tp = new Tip
			{
				ID = TFUtils.TryLoadString(row, "TipID"),
				Header = TFUtils.TryLoadString(row, "TipHeader"),
				Message = TFUtils.TryLoadString(row, "TipMessage")
			};
			string str = TFUtils.TryLoadString(row, "Context");
			if (str == null || str == string.Empty)
			{
				tp.Context = TipContext.Universal;
			}
			else
			{
				tp.Context = (TipContext)(int)Enum.Parse(typeof(TipContext), str);
			}
			tips.Add(tp.ID, tp);
			if (LoadingManager.ShouldYield())
			{
				yield return null;
			}
		}
		yield return null;
	}

	public void Destroy()
	{
		instance = null;
	}

	public Tip GetRandomTipWithContext(TipContext context)
	{
		if (context == TipContext.Universal)
		{
			return GetRandomTip();
		}
		List<Tip> list = new List<Tip>();
		foreach (KeyValuePair<string, Tip> tip in tips)
		{
			if (tip.Value != null && tip.Value.Context == context)
			{
				list.Add(tip.Value);
			}
		}
		if (list.Count > 0)
		{
			Random random = new Random();
			int index = random.Next(0, list.Count);
			return list[index];
		}
		return null;
	}

	public Tip GetRandomTip()
	{
		List<Tip> list = new List<Tip>();
		foreach (KeyValuePair<string, Tip> tip in tips)
		{
			list.Add(tip.Value);
		}
		if (list.Count > 0)
		{
			Random random = new Random();
			int index = random.Next(0, list.Count);
			return list[index];
		}
		return null;
	}
}
