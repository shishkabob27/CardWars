using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(0, Pack = 4, Size = 1)]
	[CallbackIdentity(1702)]
	public struct GCMessageFailed_t
	{
		public const int k_iCallback = 1702;
	}
}
