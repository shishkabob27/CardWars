using System;
using System.Collections.Generic;
using System.Globalization;

public class DungeonData
{
	public class SchedulePeriod
	{
		public long DateStart { get; private set; }

		public long DateEnd { get; private set; }

		public SchedulePeriod(DateTime dateStart, DateTime dateEnd)
		{
			DateStart = dateStart.Ticks;
			DateEnd = dateEnd.Ticks;
		}
	}

	public class DungeonGroup
	{
		private const int DAYSOFWEEK_NUM = 7;

		private const string DATE_FORMAT = "dd/MM/yyyy";

		public string ID;

		public int DayAvailableBits;

		public List<SchedulePeriod> PeriodsAvailable;

		public DungeonGroup(string inID)
		{
			ID = inID;
		}

		public bool IsAvailable(long timeCurr)
		{
			if (PeriodsAvailable == null)
			{
				return true;
			}
			foreach (SchedulePeriod item in PeriodsAvailable)
			{
				if (timeCurr >= item.DateStart && timeCurr <= item.DateEnd)
				{
					return true;
				}
			}
			return false;
		}

		public long GetTimeToUnlock(long timeCurr)
		{
			if (DayAvailableBits == 0)
			{
				return 0L;
			}
			DateTime dateTime = new DateTime(timeCurr, DateTimeKind.Utc);
			int dayOfWeek = (int)dateTime.DayOfWeek;
			if ((DayAvailableBits & (1 << dayOfWeek)) != 0)
			{
				return 0L;
			}
			int i;
			for (i = 1; (DayAvailableBits & (1 << (dayOfWeek + i) % 7)) == 0; i++)
			{
			}
			DateTime dateTime2 = dateTime.AddDays(i);
			return new DateTime(dateTime2.Year, dateTime2.Month, dateTime2.Day, 0, 0, 0, DateTimeKind.Utc).Ticks - timeCurr;
		}

		public long GetTimeToLock(long timeCurr)
		{
			if (DayAvailableBits == 0 || GetTimeToUnlock(timeCurr) != 0L)
			{
				return long.MaxValue;
			}
			DateTime dateTime = new DateTime(timeCurr, DateTimeKind.Utc);
			int dayOfWeek = (int)dateTime.DayOfWeek;
			int i;
			for (i = 1; (DayAvailableBits & (1 << (dayOfWeek + i) % 7)) != 0; i++)
			{
			}
			DateTime dateTime2 = dateTime.AddDays(i);
			DateTime dateTime3 = new DateTime(dateTime2.Year, dateTime2.Month, dateTime2.Day, 0, 0, 0, DateTimeKind.Utc);
			if (PeriodsAvailable != null)
			{
				foreach (SchedulePeriod item in PeriodsAvailable)
				{
					if (timeCurr >= item.DateStart && timeCurr <= item.DateEnd && item.DateEnd < dateTime3.Ticks)
					{
						dateTime2 = new DateTime(item.DateEnd, DateTimeKind.Utc);
						dateTime3 = new DateTime(dateTime2.Year, dateTime2.Month, dateTime2.Day, 0, 0, 0, DateTimeKind.Utc);
					}
				}
			}
			return dateTime3.Ticks - timeCurr;
		}

		public bool AddSchedule(string startDateString, string endDateString)
		{
			if (TryAddDaySchedule(startDateString))
			{
				return true;
			}
			return TryAddDatePeriod(startDateString, endDateString);
		}

		private bool TryAddDaySchedule(string startDateString)
		{
			if (string.IsNullOrEmpty(startDateString) || !startDateString.ToLower().Contains("day"))
			{
				return false;
			}
			string[] array = startDateString.Split(',');
			string[] array2 = array;
			foreach (string text in array2)
			{
				try
				{
					int num = (int)Enum.Parse(typeof(DayOfWeek), text.Trim(), true);
					DayAvailableBits |= 1 << num;
				}
				catch (Exception)
				{
				}
			}
			return DayAvailableBits != 0;
		}

		private bool TryAddDatePeriod(string startDateString, string endDateString)
		{
			try
			{
				DateTime dateStart = DateTime.ParseExact(startDateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
				DateTime dateEnd = DateTime.ParseExact(endDateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
				if (PeriodsAvailable == null)
				{
					PeriodsAvailable = new List<SchedulePeriod>();
				}
				if (dateEnd.Ticks <= dateStart.Ticks)
				{
					return false;
				}
				PeriodsAvailable.Add(new SchedulePeriod(dateStart, dateEnd));
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}

	public class Quest
	{
		public int Index { get; private set; }

		public string ID { get; private set; }

		public int HeartCost { get; private set; }

		public bool Completed { get; private set; }

		public bool Locked { get; private set; }

		public Quest(int inIndex, string inID, int inHeartCost, bool inLocked)
		{
			Index = inIndex;
			ID = inID;
			HeartCost = inHeartCost;
			Locked = inLocked;
			Completed = false;
		}

		public void SetLocked(bool inLocked)
		{
			Locked = inLocked;
		}

		public void SetCompleted(bool inCompleted)
		{
			Completed = inCompleted;
		}

		public override string ToString()
		{
			return string.Format("index:{0} ID:{1} GemCost:{2} Locked:{3}", Index, ID, HeartCost, Locked);
		}
	}

	private DungeonGroup Group;

	public string ID { get; private set; }

	public string Name { get; private set; }

	public string IconName { get; private set; }

	public string Info { get; private set; }

	public List<Quest> Quests { get; private set; }

	public DungeonData(string inID, string inName, DungeonGroup inGroup, string inIconName, string info)
	{
		ID = inID;
		Name = inName;
		Group = inGroup;
		IconName = inIconName;
		Info = info;
		Quests = new List<Quest>();
	}

	public bool IsAvailable(long timeCurr)
	{
		return Group.IsAvailable(timeCurr);
	}

	public bool IsUnlocked(long timeCurr)
	{
		return IsAvailable(timeCurr) && GetTimeToUnlock(timeCurr) == 0L && GetTimeToLock(timeCurr) > 0;
	}

	public long GetTimeToUnlock(long timeCurr)
	{
		return Group.GetTimeToUnlock(timeCurr);
	}

	public long GetTimeToLock(long timeCurr)
	{
		return Group.GetTimeToLock(timeCurr);
	}

	public void AppendQuest(string inQuestID, int inHeartCost, bool inLocked)
	{
		Quests.Add(new Quest(Quests.Count, inQuestID, inHeartCost, inLocked));
	}

	public void UnlockQuest(int inQuestIndex, bool completed)
	{
		if (Quests != null && inQuestIndex >= 0 && inQuestIndex < Quests.Count)
		{
			Quests[inQuestIndex].SetLocked(false);
			Quests[inQuestIndex].SetCompleted(completed);
		}
	}

	public int GetQuestIndex(Quest quest)
	{
		return Quests.IndexOf(quest);
	}

	public int GetQuestIndex(string inQuestID)
	{
		return Quests.FindIndex((Quest q) => q.ID == inQuestID);
	}
}
