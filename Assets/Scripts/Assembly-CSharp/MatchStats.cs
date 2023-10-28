using System;
using System.Collections.Generic;
using System.Text;

public class MatchStats : ICWSerializable
{
	public int Attempts { get; set; }

	public int Wins { get; set; }

	public int Losses { get; set; }

	public int Completed
	{
		get
		{
			return Wins + Losses;
		}
	}

	public int Aborted
	{
		get
		{
			return Attempts - Completed;
		}
	}

	public void Copy(MatchStats other)
	{
		Attempts = other.Attempts;
		Wins = other.Wins;
		Losses = other.Losses;
	}

	public string Serialize()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		stringBuilder.Append(PlayerInfoScript.MakeJS("Attempts", Attempts) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("Wins", Wins) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("Losses", Losses));
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	public void Deserialize(object json)
	{
		try
		{
			Dictionary<string, object> d = (Dictionary<string, object>)json;
			Attempts = TFUtils.LoadInt(d, "Attempts", 0);
			Wins = TFUtils.LoadInt(d, "Wins", 0);
			Losses = TFUtils.LoadInt(d, "Losses", 0);
		}
		catch (Exception ex)
		{
			Singleton<AnalyticsManager>.Instance.LogDebug("exception_deserialize");
			CrashAnalytics.LogException(ex);
			throw ex;
		}
	}
}
