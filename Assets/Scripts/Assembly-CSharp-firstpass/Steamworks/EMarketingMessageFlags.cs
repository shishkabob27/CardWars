using System;

namespace Steamworks
{
	[Flags]
	public enum EMarketingMessageFlags
	{
		k_EMarketingMessageFlagsNone = 0,
		k_EMarketingMessageFlagsHighPriority = 1,
		k_EMarketingMessageFlagsPlatformWindows = 2,
		k_EMarketingMessageFlagsPlatformMac = 4,
		k_EMarketingMessageFlagsPlatformLinux = 8,
		k_EMarketingMessageFlagsPlatformRestrictions = 0xE
	}
}
