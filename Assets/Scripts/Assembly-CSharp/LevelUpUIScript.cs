using Multiplayer;

public class LevelUpUIScript : AsyncData<ResponseFlag>
{
	public UILabel BannerLabel;

	public UILabel LevelLabel;

	public UILabel DeckLabel;

	public UILabel HPLabel;

	public UILabel PrefaceLabel;

	private LeaderItem leaderCard;

	public CWBattleEndWinnerStats battleEndWinner;

	private void OnEnable()
	{
		leaderCard = GameState.Instance.GetLeader(PlayerType.User);
		if (leaderCard == null)
		{
			return;
		}
		if (leaderCard.Rank >= 2)
		{
			SocialManager.Instance.ReportAchievement(SocialManager.AchievementIDs.AT_HERO_2);
		}
		if (leaderCard.Rank >= 5)
		{
			SocialManager.Instance.ReportAchievement(SocialManager.AchievementIDs.AT_HERO_5);
		}
		if (leaderCard.Rank >= 15)
		{
			SocialManager.Instance.ReportAchievement(SocialManager.AchievementIDs.AT_HERO_15);
		}
		if (leaderCard.Rank >= 30)
		{
			SocialManager.Instance.ReportAchievement(SocialManager.AchievementIDs.AT_HERO_30);
		}
		if (leaderCard.Rank >= 50)
		{
			SocialManager.Instance.ReportAchievement(SocialManager.AchievementIDs.AT_HERO_50);
		}
		Singleton<AnalyticsManager>.Instance.LogLevelUpLeader(leaderCard.Form.ID, leaderCard.Rank);
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (Asyncdata.processed && instance.MPPlayerName != null && instance.MPPlayerName != string.Empty)
		{
			Deck deck = instance.DeckManager.GetDeck(PlayerType.User);
			global::Multiplayer.Multiplayer.UpdateMultiplayerUser(SessionManager.GetInstance().theSession, instance.MPPlayerName, deck.Leader.Form.IconAtlas, instance.DeckManager.GetHighestLeaderRank(), SuccessCallback);
			if (instance.MPDeckLeaderID == leaderCard.Form.ID)
			{
				instance.DeckManager.UpdateMPDeck(SuccessCallback);
			}
		}
		if (BannerLabel != null)
		{
			BannerLabel.text = string.Format(KFFLocalization.Get("!!FORMAT_CARD_LEVELUP"), leaderCard.Form.Name, battleEndWinner.currentRank.ToString());
		}
		if (LevelLabel != null)
		{
			LevelLabel.text = battleEndWinner.currentRank.ToString();
		}
		if (DeckLabel != null)
		{
			DeckLabel.text = RankManager.Instance.FindRank(battleEndWinner.currentRank).DeckMaxSize.ToString();
		}
		if (HPLabel != null)
		{
			HPLabel.text = battleEndWinner.currentHP.ToString();
		}
		if (PrefaceLabel != null)
		{
			PrefaceLabel.text = KFFLocalization.Get("!!BS_O_PREFACE").Replace("<HERO>", leaderCard.Form.Name);
		}
	}

	private void SuccessCallback(ResponseFlag flag)
	{
		Asyncdata.Set(flag, flag);
	}

	private void Update()
	{
		if (!Asyncdata.processed)
		{
			Asyncdata.processed = true;
		}
	}
}
