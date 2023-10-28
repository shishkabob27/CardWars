using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(0, Pack = 4)]
	[CallbackIdentity(117)]
	public struct IPCFailure_t
	{
		public enum EFailureType
		{
			k_EFailureFlushedCallbackQueue,
			k_EFailurePipeFail
		}

		public const int k_iCallback = 117;

		public byte m_eFailureType;
	}
}
