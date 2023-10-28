using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Prime31;

public class GPGManager : AbstractManager
{
	[method: MethodImpl(32)]
	public static event Action<string> authenticationSucceededEvent;

	[method: MethodImpl(32)]
	public static event Action<string> authenticationFailedEvent;

	[method: MethodImpl(32)]
	public static event Action userSignedOutEvent;

	[method: MethodImpl(32)]
	public static event Action<string> reloadDataForKeyFailedEvent;

	[method: MethodImpl(32)]
	public static event Action<string> reloadDataForKeySucceededEvent;

	[method: MethodImpl(32)]
	public static event Action licenseCheckFailedEvent;

	[method: MethodImpl(32)]
	public static event Action<string> profileImageLoadedAtPathEvent;

	[method: MethodImpl(32)]
	public static event Action<string> finishedSharingEvent;

	[method: MethodImpl(32)]
	public static event Action<GPGPlayerInfo, string> loadPlayerCompletedEvent;

	[method: MethodImpl(32)]
	public static event Action<string> loadCloudDataForKeyFailedEvent;

	[method: MethodImpl(32)]
	public static event Action<int, string> loadCloudDataForKeySucceededEvent;

	[method: MethodImpl(32)]
	public static event Action<string> updateCloudDataForKeyFailedEvent;

	[method: MethodImpl(32)]
	public static event Action<int, string> updateCloudDataForKeySucceededEvent;

	[method: MethodImpl(32)]
	public static event Action<string> clearCloudDataForKeyFailedEvent;

	[method: MethodImpl(32)]
	public static event Action<string> clearCloudDataForKeySucceededEvent;

	[method: MethodImpl(32)]
	public static event Action<string> deleteCloudDataForKeyFailedEvent;

	[method: MethodImpl(32)]
	public static event Action<string> deleteCloudDataForKeySucceededEvent;

	[method: MethodImpl(32)]
	public static event Action<string, string> unlockAchievementFailedEvent;

	[method: MethodImpl(32)]
	public static event Action<string, bool> unlockAchievementSucceededEvent;

	[method: MethodImpl(32)]
	public static event Action<string, string> incrementAchievementFailedEvent;

	[method: MethodImpl(32)]
	public static event Action<string, bool> incrementAchievementSucceededEvent;

	[method: MethodImpl(32)]
	public static event Action<string, string> revealAchievementFailedEvent;

	[method: MethodImpl(32)]
	public static event Action<string> revealAchievementSucceededEvent;

	[method: MethodImpl(32)]
	public static event Action<string, string> submitScoreFailedEvent;

	[method: MethodImpl(32)]
	public static event Action<string, Dictionary<string, object>> submitScoreSucceededEvent;

	[method: MethodImpl(32)]
	public static event Action<string, string> loadScoresFailedEvent;

	[method: MethodImpl(32)]
	public static event Action<List<GPGScore>> loadScoresSucceededEvent;

	[method: MethodImpl(32)]
	public static event Action<GPGScore> loadCurrentPlayerLeaderboardScoreSucceededEvent;

	[method: MethodImpl(32)]
	public static event Action<string, string> loadCurrentPlayerLeaderboardScoreFailedEvent;

	[method: MethodImpl(32)]
	public static event Action<List<GPGEvent>> allEventsLoadedEvent;

	[method: MethodImpl(32)]
	public static event Action<GPGQuest> questListLauncherAcceptedQuestEvent;

	[method: MethodImpl(32)]
	public static event Action<GPGQuestMilestone> questClaimedRewardsForQuestMilestoneEvent;

	[method: MethodImpl(32)]
	public static event Action<GPGQuest> questCompletedEvent;

	[method: MethodImpl(32)]
	public static event Action<List<GPGQuest>> allQuestsLoadedEvent;

	[method: MethodImpl(32)]
	public static event Action<GPGSnapshotMetadata> snapshotListUserSelectedSnapshotEvent;

	[method: MethodImpl(32)]
	public static event Action snapshotListUserRequestedNewSnapshotEvent;

	[method: MethodImpl(32)]
	public static event Action snapshotListCanceledEvent;

	[method: MethodImpl(32)]
	public static event Action saveSnapshotSucceededEvent;

	[method: MethodImpl(32)]
	public static event Action<string> saveSnapshotFailedEvent;

	[method: MethodImpl(32)]
	public static event Action<GPGSnapshot> loadSnapshotSucceededEvent;

