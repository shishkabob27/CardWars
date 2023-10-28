using System;

namespace Steamworks
{
	public struct UGCUpdateHandle_t : IEquatable<UGCUpdateHandle_t>, IComparable<UGCUpdateHandle_t>
	{
		public static readonly UGCUpdateHandle_t Invalid = new UGCUpdateHandle_t(ulong.MaxValue);

		public ulong m_UGCQueryHandle;

		public UGCUpdateHandle_t(ulong value)
		{
			m_UGCQueryHandle = value;
		}

		public override string ToString()
		{
			return m_UGCQueryHandle.ToString();
		}

		public override bool Equals(object other)
		{
			return other is UGCUpdateHandle_t && this == (UGCUpdateHandle_t)other;
		}

		public override int GetHashCode()
		{
			return m_UGCQueryHandle.GetHashCode();
		}

		public bool Equals(UGCUpdateHandle_t other)
		{
			return m_UGCQueryHandle == other.m_UGCQueryHandle;
		}

		public int CompareTo(UGCUpdateHandle_t other)
		{
			return m_UGCQueryHandle.CompareTo(other.m_UGCQueryHandle);
		}

		public static bool operator ==(UGCUpdateHandle_t x, UGCUpdateHandle_t y)
		{
			return x.m_UGCQueryHandle == y.m_UGCQueryHandle;
		}

		public static bool operator !=(UGCUpdateHandle_t x, UGCUpdateHandle_t y)
		{
			return !(x == y);
		}

		public static explicit operator UGCUpdateHandle_t(ulong value)
		{
			return new UGCUpdateHandle_t(value);
		}

		public static explicit operator ulong(UGCUpdateHandle_t that)
		{
			return that.m_UGCQueryHandle;
		}
	}
}
