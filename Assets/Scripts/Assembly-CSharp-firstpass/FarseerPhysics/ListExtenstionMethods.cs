using System.Collections.Generic;

namespace FarseerPhysics
{
	public static class ListExtenstionMethods
	{
		public static float Min(this List<float> list)
		{
			float num = float.MaxValue;
			for (int i = 0; i < list.Count; i++)
			{
				num = ((num.CompareTo(list[i]) != 1) ? num : list[i]);
			}
			return num;
		}

		public static float Max(this List<float> list)
		{
			float num = float.MinValue;
			for (int i = 0; i < list.Count; i++)
			{
				num = ((num.CompareTo(list[i]) != -1) ? num : list[i]);
			}
			return num;
		}

		public static float Average(this List<float> list)
		{
			float num = 0f;
			int count = list.Count;
			if (count == 0)
			{
				return 0f;
			}
			for (int i = 0; i < count; i++)
			{
				num += list[i];
			}
			return num / (float)count;
		}
	}
}
