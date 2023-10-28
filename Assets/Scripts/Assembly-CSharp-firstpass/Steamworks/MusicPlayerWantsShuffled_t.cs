using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(0, Pack = 4)]
	[CallbackIdentity(4109)]
	public struct MusicPlayerWantsShuffled_t
	{
		public const int k_iCallback = 4109;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bShuffled;
	}
}
