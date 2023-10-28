namespace Steamworks
{
	public enum EAuthSessionResponse
	{
		k_EAuthSessionResponseOK,
		k_EAuthSessionResponseUserNotConnectedToSteam,
		k_EAuthSessionResponseNoLicenseOrExpired,
		k_EAuthSessionResponseVACBanned,
		k_EAuthSessionResponseLoggedInElseWhere,
		k_EAuthSessionResponseVACCheckTimedOut,
		k_EAuthSessionResponseAuthTicketCanceled,
		k_EAuthSessionResponseAuthTicketInvalidAlreadyUsed,
		k_EAuthSessionResponseAuthTicketInvalid
	}
}
