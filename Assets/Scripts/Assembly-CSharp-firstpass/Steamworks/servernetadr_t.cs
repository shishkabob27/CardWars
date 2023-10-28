namespace Steamworks
{
	public struct servernetadr_t
	{
		private ushort m_usConnectionPort;

		private ushort m_usQueryPort;

		private uint m_unIP;

		public void Init(uint ip, ushort usQueryPort, ushort usConnectionPort)
		{
			m_unIP = ip;
			m_usQueryPort = usQueryPort;
			m_usConnectionPort = usConnectionPort;
		}

		public ushort GetQueryPort()
		{
			return m_usQueryPort;
		}

		public void SetQueryPort(ushort usPort)
		{
			m_usQueryPort = usPort;
		}

		public ushort GetConnectionPort()
		{
			return m_usConnectionPort;
		}

		public void SetConnectionPort(ushort usPort)
		{
			m_usConnectionPort = usPort;
		}

		public uint GetIP()
		{
			return m_unIP;
		}

		public void SetIP(uint unIP)
		{
			m_unIP = unIP;
		}

		public string GetConnectionAddressString()
		{
			return ToString(m_unIP, m_usConnectionPort);
		}

		public string GetQueryAddressString()
		{
			return ToString(m_unIP, m_usQueryPort);
		}

		public static string ToString(uint unIP, ushort usPort)
		{
			return string.Format("{0}.{1}.{2}.{3}:{4}", (ulong)(unIP >> 24) & 0xFFuL, (ulong)(unIP >> 16) & 0xFFuL, (ulong)(unIP >> 8) & 0xFFuL, (ulong)unIP & 0xFFuL, usPort);
		}

		public override bool Equals(object other)
		{
			return other is servernetadr_t && this == (servernetadr_t)other;
		}

		public override int GetHashCode()
		{
			return m_unIP.GetHashCode() + m_usQueryPort.GetHashCode() + m_usConnectionPort.GetHashCode();
		}

		public bool Equals(servernetadr_t other)
		{
			return m_unIP == other.m_unIP && m_usQueryPort == other.m_usQueryPort && m_usConnectionPort == other.m_usConnectionPort;
		}

		public int CompareTo(servernetadr_t other)
		{
			return m_unIP.CompareTo(other.m_unIP) + m_usQueryPort.CompareTo(other.m_usQueryPort) + m_usConnectionPort.CompareTo(other.m_usConnectionPort);
		}

		public static bool operator <(servernetadr_t x, servernetadr_t y)
		{
			return x.m_unIP < y.m_unIP || (x.m_unIP == y.m_unIP && x.m_usQueryPort < y.m_usQueryPort);
		}

		public static bool operator >(servernetadr_t x, servernetadr_t y)
		{
			return x.m_unIP > y.m_unIP || (x.m_unIP == y.m_unIP && x.m_usQueryPort > y.m_usQueryPort);
		}

		public static bool operator ==(servernetadr_t x, servernetadr_t y)
		{
			return x.m_unIP == y.m_unIP && x.m_usQueryPort == y.m_usQueryPort && x.m_usConnectionPort == y.m_usConnectionPort;
		}

		public static bool operator !=(servernetadr_t x, servernetadr_t y)
		{
			return !(x == y);
		}
	}
}
