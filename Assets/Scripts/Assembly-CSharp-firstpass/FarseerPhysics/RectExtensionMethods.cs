using FarseerPhysics.Collision;
using UnityEngine;

namespace FarseerPhysics
{
	public static class RectExtensionMethods
	{
		public static AABB ToAABB(this Rect rect)
		{
			return new AABB(rect.center, rect.width, rect.height);
		}

		public static Rect ToRect(this AABB bbox)
		{
			return new Rect(bbox.UpperBound.x, bbox.UpperBound.y, bbox.LowerBound.x, bbox.LowerBound.y);
		}
	}
}
