namespace Steamworks
{
	public enum EUserRestriction
	{
		k_nUserRestrictionNone = 0,
		k_nUserRestrictionUnknown = 1,
		k_nUserRestrictionAnyChat = 2,
		k_nUserRestrictionVoiceChat = 4,
		k_nUserRestrictionGroupChat = 8,
		k_nUserRestrictionRating = 0x10,
		k_nUserRestrictionGameInvites = 0x20,
		k_nUserRestrictionTrading = 0x40
	}
}
