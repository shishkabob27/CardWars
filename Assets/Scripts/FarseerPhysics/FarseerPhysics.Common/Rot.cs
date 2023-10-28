using UnityEngine;

namespace FarseerPhysics.Common
{
	public struct Rot
	{
		public float s;

		public float c;

		public void Set(float angle)
		{
			s = Mathf.Sin(angle);
			c = Mathf.Cos(angle);
		}

		public Vector2 GetXAxis()
		{
			return new Vector2(c, s);
		}

		public Vector2 GetYAxis()
		{
			return new Vector2(0f - s, c);
		}
	}
}
