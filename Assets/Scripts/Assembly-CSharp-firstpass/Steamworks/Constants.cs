namespace Steamworks
{
	public static class Constants
	{
		public const string STEAMAPPLIST_INTERFACE_VERSION = "STEAMAPPLIST_INTERFACE_VERSION001";

		public const string STEAMAPPS_INTERFACE_VERSION = "STEAMAPPS_INTERFACE_VERSION006";

		public const string STEAMAPPTICKET_INTERFACE_VERSION = "STEAMAPPTICKET_INTERFACE_VERSION001";

		public const string STEAMCLIENT_INTERFACE_VERSION = "SteamClient015";

		public const string STEAMCONTROLLER_INTERFACE_VERSION = "STEAMCONTROLLER_INTERFACE_VERSION";

		public const string STEAMFRIENDS_INTERFACE_VERSION = "SteamFriends014";

		public const string STEAMGAMECOORDINATOR_INTERFACE_VERSION = "SteamGameCoordinator001";

		public const string STEAMGAMESERVER_INTERFACE_VERSION = "SteamGameServer012";

		public const string STEAMGAMESERVERSTATS_INTERFACE_VERSION = "SteamGameServerStats001";

		public const string STEAMHTTP_INTERFACE_VERSION = "STEAMHTTP_INTERFACE_VERSION002";

		public const string STEAMMATCHMAKING_INTERFACE_VERSION = "SteamMatchMaking009";

		public const string STEAMMATCHMAKINGSERVERS_INTERFACE_VERSION = "SteamMatchMakingServers002";

		public const string STEAMMUSIC_INTERFACE_VERSION = "STEAMMUSIC_INTERFACE_VERSION001";

		public const string STEAMMUSICREMOTE_INTERFACE_VERSION = "STEAMMUSICREMOTE_INTERFACE_VERSION001";

		public const string STEAMNETWORKING_INTERFACE_VERSION = "SteamNetworking005";

		public const string STEAMREMOTESTORAGE_INTERFACE_VERSION = "STEAMREMOTESTORAGE_INTERFACE_VERSION012";

		public const string STEAMSCREENSHOTS_INTERFACE_VERSION = "STEAMSCREENSHOTS_INTERFACE_VERSION002";

		public const string STEAMUGC_INTERFACE_VERSION = "STEAMUGC_INTERFACE_VERSION002";

		public const string STEAMUNIFIEDMESSAGES_INTERFACE_VERSION = "STEAMUNIFIEDMESSAGES_INTERFACE_VERSION001";

		public const string STEAMUSER_INTERFACE_VERSION = "SteamUser017";

		public const string STEAMUSERSTATS_INTERFACE_VERSION = "STEAMUSERSTATS_INTERFACE_VERSION011";

		public const string STEAMUTILS_INTERFACE_VERSION = "SteamUtils007";

		public const int k_cubAppProofOfPurchaseKeyMax = 64;

		public const int k_iSteamUserCallbacks = 100;

		public const int k_iSteamGameServerCallbacks = 200;

		public const int k_iSteamFriendsCallbacks = 300;

		public const int k_iSteamBillingCallbacks = 400;

		public const int k_iSteamMatchmakingCallbacks = 500;

		public const int k_iSteamContentServerCallbacks = 600;

		public const int k_iSteamUtilsCallbacks = 700;

		public const int k_iClientFriendsCallbacks = 800;

		public const int k_iClientUserCallbacks = 900;

		public const int k_iSteamAppsCallbacks = 1000;

		public const int k_iSteamUserStatsCallbacks = 1100;

		public const int k_iSteamNetworkingCallbacks = 1200;

		public const int k_iClientRemoteStorageCallbacks = 1300;

		public const int k_iClientDepotBuilderCallbacks = 1400;

		public const int k_iSteamGameServerItemsCallbacks = 1500;

		public const int k_iClientUtilsCallbacks = 1600;

		public const int k_iSteamGameCoordinatorCallbacks = 1700;

		public const int k_iSteamGameServerStatsCallbacks = 1800;

		public const int k_iSteam2AsyncCallbacks = 1900;

		public const int k_iSteamGameStatsCallbacks = 2000;

		public const int k_iClientHTTPCallbacks = 2100;

		public const int k_iClientScreenshotsCallbacks = 2200;

		public const int k_iSteamScreenshotsCallbacks = 2300;

		public const int k_iClientAudioCallbacks = 2400;

		public const int k_iClientUnifiedMessagesCallbacks = 2500;

		public const int k_iSteamStreamLauncherCallbacks = 2600;

		public const int k_iClientControllerCallbacks = 2700;

		public const int k_iSteamControllerCallbacks = 2800;

		public const int k_iClientParentalSettingsCallbacks = 2900;

		public const int k_iClientDeviceAuthCallbacks = 3000;

		public const int k_iClientNetworkDeviceManagerCallbacks = 3100;

		public const int k_iClientMusicCallbacks = 3200;

		public const int k_iClientRemoteClientManagerCallbacks = 3300;

		public const int k_iClientUGCCallbacks = 3400;

		public const int k_iSteamStreamClientCallbacks = 3500;

		public const int k_IClientProductBuilderCallbacks = 3600;

		public const int k_iClientShortcutsCallbacks = 3700;

		public const int k_iClientRemoteControlManagerCallbacks = 3800;

		public const int k_iSteamAppListCallbacks = 3900;

		public const int k_iSteamMusicCallbacks = 4000;

		public const int k_iSteamMusicRemoteCallbacks = 4100;

		public const int k_iClientVRCallbacks = 4200;

		public const int k_cchMaxFriendsGroupName = 64;

		public const int k_cFriendsGroupLimit = 100;

		public const int k_cEnumerateFollowersMax = 50;

		public const int k_cchPersonaNameMax = 128;

		public const int k_cwchPersonaNameMax = 32;

		public const int k_cubChatMetadataMax = 8192;

		public const int k_cchMaxRichPresenceKeys = 20;

		public const int k_cchMaxRichPresenceKeyLength = 64;

		public const int k_cchMaxRichPresenceValueLength = 256;

		public const int k_unServerFlagNone = 0;

		public const int k_unServerFlagActive = 1;

		public const int k_unServerFlagSecure = 2;

		public const int k_unServerFlagDedicated = 4;

		public const int k_unServerFlagLinux = 8;

		public const int k_unServerFlagPassworded = 16;

		public const int k_unServerFlagPrivate = 32;

		public const int k_unFavoriteFlagNone = 0;

		public const int k_unFavoriteFlagFavorite = 1;

		public const int k_unFavoriteFlagHistory = 2;

		public const int k_unMaxCloudFileChunkSize = 104857600;

		public const int k_cchPublishedDocumentTitleMax = 129;

		public const int k_cchPublishedDocumentDescriptionMax = 8000;

		public const int k_cchPublishedDocumentChangeDescriptionMax = 8000;

		public const int k_unEnumeratePublishedFilesMaxResults = 50;

		public const int k_cchTagListMax = 1025;

		public const int k_cchFilenameMax = 260;

		public const int k_cchPublishedFileURLMax = 256;

		public const int k_nScreenshotMaxTaggedUsers = 32;

		public const int k_nScreenshotMaxTaggedPublishedFiles = 32;

		public const int k_cubUFSTagTypeMax = 255;

		public const int k_cubUFSTagValueMax = 255;

		public const int k_ScreenshotThumbWidth = 200;

		public const int kNumUGCResultsPerPage = 50;

		public const int k_cchStatNameMax = 128;

		public const int k_cchLeaderboardNameMax = 128;

		public const int k_cLeaderboardDetailsMax = 64;

		public const int k_cbMaxGameServerGameDir = 32;

		public const int k_cbMaxGameServerMapName = 32;

		public const int k_cbMaxGameServerGameDescription = 64;

		public const int k_cbMaxGameServerName = 64;

		public const int k_cbMaxGameServerTags = 128;

		public const int k_cbMaxGameServerGameData = 2048;

		public const int k_unSteamAccountIDMask = -1;

		public const int k_unSteamAccountInstanceMask = 1048575;

		public const int k_unSteamUserDesktopInstance = 1;

		public const int k_unSteamUserConsoleInstance = 2;

		public const int k_unSteamUserWebInstance = 4;

		public const int k_cchGameExtraInfoMax = 64;

		public const int k_nSteamEncryptedAppTicketSymmetricKeyLen = 32;

		public const int k_cubSaltSize = 8;

		public const ulong k_GIDNil = ulong.MaxValue;

		public const ulong k_TxnIDNil = ulong.MaxValue;

		public const ulong k_TxnIDUnknown = 0uL;

		public const uint k_uPackageIdFreeSub = 0u;

		public const uint k_uPackageIdInvalid = uint.MaxValue;

		public const ulong k_ulAssetClassIdInvalid = 0uL;

		public const uint k_uPhysicalItemIdInvalid = 0u;

		public const uint k_uCellIDInvalid = uint.MaxValue;

		public const uint k_uPartnerIdInvalid = 0u;

		public const short MASTERSERVERUPDATERPORT_USEGAMESOCKETSHARE = -1;

		public const byte INVALID_HTTPREQUEST_HANDLE = 0;

		public const byte k_nMaxLobbyKeyLength = byte.MaxValue;

		public const int k_SteamMusicNameMaxLength = 255;

		public const int k_SteamMusicPNGMaxLength = 65535;

		public const int QUERY_PORT_NOT_INITIALIZED = 65535;

		public const int QUERY_PORT_ERROR = 65534;

		public const ulong STEAM_RIGHT_TRIGGER_MASK = 1uL;

		public const ulong STEAM_LEFT_TRIGGER_MASK = 2uL;

		public const ulong STEAM_RIGHT_BUMPER_MASK = 4uL;

		public const ulong STEAM_LEFT_BUMPER_MASK = 8uL;

		public const ulong STEAM_BUTTON_0_MASK = 16uL;

		public const ulong STEAM_BUTTON_1_MASK = 32uL;

		public const ulong STEAM_BUTTON_2_MASK = 64uL;

		public const ulong STEAM_BUTTON_3_MASK = 128uL;

		public const ulong STEAM_TOUCH_0_MASK = 256uL;

		public const ulong STEAM_TOUCH_1_MASK = 512uL;

		public const ulong STEAM_TOUCH_2_MASK = 1024uL;

		public const ulong STEAM_TOUCH_3_MASK = 2048uL;

		public const ulong STEAM_BUTTON_MENU_MASK = 4096uL;

		public const ulong STEAM_BUTTON_STEAM_MASK = 8192uL;

		public const ulong STEAM_BUTTON_ESCAPE_MASK = 16384uL;

		public const ulong STEAM_BUTTON_BACK_LEFT_MASK = 32768uL;

		public const ulong STEAM_BUTTON_BACK_RIGHT_MASK = 65536uL;

		public const ulong STEAM_BUTTON_LEFTPAD_CLICKED_MASK = 131072uL;

		public const ulong STEAM_BUTTON_RIGHTPAD_CLICKED_MASK = 262144uL;

		public const ulong STEAM_LEFTPAD_FINGERDOWN_MASK = 524288uL;

		public const ulong STEAM_RIGHTPAD_FINGERDOWN_MASK = 1048576uL;

		public const byte MAX_STEAM_CONTROLLERS = 8;
	}
}
