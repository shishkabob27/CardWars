using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(0, Pack = 4)]
	[CallbackIdentity(336)]
	public struct FriendRichPresenceUpdate_t
	{
		public const int k_iCallback = 336;

		public CSteamID m_steamIDFriend;

		public AppId_t m_nAppID;
	}
}
