using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(0, Pack = 4)]
	[CallbackIdentity(1301)]
	public struct RemoteStorageAppSyncedClient_t
	{
		public const int k_iCallback = 1301;

		public AppId_t m_nAppID;

		public EResult m_eResult;

		public int m_unNumDownloads;
	}
}
