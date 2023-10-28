using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(0, Pack = 4, Size = 1)]
	[CallbackIdentity(1014)]
	public struct NewLaunchQueryParameters_t
	{
		public const int k_iCallback = 1014;
	}
}
