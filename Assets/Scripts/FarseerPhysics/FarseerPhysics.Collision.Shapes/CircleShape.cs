using UnityEngine;

namespace FarseerPhysics.Collision.Shapes
{
	public class CircleShape : Shape
	{
		internal Vector2 _position;

        public CircleShape(float density) : base(density)
        {
        }

        public Vector2 Position
		{
			get
			{
				return _position;
			}
		}
	}
}
