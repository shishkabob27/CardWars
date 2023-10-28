using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(0, Pack = 1)]
	[CallbackIdentity(208)]
	public struct GSClientGroupStatus_t
	{
		public const int k_iCallback = 208;

		public CSteamID m_SteamIDUser;

		public CSteamID m_SteamIDGroup;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bMember;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bOfficer;
	}
}
