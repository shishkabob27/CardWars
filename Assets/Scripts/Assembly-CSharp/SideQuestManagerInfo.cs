using System;
using System.Collections.Generic;
using System.Text;

public class SideQuestManagerInfo : ICWSerializable
{
	private const int defaultRequireMatchLapse = 5;

	public MatchStats matchlapse = new MatchStats();

	public int RequiredMatchLapse;

	public Dictionary<int, SideQuestProgress> SideQuestsProgress = new Dictionary<int, SideQuestProgress>();

	public string Serialize()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		stringBuilder.Append(PlayerInfoScript.MakeJS("MatchLapse", matchlapse));
		stringBuilder.Append(",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("RequiredMatchLapse", RequiredMatchLapse));
		if (SideQuestsProgress.Count > 0)
		{
			stringBuilder.Append(",");
			stringBuilder.Append(PlayerInfoScript.MakeJS("SideQuestsProgress", SideQuestsProgress));
		}
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	public void Deserialize(object json)
	{
		try
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)json;
			matchlapse.Deserialize(dictionary["MatchLapse"]);
			if (!dictionary.ContainsKey("SideQuestsProgress"))
			{
				return;
			}
			Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["SideQuestsProgress"];
			foreach (KeyValuePair<string, object> item in dictionary2)
			{
				SideQuestProgress sideQuestProgress = new SideQuestProgress();
				sideQuestProgress.Deserialize(item.Value);
				int result;
				if (int.TryParse(item.Key, out result))
				{
					SideQuestsProgress[result] = sideQuestProgress;
				}
			}
			RequiredMatchLapse = TFUtils.LoadInt(dictionary, "RequiredMatchLapse", 5);
		}
		catch (Exception ex)
		{
			Singleton<AnalyticsManager>.Instance.LogDebug("exception_deserialize");
			CrashAnalytics.LogException(ex);
			throw ex;
		}
	}
}
