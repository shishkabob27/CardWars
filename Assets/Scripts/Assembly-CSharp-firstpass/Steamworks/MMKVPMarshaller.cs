using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	public class MMKVPMarshaller
	{
		private IntPtr[] m_AllocatedMemory;

		private IntPtr m_NativeArray;

		public MMKVPMarshaller(MatchMakingKeyValuePair_t[] filters)
		{
			if (filters != null)
			{
				m_AllocatedMemory = new IntPtr[filters.Length];
				int num = Marshal.SizeOf(typeof(IntPtr));
				m_NativeArray = Marshal.AllocHGlobal(num * filters.Length);
				for (int i = 0; i < filters.Length; i++)
				{
					m_AllocatedMemory[i] = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MatchMakingKeyValuePair_t)));
					Marshal.StructureToPtr(filters[i], m_AllocatedMemory[i], false);
					Marshal.WriteIntPtr(m_NativeArray, i * num, m_AllocatedMemory[i]);
				}
			}
		}

		~MMKVPMarshaller()
		{
			if (!(m_NativeArray != IntPtr.Zero))
			{
				return;
			}
			Marshal.FreeHGlobal(m_NativeArray);
			IntPtr[] allocatedMemory = m_AllocatedMemory;
			int i = 0;
			for (; i < allocatedMemory.Length; i++)
			{
				IntPtr intPtr = allocatedMemory[i];
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
		}

		public static implicit operator IntPtr(MMKVPMarshaller that)
		{
			return that.m_NativeArray;
		}
	}
}
