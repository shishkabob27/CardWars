using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(2, Pack = 4)]
	[CallbackIdentity(1101)]
	public struct UserStatsReceived_t
	{
		public const int k_iCallback = 1101;

		[FieldOffset(0)]
		public ulong m_nGameID;

		[FieldOffset(8)]
		public EResult m_eResult;

		[FieldOffset(12)]
		public CSteamID m_steamIDUser;
	}
}
