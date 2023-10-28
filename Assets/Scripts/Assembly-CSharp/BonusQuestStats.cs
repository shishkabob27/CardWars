using System;

public class BonusQuestStats
{
	public int ActiveQuestID;

	public QuestData ActiveQuest;

	public int LastReplacedQuestID;

	public int ReplacedQuestID;

	public DateTime LastPlayedTime = DateTime.MinValue;

	public MatchStats CachedMatchStats = new MatchStats();

	public bool firstAppearance;
}
