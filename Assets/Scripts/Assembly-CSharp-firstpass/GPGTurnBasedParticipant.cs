using System;
using Prime31;

public class GPGTurnBasedParticipant
{
	public GPGPlayerInfo player;

	public string participantId;

	public bool isAutoMatchedPlayer;

	public int statusInt;

	public GPGTurnBasedParticipantStatus status
	{
		get
		{
			return (GPGTurnBasedParticipantStatus)(int)Enum.ToObject(typeof(GPGTurnBasedParticipantStatus), statusInt);
		}
	}

	public string statusString
	{
		get
		{
			return status.ToString();
		}
	}

	public override string ToString()
	{
		return JsonFormatter.prettyPrint(Json.encode(this));
	}
}
