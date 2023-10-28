using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks
{
	public class UTF8Marshaler : ICustomMarshaler
	{
		public const string DoNotFree = "DoNotFree";

		private static UTF8Marshaler static_instance_free = new UTF8Marshaler(true);

		private static UTF8Marshaler static_instance = new UTF8Marshaler(false);

		private bool _freeNativeMemory;

		private UTF8Marshaler(bool freenativememory)
		{
			_freeNativeMemory = freenativememory;
		}

		public IntPtr MarshalManagedToNative(object managedObj)
		{
			if (managedObj == null)
			{
				return IntPtr.Zero;
			}
			string text = managedObj as string;
			if (text == null)
			{
				throw new Exception("UTF8Marshaler must be used on a string.");
			}
			byte[] array = new byte[Encoding.UTF8.GetByteCount(text) + 1];
			Encoding.UTF8.GetBytes(text, 0, text.Length, array, 0);
			IntPtr intPtr = Marshal.AllocHGlobal(array.Length);
			Marshal.Copy(array, 0, intPtr, array.Length);
			return intPtr;
		}

		public object MarshalNativeToManaged(IntPtr pNativeData)
		{
			int i;
			for (i = 0; Marshal.ReadByte(pNativeData, i) != 0; i++)
			{
			}
			if (i == 0)
			{
				return string.Empty;
			}
			byte[] array = new byte[i];
			Marshal.Copy(pNativeData, array, 0, array.Length);
			return Encoding.UTF8.GetString(array);
		}

		public void CleanUpNativeData(IntPtr pNativeData)
		{
			if (_freeNativeMemory)
			{
				Marshal.FreeHGlobal(pNativeData);
			}
		}

		public void CleanUpManagedData(object managedObj)
		{
		}

		public int GetNativeDataSize()
		{
			return -1;
		}

		public static ICustomMarshaler GetInstance(string cookie)
		{
			switch (cookie)
			{
			case "DoNotFree":
				return static_instance;
			default:
				return static_instance_free;
			}
		}
	}
}
