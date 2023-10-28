using System;
using System.Collections.Generic;

public class SideQuestData
{
	public enum SideQuestType
	{
		Collectible,
		Battle
	}

	public int iQuestID { get; private set; }

	public string QuestType { get; private set; }

	public string Group { get; private set; }

	public string InitialCardID { get; private set; }

	public string RewardID { get; private set; }

	public string ValidLeaderID { get; private set; }

	public string PostBattleMethodName { get; private set; }

	public string RewardCardMethodName { get; private set; }

	public Dictionary<LandscapeType, string> RewardCardList { get; private set; }

	public string DropProfileID { get; set; }

	public int NumCollectibles { get; private set; }

	public int MinCollectiblesPerQuest { get; private set; }

	public int MaxCollectiblesPerQuest { get; private set; }

	public List<int> ValidQuestIDs { get; private set; }

	public string IntroMessage { get; private set; }

	public string RewardMessage { get; private set; }

	public string FailMessage { get; private set; }

	public string CollectiblePrefab { get; private set; }

	public string TextureAtlas { get; private set; }

	public string CollectibleIcon { get; private set; }

	public string NPCIntroIcon { get; private set; }

	public string NPCRewardIcon { get; private set; }

	public string NPCFailIcon { get; private set; }

	public DateTime? EventStartDate { get; private set; }

	public DateTime? EventEndDate { get; private set; }

	public bool IsTimeLimitedEvent
	{
		get
		{
			return EventStartDate.HasValue || EventEndDate.HasValue;
		}
	}

	public bool IsEventStarted
	{
		get
		{
			return !EventStartDate.HasValue || TFUtils.ServerTime >= EventStartDate.Value;
		}
	}

	public bool IsEventEnded
	{
		get
		{
			return EventEndDate.HasValue && TFUtils.ServerTime >= EventEndDate.Value;
		}
	}

	public int? PrerequisiteMapQuestID { get; private set; }

	public List<int> UnlockQuestIDs { get; private set; }

	public int MatchLapse { get; private set; }

	public SideQuestProgress.SideQuestState State
	{
		get
		{
			return PlayerInfoScript.GetInstance().GetSideQuestState(this);
		}
	}

	public bool IsPlayable { get; internal set; }

	public void IncStartDate(TimeSpan dt)
	{
		if (EventStartDate.HasValue)
		{
			DateTime? eventStartDate = EventStartDate;
			EventStartDate = ((!eventStartDate.HasValue) ? null : new DateTime?(eventStartDate.Value + dt));
		}
	}

	public void DecStartDate(TimeSpan dt)
	{
		if (EventStartDate.HasValue)
		{
			DateTime? eventStartDate = EventStartDate;
			EventStartDate = ((!eventStartDate.HasValue) ? null : new DateTime?(eventStartDate.Value - dt));
		}
	}

	public void IncEndDate(TimeSpan dt)
	{
		if (EventEndDate.HasValue)
		{
			DateTime? eventEndDate = EventEndDate;
			EventEndDate = ((!eventEndDate.HasValue) ? null : new DateTime?(eventEndDate.Value + dt));
		}
	}

	public void DecEndDate(TimeSpan dt)
	{
		if (EventEndDate.HasValue)
		{
			DateTime? eventEndDate = EventEndDate;
			EventEndDate = ((!eventEndDate.HasValue) ? null : new DateTime?(eventEndDate.Value - dt));
		}
	}

