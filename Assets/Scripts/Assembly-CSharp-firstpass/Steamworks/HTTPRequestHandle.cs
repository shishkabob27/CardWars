using System;

namespace Steamworks
{
	public struct HTTPRequestHandle : IEquatable<HTTPRequestHandle>, IComparable<HTTPRequestHandle>
	{
		public static readonly HTTPRequestHandle Invalid = new HTTPRequestHandle(0u);

		public uint m_HTTPRequestHandle;

		public HTTPRequestHandle(uint value)
		{
			m_HTTPRequestHandle = value;
		}

		public override string ToString()
		{
			return m_HTTPRequestHandle.ToString();
		}

		public override bool Equals(object other)
		{
			return other is HTTPRequestHandle && this == (HTTPRequestHandle)other;
		}

		public override int GetHashCode()
		{
			return m_HTTPRequestHandle.GetHashCode();
		}

		public bool Equals(HTTPRequestHandle other)
		{
			return m_HTTPRequestHandle == other.m_HTTPRequestHandle;
		}

		public int CompareTo(HTTPRequestHandle other)
		{
			return m_HTTPRequestHandle.CompareTo(other.m_HTTPRequestHandle);
		}

		public static bool operator ==(HTTPRequestHandle x, HTTPRequestHandle y)
		{
			return x.m_HTTPRequestHandle == y.m_HTTPRequestHandle;
		}

		public static bool operator !=(HTTPRequestHandle x, HTTPRequestHandle y)
		{
			return !(x == y);
		}

		public static explicit operator HTTPRequestHandle(uint value)
		{
			return new HTTPRequestHandle(value);
		}

		public static explicit operator uint(HTTPRequestHandle that)
		{
			return that.m_HTTPRequestHandle;
		}
	}
}
