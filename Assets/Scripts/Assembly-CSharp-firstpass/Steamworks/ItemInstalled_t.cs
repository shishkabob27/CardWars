using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(0, Pack = 4)]
	[CallbackIdentity(3405)]
	public struct ItemInstalled_t
	{
		public const int k_iCallback = 3405;

		public AppId_t m_unAppID;

		public PublishedFileId_t m_nPublishedFileId;
	}
}
