using System;
using System.Runtime.InteropServices;

namespace Ionic.Zlib
{
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[ComVisible(true)]
	[Guid("ebc25cf6-9120-4283-b972-0e5520d0000D")]
	public sealed class ZlibCodec
	{
		public byte[] InputBuffer;

		public int NextIn;

		public int AvailableBytesIn;

		public long TotalBytesIn;

		public byte[] OutputBuffer;

		public int NextOut;

		public int AvailableBytesOut;

		public long TotalBytesOut;

		public string Message;

		internal DeflateManager dstate;

		internal InflateManager istate;

		internal uint _Adler32;

		public CompressionLevel CompressLevel = CompressionLevel.Default;

		public int WindowBits = 15;

		public CompressionStrategy Strategy = CompressionStrategy.Default;

		public int InitializeInflate(bool expectRfc1950Header)
		{
			return InitializeInflate(WindowBits, expectRfc1950Header);
		}

		public int InitializeInflate(int windowBits, bool expectRfc1950Header)
		{
			WindowBits = windowBits;
			if (dstate != null)
			{
				throw new ZlibException("You may not call InitializeInflate() after calling InitializeDeflate().");
			}
			istate = new InflateManager(expectRfc1950Header);
			return istate.Initialize(this, windowBits);
		}

		public int Inflate(FlushType flush)
		{
			if (istate == null)
			{
				throw new ZlibException("No Inflate State!");
			}
			return istate.Inflate(flush);
		}

		public int EndInflate()
		{
			if (istate == null)
			{
				throw new ZlibException("No Inflate State!");
			}
			int result = istate.End();
			istate = null;
			return result;
		}

		public int InitializeDeflate(CompressionLevel level, bool wantRfc1950Header)
		{
			CompressLevel = level;
			return _InternalInitializeDeflate(wantRfc1950Header);
		}

		private int _InternalInitializeDeflate(bool wantRfc1950Header)
		{
			if (istate != null)
			{
				throw new ZlibException("You may not call InitializeDeflate() after calling InitializeInflate().");
			}
			dstate = new DeflateManager();
			dstate.WantRfc1950HeaderBytes = wantRfc1950Header;
			return dstate.Initialize(this, CompressLevel, WindowBits, Strategy);
		}

		public int Deflate(FlushType flush)
		{
			if (dstate == null)
			{
				throw new ZlibException("No Deflate State!");
			}
			return dstate.Deflate(flush);
		}

		public int EndDeflate()
		{
			if (dstate == null)
			{
				throw new ZlibException("No Deflate State!");
			}
			dstate = null;
			return 0;
		}

		internal void flush_pending()
		{
			int num = dstate.pendingCount;
			if (num > AvailableBytesOut)
			{
				num = AvailableBytesOut;
			}
			if (num != 0)
			{
				if (dstate.pending.Length <= dstate.nextPending || OutputBuffer.Length <= NextOut || dstate.pending.Length < dstate.nextPending + num || OutputBuffer.Length < NextOut + num)
				{
					throw new ZlibException(string.Format("Invalid State. (pending.Length={0}, pendingCount={1})", dstate.pending.Length, dstate.pendingCount));
				}
				Array.Copy(dstate.pending, dstate.nextPending, OutputBuffer, NextOut, num);
				NextOut += num;
				dstate.nextPending += num;
				TotalBytesOut += num;
				AvailableBytesOut -= num;
				dstate.pendingCount -= num;
				if (dstate.pendingCount == 0)
				{
					dstate.nextPending = 0;
				}
			}
		}

		internal int read_buf(byte[] buf, int start, int size)
		{
			int num = AvailableBytesIn;
			if (num > size)
			{
				num = size;
			}
			if (num == 0)
			{
				return 0;
			}
			AvailableBytesIn -= num;
			if (dstate.WantRfc1950HeaderBytes)
			{
				_Adler32 = Adler.Adler32(_Adler32, InputBuffer, NextIn, num);
			}
			Array.Copy(InputBuffer, NextIn, buf, start, num);
			NextIn += num;
			TotalBytesIn += num;
			return num;
		}
	}
}
