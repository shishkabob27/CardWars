using System;

namespace Steamworks
{
	public struct HServerListRequest : IEquatable<HServerListRequest>
	{
		public static readonly HServerListRequest Invalid = new HServerListRequest(IntPtr.Zero);

		public IntPtr m_HServerListRequest;

		public HServerListRequest(IntPtr value)
		{
			m_HServerListRequest = value;
		}

		public override string ToString()
		{
			return m_HServerListRequest.ToString();
		}

		public override bool Equals(object other)
		{
			return other is HServerListRequest && this == (HServerListRequest)other;
		}

		public override int GetHashCode()
		{
			return m_HServerListRequest.GetHashCode();
		}

		public bool Equals(HServerListRequest other)
		{
			return m_HServerListRequest == other.m_HServerListRequest;
		}

		public static bool operator ==(HServerListRequest x, HServerListRequest y)
		{
			return x.m_HServerListRequest == y.m_HServerListRequest;
		}

		public static bool operator !=(HServerListRequest x, HServerListRequest y)
		{
			return !(x == y);
		}

		public static explicit operator HServerListRequest(IntPtr value)
		{
			return new HServerListRequest(value);
		}

		public static explicit operator IntPtr(HServerListRequest that)
		{
			return that.m_HServerListRequest;
		}
	}
}
