namespace Ionic.Zlib
{
	internal sealed class Adler
	{
		private static readonly int BASE = 65521;

		private static readonly int NMAX = 5552;

		internal static uint Adler32(uint adler, byte[] buf, int index, int len)
		{
			if (buf == null)
			{
				return 1u;
			}
			int num = (int)(adler & 0xFFFF);
			int num2 = (int)((adler >> 16) & 0xFFFF);
			while (len > 0)
			{
				int num3 = ((len < NMAX) ? len : NMAX);
				len -= num3;
				while (num3 >= 16)
				{
					num += buf[index++];
					num2 += num;
					num += buf[index++];
					num2 += num;
					num += buf[index++];
					num2 += num;
					num += buf[index++];
					num2 += num;
					num += buf[index++];
					num2 += num;
					num += buf[index++];
					num2 += num;
					num += buf[index++];
					num2 += num;
					num += buf[index++];
					num2 += num;
					num += buf[index++];
					num2 += num;
					num += buf[index++];
					num2 += num;
					num += buf[index++];
					num2 += num;
					num += buf[index++];
					num2 += num;
					num += buf[index++];
					num2 += num;
					num += buf[index++];
					num2 += num;
					num += buf[index++];
					num2 += num;
					num += buf[index++];
					num2 += num;
					num3 -= 16;
				}
				if (num3 != 0)
				{
					do
					{
						num += buf[index++];
						num2 += num;
					}
					while (--num3 != 0);
				}
				num %= BASE;
				num2 %= BASE;
			}
			return (uint)((num2 << 16) | num);
		}
	}
}
