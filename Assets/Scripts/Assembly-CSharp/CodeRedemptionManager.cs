using System;
using System.Collections;
using System.Collections.Generic;

public class CodeRedemptionManager : Singleton<CodeRedemptionManager>, ILoadable
{
	public enum RedemptionType
	{
		TYPE_NONE,
		TYPE_HERO,
		TYPE_CREATURE,
		TYPE_SPELL,
		TYPE_BUILDING
	}

	public class RedeemScheme
	{
		public DateTime start;

		public DateTime end;

		public RedemptionType RedeemType;

		public string RedeemName;
	}

	private List<RedeemScheme> schemes;

	public IEnumerator Load()
	{
		Dictionary<string, object>[] data = SQUtils.ReadJSONData("db_RedeemSchedule.json");
		if (LoadingManager.ShouldYield())
		{
			yield return null;
		}
		schemes = new List<RedeemScheme>();
		Dictionary<string, object>[] array = data;
		foreach (Dictionary<string, object> dict in array)
		{
			string stime = TFUtils.LoadNullableString(dict, "StartDate");
			string etime = TFUtils.LoadNullableString(dict, "EndDate");
			RedeemScheme newScheme = new RedeemScheme();
			if (SQUtils.StringEqual(stime.ToLower(), "default"))
			{
				newScheme.start = DateTime.MinValue;
				newScheme.end = DateTime.MaxValue;
			}
			else
			{
				try
				{
					newScheme.start = DateTime.Parse(stime);
				}
				catch (FormatException)
				{
					throw;
				}
				try
				{
					newScheme.end = DateTime.Parse(etime);
				}
				catch (FormatException)
				{
					throw;
				}
			}
			try
			{
				newScheme.RedeemType = (RedemptionType)(int)Enum.Parse(typeof(RedemptionType), TFUtils.LoadString(dict, "type"), true);
			}
			catch
			{
				newScheme.RedeemType = RedemptionType.TYPE_NONE;
			}
			newScheme.RedeemName = TFUtils.LoadString(dict, "unlock");
			schemes.Add(newScheme);
			if (LoadingManager.ShouldYield())
			{
				yield return null;
			}
		}
	}

	public RedeemScheme GetCurrentScheme()
	{
		DateTime now = TFUtils.ServerTime;
		return schemes.Find((RedeemScheme s) => now >= s.start && now <= s.end);
	}

	void ILoadable.Destroy()
	{
		Destroy();
	}
}