	[method: MethodImpl(32)]
	public static event Action<string> loadSnapshotFailedEvent;

	static GPGManager()
	{
		AbstractManager.initialize(typeof(GPGManager));
	}

	private void fireEventWithIdentifierAndError(Action<string, string> theEvent, string json)
	{
		if (theEvent != null)
		{
			Dictionary<string, object> dictionary = json.dictionaryFromJson();
			if (dictionary != null && dictionary.ContainsKey("identifier") && dictionary.ContainsKey("error"))
			{
				theEvent(dictionary["identifier"].ToString(), dictionary["error"].ToString());
			}
		}
	}

	private void fireEventWithIdentifierAndBool(Action<string, bool> theEvent, string param)
	{
		if (theEvent != null)
		{
			string[] array = param.Split(',');
			if (array.Length == 2)
			{
				theEvent(array[0], array[1] == "1");
			}
		}
	}

	private void userSignedOut(string empty)
	{
		GPGManager.userSignedOutEvent.fire();
	}

	private void reloadDataForKeyFailed(string error)
	{
		GPGManager.reloadDataForKeyFailedEvent.fire(error);
	}

	private void reloadDataForKeySucceeded(string param)
	{
		GPGManager.reloadDataForKeySucceededEvent.fire(param);
	}

	private void licenseCheckFailed(string param)
	{
		GPGManager.licenseCheckFailedEvent.fire();
	}

	private void profileImageLoadedAtPath(string path)
	{
		GPGManager.profileImageLoadedAtPathEvent.fire(path);
	}

	private void finishedSharing(string errorOrNull)
	{
		GPGManager.finishedSharingEvent.fire(errorOrNull);
	}

	private void loadPlayerCompleted(string playerOrError)
	{
		if (GPGManager.loadPlayerCompletedEvent != null)
		{
			if (playerOrError.StartsWith("{"))
			{
				GPGManager.loadPlayerCompletedEvent(Json.decode<GPGPlayerInfo>(playerOrError), null);
			}
			else
			{
				GPGManager.loadPlayerCompletedEvent(null, playerOrError);
			}
		}
	}

	private void loadCloudDataForKeyFailed(string error)
	{
		GPGManager.loadCloudDataForKeyFailedEvent.fire(error);
	}

	private void loadCloudDataForKeySucceeded(string json)
	{
		Dictionary<string, object> dictionary = json.dictionaryFromJson();
		GPGManager.loadCloudDataForKeySucceededEvent.fire(int.Parse(dictionary["key"].ToString()), dictionary["data"].ToString());
	}

	private void updateCloudDataForKeyFailed(string error)
	{
		GPGManager.updateCloudDataForKeyFailedEvent.fire(error);
	}

	private void updateCloudDataForKeySucceeded(string json)
	{
		Dictionary<string, object> dictionary = json.dictionaryFromJson();
		GPGManager.updateCloudDataForKeySucceededEvent.fire(int.Parse(dictionary["key"].ToString()), dictionary["data"].ToString());
	}

	private void clearCloudDataForKeyFailed(string error)
	{
		GPGManager.clearCloudDataForKeyFailedEvent.fire(error);
	}

	private void clearCloudDataForKeySucceeded(string param)
	{
		GPGManager.clearCloudDataForKeySucceededEvent.fire(param);
	}

	private void deleteCloudDataForKeyFailed(string error)
	{
		GPGManager.deleteCloudDataForKeyFailedEvent.fire(error);
	}

	private void deleteCloudDataForKeySucceeded(string param)
	{
		GPGManager.deleteCloudDataForKeySucceededEvent.fire(param);
	}

	private void unlockAchievementFailed(string json)
	{
		fireEventWithIdentifierAndError(GPGManager.unlockAchievementFailedEvent, json);
	}

	private void unlockAchievementSucceeded(string param)
	{
		fireEventWithIdentifierAndBool(GPGManager.unlockAchievementSucceededEvent, param);
	}

	private void incrementAchievementFailed(string json)
	{
		fireEventWithIdentifierAndError(GPGManager.incrementAchievementFailedEvent, json);
	}

	private void incrementAchievementSucceeded(string param)
	{
		string[] array = param.Split(',');
		if (array.Length == 2)
		{
			GPGManager.incrementAchievementSucceededEvent.fire(array[0], array[1] == "1");
		}
	}

	private void revealAchievementFailed(string json)
	{
		fireEventWithIdentifierAndError(GPGManager.revealAchievementFailedEvent, json);
	}

