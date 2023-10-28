using System;
using System.Collections.Generic;
using System.Text;

public class SideQuestProgress : ICWSerializable
{
	public enum SideQuestState
	{
		Inactive,
		Pending,
		InProgress,
		Expired,
		Accomplished,
		Complete,
		Failed
	}

	public Dictionary<int, int> CollectedPerQuestnode = new Dictionary<int, int>();

	public Dictionary<LandscapeType, int> DeployedLandscapeCount = new Dictionary<LandscapeType, int>();

	public SideQuestState State { get; internal set; }

	public int Collected { get; set; }

	public SideQuestProgress()
	{
		Reset();
	}

	public void Reset()
	{
		State = SideQuestState.Inactive;
		Collected = 0;
		CollectedPerQuestnode.Clear();
		DeployedLandscapeCount.Clear();
	}

	public string Serialize()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		stringBuilder.Append(PlayerInfoScript.MakeJS("State", State.ToString()));
		if (State == SideQuestState.Pending || State == SideQuestState.InProgress || State == SideQuestState.Accomplished)
		{
			stringBuilder.Append(",");
			stringBuilder.Append(PlayerInfoScript.MakeJS("Collected", Collected) + ",");
			stringBuilder.Append(PlayerInfoScript.MakeJS("CollectedPerQuestNode", CollectedPerQuestnode) + ",");
			stringBuilder.Append(PlayerInfoScript.MakeJS("DeployedLandscapeCount", DeployedLandscapeCount));
		}
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	public void Deserialize(object json)
	{
		try
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)json;
			string value = TFUtils.LoadString(dictionary, "State", string.Empty);
			State = (SideQuestState)(int)Enum.Parse(typeof(SideQuestState), value);
			if (State != SideQuestState.Pending && State != SideQuestState.InProgress && State != SideQuestState.Accomplished)
			{
				return;
			}
			Collected = TFUtils.LoadInt(dictionary, "Collected", 0);
			Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["CollectedPerQuestNode"];
			foreach (KeyValuePair<string, object> item in dictionary2)
			{
				CollectedPerQuestnode.Add(int.Parse(item.Key), (int)item.Value);
			}
			Dictionary<string, object> dictionary3 = (Dictionary<string, object>)dictionary["DeployedLandscapeCount"];
			foreach (KeyValuePair<string, object> item2 in dictionary3)
			{
				DeployedLandscapeCount.Add((LandscapeType)(int)Enum.Parse(typeof(LandscapeType), item2.Key), (int)item2.Value);
			}
		}
		catch (Exception ex)
		{
			Singleton<AnalyticsManager>.Instance.LogDebug("exception_deserialize");
			CrashAnalytics.LogException(ex);
			throw ex;
		}
	}
}
