using System;
using System.Collections.Generic;
using UnityEngine;

public class GooglePlaySocial : SocialManager
{
	private const string MANIFEST_ACHIEVEMENTKEY_PREFIX = "achievement_";

	private const string IOS_GPGID = "363866237318-9c31mcl7tcqrnkue399nlrep9levhmb5.apps.googleusercontent.com";

	private static readonly string[] NORETRYONFAIL_ERROR_IDS = new string[2] { "Canceled", "INTERNAL_ERROR" };

	private static string[] _gpgAchievementIDs = null;

	private bool _initialized;

	private string _playerId = string.Empty;

	private List<GPGAchievementMetadata> _achievementMetaData;

	public GooglePlaySocial()
	{
		GPGManager.authenticationSucceededEvent += delegate(string playerId)
		{
			_playerId = playerId;
			playerDidAuthenticate();
		};
		GPGManager.authenticationFailedEvent += delegate(string error)
		{
			_playerId = string.Empty;
			playerAuthenticationFailed(error);
		};
		GPGManager.userSignedOutEvent += base.playerDidLogOut;
		if (_gpgAchievementIDs == null)
		{
			string[] names = Enum.GetNames(typeof(AchievementIDs));
			for (int i = 0; i < names.Length; i++)
			{
				names[i] = "achievement_" + names[i];
			}
			_gpgAchievementIDs = KFFAndroidPlugin.GetManifestKeyStrings(names);
			for (int j = 0; j < names.Length; j++)
			{
				//TFUtils.DebugLog("Achievement ID mapping: " + Enum.GetName(typeof(AchievementIDs), j) + " => " + _gpgAchievementIDs[j]);
			}
		}
	}

	public override void AuthenticatePlayer(bool silent)
	{
		InitServices();
		if (silent)
		{
			PlayGameServices.attemptSilentAuthentication();
		}
		else
		{
			PlayGameServices.authenticate();
		}
	}

	public override bool IsPlayerAuthenticated()
	{
		if (!_initialized)
		{
			return false;
		}
		return PlayGameServices.isSignedIn();
	}

	public override bool IsAgeGateRequired()
	{
		return true;
	}

	public override bool IsRetryAuth(string error)
	{
		string[] nORETRYONFAIL_ERROR_IDS = NORETRYONFAIL_ERROR_IDS;
		foreach (string value in nORETRYONFAIL_ERROR_IDS)
		{
			if (error.Contains(value))
			{
				return false;
			}
		}
		return true;
	}

	public override string PlayerIdentifier()
	{
		return _playerId;
	}

	public override string PlayerIdentifierHash()
	{
		string data = PlayerIdentifier();
		XorCrypto xorCrypto = new XorCrypto("AdventureTime");
		return "GP_" + xorCrypto.Encrypt(data);
	}

	public override void ReportAchievement(AchievementIDs aID, float aPercent = 100f)
	{
		if (!IsPlayerAuthenticated())
		{
			return;
		}
		GPGAchievementMetadata achievementMetaData = GetAchievementMetaData(aID);
		if (achievementMetaData == null)
		{
			TFUtils.ErrorLog("Trying to report 'bad' achievement '" + aID.ToString() + "'. Ignoring...");
			return;
		}
		int numberOfSteps = achievementMetaData.numberOfSteps;
		if (numberOfSteps <= 1)
		{
			if (aPercent >= 100f && achievementMetaData.completedSteps <= 0)
			{
				TFUtils.DebugLog("One-step achievement reported");
				PlayGameServices.unlockAchievement(_gpgAchievementIDs[(int)aID]);
				achievementMetaData.completedSteps = 1;
			}
			return;
		}
		int num = Mathf.RoundToInt((float)numberOfSteps * aPercent);
		int num2 = Mathf.Max(0, achievementMetaData.completedSteps - num);
		if (num2 > 0)
		{
			TFUtils.DebugLog("Multi-step achievement reported (" + num + "/" + numberOfSteps + ")");
			PlayGameServices.incrementAchievement(_gpgAchievementIDs[(int)aID], num2);
			achievementMetaData.completedSteps = num;
		}
	}

	public override void ShowAchievements()
	{
		if (IsPlayerAuthenticated())
		{
			PlayGameServices.showAchievements();
		}
	}

	public override void ShowBannerAchievement()
	{
	}

	private GPGAchievementMetadata GetAchievementMetaData(AchievementIDs aID)
	{
		if (_achievementMetaData == null)
		{
			_achievementMetaData = PlayGameServices.getAllAchievementMetadata();
		}
		int aIdx = (int)aID;
		return _achievementMetaData.Find((GPGAchievementMetadata obj) => obj.achievementId == _gpgAchievementIDs[aIdx]);
	}

	private void InitServices()
	{
		if (!_initialized)
		{
			PlayGameServices.init("363866237318-9c31mcl7tcqrnkue399nlrep9levhmb5.apps.googleusercontent.com", false);
			_initialized = true;
		}
	}
}
