using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(0, Pack = 4)]
	[CallbackIdentity(1305)]
	public struct RemoteStorageAppSyncStatusCheck_t
	{
		public const int k_iCallback = 1305;

		public AppId_t m_nAppID;

		public EResult m_eResult;
	}
}
