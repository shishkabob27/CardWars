using UnityEngine;

namespace FarseerPhysics.Common
{
	public static class MathUtils
	{
		public static Vector2 Mul(ref Transform2D T, Vector2 v)
		{
			return Mul(ref T, ref v);
		}

		public static Vector2 Mul(ref Transform2D T, ref Vector2 v)
		{
			float x = T.q.c * v.x - T.q.s * v.y + T.p.x;
			float y = T.q.s * v.x + T.q.c * v.y + T.p.y;
			return new Vector2(x, y);
		}

		public static Vector2 Mul(Rot q, Vector2 v)
		{
			return new Vector2(q.c * v.x - q.s * v.y, q.s * v.x + q.c * v.y);
		}
	}
}
