using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JsonFx.Json;

public class ScheduleDataManager : ILoadable
{
	private const string BASE_FILEPATH = "Blueprints";

	private const string DATA_FILEPATH = "db_Schedule.json";

	private static ScheduleDataManager sInstance;

	private static List<ScheduleData> sEmptyList;

	private Dictionary<string, List<ScheduleData>> Schedules;

	private static List<ScheduleData> EmptyList
	{
		get
		{
			if (sEmptyList == null)
			{
				sEmptyList = new List<ScheduleData>();
			}
			return sEmptyList;
		}
	}

	public static ScheduleDataManager Instance
	{
		get
		{
			if (sInstance == null)
			{
				sInstance = new ScheduleDataManager();
			}
			return sInstance;
		}
	}

	public bool Loaded { get; private set; }

	private ScheduleDataManager()
	{
		Schedules = new Dictionary<string, List<ScheduleData>>();
	}

	public List<ScheduleData> GetItems(string category)
	{
		if (!Schedules.ContainsKey(category))
		{
			return EmptyList;
		}
		return Schedules[category];
	}

	public ScheduleData GetItem(string category, string itemID)
	{
		try
		{
			return Schedules[category].Find((ScheduleData item) => item.ID == itemID);
		}
		catch (Exception)
		{
		}
		return null;
	}

	public List<ScheduleData> GetItemsAvailable(string category, long timeCurr)
	{
		if (!Schedules.ContainsKey(category))
		{
			return EmptyList;
		}
		List<ScheduleData> list = new List<ScheduleData>();
		foreach (ScheduleData item in Schedules[category])
		{
			if (item.IsAvailable(timeCurr))
			{
				list.Add(item);
			}
		}
		return list;
	}

	public List<ScheduleData> GetItemsAvailableAndUnlocked(string category, long timeCurr)
	{
		if (!Schedules.ContainsKey(category))
		{
			return EmptyList;
		}
		List<ScheduleData> list = new List<ScheduleData>();
		foreach (ScheduleData item in Schedules[category])
		{
			if (item.IsAvailable(timeCurr) && item.GetTimeToUnlock(timeCurr) <= 0)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public ScheduleData GetFirstItemAvailableAndUnlocked(string category, long timeCurr)
	{
		if (!Schedules.ContainsKey(category))
		{
			return null;
		}
		foreach (ScheduleData item in Schedules[category])
		{
			if (!item.IsAvailable(timeCurr) || item.GetTimeToUnlock(timeCurr) > 0)
			{
				continue;
			}
			return item;
		}
		return null;
	}

	public bool IsItemAvailableAndUnlocked(string category, string itemID, long timeCurr)
	{
		if (!Schedules.ContainsKey(category))
		{
			return false;
		}
		ScheduleData scheduleData = Schedules[category].Find((ScheduleData item) => item.ID == itemID);
		if (scheduleData == null)
		{
			return false;
		}
		return scheduleData.IsAvailable(timeCurr) && scheduleData.GetTimeToUnlock(timeCurr) <= 0;
	}

	public IEnumerator Load()
	{
		string prevStartDate = string.Empty;
		string prevEndDate = string.Empty;
		Dictionary<string, object>[] array = LoadJsonData("db_Schedule.json");
		foreach (Dictionary<string, object> scheduleDef in array)
		{
			if (LoadingManager.ShouldYield())
			{
				yield return null;
			}
			string category = TFUtils.LoadString(scheduleDef, "category");
			string itemID = TFUtils.LoadString(scheduleDef, "item");
			string startDate = TFUtils.LoadString(scheduleDef, "startDate", null);
			if (string.IsNullOrEmpty(startDate))
			{
				startDate = prevStartDate;
			}
			string endDate = TFUtils.LoadString(scheduleDef, "endDate", null);
			if (string.IsNullOrEmpty(endDate))
			{
				endDate = prevEndDate;
			}
			if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
			{
				TFUtils.WarnLog("Incomplete schedule data. Ignoring: " + itemID + "(" + category + ") - " + startDate + " -> " + endDate);
				continue;
			}
			List<ScheduleData> categoryItems;
			if (Schedules.ContainsKey(category))
			{
				categoryItems = Schedules[category];
			}
			else
			{
				categoryItems = new List<ScheduleData>();
				Schedules.Add(category, categoryItems);
			}
			ScheduleData item = categoryItems.Find((ScheduleData elem) => elem.ID == itemID);
			if (item == null)
			{
				item = new ScheduleData(itemID);
			}
			if (!item.AddPeriod(startDate, endDate))
			{
				TFUtils.WarnLog("Invalid schedule data. Ignoring: " + itemID + "(" + category + ") - " + startDate + " -> " + endDate);
			}
			else
			{
				categoryItems.Add(item);
				prevStartDate = startDate;
				prevEndDate = endDate;
			}
		}
		Loaded = true;
	}

	public void Destroy()
	{
		if (sInstance == this)
		{
			sInstance = null;
		}
	}

	private Dictionary<string, object>[] LoadJsonData(string fileName)
	{
		string text = Path.Combine("Blueprints", fileName);
		TFUtils.DebugLog("Loading schedule data from path: " + text);
		string jsonFileContent = TFUtils.GetJsonFileContent(text);
		return JsonReader.Deserialize<Dictionary<string, object>[]>(jsonFileContent);
	}
}
