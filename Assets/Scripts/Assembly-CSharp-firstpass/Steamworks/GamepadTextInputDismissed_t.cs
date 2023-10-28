using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(0, Pack = 4)]
	[CallbackIdentity(714)]
	public struct GamepadTextInputDismissed_t
	{
		public const int k_iCallback = 714;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bSubmitted;

		public uint m_unSubmittedText;
	}
}
