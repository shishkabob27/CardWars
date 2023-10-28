using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(0, Pack = 4)]
	[CallbackIdentity(103)]
	public struct SteamServersDisconnected_t
	{
		public const int k_iCallback = 103;

		public EResult m_eResult;
	}
}
