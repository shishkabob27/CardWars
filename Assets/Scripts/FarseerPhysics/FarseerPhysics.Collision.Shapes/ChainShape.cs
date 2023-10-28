using FarseerPhysics.Common;
using UnityEngine;

namespace FarseerPhysics.Collision.Shapes
{
	public class ChainShape : Shape
	{
		public Vertices Vertices;

		private Vector2 _prevVertex;

		private Vector2 _nextVertex;

		private bool _hasPrevVertex;

		private bool _hasNextVertex;

        public ChainShape(float density) : base(density)
        {
        }
    }
}
