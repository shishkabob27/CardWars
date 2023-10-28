using UnityEngine;

namespace FarseerPhysics.Common.FakeXna
{
	public static class VectorHelper
	{
		public static float Dot(Vector2 value1, Vector2 value2)
		{
			return value1.x * value2.x + value1.y * value2.y;
		}
	}
}
