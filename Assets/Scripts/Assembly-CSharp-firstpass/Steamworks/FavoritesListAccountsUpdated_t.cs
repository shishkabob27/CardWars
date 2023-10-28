using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(0, Pack = 4)]
	[CallbackIdentity(516)]
	public struct FavoritesListAccountsUpdated_t
	{
		public const int k_iCallback = 516;

		public EResult m_eResult;
	}
}
