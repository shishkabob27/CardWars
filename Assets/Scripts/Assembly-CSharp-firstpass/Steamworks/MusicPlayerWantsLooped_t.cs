using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(0, Pack = 4)]
	[CallbackIdentity(4110)]
	public struct MusicPlayerWantsLooped_t
	{
		public const int k_iCallback = 4110;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bLooped;
	}
}
