using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.Networking;

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
				instance = new SocialManager();
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

    [method: MethodImpl(32)]
    public event Action playerAccountDoesNotExist;

    public virtual void AuthenticatePlayer(bool silent)
    {
		authed = false;
        WWW www = new WWW(SQSettings.SERVER_URL + "account/auth?user="+PlayerPrefs.GetString("user")+"&pass="+ PlayerPrefs.GetString("pass"));
        UnityEngine.Debug.Log("Attempting to authenticate player: " + www.url);
		
		while (!www.isDone)
		{
        }

		object response = null;


        try
		{
            response = MiniJSON.Json.Deserialize(www.text);
        }
		catch (Exception e)
		{
            playerAuthenticationFailed(www.error);
        }

        UnityEngine.Debug.Log(response);

        if (response == null)
        {
            UnityEngine.Debug.Log(www.error);
			playerAuthenticationFailed(www.error);
        }
        else
        {
            Dictionary<string, object> responseData = (Dictionary<string, object>)response;

            if (responseData.ContainsKey("success") && (bool)responseData["success"])
            {
                UnityEngine.Debug.Log("Authentication successful");

                playerDidAuthenticate();
            }
            else
            {
                UnityEngine.Debug.Log("Authentication failed");
                playerAuthenticationFailed((string)responseData["message"]);
            }
        }
	}

	bool authed = false;

	public virtual bool IsPlayerAuthenticated()
	{
		return authed;
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

	public void playerDidLogOut()
	{
		if (this.playerLoggedOut != null)
		{
			authed = false;
			this.playerLoggedOut();
		}
	}

	protected void playerDidAuthenticate()
	{
		if (this.playerAuthenticated != null)
		{
			authed = true;
			this.playerAuthenticated();
		}
	}

	protected void playerAuthenticationFailed(string error)
	{
		if (this.playerFailedToAuthenticate != null)
		{
			authed = false;
			this.playerFailedToAuthenticate(error);
		}
	}

    protected void playerAccountNotExist()
    {
        if (this.playerAccountDoesNotExist != null)
        {
			authed = false;
            this.playerAccountDoesNotExist();
        }
    }
}
