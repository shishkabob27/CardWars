using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(0, Pack = 4, Size = 1)]
	[CallbackIdentity(2302)]
	public struct ScreenshotRequested_t
	{
		public const int k_iCallback = 2302;
	}
}
