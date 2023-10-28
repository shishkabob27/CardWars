using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(0, Pack = 4)]
	[CallbackIdentity(209)]
	public struct GSReputation_t
	{
		public const int k_iCallback = 209;

		public EResult m_eResult;

		public uint m_unReputationScore;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bBanned;

		public uint m_unBannedIP;

		public ushort m_usBannedPort;

		public ulong m_ulBannedGameID;

		public uint m_unBanExpires;
	}
}
