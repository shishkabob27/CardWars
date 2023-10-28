using UnityEngine;

namespace FarseerPhysics.Common
{
	public struct Transform2D
	{
		public Vector2 p;

		public Rot q;

		public void Set(Vector2 position, float angle)
		{
			p = position;
			q.Set(angle);
		}
	}
}
