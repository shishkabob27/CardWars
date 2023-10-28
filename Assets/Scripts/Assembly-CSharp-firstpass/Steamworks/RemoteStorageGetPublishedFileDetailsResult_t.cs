using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(0, Pack = 4)]
	[CallbackIdentity(1318)]
	public struct RemoteStorageGetPublishedFileDetailsResult_t
	{
		public const int k_iCallback = 1318;

		public EResult m_eResult;

		public PublishedFileId_t m_nPublishedFileId;

		public AppId_t m_nCreatorAppID;

		public AppId_t m_nConsumerAppID;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 129)]
		public string m_rgchTitle;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8000)]
		public string m_rgchDescription;

		public UGCHandle_t m_hFile;

		public UGCHandle_t m_hPreviewFile;

		public ulong m_ulSteamIDOwner;

		public uint m_rtimeCreated;

		public uint m_rtimeUpdated;

		public ERemoteStoragePublishedFileVisibility m_eVisibility;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bBanned;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1025)]
		public string m_rgchTags;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bTagsTruncated;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string m_pchFileName;

		public int m_nFileSize;

		public int m_nPreviewFileSize;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string m_rgchURL;

		public EWorkshopFileType m_eFileType;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bAcceptedForUse;
	}
}
