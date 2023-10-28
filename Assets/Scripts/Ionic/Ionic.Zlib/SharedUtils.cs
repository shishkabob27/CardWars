namespace Ionic.Zlib
{
	internal class SharedUtils
	{
		public static int URShift(int number, int bits)
		{
			return (int)((uint)number >> bits);
		}
	}
}
