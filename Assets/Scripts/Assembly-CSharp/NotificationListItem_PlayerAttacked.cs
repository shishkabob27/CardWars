using Multiplayer;

public class NotificationListItem_PlayerAttacked : NotificationListItem
{
	public UILabel titleLabel;

	public UILabel currentRankLabel;

	public UILabel currentRankValueLabel;

	public UILabel previousRankLabel;

	public UILabel previousRankValueLabel;

	public UILabel winsLabel;

	public UILabel winsValueLabel;

	public UILabel lossesLabel;

	public UILabel lossesValueLabel;

	public UISprite trophyIcon;

	public UILabel trophiesPlusMinusLabel;

	public UILabel trophiesLabel;

	public UILabel bestStreakLabel;

	public UILabel bestStreakValueLabel;

	public override void Setup(RecentNotification recent)
	{
		base.Setup(recent);
		if (titleLabel != null)
		{
			titleLabel.enabled = true;
			titleLabel.text = KFFLocalization.Get("!!6_NOTIFICATIONS_DECKATTACK");
		}
		if (currentRankLabel != null)
		{
			currentRankLabel.enabled = true;
		}
		if (currentRankValueLabel != null)
		{
			currentRankValueLabel.enabled = true;
			currentRankValueLabel.text = string.Empty + recent.rank;
		}
		string multiplayerRank = PlayerInfoScript.GetInstance().MultiplayerRank;
		if (previousRankLabel != null)
		{
			previousRankLabel.enabled = !string.IsNullOrEmpty(multiplayerRank);
		}
		if (previousRankValueLabel != null)
		{
			previousRankValueLabel.enabled = !string.IsNullOrEmpty(multiplayerRank);
			previousRankValueLabel.text = multiplayerRank;
		}
		if (winsLabel != null)
		{
			winsLabel.enabled = true;
		}
		if (winsValueLabel != null)
		{
			winsValueLabel.enabled = true;
			winsValueLabel.text = string.Empty + recent.wins;
		}
		if (lossesLabel != null)
		{
			lossesLabel.enabled = true;
		}
		if (lossesValueLabel != null)
		{
			lossesValueLabel.enabled = true;
			lossesValueLabel.text = string.Empty + recent.losses;
		}
		if (trophyIcon != null)
		{
			trophyIcon.enabled = false;
		}
		if (trophiesPlusMinusLabel != null)
		{
			trophiesPlusMinusLabel.enabled = false;
		}
		if (trophiesLabel != null)
		{
			trophiesLabel.enabled = false;
		}
		if (bestStreakLabel != null)
		{
			bestStreakLabel.enabled = false;
		}
		if (bestStreakValueLabel != null)
		{
			bestStreakValueLabel.enabled = false;
		}
	}

	public override void Setup(string newRank, string prevRank)
	{
		base.Setup(newRank, prevRank);
		if (newRank == prevRank)
		{
			prevRank = null;
		}
		if (titleLabel != null)
		{
			titleLabel.enabled = true;
			if (newRank == prevRank || string.IsNullOrEmpty(prevRank))
			{
				titleLabel.text = string.Format(KFFLocalization.Get("!!6_NOTIFICATIONS_CURRENT_RANK_FORMAT"), string.Empty + newRank);
			}
			else
			{
				titleLabel.text = KFFLocalization.Get("!!6_NOTIFICATIONS_RANK_CHANGED");
			}
		}
		if (currentRankLabel != null)
		{
			currentRankLabel.enabled = true;
		}
		if (currentRankValueLabel != null)
		{
			currentRankValueLabel.enabled = true;
			currentRankValueLabel.text = newRank;
		}
		if (previousRankLabel != null)
		{
			previousRankLabel.enabled = !string.IsNullOrEmpty(prevRank);
		}
		if (previousRankValueLabel != null)
		{
			previousRankValueLabel.enabled = !string.IsNullOrEmpty(prevRank);
			previousRankValueLabel.text = prevRank;
		}
		if (winsLabel != null)
		{
			winsLabel.enabled = false;
		}
		if (winsValueLabel != null)
		{
			winsValueLabel.enabled = false;
		}
		if (lossesLabel != null)
		{
			lossesLabel.enabled = false;
		}
		if (lossesValueLabel != null)
		{
			lossesValueLabel.enabled = false;
		}
		if (trophyIcon != null)
		{
			trophyIcon.enabled = false;
		}
		if (trophiesPlusMinusLabel != null)
		{
			trophiesPlusMinusLabel.enabled = false;
		}
		if (trophiesLabel != null)
		{
			trophiesLabel.enabled = false;
		}
		if (bestStreakLabel != null)
		{
			bestStreakLabel.enabled = false;
		}
		if (bestStreakValueLabel != null)
		{
			bestStreakValueLabel.enabled = false;
		}
	}
}
