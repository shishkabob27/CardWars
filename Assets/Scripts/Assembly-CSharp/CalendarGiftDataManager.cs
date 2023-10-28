using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JsonFx.Json;

public class CalendarGiftDataManager : ILoadable
{
	private const string kCalendarGiftFileName = "db_CalendarGift.json";

	private static readonly CalendarGift kEmptyGiftTemplate = new CalendarGift
	{
		Group = "Default",
		GiftType = "None",
		Name = string.Empty,
		Icon = null,
		IconBorder = null,
		CalendarFrame = null,
		CountdownBG = null,
		HeaderBG = null,
		Quantity = 0,
		IsCatchup = false,
		CatchupCost = 2
	};

	private static CalendarGiftDataManager instance = null;

	public Dictionary<string, List<CalendarGift>> GiftData = new Dictionary<string, List<CalendarGift>>();

	public static CalendarGiftDataManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new CalendarGiftDataManager();
			}
			return instance;
		}
	}

	public bool Loaded { get; private set; }

	public CalendarGiftDataManager()
	{
		Loaded = false;
	}

	private Dictionary<string, object>[] LoadCalendarGiftData()
	{
		string text = Path.Combine("Blueprints", "db_CalendarGift.json");
		TFUtils.DebugLog("Loading Calendar Gift data from path: " + text);
		string jsonFileContent = TFUtils.GetJsonFileContent(text);
		return JsonReader.Deserialize<Dictionary<string, object>[]>(jsonFileContent);
	}

	public IEnumerator Load()
	{
		Dictionary<string, object>[] giftData = LoadCalendarGiftData();
		int currentDayIndex = 0;
		Dictionary<string, object>[] array = giftData;
		foreach (Dictionary<string, object> dict in array)
		{
			CalendarGift gift;
			try
			{
				gift = new CalendarGift
				{
					Group = TFUtils.LoadString(dict, "Group"),
					GiftType = TFUtils.LoadString(dict, "Type"),
					Name = TFUtils.LoadString(dict, "Name"),
					Icon = TFUtils.LoadString(dict, "Icon"),
					IconBorder = TFUtils.LoadString(dict, "IconBorder", null),
					CalendarFrame = TFUtils.LoadString(dict, "CalendarFrame", null),
					HeaderBG = TFUtils.LoadString(dict, "HeaderBG", null),
					CountdownBG = TFUtils.LoadString(dict, "CountdownBG", null),
					Quantity = TFUtils.LoadInt(dict, "Quantity"),
					IsCatchup = TFUtils.LoadBool(dict, "IsCatchup", false),
					CatchupCost = TFUtils.LoadInt(dict, "CatchupCost", 2)
				};
			}
			catch (Exception)
			{
				TFUtils.WarnLog("CalendarGiftDataManager: Error parsing calender gift entry '" + currentDayIndex + "'. Treat the entry as default calendar gift.");
				gift = kEmptyGiftTemplate;
			}
			if (!GiftData.ContainsKey(gift.Group))
			{
				GiftData[gift.Group] = new List<CalendarGift>();
			}
			GiftData[gift.Group].Add(gift);
			currentDayIndex++;
			if (LoadingManager.ShouldYield())
			{
				yield return null;
			}
		}
		Loaded = true;
	}

	private DateTime GetNow()
	{
		DateTime result = TFUtils.ServerTime;
		if (null != DebugFlagsScript.GetInstance() && DebugFlagsScript.GetInstance().specifyCalendarDate != string.Empty)
		{
			try
			{
				result = Convert.ToDateTime(DebugFlagsScript.GetInstance().specifyCalendarDate);
			}
			catch (FormatException)
			{
			}
		}
		return result;
	}

	public TimeSpan GetTimeLeftInCalendar()
	{
		ScheduleData activeCalendar = GetActiveCalendar();
		if (activeCalendar == null)
		{
			return new TimeSpan(0, 0, 0, 0);
		}
		DateTime now = GetNow();
		return new TimeSpan(new DateTime(now.Year, now.Month, 1, 11, 59, 59).AddMonths(1).AddDays(-1.0).Ticks - now.Ticks);
	}

	public ScheduleData GetActiveCalendar()
	{
		DateTime now = GetNow();
		ScheduleDataManager scheduleDataManager = ScheduleDataManager.Instance;
		List<ScheduleData> itemsAvailable = scheduleDataManager.GetItemsAvailable("calendar_gift", now.Ticks);
		return (!itemsAvailable.Any()) ? null : itemsAvailable.First();
	}

	public ScheduleData GetPreviousCalendar()
	{
		DateTime dateTime = GetNow().AddMonths(-1);
		ScheduleDataManager scheduleDataManager = ScheduleDataManager.Instance;
		List<ScheduleData> itemsAvailable = scheduleDataManager.GetItemsAvailable("calendar_gift", dateTime.Ticks);
		return (!itemsAvailable.Any()) ? null : itemsAvailable.First();
	}

	public List<CalendarGift> GetCalendarGifts(ScheduleData calendar = null)
	{
		if (calendar == null)
		{
			calendar = GetActiveCalendar();
		}
		List<CalendarGift> value;
		if (calendar != null && GiftData.TryGetValue(calendar.ID, out value))
		{
			return value;
		}
		return new List<CalendarGift>();
	}

	public int GetNumCalendarGifts(ScheduleData calendar = null)
	{
		return GetCalendarGifts(calendar).Count;
	}

	public int GetCatchupDayIndex(ScheduleData calendar = null)
	{
		if (calendar == null)
		{
			calendar = GetActiveCalendar();
		}
		if (calendar != null)
		{
			List<CalendarGift> calendarGifts = GetCalendarGifts(calendar);
			if (calendarGifts != null && calendarGifts.Any())
			{
				int num = calendarGifts.FindIndex((CalendarGift x) => x.IsCatchup);
				return (num == -1) ? (calendarGifts.Count - 1) : num;
			}
		}
		return -1;
	}

	public void Destroy()
	{
		instance = null;
	}
}
