using System;
using System.Collections.Generic;
using System.Text;

public class XorCrypto
{
	private byte[] mask;

	public XorCrypto(string asciiMask)
		: this(asciiMask.ToCharArray())
	{
	}

	public XorCrypto(char[] asciiMask)
		: this(Encoding.ASCII.GetBytes(asciiMask))
	{
	}

	public XorCrypto(byte[] inMask)
	{
		mask = inMask;
	}

	private byte[] Xor(byte[] bytes)
	{
		List<byte> list = new List<byte>();
		int num = 0;
		foreach (byte b in bytes)
		{
			list.Add((byte)(b ^ mask[num++ % mask.Length]));
		}
		return list.ToArray();
	}

	public string Encrypt(string data)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(data.ToCharArray());
		return Convert.ToBase64String(Xor(bytes));
	}

	public string Decrypt(string data)
	{
		byte[] bytes = Convert.FromBase64String(data);
		return Encoding.UTF8.GetString(Xor(bytes));
	}
}