	public void Deserialize(Dictionary<string, object> dict)
	{
		string text = TFUtils.LoadString(dict, "QuestType", "fc");
		if (string.IsNullOrEmpty(text))
		{
			throw new ArgumentException(string.Format("Invalid Side QuestType '{0}'", text));
		}
		string text2 = TFUtils.LoadString(dict, "QuestID", string.Empty);
		int result = -1;
		if (!int.TryParse(text2, out result))
		{
			throw new ArgumentException(string.Format("Invalid Side QuestID '{0}'", text2));
		}
		iQuestID = result;
		QuestType = text;
		ValidLeaderID = TFUtils.LoadString(dict, "ValidLeaderID", string.Empty);
		Group = TFUtils.LoadString(dict, "SideQuestGroup", string.Empty);
		InitialCardID = TFUtils.LoadString(dict, "InitialCardID", string.Empty);
		RewardID = TFUtils.LoadString(dict, "RewardID", string.Empty);
		PostBattleMethodName = TFUtils.LoadString(dict, "PostBattleMethod", string.Empty);
		RewardCardMethodName = TFUtils.LoadString(dict, "RewardCardMethodName", string.Empty);
		RewardCardList = ParseRewardCardLists(TFUtils.LoadString(dict, "RewardCardsList", string.Empty));
		DropProfileID = TFUtils.LoadString(dict, "DropProfileID", string.Empty);
		NumCollectibles = TFUtils.LoadInt(dict, "NumCollectibles", 0);
		MinCollectiblesPerQuest = TFUtils.LoadInt(dict, "MinCollectiblesPerQuest", 0);
		MaxCollectiblesPerQuest = TFUtils.LoadInt(dict, "MaxCollectiblesPerQuest", 0);
		ValidQuestIDs = TFUtils.LoadRange(dict, "ValidQuestIDs");
		IntroMessage = TFUtils.LoadString(dict, "IntroMessage", string.Empty);
		RewardMessage = TFUtils.LoadString(dict, "RewardMessage", string.Empty);
		FailMessage = TFUtils.LoadString(dict, "FailMessage", string.Empty);
		CollectiblePrefab = TFUtils.LoadString(dict, "CollectiblePrefab", string.Empty);
		TextureAtlas = TFUtils.LoadString(dict, "TextureAtlas", string.Empty);
		CollectibleIcon = TFUtils.LoadString(dict, "CollectibleIcon", string.Empty);
		NPCIntroIcon = TFUtils.LoadString(dict, "NPCIntroIcon", string.Empty);
		NPCRewardIcon = TFUtils.LoadString(dict, "NPCRewardIcon", string.Empty);
		NPCFailIcon = TFUtils.LoadString(dict, "NPCFailIcon", string.Empty);
		MatchLapse = TFUtils.LoadInt(dict, "MatchLapse", 0);
		UnlockQuestIDs = new List<int>();
		string text3 = TFUtils.LoadString(dict, "UnlockQuestIDs", string.Empty);
		if (!string.IsNullOrEmpty(text3))
		{
			string[] array = text3.Split(',');
			for (int i = 0; i < array.Length; i++)
			{
				if (int.TryParse(array[i], out result))
				{
					UnlockQuestIDs.Add(result);
				}
			}
		}
		int num = TFUtils.LoadInt(dict, "PrerequisiteMapQuestID", -1);
		if (num > 0)
		{
			PrerequisiteMapQuestID = num;
		}
		else
		{
			PrerequisiteMapQuestID = null;
		}
		text3 = TFUtils.LoadString(dict, "StartDate", string.Empty);
		if (!string.IsNullOrEmpty(text3))
		{
			EventStartDate = DateTime.Parse(text3);
		}
		text3 = TFUtils.LoadString(dict, "EndDate", string.Empty);
		if (!string.IsNullOrEmpty(text3))
		{
			EventEndDate = DateTime.Parse(text3);
		}
	}

	public static SideQuestData Create(Dictionary<string, object> dict)
	{
		try
		{
			SideQuestData sideQuestData = new SideQuestData();
			sideQuestData.Deserialize(dict);
			return sideQuestData;
		}
		catch (ArgumentException)
		{
		}
		return null;
	}

	public Dictionary<LandscapeType, string> ParseRewardCardLists(string dataStr)
	{
		Dictionary<LandscapeType, string> dictionary = new Dictionary<LandscapeType, string>();
		dataStr = dataStr.Trim();
		string[] array = dataStr.Split('|');
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(':');
			if (array2.Length == 2)
			{
				try
				{
					LandscapeType key = (LandscapeType)(int)Enum.Parse(typeof(LandscapeType), array2[0].Trim());
					string value = array2[1].Trim();
					dictionary.Add(key, value);
				}
				catch
				{
				}
			}
		}
		return dictionary;
	}

	public string GetRewardCardByLandScape(LandscapeType land)
	{
		string result = string.Empty;
		if (RewardCardList.ContainsKey(land))
		{
			result = RewardCardList[land];
		}
		return result;
	}
}
