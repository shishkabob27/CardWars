using System;

namespace Steamworks
{
	[Flags]
	public enum EChatMemberStateChange
	{
		k_EChatMemberStateChangeEntered = 1,
		k_EChatMemberStateChangeLeft = 2,
		k_EChatMemberStateChangeDisconnected = 4,
		k_EChatMemberStateChangeKicked = 8,
		k_EChatMemberStateChangeBanned = 0x10
	}
}
