using Multiplayer;

public class TournamentYourRank : AsyncData<string>
{
	public UILabel YourRank;

	public UILabel YourRankVal;

	public bool IsGlobal;

	private void OnEnable()
	{
		if (Asyncdata.processed)
		{
			global::Multiplayer.Multiplayer.GetRank(SessionManager.GetInstance().theSession, IsGlobal, StringCallback);
		}
	}

	public void StringCallback(string data, ResponseFlag flag)
	{
		Asyncdata.Set(flag, data);
	}

	private void Update()
	{
		if (Asyncdata.processed)
		{
			return;
		}
		Asyncdata.processed = true;
		if (Asyncdata.MP_Data != null)
		{
			if ((bool)YourRank)
			{
				YourRank.text = KFFLocalization.Get("!!F_4_CURRENTRANK");
			}
			if (!YourRankVal)
			{
				return;
			}
			YourRankVal.text = Asyncdata.MP_Data;
			int result = int.MaxValue;
			if (int.TryParse(Asyncdata.MP_Data, out result))
			{
				if (result <= 50)
				{
					SocialManager.Instance.ReportAchievement(SocialManager.AchievementIDs.AT_ARENA_50);
				}
				if (result <= 10)
				{
					SocialManager.Instance.ReportAchievement(SocialManager.AchievementIDs.AT_ARENA_10);
				}
				if (result <= 1)
				{
					SocialManager.Instance.ReportAchievement(SocialManager.AchievementIDs.AT_ARENA_1);
				}
			}
		}
		else
		{
			if ((bool)YourRank)
			{
				YourRank.text = KFFLocalization.Get("!!YOU_ARE_NOT_RANKED");
			}
			if ((bool)YourRankVal)
			{
				YourRankVal.text = string.Empty;
			}
		}
	}
}
