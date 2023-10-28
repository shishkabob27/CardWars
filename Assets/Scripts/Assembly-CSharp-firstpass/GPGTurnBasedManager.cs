using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Prime31;

public class GPGTurnBasedManager : AbstractManager
{
	[method: MethodImpl(32)]
	public static event Action<string> onInvitationReceivedEvent;

	[method: MethodImpl(32)]
	public static event Action<string> onInvitationRemovedEvent;

	[method: MethodImpl(32)]
	public static event Action<GPGTurnBasedMatch> matchChangedEvent;

	[method: MethodImpl(32)]
	public static event Action<string> matchFailedEvent;

	[method: MethodImpl(32)]
	public static event Action<GPGTurnBasedMatch> matchEndedEvent;

	[method: MethodImpl(32)]
	public static event Action playerSelectorCanceledEvent;

	[method: MethodImpl(32)]
	public static event Action<bool, string, List<GPGTurnBasedMatch>> loadMatchesCompletedEvent;

	[method: MethodImpl(32)]
	public static event Action<bool, string> takeTurnCompletedEvent;

	[method: MethodImpl(32)]
	public static event Action<bool, string> finishMatchCompletedEvent;

	[method: MethodImpl(32)]
	public static event Action<bool, string> dismissMatchCompletedEvent;

	[method: MethodImpl(32)]
	public static event Action<bool, string> leaveDuringTurnCompletedEvent;

	[method: MethodImpl(32)]
	public static event Action<bool, string> leaveOutOfTurnCompletedEvent;

	[method: MethodImpl(32)]
	public static event Action<GPGTurnBasedInvitation> invitationReceivedEvent;

	static GPGTurnBasedManager()
	{
		AbstractManager.initialize(typeof(GPGTurnBasedManager));
	}

	private void onInvitationReceived(string invitationId)
	{
		GPGTurnBasedManager.onInvitationReceivedEvent.fire(invitationId);
	}

	private void onInvitationRemoved(string invitationId)
	{
		GPGTurnBasedManager.onInvitationRemovedEvent.fire(invitationId);
	}

	private void matchChanged(string json)
	{
		if (GPGTurnBasedManager.matchChangedEvent != null)
		{
			GPGTurnBasedManager.matchChangedEvent(Json.decode<GPGTurnBasedMatch>(json));
		}
	}

	private void matchFailed(string error)
	{
		GPGTurnBasedManager.matchFailedEvent.fire(error);
	}

	private void matchEnded(string json)
	{
		if (GPGTurnBasedManager.matchEndedEvent != null)
		{
			GPGTurnBasedManager.matchEndedEvent(Json.decode<GPGTurnBasedMatch>(json));
		}
	}

	private void playerSelectorCanceled(string empty)
	{
		GPGTurnBasedManager.playerSelectorCanceledEvent.fire();
	}

	private void loadMatchesFailed(string error)
	{
		GPGTurnBasedManager.loadMatchesCompletedEvent(false, error, null);
	}

	private void loadMatchesSucceeded(string json)
	{
		if (GPGTurnBasedManager.loadMatchesCompletedEvent != null)
		{
			GPGTurnBasedManager.loadMatchesCompletedEvent.fire(true, null, Json.decode<List<GPGTurnBasedMatch>>(json));
		}
	}

	private void takeTurnFailed(string error)
	{
		GPGTurnBasedManager.takeTurnCompletedEvent.fire(false, error);
	}

	private void takeTurnSucceeded(string empty)
	{
		GPGTurnBasedManager.takeTurnCompletedEvent.fire(true, null);
	}

	private void finishMatchFailed(string error)
	{
		GPGTurnBasedManager.finishMatchCompletedEvent.fire(false, error);
	}

	private void finishMatchSucceeded(string empty)
	{
		GPGTurnBasedManager.finishMatchCompletedEvent.fire(true, null);
	}

	private void dismissMatchFailed(string error)
	{
		GPGTurnBasedManager.dismissMatchCompletedEvent.fire(false, error);
	}

	private void dismissMatchSucceeded(string empty)
	{
		GPGTurnBasedManager.dismissMatchCompletedEvent.fire(true, null);
	}

	private void leaveDuringTurnFailed(string error)
	{
		GPGTurnBasedManager.leaveDuringTurnCompletedEvent.fire(false, error);
	}

	private void leaveDuringTurnSucceeded(string empty)
	{
		GPGTurnBasedManager.leaveDuringTurnCompletedEvent.fire(true, null);
	}

	private void leaveOutOfTurnFailed(string error)
	{
		GPGTurnBasedManager.leaveOutOfTurnCompletedEvent.fire(false, error);
	}

	private void leaveOutOfTurnSucceeded(string empty)
	{
		GPGTurnBasedManager.leaveOutOfTurnCompletedEvent.fire(true, null);
	}

	private void invitationReceived(string json)
	{
		if (GPGTurnBasedManager.invitationReceivedEvent != null)
		{
			GPGTurnBasedManager.invitationReceivedEvent(Json.decode<GPGTurnBasedInvitation>(json));
		}
	}
}
