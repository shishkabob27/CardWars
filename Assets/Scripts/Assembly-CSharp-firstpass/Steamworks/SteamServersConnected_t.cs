using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(0, Pack = 4, Size = 1)]
	[CallbackIdentity(101)]
	public struct SteamServersConnected_t
	{
		public const int k_iCallback = 101;
	}
}