	private void revealAchievementSucceeded(string achievementId)
	{
		GPGManager.revealAchievementSucceededEvent.fire(achievementId);
	}

	private void submitScoreFailed(string json)
	{
		fireEventWithIdentifierAndError(GPGManager.submitScoreFailedEvent, json);
	}

	private void submitScoreSucceeded(string json)
	{
		if (GPGManager.submitScoreSucceededEvent != null)
		{
			Dictionary<string, object> dictionary = json.dictionaryFromJson();
			string arg = "Unknown";
			if (dictionary.ContainsKey("leaderboardId"))
			{
				arg = dictionary["leaderboardId"].ToString();
			}
			GPGManager.submitScoreSucceededEvent(arg, dictionary);
		}
	}

	private void loadScoresFailed(string json)
	{
		fireEventWithIdentifierAndError(GPGManager.loadScoresFailedEvent, json);
	}

	private void loadScoresSucceeded(string json)
	{
		if (GPGManager.loadScoresSucceededEvent != null)
		{
			GPGManager.loadScoresSucceededEvent(Json.decode<List<GPGScore>>(json));
		}
	}

	private void loadCurrentPlayerLeaderboardScoreSucceeded(string json)
	{
		if (GPGManager.loadCurrentPlayerLeaderboardScoreSucceededEvent != null)
		{
			GPGManager.loadCurrentPlayerLeaderboardScoreSucceededEvent(Json.decode<GPGScore>(json));
		}
	}

	private void loadCurrentPlayerLeaderboardScoreFailed(string json)
	{
		fireEventWithIdentifierAndError(GPGManager.loadCurrentPlayerLeaderboardScoreFailedEvent, json);
	}

	private void authenticationSucceeded(string param)
	{
		GPGManager.authenticationSucceededEvent.fire(param);
	}

	private void authenticationFailed(string error)
	{
		GPGManager.authenticationFailedEvent.fire(error);
	}

	private void allEventsLoaded(string json)
	{
		if (GPGManager.allEventsLoadedEvent != null)
		{
			GPGManager.allEventsLoadedEvent(Json.decode<List<GPGEvent>>(json));
		}
	}

	private void questListLauncherClaimedRewardsForQuestMilestone(string json)
	{
		if (GPGManager.questClaimedRewardsForQuestMilestoneEvent != null)
		{
			GPGManager.questClaimedRewardsForQuestMilestoneEvent(Json.decode<GPGQuestMilestone>(json));
		}
	}

	private void questCompleted(string json)
	{
		if (GPGManager.questCompletedEvent != null)
		{
			GPGManager.questCompletedEvent(Json.decode<GPGQuest>(json));
		}
	}

	private void questListLauncherAcceptedQuest(string json)
	{
		if (GPGManager.questListLauncherAcceptedQuestEvent != null)
		{
			GPGManager.questListLauncherAcceptedQuestEvent(Json.decode<GPGQuest>(json));
		}
	}

	private void allQuestsLoaded(string json)
	{
		if (GPGManager.allQuestsLoadedEvent != null)
		{
			GPGManager.allQuestsLoadedEvent(Json.decode<List<GPGQuest>>(json));
		}
	}

	private void snapshotListUserSelectedSnapshot(string json)
	{
		if (GPGManager.snapshotListUserSelectedSnapshotEvent != null)
		{
			GPGManager.snapshotListUserSelectedSnapshotEvent(Json.decode<GPGSnapshotMetadata>(json));
		}
	}

	private void snapshotListUserRequestedNewSnapshot(string empty)
	{
		GPGManager.snapshotListUserRequestedNewSnapshotEvent.fire();
	}

	private void snapshotListCanceled(string empty)
	{
		GPGManager.snapshotListCanceledEvent.fire();
	}

	private void saveSnapshotSucceeded(string empty)
	{
		GPGManager.saveSnapshotSucceededEvent.fire();
	}

	private void saveSnapshotFailed(string error)
	{
		GPGManager.saveSnapshotFailedEvent.fire(error);
	}

	private void loadSnapshotSucceeded(string json)
	{
		if (GPGManager.loadSnapshotSucceededEvent != null)
		{
			GPGManager.loadSnapshotSucceededEvent(Json.decode<GPGSnapshot>(json));
		}
	}

	private void loadSnapshotFailed(string error)
	{
		GPGManager.loadSnapshotFailedEvent.fire(error);
	}
}
