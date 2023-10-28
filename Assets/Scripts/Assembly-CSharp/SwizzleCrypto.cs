#define ASSERTS_ON
using UnityEngine;

public class SwizzleCrypto
{
	private int[] swizzleMap;

	public SwizzleCrypto(int[] inSwizzleMap)
	{
		swizzleMap = inSwizzleMap;
	}

	public uint Encrypt(byte key)
	{
		TFUtils.Assert(swizzleMap.Length >= 8, "Bad swizzle operation - swizzle map not large enough to handle key");
		uint num = (uint)(Random.value * 4.2949673E+09f);
		int i = 0;
		for (int num2 = swizzleMap.Length; i < num2; i++)
		{
			int num3 = swizzleMap[i];
			int num4 = ~(1 << num3);
			num = (uint)((int)(num & num4) | (((key >> i) & 1) << num3));
		}
		return num;
	}

	public byte DecryptByte(uint key)
	{
		TFUtils.Assert(swizzleMap.Length >= 8, "Bad unswizzle operation - swizzle map not large enough to handle key");
		byte b = 0;
		int i = 0;
		for (int num = swizzleMap.Length; i < num; i++)
		{
			int num2 = swizzleMap[i];
			b |= (byte)(((key >> num2) & 1) << i);
		}
		return b;
	}
}
