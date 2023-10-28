public class GPGTurnBasedParticipantResult
{
	public string participantId;

	public int result;

	public int placing;

	public GPGTurnBasedParticipantResult(string participantId, GPGTurnBasedParticipantResultStatus result)
	{
		this.participantId = participantId;
		this.result = (int)result;
	}

	public GPGTurnBasedParticipantResult(string participantId, int placing)
	{
		this.participantId = participantId;
		this.placing = placing;
	}

	public GPGTurnBasedParticipantResult(string participantId, GPGTurnBasedParticipantResultStatus result, int placing)
	{
		this.participantId = participantId;
		this.result = (int)result;
		this.placing = placing;
	}
}
