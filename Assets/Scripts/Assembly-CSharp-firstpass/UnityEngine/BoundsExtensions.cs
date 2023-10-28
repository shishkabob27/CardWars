namespace UnityEngine
{
	public static class BoundsExtensions
	{
		public static Rect ToRect(this Bounds value)
		{
			Rect result = new Rect(0f, 0f, 0f, 0f);
			result.xMax = value.max.x;
			result.yMax = value.max.y;
			result.xMin = value.min.x;
			result.yMin = value.min.y;
			return result;
		}
	}
}
