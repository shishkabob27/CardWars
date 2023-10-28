using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(0, Pack = 4)]
	public struct SteamParamStringArray_t
	{
		public IntPtr m_ppStrings;

		public int m_nNumStrings;
	}
}
