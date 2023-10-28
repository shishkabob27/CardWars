using UnityEngine;

namespace FarseerPhysics.Collision
{
	public struct AABB
	{
		private static DistanceInput _input = new DistanceInput();

		public Vector2 LowerBound;

		public Vector2 UpperBound;

		public AABB(Vector2 center, float width, float height)
		{
			LowerBound = center - new Vector2(width / 2f, height / 2f);
			UpperBound = center + new Vector2(width / 2f, height / 2f);
		}
	}
}
