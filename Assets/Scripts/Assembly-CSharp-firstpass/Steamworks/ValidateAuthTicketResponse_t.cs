using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(0, Pack = 4)]
	[CallbackIdentity(143)]
	public struct ValidateAuthTicketResponse_t
	{
		public const int k_iCallback = 143;

		public CSteamID m_SteamID;

		public EAuthSessionResponse m_eAuthSessionResponse;

		public CSteamID m_OwnerSteamID;
	}
}
