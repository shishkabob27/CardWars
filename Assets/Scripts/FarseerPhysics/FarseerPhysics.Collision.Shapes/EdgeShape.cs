using UnityEngine;

namespace FarseerPhysics.Collision.Shapes
{
	public class EdgeShape : Shape
	{
		public bool HasVertex0;

		public bool HasVertex3;

		public Vector2 Vertex0;

		public Vector2 Vertex3;

		internal Vector2 _vertex1;

		internal Vector2 _vertex2;

		public Vector2 Vertex1
		{
			get
			{
				return _vertex1;
			}
		}

		public Vector2 Vertex2
		{
			get
			{
				return _vertex2;
			}
		}

		internal EdgeShape()
			: base(0f)
		{
			base.ShapeType = ShapeType.Edge;
			_radius = 0.01f;
		}
	}
}
