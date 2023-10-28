using System;

namespace Ionic.Zlib
{
	internal sealed class InflateCodes
	{
		private const int START = 0;

		private const int LEN = 1;

		private const int LENEXT = 2;

		private const int DIST = 3;

		private const int DISTEXT = 4;

		private const int COPY = 5;

		private const int LIT = 6;

		private const int WASH = 7;

		private const int END = 8;

		private const int BADCODE = 9;

		internal int mode;

		internal int len;

		internal int[] tree;

		internal int tree_index = 0;

		internal int need;

		internal int lit;

		internal int bitsToGet;

		internal int dist;

		internal byte lbits;

		internal byte dbits;

		internal int[] ltree;

		internal int ltree_index;

		internal int[] dtree;

		internal int dtree_index;

		internal InflateCodes()
		{
		}

		internal void Init(int bl, int bd, int[] tl, int tl_index, int[] td, int td_index)
		{
			mode = 0;
			lbits = (byte)bl;
			dbits = (byte)bd;
			ltree = tl;
			ltree_index = tl_index;
			dtree = td;
			dtree_index = td_index;
			tree = null;
		}

		internal int Process(InflateBlocks blocks, int r)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			ZlibCodec codec = blocks._codec;
			num3 = codec.NextIn;
			int num4 = codec.AvailableBytesIn;
			num = blocks.bitb;
			num2 = blocks.bitk;
			int num5 = blocks.writeAt;
			int num6 = ((num5 < blocks.readAt) ? (blocks.readAt - num5 - 1) : (blocks.end - num5));
			while (true)
			{
				bool flag = true;
				switch (mode)
				{
				case 0:
					if (num6 >= 258 && num4 >= 10)
					{
						blocks.bitb = num;
						blocks.bitk = num2;
						codec.AvailableBytesIn = num4;
						codec.TotalBytesIn += num3 - codec.NextIn;
						codec.NextIn = num3;
						blocks.writeAt = num5;
						r = InflateFast(lbits, dbits, ltree, ltree_index, dtree, dtree_index, blocks, codec);
						num3 = codec.NextIn;
						num4 = codec.AvailableBytesIn;
						num = blocks.bitb;
						num2 = blocks.bitk;
						num5 = blocks.writeAt;
						num6 = ((num5 < blocks.readAt) ? (blocks.readAt - num5 - 1) : (blocks.end - num5));
						if (r != 0)
						{
							mode = ((r == 1) ? 7 : 9);
							break;
						}
					}
					need = lbits;
					tree = ltree;
					tree_index = ltree_index;
					mode = 1;
					goto case 1;
				case 1:
				{
					int num7;
					for (num7 = need; num2 < num7; num2 += 8)
					{
						if (num4 != 0)
						{
							r = 0;
							num4--;
							num |= (codec.InputBuffer[num3++] & 0xFF) << num2;
							continue;
						}
						blocks.bitb = num;
						blocks.bitk = num2;
						codec.AvailableBytesIn = num4;
						codec.TotalBytesIn += num3 - codec.NextIn;
						codec.NextIn = num3;
						blocks.writeAt = num5;
						return blocks.Flush(r);
					}
					int num8 = (tree_index + (num & InternalInflateConstants.InflateMask[num7])) * 3;
					num >>= tree[num8 + 1];
					num2 -= tree[num8 + 1];
					int num9 = tree[num8];
					if (num9 == 0)
					{
						lit = tree[num8 + 2];
						mode = 6;
						break;
					}
					if (((uint)num9 & 0x10u) != 0)
					{
						bitsToGet = num9 & 0xF;
						len = tree[num8 + 2];
						mode = 2;
						break;
					}
					if ((num9 & 0x40) == 0)
					{
						need = num9;
						tree_index = num8 / 3 + tree[num8 + 2];
						break;
					}
					if (((uint)num9 & 0x20u) != 0)
					{
						mode = 7;
						break;
					}
					mode = 9;
					codec.Message = "invalid literal/length code";
					r = -3;
					blocks.bitb = num;
					blocks.bitk = num2;
					codec.AvailableBytesIn = num4;
					codec.TotalBytesIn += num3 - codec.NextIn;
					codec.NextIn = num3;
					blocks.writeAt = num5;
					return blocks.Flush(r);
				}
				case 2:
				{
					int num7;
					for (num7 = bitsToGet; num2 < num7; num2 += 8)
					{
						if (num4 != 0)
						{
							r = 0;
							num4--;
							num |= (codec.InputBuffer[num3++] & 0xFF) << num2;
							continue;
						}
						blocks.bitb = num;
						blocks.bitk = num2;
						codec.AvailableBytesIn = num4;
						codec.TotalBytesIn += num3 - codec.NextIn;
						codec.NextIn = num3;
						blocks.writeAt = num5;
						return blocks.Flush(r);
					}
					len += num & InternalInflateConstants.InflateMask[num7];
					num >>= num7;
					num2 -= num7;
					need = dbits;
					tree = dtree;
					tree_index = dtree_index;
					mode = 3;
					goto case 3;
				}
				case 3:
				{
					int num7;
					for (num7 = need; num2 < num7; num2 += 8)
					{
						if (num4 != 0)
						{
							r = 0;
							num4--;
							num |= (codec.InputBuffer[num3++] & 0xFF) << num2;
							continue;
						}
						blocks.bitb = num;
						blocks.bitk = num2;
						codec.AvailableBytesIn = num4;
						codec.TotalBytesIn += num3 - codec.NextIn;
						codec.NextIn = num3;
						blocks.writeAt = num5;
						return blocks.Flush(r);
					}
					int num8 = (tree_index + (num & InternalInflateConstants.InflateMask[num7])) * 3;
					num >>= tree[num8 + 1];
					num2 -= tree[num8 + 1];
					int num9 = tree[num8];
					if (((uint)num9 & 0x10u) != 0)
					{
						bitsToGet = num9 & 0xF;
						dist = tree[num8 + 2];
						mode = 4;
						break;
					}
					if ((num9 & 0x40) == 0)
					{
						need = num9;
						tree_index = num8 / 3 + tree[num8 + 2];
						break;
					}
					mode = 9;
					codec.Message = "invalid distance code";
					r = -3;
					blocks.bitb = num;
					blocks.bitk = num2;
					codec.AvailableBytesIn = num4;
					codec.TotalBytesIn += num3 - codec.NextIn;
					codec.NextIn = num3;
					blocks.writeAt = num5;
					return blocks.Flush(r);
				}
				case 4:
				{
					int num7;
					for (num7 = bitsToGet; num2 < num7; num2 += 8)
					{
						if (num4 != 0)
						{
							r = 0;
							num4--;
							num |= (codec.InputBuffer[num3++] & 0xFF) << num2;
							continue;
						}
						blocks.bitb = num;
						blocks.bitk = num2;
						codec.AvailableBytesIn = num4;
						codec.TotalBytesIn += num3 - codec.NextIn;
						codec.NextIn = num3;
						blocks.writeAt = num5;
						return blocks.Flush(r);
					}
					dist += num & InternalInflateConstants.InflateMask[num7];
					num >>= num7;
					num2 -= num7;
					mode = 5;
					goto case 5;
				}
				case 5:
				{
					int i;
					for (i = num5 - dist; i < 0; i += blocks.end)
					{
					}
					while (len != 0)
					{
						if (num6 == 0)
						{
							if (num5 == blocks.end && blocks.readAt != 0)
							{
								num5 = 0;
								num6 = ((num5 < blocks.readAt) ? (blocks.readAt - num5 - 1) : (blocks.end - num5));
							}
							if (num6 == 0)
							{
								blocks.writeAt = num5;
								r = blocks.Flush(r);
								num5 = blocks.writeAt;
								num6 = ((num5 < blocks.readAt) ? (blocks.readAt - num5 - 1) : (blocks.end - num5));
								if (num5 == blocks.end && blocks.readAt != 0)
								{
									num5 = 0;
									num6 = ((num5 < blocks.readAt) ? (blocks.readAt - num5 - 1) : (blocks.end - num5));
								}
								if (num6 == 0)
								{
									blocks.bitb = num;
									blocks.bitk = num2;
									codec.AvailableBytesIn = num4;
									codec.TotalBytesIn += num3 - codec.NextIn;
									codec.NextIn = num3;
									blocks.writeAt = num5;
									return blocks.Flush(r);
								}
							}
						}
						blocks.window[num5++] = blocks.window[i++];
						num6--;
						if (i == blocks.end)
						{
							i = 0;
						}
						len--;
					}
					mode = 0;
					break;
				}
				case 6:
					if (num6 == 0)
					{
						if (num5 == blocks.end && blocks.readAt != 0)
						{
							num5 = 0;
							num6 = ((num5 < blocks.readAt) ? (blocks.readAt - num5 - 1) : (blocks.end - num5));
						}
						if (num6 == 0)
						{
							blocks.writeAt = num5;
							r = blocks.Flush(r);
							num5 = blocks.writeAt;
							num6 = ((num5 < blocks.readAt) ? (blocks.readAt - num5 - 1) : (blocks.end - num5));
							if (num5 == blocks.end && blocks.readAt != 0)
							{
								num5 = 0;
								num6 = ((num5 < blocks.readAt) ? (blocks.readAt - num5 - 1) : (blocks.end - num5));
							}
							if (num6 == 0)
							{
								blocks.bitb = num;
								blocks.bitk = num2;
								codec.AvailableBytesIn = num4;
								codec.TotalBytesIn += num3 - codec.NextIn;
								codec.NextIn = num3;
								blocks.writeAt = num5;
								return blocks.Flush(r);
							}
						}
					}
					r = 0;
					blocks.window[num5++] = (byte)lit;
					num6--;
					mode = 0;
					break;
				case 7:
					if (num2 > 7)
					{
						num2 -= 8;
						num4++;
						num3--;
					}
					blocks.writeAt = num5;
					r = blocks.Flush(r);
					num5 = blocks.writeAt;
					num6 = ((num5 < blocks.readAt) ? (blocks.readAt - num5 - 1) : (blocks.end - num5));
					if (blocks.readAt != blocks.writeAt)
					{
						blocks.bitb = num;
						blocks.bitk = num2;
						codec.AvailableBytesIn = num4;
						codec.TotalBytesIn += num3 - codec.NextIn;
						codec.NextIn = num3;
						blocks.writeAt = num5;
						return blocks.Flush(r);
					}
					mode = 8;
					goto case 8;
				case 8:
					r = 1;
					blocks.bitb = num;
					blocks.bitk = num2;
					codec.AvailableBytesIn = num4;
					codec.TotalBytesIn += num3 - codec.NextIn;
					codec.NextIn = num3;
					blocks.writeAt = num5;
					return blocks.Flush(r);
				case 9:
					r = -3;
					blocks.bitb = num;
					blocks.bitk = num2;
					codec.AvailableBytesIn = num4;
					codec.TotalBytesIn += num3 - codec.NextIn;
					codec.NextIn = num3;
					blocks.writeAt = num5;
					return blocks.Flush(r);
				default:
					r = -2;
					blocks.bitb = num;
					blocks.bitk = num2;
					codec.AvailableBytesIn = num4;
					codec.TotalBytesIn += num3 - codec.NextIn;
					codec.NextIn = num3;
					blocks.writeAt = num5;
					return blocks.Flush(r);
				}
			}
		}

