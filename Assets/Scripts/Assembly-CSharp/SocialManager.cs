using System;
using System.Runtime.CompilerServices;

public class SocialManager
{
	public enum AchievementIDs
	{
		AT_HERO_2,
		AT_HERO_5,
		AT_HERO_15,
		AT_HERO_30,
		AT_HERO_50,
		AT_QUEST_3,
		AT_QUEST_8,
		AT_QUEST_15,
		AT_QUEST_30,
		AT_QUEST_50,
		AT_QUEST_99,
		AT_ARENA,
		AT_ARENA_50,
		AT_ARENA_10,
		AT_ARENA_1,
		AT_ALL_HEROS
	}

	private static SocialManager instance;

	public static SocialManager Instance
	{
		get
		{
			if (instance == null)
			{
				if (!KFFCSUtils.GetManifestKeyBool("force_amazon_store"))
				{
					instance = new GooglePlaySocial();
				}
				else
				{
					instance = new SocialManager();
				}
			}
			return instance;
		}
	}

	[method: MethodImpl(32)]
	public event Action playerAuthenticated;

	[method: MethodImpl(32)]
	public event Action<string> playerFailedToAuthenticate;

	[method: MethodImpl(32)]
	public event Action playerLoggedOut;

	public virtual void AuthenticatePlayer(bool silent)
	{
		playerDidAuthenticate();
	}

	public virtual bool IsPlayerAuthenticated()
	{
		return false;
	}

	public virtual string PlayerIdentifier()
	{
		return string.Empty;
	}

	public virtual string PlayerIdentifierHash()
	{
		return string.Empty;
	}

	public virtual bool IsAgeGateRequired()
	{
		return false;
	}

	public virtual bool IsRetryAuth(string error)
	{
		return false;
	}

	public virtual void ReportAchievement(AchievementIDs aID, float aPercent = 100f)
	{
	}

	public virtual void ShowAchievements()
	{
	}

	public virtual void ShowBannerAchievement()
	{
	}

	public virtual void ResetAllAchievements()
	{
	}

	protected void playerDidLogOut()
	{
		if (this.playerLoggedOut != null)
		{
			this.playerLoggedOut();
		}
	}

	protected void playerDidAuthenticate()
	{
		if (this.playerAuthenticated != null)
		{
			this.playerAuthenticated();
		}
	}

	protected void playerAuthenticationFailed(string error)
	{
		if (this.playerFailedToAuthenticate != null)
		{
			this.playerFailedToAuthenticate(error);
		}
	}
}
