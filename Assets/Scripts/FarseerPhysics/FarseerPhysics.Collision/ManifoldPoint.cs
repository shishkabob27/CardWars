using UnityEngine;

namespace FarseerPhysics.Collision
{
	public struct ManifoldPoint
	{
		public ContactID Id;

		public Vector2 LocalPoint;

		public float NormalImpulse;

		public float TangentImpulse;
	}
}