		internal int InflateFast(int bl, int bd, int[] tl, int tl_index, int[] td, int td_index, InflateBlocks s, ZlibCodec z)
		{
			int nextIn = z.NextIn;
			int num = z.AvailableBytesIn;
			int num2 = s.bitb;
			int num3 = s.bitk;
			int num4 = s.writeAt;
			int num5 = ((num4 < s.readAt) ? (s.readAt - num4 - 1) : (s.end - num4));
			int num6 = InternalInflateConstants.InflateMask[bl];
			int num7 = InternalInflateConstants.InflateMask[bd];
			int num12;
			while (true)
			{
				if (num3 < 20)
				{
					num--;
					num2 |= (z.InputBuffer[nextIn++] & 0xFF) << num3;
					num3 += 8;
					continue;
				}
				int num8 = num2 & num6;
				int[] array = tl;
				int num9 = tl_index;
				int num10 = (num9 + num8) * 3;
				int num11;
				if ((num11 = array[num10]) == 0)
				{
					num2 >>= array[num10 + 1];
					num3 -= array[num10 + 1];
					s.window[num4++] = (byte)array[num10 + 2];
					num5--;
				}
				else
				{
					while (true)
					{
						num2 >>= array[num10 + 1];
						num3 -= array[num10 + 1];
						if (((uint)num11 & 0x10u) != 0)
						{
							num11 &= 0xF;
							num12 = array[num10 + 2] + (num2 & InternalInflateConstants.InflateMask[num11]);
							num2 >>= num11;
							for (num3 -= num11; num3 < 15; num3 += 8)
							{
								num--;
								num2 |= (z.InputBuffer[nextIn++] & 0xFF) << num3;
							}
							num8 = num2 & num7;
							array = td;
							num9 = td_index;
							num10 = (num9 + num8) * 3;
							num11 = array[num10];
							while (true)
							{
								num2 >>= array[num10 + 1];
								num3 -= array[num10 + 1];
								if (((uint)num11 & 0x10u) != 0)
								{
									break;
								}
								if ((num11 & 0x40) == 0)
								{
									num8 += array[num10 + 2];
									num8 += num2 & InternalInflateConstants.InflateMask[num11];
									num10 = (num9 + num8) * 3;
									num11 = array[num10];
									bool flag = true;
									continue;
								}
								z.Message = "invalid distance code";
								num12 = z.AvailableBytesIn - num;
								num12 = ((num3 >> 3 < num12) ? (num3 >> 3) : num12);
								num += num12;
								nextIn -= num12;
								num3 -= num12 << 3;
								s.bitb = num2;
								s.bitk = num3;
								z.AvailableBytesIn = num;
								z.TotalBytesIn += nextIn - z.NextIn;
								z.NextIn = nextIn;
								s.writeAt = num4;
								return -3;
							}
							for (num11 &= 0xF; num3 < num11; num3 += 8)
							{
								num--;
								num2 |= (z.InputBuffer[nextIn++] & 0xFF) << num3;
							}
							int num13 = array[num10 + 2] + (num2 & InternalInflateConstants.InflateMask[num11]);
							num2 >>= num11;
							num3 -= num11;
							num5 -= num12;
							int num14;
							if (num4 >= num13)
							{
								num14 = num4 - num13;
								if (num4 - num14 > 0 && 2 > num4 - num14)
								{
									s.window[num4++] = s.window[num14++];
									s.window[num4++] = s.window[num14++];
									num12 -= 2;
								}
								else
								{
									Array.Copy(s.window, num14, s.window, num4, 2);
									num4 += 2;
									num14 += 2;
									num12 -= 2;
								}
							}
							else
							{
								num14 = num4 - num13;
								do
								{
									num14 += s.end;
								}
								while (num14 < 0);
								num11 = s.end - num14;
								if (num12 > num11)
								{
									num12 -= num11;
									if (num4 - num14 > 0 && num11 > num4 - num14)
									{
										do
										{
											s.window[num4++] = s.window[num14++];
										}
										while (--num11 != 0);
									}
									else
									{
										Array.Copy(s.window, num14, s.window, num4, num11);
										num4 += num11;
										num14 += num11;
										num11 = 0;
									}
									num14 = 0;
								}
							}
							if (num4 - num14 > 0 && num12 > num4 - num14)
							{
								do
								{
									s.window[num4++] = s.window[num14++];
								}
								while (--num12 != 0);
								break;
							}
							Array.Copy(s.window, num14, s.window, num4, num12);
							num4 += num12;
							num14 += num12;
							num12 = 0;
							break;
						}
						if ((num11 & 0x40) == 0)
						{
							num8 += array[num10 + 2];
							num8 += num2 & InternalInflateConstants.InflateMask[num11];
							num10 = (num9 + num8) * 3;
							if ((num11 = array[num10]) == 0)
							{
								num2 >>= array[num10 + 1];
								num3 -= array[num10 + 1];
								s.window[num4++] = (byte)array[num10 + 2];
								num5--;
								break;
							}
							bool flag = true;
							continue;
						}
						if (((uint)num11 & 0x20u) != 0)
						{
							num12 = z.AvailableBytesIn - num;
							num12 = ((num3 >> 3 < num12) ? (num3 >> 3) : num12);
							num += num12;
							nextIn -= num12;
							num3 -= num12 << 3;
							s.bitb = num2;
							s.bitk = num3;
							z.AvailableBytesIn = num;
							z.TotalBytesIn += nextIn - z.NextIn;
							z.NextIn = nextIn;
							s.writeAt = num4;
							return 1;
						}
						z.Message = "invalid literal/length code";
						num12 = z.AvailableBytesIn - num;
						num12 = ((num3 >> 3 < num12) ? (num3 >> 3) : num12);
						num += num12;
						nextIn -= num12;
						num3 -= num12 << 3;
						s.bitb = num2;
						s.bitk = num3;
						z.AvailableBytesIn = num;
						z.TotalBytesIn += nextIn - z.NextIn;
						z.NextIn = nextIn;
						s.writeAt = num4;
						return -3;
					}
				}
				if (num5 < 258 || num < 10)
				{
					break;
				}
			}
			num12 = z.AvailableBytesIn - num;
			num12 = ((num3 >> 3 < num12) ? (num3 >> 3) : num12);
			num += num12;
			nextIn -= num12;
			num3 -= num12 << 3;
			s.bitb = num2;
			s.bitk = num3;
			z.AvailableBytesIn = num;
			z.TotalBytesIn += nextIn - z.NextIn;
			z.NextIn = nextIn;
			s.writeAt = num4;
			return 0;
		}
	}
}
