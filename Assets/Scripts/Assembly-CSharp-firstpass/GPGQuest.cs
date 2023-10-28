using System;

public class GPGQuest
{
	public string questId;

	public string name;

	public string questDescription;

	public string iconUrl;

	public string bannerUrl;

	public int state;

	public DateTime startTimestamp;

	public DateTime expirationTimestamp;

	public DateTime acceptedTimestamp;

	public GPGQuestMilestone currentMilestone;

	public GPGQuestState stateEnum
	{
		get
		{
			return (GPGQuestState)state;
		}
	}
}
