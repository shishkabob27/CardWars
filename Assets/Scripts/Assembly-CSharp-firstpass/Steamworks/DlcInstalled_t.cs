using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(0, Pack = 4)]
	[CallbackIdentity(1005)]
	public struct DlcInstalled_t
	{
		public const int k_iCallback = 1005;

		public AppId_t m_nAppID;
	}
}
