using FarseerPhysics.Common;
using UnityEngine;

namespace FarseerPhysics.Collision
{
	public struct Manifold
	{
		public Vector2 LocalNormal;

		public Vector2 LocalPoint;

		public int PointCount;

		public FixedArray2<ManifoldPoint> Points;

		public ManifoldType Type;
	}
}
