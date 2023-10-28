using System.Collections.Generic;
using Prime31;
using UnityEngine;

public class GPGTurnBasedMultiplayer
{
	private static AndroidJavaObject _plugin;

	static GPGTurnBasedMultiplayer()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.prime31.PlayGameServicesPlugin"))
		{
			_plugin = androidJavaClass.CallStatic<AndroidJavaObject>("turnBasedMultiplayerInstance", new object[0]);
		}
	}

	public static void checkForInvitesAndMatches()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("checkForInvitesAndMatches");
		}
	}

	public static void showInbox()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("showInbox");
		}
	}

	public static void showPlayerSelector(int minPlayersToPick, int maxPlayersToPick)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("showPlayerSelector", minPlayersToPick, maxPlayersToPick);
		}
	}

	public static void createMatchProgrammatically(int minAutoMatchPlayers, int maxAutoMatchPlayers, long exclusiveBitmask = 0, int variant = 1)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("createMatchProgrammatically", minAutoMatchPlayers, maxAutoMatchPlayers, exclusiveBitmask, variant);
		}
	}

	public static void loadAllMatches()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("loadAllMatches");
		}
	}

	public static void takeTurn(string matchId, byte[] matchData, string pendingParticipantId)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("takeTurn", matchId, matchData, pendingParticipantId);
		}
	}

	public static void leaveDuringTurn(string matchId, string pendingParticipantId)
	{
		if (pendingParticipantId != null && Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("leaveDuringTurn", matchId, pendingParticipantId);
		}
	}

	public static void leaveOutOfTurn(string matchId)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("leaveOutOfTurn", matchId);
		}
	}

	public static void finishMatchWithData(string matchId, byte[] matchData, List<GPGTurnBasedParticipantResult> results)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("finishMatchWithData", matchId, matchData, Json.encode(results));
		}
	}

	public static void finishMatchWithoutData(string matchId)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("finishMatchWithoutData", matchId);
		}
	}

	public static void dismissMatch(string matchId)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("dismissMatch", matchId);
		}
	}

	public static void rematch(string matchId)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("rematch", matchId);
		}
	}

	public static void joinMatchWithInvitation(string invitationId)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("joinMatchWithInvitation", invitationId);
		}
	}

	public static void declineMatchWithInvitation(string invitationId)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("declineMatchWithInvitation", invitationId);
		}
	}
}
