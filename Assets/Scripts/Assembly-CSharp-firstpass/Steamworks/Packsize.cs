using System.Runtime.InteropServices;

namespace Steamworks
{
	public static class Packsize
	{
		[StructLayout(0, Pack = 4)]
		private struct ValvePackingSentinel_t
		{
			private uint m_u32;

			private ulong m_u64;

			private ushort m_u16;

			private double m_d;
		}

		public const int value = 4;

		public static bool Test()
		{
			int num = Marshal.SizeOf(typeof(ValvePackingSentinel_t));
			int num2 = Marshal.SizeOf(typeof(RemoteStorageEnumerateUserSubscribedFilesResult_t));
			if (num != 24 || num2 != 612)
			{
				return false;
			}
			return true;
		}
	}
}
